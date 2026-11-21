namespace FreshCart.Common.Orders;

public sealed class ShopOrderCreateCommand
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public Guid ShopId { get; set; }

    public Guid BranchId { get; set; }

    public Guid DeliverySlotId { get; set; }

    public decimal DeliveryFee { get; set; }
}
