namespace FreshCart.Common.Orders;

public sealed class ShopOrderSummary
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public Guid ShopId { get; set; }

    public Guid ShopBranchId { get; set; }

    public byte Status { get; set; }

    public decimal DeliveryFee { get; set; }

    public string OrderNumber { get; set; } = string.Empty;

    public decimal ShopOrderTotal { get; set; }

    public DateTime CreatedOn { get; set; }
}
