using System.ComponentModel.DataAnnotations;
using FreshCart.Common.Constants;

namespace FreshCart.Common.Orders;

public sealed class OrderStatusRequest
{
    [Range(OrderStatuses.Confirmed, OrderStatuses.Cancelled)]
    public byte Status { get; set; }

    [MaxLength(500)]
    public string? Note { get; set; }
}
