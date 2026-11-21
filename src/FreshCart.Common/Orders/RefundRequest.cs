using System.ComponentModel.DataAnnotations;

namespace FreshCart.Common.Orders;

public sealed class RefundRequest
{
    [Range(typeof(decimal), "0.01", "999999999")]
    public decimal Amount { get; set; }

    [Required, MaxLength(500)]
    public string Reason { get; set; } = string.Empty;
}
