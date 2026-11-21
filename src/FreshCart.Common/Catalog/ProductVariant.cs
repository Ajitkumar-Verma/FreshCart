namespace FreshCart.Common.Catalog;

public sealed class ProductVariant
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public string SKU { get; set; } = string.Empty;

    public string UnitName { get; set; } = string.Empty;

    public decimal UnitQuantity { get; set; }

    public decimal UnitPrice { get; set; }

    public string CurrencyCode { get; set; } = string.Empty;

    public bool IsDefault { get; set; }

    public bool IsActive { get; set; }
}
