namespace FreshCart.Common.Orders;

public sealed class OrderItemCreateCommand
{
    public Guid Id { get; set; }

    public Guid ShopOrderId { get; set; }

    public Guid ProductVariantId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string UnitName { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal LineTotal { get; set; }
}
