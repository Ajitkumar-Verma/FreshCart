using FreshCart.Common.Auth;

namespace FreshCart.IRepository.Auth;

public interface IUserRepository
{
    /// <summary>Finds a non-deleted account by normalized email for credential verification.</summary>
    Task<UserAccount?> GetByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default);

    /// <summary>Returns role names assigned to an active account.</summary>
    Task<IReadOnlyList<string>> GetRolesAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Finds a configured role by its normalized name.</summary>
    Task<Guid?> GetRoleIdByNormalizedNameAsync(string normalizedRoleName, CancellationToken cancellationToken = default);

    /// <summary>Checks whether any active account currently has the supplied role.</summary>
    Task<bool> HasActiveUserInRoleAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>Inserts one user record and propagates persistence failures.</summary>
    Task CreateAsync(UserAccount user, CancellationToken cancellationToken = default);

    /// <summary>Inserts one user-to-role assignment and propagates persistence failures.</summary>
    Task AssignRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
}
