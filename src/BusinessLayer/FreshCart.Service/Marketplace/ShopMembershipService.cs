using FreshCart.Common.Exceptions;
using FreshCart.Common.Constants;
using FreshCart.Common.Marketplace;
using FreshCart.Common.Persistence;
using FreshCart.IRepository.Marketplace;
using FreshCart.IService.Marketplace;

namespace FreshCart.Service.Marketplace;

public sealed class ShopMembershipService(
    IShopMembershipRepository memberships,
    IShopRepository shops,
    IIdGenerator idGenerator) : IShopMembershipService
{
    public async Task<IReadOnlyList<ShopMembership>> GetByShopAsync(Guid shopId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        await EnsureCanManageAsync(shopId, actorUserId, cancellationToken);
        return await memberships.GetByShopAsync(shopId, cancellationToken);
    }

    public async Task<ShopMembership> AddAsync(Guid shopId, ShopMembershipRequest request, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        await EnsureCanManageAsync(shopId, actorUserId, cancellationToken);
        var role = NormalizeRole(request.RoleName);
        if (request.UserId == Guid.Empty)
            throw new ValidationAppException();
        if (await memberships.ExistsAsync(shopId, request.UserId, cancellationToken))
            throw new ConflictAppException();

        var membership = new ShopMembership
        {
            Id = idGenerator.NewId(),
            ShopId = shopId,
            UserId = request.UserId,
            RoleName = role,
            IsActive = true
        };
        return await memberships.CreateAsync(membership, actorUserId, cancellationToken);
    }

    public async Task<ShopMembership> UpdateAsync(Guid shopId, Guid membershipId, ShopMembershipRequest request, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        await EnsureCanManageAsync(shopId, actorUserId, cancellationToken);
        var current = await memberships.GetByIdAsync(shopId, membershipId, cancellationToken)
            ?? throw new NotFoundAppException();
        current.RoleName = NormalizeRole(request.RoleName);
        return await memberships.UpdateAsync(current, actorUserId, cancellationToken)
            ?? throw new NotFoundAppException();
    }

    public async Task DeleteAsync(Guid shopId, Guid membershipId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        await EnsureCanManageAsync(shopId, actorUserId, cancellationToken);
        if (!await memberships.SoftDeleteAsync(shopId, membershipId, actorUserId, cancellationToken))
            throw new NotFoundAppException();
    }

    private async Task EnsureCanManageAsync(Guid shopId, Guid actorUserId, CancellationToken cancellationToken)
    {
        if (!await shops.CanManageAsync(shopId, actorUserId, cancellationToken))
            throw new ForbiddenAppException();
    }

    private static string NormalizeRole(string roleName)
    {
        if (string.Equals(roleName?.Trim(), ShopMembershipRoles.Staff, StringComparison.OrdinalIgnoreCase))
            return ShopMembershipRoles.Staff;
        if (string.Equals(roleName?.Trim(), ShopMembershipRoles.Manager, StringComparison.OrdinalIgnoreCase))
            return ShopMembershipRoles.Manager;
        throw new ValidationAppException();
    }
}
