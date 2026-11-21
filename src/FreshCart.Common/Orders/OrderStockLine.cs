namespace FreshCart.Common.Orders;

public sealed class OrderStockLine
{
    public Guid ShopBranchId { get; set; }

    public Guid ProductVariantId { get; set; }

    public decimal Quantity { get; set; }
}
