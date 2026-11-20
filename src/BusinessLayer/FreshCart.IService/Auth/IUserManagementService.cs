using FreshCart.Common.Auth;

namespace FreshCart.IService.Auth;

public interface IUserManagementService
{
    /// <summary>Returns all active accounts, including their assigned roles.</summary>
    Task<IReadOnlyList<ManagedUserResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns one active account or raises a not-found application error.</summary>
    Task<ManagedUserResponse> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Creates an account with an allowed non-administrator role.</summary>
    Task<ManagedUserResponse> CreateAsync(CreateManagedUserRequest request, CancellationToken cancellationToken = default);

    /// <summary>Updates an account profile without changing its role or password.</summary>
    Task<ManagedUserResponse> UpdateAsync(Guid userId, UpdateManagedUserRequest request, Guid updatedBy, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes an account while preserving its database history.</summary>
    Task DeleteAsync(Guid userId, Guid deletedBy, CancellationToken cancellationToken = default);
}
