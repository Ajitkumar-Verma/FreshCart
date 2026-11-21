using FreshCart.Common.Orders;

namespace FreshCart.IService.Orders;

public interface IOrderService
{
    /// <summary>Lists orders belonging to the authenticated customer.</summary>
    Task<IReadOnlyList<OrderSummary>> GetMineAsync(Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Creates an order from the authenticated user's cart and server-validated delivery selection.</summary>
    Task<OrderSummary> CheckoutAsync(CheckoutRequest request, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Returns payment status after verifying order ownership or platform administration access.</summary>
    Task<PaymentRecord> GetPaymentAsync(Guid orderId, Guid actorUserId, bool isPlatformAdmin, CancellationToken cancellationToken = default);

    /// <summary>Confirms a delivered cash-on-delivery payment using an administrator-entered receipt reference.</summary>
    Task<PaymentRecord> ConfirmCashPaymentAsync(Guid orderId, CashPaymentConfirmationRequest request, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Gets one order after confirming customer or platform administrator access.</summary>
    Task<OrderSummary> GetByIdAsync(Guid orderId, Guid actorUserId, bool isPlatformAdmin, CancellationToken cancellationToken = default);

    /// <summary>Lists customer orders routed to a shop managed by the authenticated user.</summary>
    Task<IReadOnlyList<ShopOrderSummary>> GetShopOrdersAsync(Guid shopId, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Updates one shop order state and records its status history.</summary>
    Task<ShopOrderSummary> UpdateShopOrderStatusAsync(Guid shopId, Guid shopOrderId, OrderStatusRequest request, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Cancels an eligible customer order without deleting its history.</summary>
    Task<OrderSummary> CancelAsync(Guid orderId, Guid actorUserId, CancellationToken cancellationToken = default);
}
