namespace FreshCart.Common.Inventory;

public sealed class InventoryItem
{
    public Guid Id { get; set; }

    public Guid ShopBranchId { get; set; }

    public Guid ProductVariantId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string VariantName { get; set; } = string.Empty;

    public decimal QuantityOnHand { get; set; }

    public decimal ReorderLevel { get; set; }
}
