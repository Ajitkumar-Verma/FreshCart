using Dapper;
using FreshCart.Common.Constants;
using FreshCart.Common.Orders;
using FreshCart.Common.Persistence;
using FreshCart.IRepository.Orders;
using System.Data;

namespace FreshCart.Repository.Orders;

public sealed class OrderRepository(IUnitOfWork unitOfWork) : IOrderRepository
{
    public async Task<OrderSummary> CreateAsync(OrderCreateCommand order, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Orders.Create,
                    parameters: new
                    {
                        order.Id,
                        order.OrderNumber,
                        order.UserId,
                        order.AddressId,
                        order.OrderStatus,
                        order.PaymentStatus,
                        order.Subtotal,
                        order.DeliveryFee,
                        order.DiscountAmount,
                        order.GrandTotal,
                        order.CurrencyCode,
                        CreatedBy = actorUserId
                    },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleAsync<OrderSummary>(command);
            }, cancellationToken);
    }

    public async Task<Guid> CreateShopOrderAsync(ShopOrderCreateCommand shopOrder, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Orders.CreateShopOrder,
                    parameters: new
                    {
                        shopOrder.Id,
                        shopOrder.OrderId,
                        shopOrder.ShopId,
                        ShopBranchId = shopOrder.BranchId,
                        shopOrder.DeliverySlotId,
                        shopOrder.DeliveryFee,
                        CreatedBy = actorUserId
                    },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleAsync<Guid>(command);
            }, cancellationToken);
    }

    public async Task CreateItemAsync(OrderItemCreateCommand item, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Orders.CreateItem,
                    parameters: new
                    {
                        item.Id,
                        item.ShopOrderId,
                        item.ProductVariantId,
                        item.ProductName,
                        item.UnitName,
                        item.Quantity,
                        item.UnitPrice,
                        item.LineTotal,
                        CreatedBy = actorUserId
                    },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }, cancellationToken);
    }

    public async Task CreatePaymentAsync(PaymentCreateCommand payment, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Orders.CreatePayment,
                    parameters: new
                    {
                        payment.Id,
                        payment.OrderId,
                        payment.Method,
                        payment.Status,
                        payment.Amount,
                        CreatedBy = actorUserId
                    },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }, cancellationToken);
    }

    public async Task<PaymentRecord?> GetPaymentByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Orders.GetPaymentByOrderId,
                    parameters: new { OrderId = orderId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<PaymentRecord>(command);
            }, cancellationToken);
    }

    public async Task<PaymentRecord?> ConfirmCashPaymentAsync(Guid orderId, string receiptReference, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Orders.ConfirmCashPayment,
                    parameters: new
                    {
                        OrderId = orderId,
                        ReceiptReference = receiptReference,
                        CashOnDeliveryMethod = PaymentMethods.CashOnDelivery,
                        PaidStatus = PaymentStatuses.Paid,
                        PendingStatus = PaymentStatuses.Pending,
                        UpdatedBy = actorUserId
                    },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<PaymentRecord>(command);
            }, cancellationToken);
    }

    public async Task CreateCouponUsageAsync(CouponUsageCreateCommand usage, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Orders.CreateCouponUsage,
                    parameters: new
                    {
                        usage.Id,
                        usage.CouponId,
                        usage.OrderId,
                        usage.UserId,
                        usage.DiscountAmount,
                        CreatedBy = actorUserId
                    },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }, cancellationToken);
    }

    public async Task<IReadOnlyList<OrderSummary>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Orders.GetByUser,
                    parameters: new { UserId = userId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return (await connection.QueryAsync<OrderSummary>(command)).AsList();
            }, cancellationToken);
    }

    public async Task<OrderSummary?> GetByIdAsync(Guid orderId, Guid? userId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Orders.GetById,
                    parameters: new { OrderId = orderId, UserId = userId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<OrderSummary>(command);
            }, cancellationToken);
    }

    public async Task<IReadOnlyList<ShopOrderSummary>> GetShopOrdersAsync(Guid shopId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Orders.GetShopOrders,
                    parameters: new { ShopId = shopId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return (await connection.QueryAsync<ShopOrderSummary>(command)).AsList();
            }, cancellationToken);
    }

    public async Task<IReadOnlyList<ShopOrderSummary>> GetShopOrdersByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Orders.GetShopOrdersByOrderId,
                    parameters: new { OrderId = orderId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return (await connection.QueryAsync<ShopOrderSummary>(command)).AsList();
            }, cancellationToken);
    }

    public async Task<IReadOnlyList<OrderStockLine>> GetStockLinesByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Orders.GetStockLinesByOrderId,
                    parameters: new { OrderId = orderId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return (await connection.QueryAsync<OrderStockLine>(command)).AsList();
            }, cancellationToken);
    }

    public async Task<IReadOnlyList<OrderStockLine>> GetStockLinesByShopOrderIdAsync(Guid shopOrderId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Orders.GetStockLinesByShopOrderId,
                    parameters: new { ShopOrderId = shopOrderId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return (await connection.QueryAsync<OrderStockLine>(command)).AsList();
            }, cancellationToken);
    }

    public async Task<ShopOrderSummary?> GetShopOrderAsync(Guid shopOrderId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Orders.GetShopId,
                    parameters: new { ShopOrderId = shopOrderId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<ShopOrderSummary>(command);
            }, cancellationToken);
    }

    public async Task<bool> UpdateShopOrderStatusAsync(Guid shopOrderId, byte status, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Orders.UpdateShopStatus,
                    parameters: new { ShopOrderId = shopOrderId, Status = status, UpdatedBy = actorUserId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleAsync<bool>(command);
            }, cancellationToken);
    }

    public async Task<bool> SetOrderStatusAsync(Guid orderId, byte status, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Orders.SetOrderStatus,
                    parameters: new { OrderId = orderId, Status = status, UpdatedBy = actorUserId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleAsync<bool>(command);
            }, cancellationToken);
    }

    public async Task<bool> SetPaymentStatusAsync(Guid orderId, byte status, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Orders.SetPaymentStatus,
                    parameters: new { OrderId = orderId, Status = status, UpdatedBy = actorUserId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleAsync<bool>(command);
            }, cancellationToken);
    }

    public async Task<bool> SetOrderPaymentStatusAsync(Guid orderId, byte status, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Orders.SetOrderPaymentStatus,
                    parameters: new { OrderId = orderId, Status = status, UpdatedBy = actorUserId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleAsync<bool>(command);
            }, cancellationToken);
    }

    public async Task CreateStatusHistoryAsync(Guid historyId, Guid orderId, Guid? shopOrderId, byte status, string? note, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Orders.AddStatusHistory,
                    parameters: new { Id = historyId, OrderId = orderId, ShopOrderId = shopOrderId, Status = status, Note = note, ChangedByUserId = actorUserId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }, cancellationToken);
    }
}
