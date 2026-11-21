namespace FreshCart.Common.Orders;

public sealed class ShopCheckoutSelection
{
    public Guid ShopId { get; set; }

    public Guid BranchId { get; set; }

    public Guid DeliverySlotId { get; set; }
}
