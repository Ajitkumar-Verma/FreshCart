namespace FreshCart.Common.Marketplace;

public sealed class ShopBranch
{
    public Guid Id { get; set; }

    public Guid ShopId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string AddressLine { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public bool IsActive { get; set; }
}
