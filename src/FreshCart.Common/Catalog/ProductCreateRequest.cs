using System.ComponentModel.DataAnnotations;

namespace FreshCart.Common.Catalog;

public sealed class ProductCreateRequest
{
    [Required]
    public Guid ShopId { get; set; }

    [Required]
    public Guid CategoryId { get; set; }

    [Required, MaxLength(180)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

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
}
