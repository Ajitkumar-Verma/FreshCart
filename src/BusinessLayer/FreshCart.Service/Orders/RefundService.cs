using FreshCart.Common.Constants;
using FreshCart.Common.Exceptions;
using FreshCart.Common.Orders;
using FreshCart.Common.Persistence;
using FreshCart.IRepository.Orders;
using FreshCart.IService.Orders;
using System.Data;

namespace FreshCart.Service.Orders;

public sealed class RefundService(
    IRefundRepository refunds,
    IOrderRepository orders,
    IUnitOfWork unitOfWork,
    IIdGenerator idGenerator) : IRefundService
{
    public async Task<RefundRecord> RequestAsync(Guid orderId, RefundRequest request, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        if (request.Amount <= 0 || string.IsNullOrWhiteSpace(request.Reason))
            throw new ValidationAppException();
        var amount = decimal.Round(request.Amount, 2, MidpointRounding.AwayFromZero);
        if (amount <= 0)
            throw new ValidationAppException();

        var order = await orders.GetByIdAsync(orderId, actorUserId, cancellationToken)
            ?? throw new NotFoundAppException();
        if (order.PaymentStatus is not PaymentStatuses.Paid and not PaymentStatuses.PartiallyRefunded)
            throw new ConflictAppException();

        var payment = await orders.GetPaymentByOrderIdAsync(orderId, cancellationToken)
            ?? throw new NotFoundAppException();
        var reservedAmount = await refunds.GetReservedAmountAsync(payment.Id, cancellationToken);
        if (amount > payment.Amount - reservedAmount)
            throw new ConflictAppException();

        var refund = new RefundRecord
        {
            Id = idGenerator.NewId(),
            PaymentId = payment.Id,
            OrderId = orderId,
            Amount = amount,
            Reason = request.Reason.Trim(),
            Status = RefundStatuses.Pending
        };
        return await refunds.CreateAsync(refund, actorUserId, cancellationToken);
    }

    public async Task<IReadOnlyList<RefundRecord>> GetPendingAsync(CancellationToken cancellationToken = default)
    {
        return await refunds.GetPendingAsync(cancellationToken);
    }

    public async Task<RefundRecord> SettleAsync(Guid refundId, RefundSettlementRequest request, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        if (request.Status is not RefundStatuses.Completed and not RefundStatuses.Rejected)
            throw new ValidationAppException();
        if (request.Status == RefundStatuses.Completed && string.IsNullOrWhiteSpace(request.GatewayReference))
            throw new ValidationAppException();

        await unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        try
        {
            var existing = await refunds.GetByIdAsync(refundId, cancellationToken)
                ?? throw new NotFoundAppException();
            if (existing.Status != RefundStatuses.Pending)
                throw new ConflictAppException();

            var settled = await refunds.SettleAsync(
                refundId,
                request.Status,
                request.GatewayReference?.Trim(),
                actorUserId,
                cancellationToken)
                ?? throw new ConflictAppException();

            if (request.Status == RefundStatuses.Completed)
            {
                var payment = await orders.GetPaymentByOrderIdAsync(existing.OrderId, cancellationToken)
                    ?? throw new NotFoundAppException();
                var refundedAmount = await refunds.GetCompletedAmountAsync(existing.PaymentId, cancellationToken);
                var paymentStatus = refundedAmount >= payment.Amount
                    ? PaymentStatuses.Refunded
                    : PaymentStatuses.PartiallyRefunded;

                if (!await orders.SetPaymentStatusAsync(existing.OrderId, paymentStatus, actorUserId, cancellationToken)
                    || !await orders.SetOrderPaymentStatusAsync(existing.OrderId, paymentStatus, actorUserId, cancellationToken))
                    throw new ConflictAppException();
            }

            await unitOfWork.CommitAsync(cancellationToken);
            return settled;
        }
        catch
        {
            await unitOfWork.RollbackAsync(CancellationToken.None);
            throw;
        }
    }
}
