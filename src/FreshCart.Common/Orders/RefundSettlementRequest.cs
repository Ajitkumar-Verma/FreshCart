using System.ComponentModel.DataAnnotations;
using FreshCart.Common.Constants;

namespace FreshCart.Common.Orders;

public sealed class RefundSettlementRequest
{
    [Range(RefundStatuses.Completed, RefundStatuses.Rejected)]
    public byte Status { get; set; }

    [MaxLength(200)]
    public string? GatewayReference { get; set; }
}
