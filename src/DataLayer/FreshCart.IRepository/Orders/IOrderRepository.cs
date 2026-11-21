using FreshCart.Common.Orders;

namespace FreshCart.IRepository.Orders;

public interface IOrderRepository
{
    /// <summary>Returns orders created by one customer.</summary>
    Task<IReadOnlyList<OrderSummary>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Creates the customer order header.</summary>
    Task<OrderSummary> CreateAsync(OrderCreateCommand order, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Creates one shop order under the customer order.</summary>
    Task<Guid> CreateShopOrderAsync(ShopOrderCreateCommand shopOrder, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Creates one immutable order item snapshot.</summary>
    Task CreateItemAsync(OrderItemCreateCommand item, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Creates the pending payment record selected at checkout.</summary>
    Task CreatePaymentAsync(PaymentCreateCommand payment, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Returns the payment record for an order.</summary>
    Task<PaymentRecord?> GetPaymentByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);

    /// <summary>Marks a pending COD payment received and records the external receipt reference.</summary>
    Task<PaymentRecord?> ConfirmCashPaymentAsync(Guid orderId, string receiptReference, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Records coupon use for one successfully created order.</summary>
    Task CreateCouponUsageAsync(CouponUsageCreateCommand usage, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Finds an order belonging to a customer or platform administration scope.</summary>
    Task<OrderSummary?> GetByIdAsync(Guid orderId, Guid? userId, CancellationToken cancellationToken = default);

    /// <summary>Lists orders for one shop.</summary>
    Task<IReadOnlyList<ShopOrderSummary>> GetShopOrdersAsync(Guid shopId, CancellationToken cancellationToken = default);

    /// <summary>Lists child shop orders for one customer order.</summary>
    Task<IReadOnlyList<ShopOrderSummary>> GetShopOrdersByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);

    /// <summary>Returns branch and variant quantities to restore if a customer cancels their order.</summary>
    Task<IReadOnlyList<OrderStockLine>> GetStockLinesByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);

    /// <summary>Returns branch and variant quantities to restore if a shop order is cancelled.</summary>
    Task<IReadOnlyList<OrderStockLine>> GetStockLinesByShopOrderIdAsync(Guid shopOrderId, CancellationToken cancellationToken = default);

    /// <summary>Finds the shop order row and its parent order for authorization and transitions.</summary>
    Task<ShopOrderSummary?> GetShopOrderAsync(Guid shopOrderId, CancellationToken cancellationToken = default);

    /// <summary>Sets the status on one shop order.</summary>
    Task<bool> UpdateShopOrderStatusAsync(Guid shopOrderId, byte status, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Sets the parent order status and captures its audit note.</summary>
    Task<bool> SetOrderStatusAsync(Guid orderId, byte status, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Sets the payment status for all active payment records of an order.</summary>
    Task<bool> SetPaymentStatusAsync(Guid orderId, byte status, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Sets the payment status on the parent customer order.</summary>
    Task<bool> SetOrderPaymentStatusAsync(Guid orderId, byte status, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Inserts a status history record for one customer or shop order transition.</summary>
    Task CreateStatusHistoryAsync(Guid historyId, Guid orderId, Guid? shopOrderId, byte status, string? note, Guid actorUserId, CancellationToken cancellationToken = default);
}
