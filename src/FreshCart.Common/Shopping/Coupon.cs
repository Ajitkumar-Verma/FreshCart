namespace FreshCart.Common.Shopping;

public sealed class Coupon
{
    public Guid Id { get; set; }

    public Guid? ShopId { get; set; }

    public string Code { get; set; } = string.Empty;

    public byte DiscountType { get; set; }

    public decimal DiscountValue { get; set; }

    public decimal MinimumOrder { get; set; }

    public DateTime StartsAt { get; set; }

    public DateTime EndsAt { get; set; }

    public int? UsageLimit { get; set; }

    public bool IsActive { get; set; }
}
