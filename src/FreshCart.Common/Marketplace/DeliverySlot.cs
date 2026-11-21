namespace FreshCart.Common.Marketplace;

public sealed class DeliverySlot
{
    public Guid Id { get; set; }

    public Guid ShopBranchId { get; set; }

    public TimeSpan StartsAt { get; set; }

    public TimeSpan EndsAt { get; set; }

    public int Capacity { get; set; }

    public bool IsActive { get; set; }
}
