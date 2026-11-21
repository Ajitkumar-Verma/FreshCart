using FreshCart.Common.Inventory;

namespace FreshCart.IService.Inventory;

public interface IInventoryService
{
    /// <summary>Lists inventory for branches managed by the caller's selected shop.</summary>
    Task<IReadOnlyList<InventoryItem>> GetByShopAsync(Guid shopId, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Creates or updates one branch and variant stock quantity and records its stock movement.</summary>
    Task<InventoryItem> AdjustAsync(Guid shopId, Guid inventoryId, InventoryAdjustmentRequest request, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Creates an inventory row for a branch and variant before its first quantity adjustment.</summary>
    Task<InventoryItem> CreateAsync(Guid shopId, Guid branchId, Guid productVariantId, Guid actorUserId, CancellationToken cancellationToken = default);
}
