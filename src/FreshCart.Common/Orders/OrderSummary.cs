namespace FreshCart.Common.Orders;

public sealed class OrderSummary
{
    public Guid Id { get; set; }

    public string OrderNumber { get; set; } = string.Empty;

    public byte OrderStatus { get; set; }

    public byte PaymentStatus { get; set; }

    public decimal Subtotal { get; set; }

    public decimal DeliveryFee { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal GrandTotal { get; set; }

    public string CurrencyCode { get; set; } = string.Empty;

    public DateTime CreatedOn { get; set; }
}
