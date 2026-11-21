namespace FreshCart.Common.Catalog;

public sealed class Product
{
    public Guid ProductId { get; set; }

    public Guid ProductVariantId { get; set; }

    public Guid ShopId { get; set; }

    public Guid CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string SKU { get; set; } = string.Empty;

    public string UnitName { get; set; } = string.Empty;

    public decimal UnitQuantity { get; set; }

    public decimal Price { get; set; }

    public string CurrencyCode { get; set; } = string.Empty;

    public bool IsAvailable { get; set; }
}
