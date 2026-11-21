using System.ComponentModel.DataAnnotations;

namespace FreshCart.Common.Marketplace;

public sealed class DeliverySlotRequest
{
    public TimeSpan StartsAt { get; set; }

    public TimeSpan EndsAt { get; set; }

    [Range(1, int.MaxValue)]
    public int Capacity { get; set; }

    public bool IsActive { get; set; } = true;
}
