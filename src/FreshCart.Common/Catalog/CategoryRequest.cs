using System.ComponentModel.DataAnnotations;

namespace FreshCart.Common.Catalog;

public sealed class CategoryRequest
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, RegularExpression("^[a-z0-9]+(?:-[a-z0-9]+)*$"), MaxLength(120)]
    public string Slug { get; set; } = string.Empty;

    public Guid? ParentCategoryId { get; set; }

    public bool IsActive { get; set; } = true;

    [Range(0, int.MaxValue)]
    public int SortOrder { get; set; }
}
