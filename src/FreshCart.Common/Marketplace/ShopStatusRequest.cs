using System.ComponentModel.DataAnnotations;

namespace FreshCart.Common.Marketplace;

public sealed class ShopStatusRequest
{
    [Range(ShopStatus.PendingApproval, ShopStatus.Suspended)]
    public byte Status { get; set; }
}
