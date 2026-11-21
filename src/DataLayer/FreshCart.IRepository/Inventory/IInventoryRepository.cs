using FreshCart.Common.Inventory;

namespace FreshCart.IRepository.Inventory;

public interface IInventoryRepository
{
    /// <summary>Lists inventory for all active branches of a shop.</summary>
    Task<IReadOnlyList<InventoryItem>> GetByShopAsync(Guid shopId, CancellationToken cancellationToken = default);

    /// <summary>Finds an active inventory row by identifier.</summary>
    Task<InventoryItem?> GetByIdAsync(Guid inventoryId, CancellationToken cancellationToken = default);

    /// <summary>Finds an active inventory row for one branch and product variant.</summary>
    Task<InventoryItem?> GetByBranchAndVariantAsync(Guid branchId, Guid productVariantId, CancellationToken cancellationToken = default);

    /// <summary>Creates an empty inventory row for a branch and variant.</summary>
    Task<InventoryItem> CreateAsync(Guid id, Guid branchId, Guid productVariantId, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Applies a signed quantity change and returns whether stock constraints allowed it.</summary>
    Task<bool> UpdateQuantityAsync(Guid inventoryId, decimal quantityChange, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Records one quantity movement with its reason and notes.</summary>
    Task CreateMovementAsync(Guid movementId, Guid inventoryId, decimal quantityChange, string reason, string? notes, Guid actorUserId, CancellationToken cancellationToken = default);
}
