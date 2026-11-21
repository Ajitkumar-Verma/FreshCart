using FreshCart.Common.Orders;

namespace FreshCart.IRepository.Orders;

public interface IRefundRepository
{
    /// <summary>Returns one refund and its parent order identifiers.</summary>
    Task<RefundRecord?> GetByIdAsync(Guid refundId, CancellationToken cancellationToken = default);

    /// <summary>Returns pending refund requests for platform administration.</summary>
    Task<IReadOnlyList<RefundRecord>> GetPendingAsync(CancellationToken cancellationToken = default);

    /// <summary>Counts pending and completed refund value for one payment.</summary>
    Task<decimal> GetReservedAmountAsync(Guid paymentId, CancellationToken cancellationToken = default);

    /// <summary>Counts completed refund value for one payment.</summary>
    Task<decimal> GetCompletedAmountAsync(Guid paymentId, CancellationToken cancellationToken = default);

    /// <summary>Creates a pending refund request.</summary>
    Task<RefundRecord> CreateAsync(RefundRecord refund, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Changes settlement status and stores an optional external receipt reference.</summary>
    Task<RefundRecord?> SettleAsync(Guid refundId, byte status, string? gatewayReference, Guid actorUserId, CancellationToken cancellationToken = default);
}
