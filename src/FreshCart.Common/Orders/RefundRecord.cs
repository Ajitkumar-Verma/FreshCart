namespace FreshCart.Common.Orders;

public sealed class RefundRecord
{
    public Guid Id { get; set; }

    public Guid PaymentId { get; set; }

    public Guid OrderId { get; set; }

    public decimal Amount { get; set; }

    public string Reason { get; set; } = string.Empty;

    public byte Status { get; set; }

    public string? GatewayReference { get; set; }

    public DateTime CreatedOn { get; set; }
}
