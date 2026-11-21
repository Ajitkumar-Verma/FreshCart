using FreshCart.Common.Marketplace;

namespace FreshCart.IRepository.Marketplace;

public interface IShopRepository
{
    /// <summary>Returns active shops approved for the public storefront.</summary>
    Task<IReadOnlyList<Shop>> GetApprovedAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns shops the specified user owns or is actively assigned to manage.</summary>
    Task<IReadOnlyList<Shop>> GetManagedByUserAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Finds an active shop by identifier.</summary>
    Task<Shop?> GetByIdAsync(Guid shopId, CancellationToken cancellationToken = default);

    /// <summary>Finds an existing shop slug for uniqueness validation.</summary>
    Task<Shop?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>Checks whether the user owns the shop or has an active shop membership.</summary>
    Task<bool> CanManageAsync(Guid shopId, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Inserts a shop and returns the persisted shop row.</summary>
    Task<Shop> CreateAsync(Shop shop, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Updates shop profile fields and returns the updated shop.</summary>
    Task<Shop?> UpdateAsync(Shop shop, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Changes the approval status of one active shop.</summary>
    Task<Shop?> SetStatusAsync(Guid shopId, byte status, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes one shop and preserves its data.</summary>
    Task<bool> SoftDeleteAsync(Guid shopId, Guid actorUserId, CancellationToken cancellationToken = default);
}
