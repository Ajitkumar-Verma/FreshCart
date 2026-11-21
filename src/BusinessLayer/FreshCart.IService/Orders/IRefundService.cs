using FreshCart.Common.Orders;

namespace FreshCart.IService.Orders;

public interface IRefundService
{
    /// <summary>Creates a pending refund request for an order owned by the customer.</summary>
    Task<RefundRecord> RequestAsync(Guid orderId, RefundRequest request, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Lists refund requests awaiting platform settlement.</summary>
    Task<IReadOnlyList<RefundRecord>> GetPendingAsync(CancellationToken cancellationToken = default);

    /// <summary>Records an administrator's completed or rejected external refund action.</summary>
    Task<RefundRecord> SettleAsync(Guid refundId, RefundSettlementRequest request, Guid actorUserId, CancellationToken cancellationToken = default);
}
