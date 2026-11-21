namespace FreshCart.Common.Shopping;

public sealed class WishlistItem
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public Guid ShopId { get; set; }

    public decimal Price { get; set; }

    public string CurrencyCode { get; set; } = string.Empty;
}
