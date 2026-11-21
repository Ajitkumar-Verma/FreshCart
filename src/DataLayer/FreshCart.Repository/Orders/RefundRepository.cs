using Dapper;
using FreshCart.Common.Constants;
using FreshCart.Common.Orders;
using FreshCart.Common.Persistence;
using FreshCart.IRepository.Orders;
using System.Data;

namespace FreshCart.Repository.Orders;

public sealed class RefundRepository(IUnitOfWork unitOfWork) : IRefundRepository
{
    public async Task<RefundRecord?> GetByIdAsync(Guid refundId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Refunds.GetById,
                    parameters: new { RefundId = refundId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<RefundRecord>(command);
            }, cancellationToken);
    }

    public async Task<IReadOnlyList<RefundRecord>> GetPendingAsync(CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Refunds.GetPending,
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return (await connection.QueryAsync<RefundRecord>(command)).AsList();
            }, cancellationToken);
    }

    public async Task<decimal> GetReservedAmountAsync(Guid paymentId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Refunds.GetReservedAmount,
                    parameters: new { PaymentId = paymentId, RejectedStatus = RefundStatuses.Rejected },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleAsync<decimal>(command);
            }, cancellationToken);
    }

    public async Task<decimal> GetCompletedAmountAsync(Guid paymentId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Refunds.GetCompletedAmount,
                    parameters: new { PaymentId = paymentId, CompletedStatus = RefundStatuses.Completed },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleAsync<decimal>(command);
            }, cancellationToken);
    }

    public async Task<RefundRecord> CreateAsync(RefundRecord refund, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Refunds.Create,
                    parameters: new
                    {
                        refund.Id,
                        refund.PaymentId,
                        refund.Amount,
                        refund.Reason,
                        refund.Status,
                        PendingStatus = RefundStatuses.Pending,
                        CreatedBy = actorUserId
                    },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleAsync<RefundRecord>(command);
            }, cancellationToken);
    }

    public async Task<RefundRecord?> SettleAsync(Guid refundId, byte status, string? gatewayReference, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Refunds.Settle,
                    parameters: new
                    {
                        RefundId = refundId,
                        Status = status,
                        PendingStatus = RefundStatuses.Pending,
                        GatewayReference = gatewayReference,
                        UpdatedBy = actorUserId
                    },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<RefundRecord>(command);
            }, cancellationToken);
    }
}
