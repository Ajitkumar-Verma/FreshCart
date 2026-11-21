using System.ComponentModel.DataAnnotations;

namespace FreshCart.Common.Orders;

public sealed class CashPaymentConfirmationRequest
{
    [Required, MaxLength(200)]
    public string ReceiptReference { get; set; } = string.Empty;
}
