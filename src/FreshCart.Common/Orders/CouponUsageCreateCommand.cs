namespace FreshCart.Common.Orders;

public sealed class CouponUsageCreateCommand
{
    public Guid Id { get; set; }

    public Guid CouponId { get; set; }

    public Guid OrderId { get; set; }

    public Guid UserId { get; set; }

    public decimal DiscountAmount { get; set; }
}
