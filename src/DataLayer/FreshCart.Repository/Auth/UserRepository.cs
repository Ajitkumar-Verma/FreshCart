using Dapper;
using FreshCart.Common.Auth;
using FreshCart.Common.Constants;
using FreshCart.Common.Persistence;
using FreshCart.IRepository.Auth;
using System.Data;

namespace FreshCart.Repository.Auth;

public sealed class UserRepository(IUnitOfWork unitOfWork) : IUserRepository
{
    public async Task<UserAccount?> GetByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
    {
        var connection = await unitOfWork.GetConnectionAsync(cancellationToken);
        var command = new CommandDefinition(StoredProcedure.Auth.UserGetByEmail, new { NormalizedEmail = normalizedEmail },
            transaction: unitOfWork.Transaction, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<UserAccount>(command);
    }

    public async Task<IReadOnlyList<string>> GetRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var connection = await unitOfWork.GetConnectionAsync(cancellationToken);
        var command = new CommandDefinition(StoredProcedure.Auth.UserGetRoles, new { UserId = userId },
            transaction: unitOfWork.Transaction, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);
        return (await connection.QueryAsync<string>(command)).AsList();
    }

    public async Task<CreateUserResult> CreateAsync(UserAccount user, string roleName, CancellationToken cancellationToken = default)
    {
        var connection = await unitOfWork.GetConnectionAsync(cancellationToken);
        var command = new CommandDefinition(StoredProcedure.Auth.UserCreate, new
        {
            user.Id,
            user.Email,
            user.NormalizedEmail,
            user.UserName,
            user.NormalizedUserName,
            user.DisplayName,
            user.PhoneNumber,
            user.PasswordHash,
            RoleName = roleName
        }, transaction: unitOfWork.Transaction, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);
        return await connection.QuerySingleAsync<CreateUserResult>(command);
    }

    public async Task<CreateUserResult> BootstrapPlatformAdminAsync(UserAccount user, CancellationToken cancellationToken = default)
    {
        var connection = await unitOfWork.GetConnectionAsync(cancellationToken);
        var command = new CommandDefinition(StoredProcedure.Auth.BootstrapPlatformAdmin, new
        {
            user.Id,
            user.Email,
            user.NormalizedEmail,
            user.UserName,
            user.NormalizedUserName,
            user.DisplayName,
            user.PhoneNumber,
            user.PasswordHash,
            RoleName = AppRoles.PlatformAdmin,
            NormalizedRoleName = AppRoles.PlatformAdmin.ToUpperInvariant()
        }, transaction: unitOfWork.Transaction, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);
        return await connection.QuerySingleAsync<CreateUserResult>(command);
    }
}
