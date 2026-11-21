using System.ComponentModel.DataAnnotations;

namespace FreshCart.Common.Shopping;

public sealed class CouponRequest
{
    public Guid? ShopId { get; set; }

    [Required, MaxLength(40)]
    public string Code { get; set; } = string.Empty;

    [Range(0, 1)]
    public byte DiscountType { get; set; }

    [Range(typeof(decimal), "0.01", "999999999")]
    public decimal DiscountValue { get; set; }

    [Range(typeof(decimal), "0", "999999999")]
    public decimal MinimumOrder { get; set; }

    public DateTime StartsAt { get; set; }

    public DateTime EndsAt { get; set; }

    [Range(1, int.MaxValue)]
    public int? UsageLimit { get; set; }

    public bool IsActive { get; set; } = true;
}
