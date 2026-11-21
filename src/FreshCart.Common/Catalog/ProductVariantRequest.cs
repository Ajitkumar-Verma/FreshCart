using System.ComponentModel.DataAnnotations;

namespace FreshCart.Common.Catalog;

public sealed class ProductVariantRequest
{
    [Required, MaxLength(80)]
    public string SKU { get; set; } = string.Empty;

    [Required, MaxLength(40)]
    public string UnitName { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.001", "9999999")]
    public decimal UnitQuantity { get; set; }

    [Range(typeof(decimal), "0", "9999999")]
    public decimal UnitPrice { get; set; }

    [Required, StringLength(3, MinimumLength = 3)]
    public string CurrencyCode { get; set; } = "INR";

    public bool IsDefault { get; set; }

    public bool IsActive { get; set; } = true;
}
