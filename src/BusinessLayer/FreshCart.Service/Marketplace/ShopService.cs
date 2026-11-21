using FreshCart.Common.Exceptions;
using FreshCart.Common.Marketplace;
using FreshCart.Common.Persistence;
using FreshCart.IRepository.Marketplace;
using FreshCart.IService.Marketplace;

namespace FreshCart.Service.Marketplace;

public sealed class ShopService(IShopRepository shops, IIdGenerator idGenerator) : IShopService
{
    public async Task<IReadOnlyList<Shop>> GetApprovedAsync(CancellationToken cancellationToken = default)
    {
        return await shops.GetApprovedAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Shop>> GetManagedAsync(Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await shops.GetManagedByUserAsync(actorUserId, cancellationToken);
    }

    public async Task<Shop> GetManagedByIdAsync(Guid shopId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        var shop = await shops.GetByIdAsync(shopId, cancellationToken)
            ?? throw new NotFoundAppException();
        await EnsureCanManageAsync(shopId, actorUserId, cancellationToken);
        return shop;
    }

    public async Task<Shop> CreateAsync(ShopRequest request, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        var shop = Map(request, idGenerator.NewId());
        if (await shops.GetBySlugAsync(shop.Slug, cancellationToken) is not null)
            throw new ConflictAppException();

        shop.OwnerUserId = actorUserId;
        shop.Status = ShopStatus.PendingApproval;
        return await shops.CreateAsync(shop, actorUserId, cancellationToken);
    }

    public async Task<Shop> UpdateAsync(Guid shopId, ShopRequest request, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        var existing = await shops.GetByIdAsync(shopId, cancellationToken)
            ?? throw new NotFoundAppException();
        await EnsureCanManageAsync(shopId, actorUserId, cancellationToken);

        var shop = Map(request, shopId);
        var slugOwner = await shops.GetBySlugAsync(shop.Slug, cancellationToken);
        if (slugOwner is not null && slugOwner.Id != shopId)
            throw new ConflictAppException();

        shop.OwnerUserId = existing.OwnerUserId;
        shop.Status = existing.Status;
        shop.PlatformFeePercent = existing.PlatformFeePercent;
        return await shops.UpdateAsync(shop, actorUserId, cancellationToken)
            ?? throw new NotFoundAppException();
    }

    public async Task<Shop> SetStatusAsync(Guid shopId, byte status, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        if (status is not ShopStatus.PendingApproval and not ShopStatus.Approved and not ShopStatus.Suspended)
            throw new ValidationAppException();
        return await shops.SetStatusAsync(shopId, status, actorUserId, cancellationToken)
            ?? throw new NotFoundAppException();
    }

    public async Task DeleteAsync(Guid shopId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        await EnsureCanManageAsync(shopId, actorUserId, cancellationToken);
        if (!await shops.SoftDeleteAsync(shopId, actorUserId, cancellationToken))
            throw new NotFoundAppException();
    }

    private async Task EnsureCanManageAsync(Guid shopId, Guid actorUserId, CancellationToken cancellationToken)
    {
        if (!await shops.CanManageAsync(shopId, actorUserId, cancellationToken))
            throw new ForbiddenAppException();
    }

    private static Shop Map(ShopRequest request, Guid id) => new()
    {
        Id = id,
        Name = request.Name.Trim(),
        Slug = request.Slug.Trim().ToLowerInvariant(),
        Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim()
    };
}
