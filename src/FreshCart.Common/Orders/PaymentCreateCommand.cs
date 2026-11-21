namespace FreshCart.Common.Orders;

public sealed class PaymentCreateCommand
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public byte Method { get; set; }

    public byte Status { get; set; }

    public decimal Amount { get; set; }
}
