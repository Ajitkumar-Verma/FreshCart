using System.ComponentModel.DataAnnotations;

namespace FreshCart.Common.Marketplace;

public sealed class ShopRequest
{
    [Required, MaxLength(160)]
    public string Name { get; set; } = string.Empty;

    [Required, RegularExpression("^[a-z0-9]+(?:-[a-z0-9]+)*$"), MaxLength(180)]
    public string Slug { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }
}
