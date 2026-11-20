using FreshCart.Common.Auth;

namespace FreshCart.IRepository.Auth;

public interface IUserRepository
{
    /// <summary>Finds a non-deleted account by normalized email for credential verification.</summary>
    Task<UserAccount?> GetByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default);

    /// <summary>Returns role names assigned to an active account.</summary>
    Task<IReadOnlyList<string>> GetRolesAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Creates an account and assigns the supplied existing role, reporting email conflicts as a result.</summary>
    Task<CreateUserResult> CreateAsync(UserAccount user, string roleName, CancellationToken cancellationToken = default);

    /// <summary>Atomically creates the first platform administrator if none exists.</summary>
    Task<CreateUserResult> BootstrapPlatformAdminAsync(UserAccount user, CancellationToken cancellationToken = default);
}
