namespace FreshCart.Common.Marketplace;

public sealed class DeliveryZone
{
    public Guid Id { get; set; }

    public Guid ShopBranchId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public decimal DeliveryFee { get; set; }

    public decimal MinimumOrder { get; set; }

    public bool IsActive { get; set; }
}
