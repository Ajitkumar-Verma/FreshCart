namespace FreshCart.Common.Shopping;

public sealed class CartItem
{
    public Guid Id { get; set; }

    public Guid ProductVariantId { get; set; }

    public Guid ShopId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string SKU { get; set; } = string.Empty;

    public string UnitName { get; set; } = string.Empty;

    public decimal UnitQuantity { get; set; }

    public decimal UnitPrice { get; set; }

    public string CurrencyCode { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public decimal LineTotal { get; set; }

    public bool IsAvailable { get; set; }

    public bool IsDeleted { get; set; }
}
