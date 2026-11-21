using FreshCart.Common.Exceptions;
using FreshCart.Common.Marketplace;
using FreshCart.Common.Persistence;
using FreshCart.IRepository.Marketplace;
using FreshCart.IService.Marketplace;

namespace FreshCart.Service.Marketplace;

public sealed class ShopBranchService(
    IShopBranchRepository branches,
    IShopRepository shops,
    IIdGenerator idGenerator) : IShopBranchService
{
    public async Task<IReadOnlyList<ShopBranch>> GetPublicAsync(Guid shopId, CancellationToken cancellationToken = default)
    {
        return await branches.GetPublicAsync(shopId, cancellationToken);
    }

    public async Task<ShopBranch> GetByIdAsync(Guid shopId, Guid branchId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        await EnsureCanManageAsync(shopId, actorUserId, cancellationToken);
        return await branches.GetByIdAsync(shopId, branchId, cancellationToken)
            ?? throw new NotFoundAppException();
    }

    public async Task<ShopBranch> CreateAsync(Guid shopId, ShopBranchRequest request, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        await EnsureCanManageAsync(shopId, actorUserId, cancellationToken);
        var branch = Map(request, idGenerator.NewId(), shopId);
        return await branches.CreateAsync(branch, actorUserId, cancellationToken);
    }

    public async Task<ShopBranch> UpdateAsync(Guid shopId, Guid branchId, ShopBranchRequest request, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        await EnsureCanManageAsync(shopId, actorUserId, cancellationToken);
        _ = await branches.GetByIdAsync(shopId, branchId, cancellationToken)
            ?? throw new NotFoundAppException();
        return await branches.UpdateAsync(Map(request, branchId, shopId), actorUserId, cancellationToken)
            ?? throw new NotFoundAppException();
    }

    public async Task DeleteAsync(Guid shopId, Guid branchId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        await EnsureCanManageAsync(shopId, actorUserId, cancellationToken);
        if (await branches.GetByIdAsync(shopId, branchId, cancellationToken) is null)
            throw new NotFoundAppException();
        if (!await branches.SoftDeleteAsync(shopId, branchId, actorUserId, cancellationToken))
            throw new NotFoundAppException();
    }

    private async Task EnsureCanManageAsync(Guid shopId, Guid actorUserId, CancellationToken cancellationToken)
    {
        if (!await shops.CanManageAsync(shopId, actorUserId, cancellationToken))
            throw new ForbiddenAppException();
    }

    private static ShopBranch Map(ShopBranchRequest request, Guid id, Guid shopId) => new()
    {
        Id = id,
        ShopId = shopId,
        Name = request.Name.Trim(),
        AddressLine = request.AddressLine.Trim(),
        City = request.City.Trim(),
        PostalCode = request.PostalCode.Trim(),
        Latitude = request.Latitude,
        Longitude = request.Longitude,
        IsActive = request.IsActive
    };
}
