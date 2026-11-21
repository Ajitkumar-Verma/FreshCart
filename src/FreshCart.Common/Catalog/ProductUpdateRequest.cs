using System.ComponentModel.DataAnnotations;

namespace FreshCart.Common.Catalog;

public sealed class ProductUpdateRequest
{
    [Required]
    public Guid CategoryId { get; set; }

    [Required, MaxLength(180)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
}
