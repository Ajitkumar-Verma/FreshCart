namespace FreshCart.Common.Catalog;

public sealed class Category
{
    public Guid Id { get; set; }

    public Guid? ParentCategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public int SortOrder { get; set; }
}
