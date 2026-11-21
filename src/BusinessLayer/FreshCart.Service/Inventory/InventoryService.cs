using FreshCart.Common.Exceptions;
using FreshCart.Common.Inventory;
using FreshCart.Common.Persistence;
using FreshCart.IRepository.Catalog;
using FreshCart.IRepository.Inventory;
using FreshCart.IRepository.Marketplace;
using FreshCart.IService.Inventory;

namespace FreshCart.Service.Inventory;

public sealed class InventoryService(
    IInventoryRepository inventory,
    IShopRepository shops,
    IShopBranchRepository branches,
    IProductRepository products,
    IUnitOfWork unitOfWork,
    IIdGenerator idGenerator) : IInventoryService
{
    public async Task<IReadOnlyList<InventoryItem>> GetByShopAsync(Guid shopId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        await EnsureCanManageAsync(shopId, actorUserId, cancellationToken);
        return await inventory.GetByShopAsync(shopId, cancellationToken);
    }

    public async Task<InventoryItem> CreateAsync(Guid shopId, Guid branchId, Guid productVariantId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        await EnsureCanManageAsync(shopId, actorUserId, cancellationToken);
        if (await branches.GetByIdAsync(shopId, branchId, cancellationToken) is null)
            throw new NotFoundAppException();

        var productId = await products.GetProductIdByVariantIdAsync(productVariantId, cancellationToken);
        var product = productId.HasValue ? await products.GetForManagementAsync(productId.Value, cancellationToken) : null;
        if (product is null || product.ShopId != shopId)
            throw new NotFoundAppException();

        var existing = await inventory.GetByBranchAndVariantAsync(branchId, productVariantId, cancellationToken);
        if (existing is not null)
            throw new ConflictAppException();

        return await inventory.CreateAsync(idGenerator.NewId(), branchId, productVariantId, actorUserId, cancellationToken);
    }

    public async Task<InventoryItem> AdjustAsync(Guid shopId, Guid inventoryId, InventoryAdjustmentRequest request, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        await EnsureCanManageAsync(shopId, actorUserId, cancellationToken);
        var current = await inventory.GetByIdAsync(inventoryId, cancellationToken)
            ?? throw new NotFoundAppException();
        if (await branches.GetByIdAsync(shopId, current.ShopBranchId, cancellationToken) is null)
            throw new NotFoundAppException();

        var reason = request.Reason.Trim();
        if (reason.Length == 0 || request.Quantity == 0)
            throw new ValidationAppException();

        await unitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);
        try
        {
            var changed = await inventory.UpdateQuantityAsync(inventoryId, request.Quantity, actorUserId, cancellationToken);
            if (!changed)
                throw new ConflictAppException();

            await inventory.CreateMovementAsync(idGenerator.NewId(), inventoryId, request.Quantity, reason, request.Notes?.Trim(), actorUserId, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }

        return await inventory.GetByIdAsync(inventoryId, cancellationToken) ?? throw new NotFoundAppException();
    }

    private async Task EnsureCanManageAsync(Guid shopId, Guid actorUserId, CancellationToken cancellationToken)
    {
        if (!await shops.CanManageAsync(shopId, actorUserId, cancellationToken))
            throw new ForbiddenAppException();
    }
}
