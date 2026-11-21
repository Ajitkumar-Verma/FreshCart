namespace FreshCart.Common.Marketplace;

public sealed class Shop
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid OwnerUserId { get; set; }

    public byte Status { get; set; }

    public decimal PlatformFeePercent { get; set; }
}
