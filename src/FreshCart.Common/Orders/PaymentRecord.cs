namespace FreshCart.Common.Orders;

public sealed class PaymentRecord
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public byte Method { get; set; }

    public byte Status { get; set; }

    public decimal Amount { get; set; }

    public string? GatewayReference { get; set; }

    public DateTime? PaidAt { get; set; }
}
