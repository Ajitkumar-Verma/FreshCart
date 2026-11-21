namespace FreshCart.Common.Shopping;

public sealed class Cart
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public IReadOnlyList<CartItem> Items { get; set; } = Array.Empty<CartItem>();

    public decimal Subtotal { get; set; }
}
