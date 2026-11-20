using FreshCart.Common.Auth;
using FreshCart.Common.Constants;
using FreshCart.Common.Configuration;
using FreshCart.Common.Exceptions;
using FreshCart.IRepository.Auth;
using FreshCart.IService.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace FreshCart.Service.Auth;

public sealed class AuthService(
    IUserRepository users,
    IPasswordHasher<UserAccount> passwordHasher,
    IJwtTokenService tokenService,
    IOptions<BootstrapOptions> bootstrapOptions,
    FreshCart.Common.Persistence.IIdGenerator idGenerator) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var user = MakeUser(request.Email, request.DisplayName, request.PhoneNumber, request.Password);
        var result = await users.CreateAsync(user, AppRoles.Customer, cancellationToken);
        if (result.AlreadyExists) throw new ConflictAppException(AppErrorCodes.AccountExists);
        var roles = await users.GetRolesAsync(result.UserId, cancellationToken);
        return tokenService.CreateToken(user, roles);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await users.GetByNormalizedEmailAsync(NormalizeEmail(request.Email), cancellationToken);
        if (user is null || passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            throw new UnauthorizedAppException(AppErrorCodes.InvalidCredentials);
        var roles = await users.GetRolesAsync(user.Id, cancellationToken);
        return tokenService.CreateToken(user, roles);
    }

    public async Task<AuthResponse> CreatePlatformAdminAsync(CreateSuperUserRequest request, CancellationToken cancellationToken = default)
    {
        var user = MakeUser(request.Email, request.DisplayName, null, request.Password);
        var result = await users.CreateAsync(user, AppRoles.PlatformAdmin, cancellationToken);
        if (result.AlreadyExists) throw new ConflictAppException(AppErrorCodes.AccountExists);
        return tokenService.CreateToken(user, new[] { AppRoles.PlatformAdmin });
    }

    public async Task<AuthResponse> BootstrapPlatformAdminAsync(CreateSuperUserRequest request, string? suppliedKey, CancellationToken cancellationToken = default)
    {
        var expectedKey = bootstrapOptions.Value.SuperUserKey;
        if (string.IsNullOrWhiteSpace(expectedKey) || string.IsNullOrWhiteSpace(suppliedKey) ||
            !CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(expectedKey), Encoding.UTF8.GetBytes(suppliedKey)))
            throw new ForbiddenAppException(AppErrorCodes.BootstrapKeyInvalid);

        var user = MakeUser(request.Email, request.DisplayName, null, request.Password);
        var result = await users.BootstrapPlatformAdminAsync(user, cancellationToken);
        if (result.AlreadyExists) throw new ConflictAppException(AppErrorCodes.BootstrapAlreadyUsed);
        return tokenService.CreateToken(user, new[] { AppRoles.PlatformAdmin });
    }

    private UserAccount MakeUser(string email, string displayName, string? phoneNumber, string password)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new ValidationAppException(AppErrorCodes.EmailRequired);
        if (string.IsNullOrWhiteSpace(displayName)) throw new ValidationAppException(AppErrorCodes.DisplayNameRequired);
        if (password.Length < AuthPolicy.MinimumPasswordLength) throw new ValidationAppException(AppErrorCodes.PasswordTooShort);
        email = email.Trim();
        displayName = displayName.Trim();
        var normalized = NormalizeEmail(email);
        var account = new UserAccount
        {
            Id = idGenerator.NewId(), Email = email, NormalizedEmail = normalized,
            UserName = email, NormalizedUserName = normalized, DisplayName = displayName,
            PhoneNumber = phoneNumber?.Trim()
        };
        return new UserAccount
        {
            Id = account.Id, Email = account.Email, NormalizedEmail = account.NormalizedEmail,
            UserName = account.UserName, NormalizedUserName = account.NormalizedUserName,
            DisplayName = account.DisplayName, PhoneNumber = account.PhoneNumber,
            PasswordHash = passwordHasher.HashPassword(account, password)
        };
    }

    private static string NormalizeEmail(string email) => email.Trim().ToUpperInvariant();
}


