using System.ComponentModel.DataAnnotations;

namespace FreshCart.Common.Orders;

public sealed class CheckoutRequest
{
    [Required]
    public Guid AddressId { get; set; }

    [Required]
    public IReadOnlyList<ShopCheckoutSelection> Shops { get; set; } = Array.Empty<ShopCheckoutSelection>();

    [MaxLength(40)]
    public string? CouponCode { get; set; }

    public byte PaymentMethod { get; set; }
}
