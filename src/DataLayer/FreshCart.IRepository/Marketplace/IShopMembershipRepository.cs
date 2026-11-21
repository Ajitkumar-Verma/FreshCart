using FreshCart.Common.Marketplace;

namespace FreshCart.IRepository.Marketplace;

public interface IShopMembershipRepository
{
    /// <summary>Returns active memberships for one shop.</summary>
    Task<IReadOnlyList<ShopMembership>> GetByShopAsync(Guid shopId, CancellationToken cancellationToken = default);

    /// <summary>Finds a non-deleted membership by shop and membership identifiers.</summary>
    Task<ShopMembership?> GetByIdAsync(Guid shopId, Guid membershipId, CancellationToken cancellationToken = default);

    /// <summary>Checks whether a non-deleted membership exists for one user and shop.</summary>
    Task<bool> ExistsAsync(Guid shopId, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Creates one staff membership row.</summary>
    Task<ShopMembership> CreateAsync(ShopMembership membership, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Updates a membership role and active state.</summary>
    Task<ShopMembership?> UpdateAsync(ShopMembership membership, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes and deactivates a membership.</summary>
    Task<bool> SoftDeleteAsync(Guid shopId, Guid membershipId, Guid actorUserId, CancellationToken cancellationToken = default);
}
