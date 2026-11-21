using FreshCart.Common.Marketplace;

namespace FreshCart.IService.Marketplace;

public interface IShopMembershipService
{
    /// <summary>Lists active staff memberships after verifying shop management access.</summary>
    Task<IReadOnlyList<ShopMembership>> GetByShopAsync(Guid shopId, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Adds a staff member to a shop under a supported membership role.</summary>
    Task<ShopMembership> AddAsync(Guid shopId, ShopMembershipRequest request, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Updates the role of one shop membership.</summary>
    Task<ShopMembership> UpdateAsync(Guid shopId, Guid membershipId, ShopMembershipRequest request, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Deactivates a membership while preserving its audit history.</summary>
    Task DeleteAsync(Guid shopId, Guid membershipId, Guid actorUserId, CancellationToken cancellationToken = default);
}
