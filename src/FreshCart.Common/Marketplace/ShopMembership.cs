namespace FreshCart.Common.Marketplace;

public sealed class ShopMembership
{
    public Guid Id { get; set; }

    public Guid ShopId { get; set; }

    public Guid UserId { get; set; }

    public string RoleName { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}
