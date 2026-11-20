using FreshCart.Common.Auth;
using FreshCart.Common.Constants;
using FreshCart.Common.Exceptions;
using FreshCart.Common.Persistence;
using FreshCart.IRepository.Auth;
using FreshCart.IService.Auth;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace FreshCart.Service.Auth;

public sealed class UserManagementService(
    IUserRepository users,
    IPasswordHasher<UserAccount> passwordHasher,
    IIdGenerator idGenerator,
    IUnitOfWork unitOfWork) : IUserManagementService
{
    private static readonly string[] AssignableRoles = [AppRoles.Customer, AppRoles.ShopOwner, AppRoles.ShopStaff];

    public async Task<IReadOnlyList<ManagedUserResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var records = await users.GetAllManagedUsersAsync(cancellationToken);
        var assignments = await users.GetManagedUserRoleAssignmentsAsync(cancellationToken);
        var rolesByUser = assignments
            .GroupBy(assignment => assignment.UserId)
            .ToDictionary(group => group.Key, group => (IReadOnlyList<string>)group.Select(item => item.RoleName).Distinct().Order().ToArray());

        return records.Select(record => ToResponse(record, rolesByUser.GetValueOrDefault(record.Id, Array.Empty<string>()))).ToArray();
    }

    public async Task<ManagedUserResponse> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var record = await users.GetManagedUserByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundAppException();
        return ToResponse(record, await users.GetRolesAsync(userId, cancellationToken));
    }

    public async Task<ManagedUserResponse> CreateAsync(CreateManagedUserRequest request, CancellationToken cancellationToken = default)
    {
        var email = NormalizeEmailInput(request.Email);
        var displayName = NormalizeDisplayName(request.DisplayName);
        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < AuthPolicy.MinimumPasswordLength)
            throw new ValidationAppException(AppErrorCodes.PasswordTooShort);
        var roleName = ResolveAssignableRole(request.RoleName);
        var normalizedEmail = NormalizeEmail(email);
        var account = new UserAccount
        {
            Id = idGenerator.NewId(),
            Email = email,
            NormalizedEmail = normalizedEmail,
            UserName = email,
            NormalizedUserName = normalizedEmail,
            DisplayName = displayName,
            PhoneNumber = request.PhoneNumber?.Trim()
        };
        account = WithPasswordHash(account, passwordHasher.HashPassword(account, request.Password));

        await unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        try
        {
            if (await users.GetByNormalizedEmailAsync(normalizedEmail, cancellationToken) is not null)
                throw new ConflictAppException(AppErrorCodes.AccountExists);
            var roleId = await users.GetRoleIdByNormalizedNameAsync(roleName.ToUpperInvariant(), cancellationToken)
                ?? throw new ConfigurationAppException(AppErrorCodes.RoleNotConfigured);

            await users.CreateAsync(account, cancellationToken);
            await users.AssignRoleAsync(account.Id, roleId, cancellationToken);
            var response = await GetByIdAsync(account.Id, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return response;
        }
        catch
        {
            await unitOfWork.RollbackAsync(CancellationToken.None);
            throw;
        }
    }

    public async Task<ManagedUserResponse> UpdateAsync(Guid userId, UpdateManagedUserRequest request, Guid updatedBy, CancellationToken cancellationToken = default)
    {
        var email = NormalizeEmailInput(request.Email);
        var normalizedEmail = NormalizeEmail(email);
        var displayName = NormalizeDisplayName(request.DisplayName);

        await unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        try
        {
            var existing = await users.GetManagedUserByIdAsync(userId, cancellationToken)
                ?? throw new NotFoundAppException();
            var emailOwner = await users.GetByNormalizedEmailAsync(normalizedEmail, cancellationToken);
            if (emailOwner is not null && emailOwner.Id != userId)
                throw new ConflictAppException(AppErrorCodes.AccountExists);

            await users.UpdateManagedUserAsync(userId, email, normalizedEmail, displayName, request.PhoneNumber?.Trim(), updatedBy, cancellationToken);
            var updated = await users.GetManagedUserByIdAsync(existing.Id, cancellationToken)
                ?? throw new NotFoundAppException();
            var response = ToResponse(updated, await users.GetRolesAsync(userId, cancellationToken));
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return response;
        }
        catch
        {
            await unitOfWork.RollbackAsync(CancellationToken.None);
            throw;
        }
    }

    public async Task DeleteAsync(Guid userId, Guid deletedBy, CancellationToken cancellationToken = default)
    {
        await unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        try
        {
            _ = await users.GetManagedUserByIdAsync(userId, cancellationToken)
                ?? throw new NotFoundAppException();
            if (userId == deletedBy)
                throw new ConflictAppException(AppErrorCodes.CannotDeleteCurrentUser);

            var roles = await users.GetRolesAsync(userId, cancellationToken);
            if (roles.Contains(AppRoles.PlatformAdmin, StringComparer.OrdinalIgnoreCase))
            {
                var platformAdminRoleId = await users.GetRoleIdByNormalizedNameAsync(AppRoles.PlatformAdmin.ToUpperInvariant(), cancellationToken)
                    ?? throw new ConfigurationAppException(AppErrorCodes.RoleNotConfigured);
                if (await users.GetActiveUserCountInRoleAsync(platformAdminRoleId, cancellationToken) <= 1)
                    throw new ConflictAppException(AppErrorCodes.LastPlatformAdmin);
            }

            await users.SoftDeleteAsync(userId, deletedBy, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            await unitOfWork.RollbackAsync(CancellationToken.None);
            throw;
        }
    }

    private static string ResolveAssignableRole(string? requestedRole)
    {
        if (string.IsNullOrWhiteSpace(requestedRole)) return AppRoles.Customer;
        var roleName = AssignableRoles.FirstOrDefault(role => role.Equals(requestedRole.Trim(), StringComparison.OrdinalIgnoreCase));
        return roleName ?? throw new ValidationAppException(AppErrorCodes.RoleAssignmentNotAllowed);
    }

    private static ManagedUserResponse ToResponse(ManagedUserRecord record, IReadOnlyList<string> roles) =>
        new(record.Id, record.Email, record.DisplayName, record.PhoneNumber, record.CreatedOn, record.UpdatedOn, roles);

    private static string NormalizeEmailInput(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new ValidationAppException(AppErrorCodes.EmailRequired);
        return email.Trim();
    }

    private static string NormalizeDisplayName(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName)) throw new ValidationAppException(AppErrorCodes.DisplayNameRequired);
        return displayName.Trim();
    }

    private static string NormalizeEmail(string email) => email.ToUpperInvariant();

    private static UserAccount WithPasswordHash(UserAccount account, string passwordHash) => new()
    {
        Id = account.Id,
        Email = account.Email,
        NormalizedEmail = account.NormalizedEmail,
        UserName = account.UserName,
        NormalizedUserName = account.NormalizedUserName,
        DisplayName = account.DisplayName,
        PhoneNumber = account.PhoneNumber,
        PasswordHash = passwordHash
    };
}
