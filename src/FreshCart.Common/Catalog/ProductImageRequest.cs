using System.ComponentModel.DataAnnotations;

namespace FreshCart.Common.Catalog;

public sealed class ProductImageRequest
{
    [Required, Url, MaxLength(2048)]
    public string ImageUrl { get; set; } = string.Empty;

    [MaxLength(250)]
    public string? AltText { get; set; }

    [Range(0, int.MaxValue)]
    public int SortOrder { get; set; }
}
