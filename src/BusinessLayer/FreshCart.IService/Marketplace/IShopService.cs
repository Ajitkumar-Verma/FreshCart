using FreshCart.Common.Marketplace;

namespace FreshCart.IService.Marketplace;

public interface IShopService
{
    /// <summary>Lists active approved shops for storefront discovery.</summary>
    Task<IReadOnlyList<Shop>> GetApprovedAsync(CancellationToken cancellationToken = default);

    /// <summary>Lists active shops owned by or assigned to the current user.</summary>
    Task<IReadOnlyList<Shop>> GetManagedAsync(Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Returns one active shop after verifying that the caller can manage it.</summary>
    Task<Shop> GetManagedByIdAsync(Guid shopId, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Creates a shop in pending-approval state for the current owner.</summary>
    Task<Shop> CreateAsync(ShopRequest request, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Updates a shop after verifying the caller manages it.</summary>
    Task<Shop> UpdateAsync(Guid shopId, ShopRequest request, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Changes shop approval status; only a platform administrator may call this operation.</summary>
    Task<Shop> SetStatusAsync(Guid shopId, byte status, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes a shop after verifying the caller manages it.</summary>
    Task DeleteAsync(Guid shopId, Guid actorUserId, CancellationToken cancellationToken = default);
}
