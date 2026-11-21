using FreshCart.Common.Exceptions;
using FreshCart.Common.Constants;
using FreshCart.Common.Orders;
using FreshCart.Common.Persistence;
using FreshCart.Common.Shopping;
using FreshCart.IRepository.Marketplace;
using FreshCart.IRepository.Orders;
using FreshCart.IRepository.Inventory;
using FreshCart.IRepository.Shopping;
using FreshCart.IService.Orders;
using FreshCart.IService.Shopping;
using System.Data;

namespace FreshCart.Service.Orders;

public sealed class OrderService(
    IOrderRepository orders,
    IShopRepository shops,
    ICartRepository carts,
    IAddressRepository addresses,
    IInventoryRepository inventory,
    IShopBranchRepository branches,
    IDeliveryConfigurationRepository delivery,
    ICouponService coupons,
    IUnitOfWork unitOfWork,
    IIdGenerator idGenerator) : IOrderService
{
    public async Task<IReadOnlyList<OrderSummary>> GetMineAsync(Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await orders.GetByUserAsync(actorUserId, cancellationToken);
    }

    public async Task<OrderSummary> CheckoutAsync(CheckoutRequest request, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        if (request.AddressId == Guid.Empty || request.PaymentMethod != PaymentMethods.CashOnDelivery)
            throw new ValidationAppException();

        Guid createdOrderId;
        await unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        try
        {
            var cart = await carts.GetByUserIdAsync(actorUserId, cancellationToken)
                ?? throw new ConflictAppException();
            var items = await carts.GetItemsAsync(cart.Id, cancellationToken);
            if (items.Count == 0 || items.Any(item => !item.IsAvailable || item.Quantity <= 0 || item.ShopId == Guid.Empty))
                throw new ConflictAppException();

            var address = await addresses.GetByIdAsync(actorUserId, request.AddressId, cancellationToken)
                ?? throw new NotFoundAppException();
            var groups = items.GroupBy(item => item.ShopId).ToList();
            ValidateShopSelections(groups.Select(group => group.Key), request.Shops);

            var currencyCodes = items.Select(item => item.CurrencyCode).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
            if (currencyCodes.Length != 1 || string.IsNullOrWhiteSpace(currencyCodes[0]))
                throw new ConflictAppException();

            var shopSelections = request.Shops.ToDictionary(selection => selection.ShopId);
            var checkoutLines = new List<CheckoutLine>();
            decimal subtotal = 0;
            decimal deliveryFee = 0;

            foreach (var group in groups)
            {
                var selection = shopSelections[group.Key];
                var branch = await branches.GetByIdAsync(group.Key, selection.BranchId, cancellationToken)
                    ?? throw new ValidationAppException();
                if (!branch.IsActive)
                    throw new ConflictAppException();

                var slot = await delivery.GetSlotByIdAsync(selection.BranchId, selection.DeliverySlotId, cancellationToken)
                    ?? throw new ValidationAppException();
                if (!slot.IsActive)
                    throw new ConflictAppException();

                var zone = (await delivery.GetZonesByPostalCodeAsync(address.PostalCode, cancellationToken))
                    .SingleOrDefault(candidate => candidate.ShopBranchId == selection.BranchId)
                    ?? throw new ConflictAppException();

                var shopSubtotal = group.Sum(item => RoundMoney(item.UnitPrice * item.Quantity));
                if (shopSubtotal < zone.MinimumOrder)
                    throw new ConflictAppException();

                subtotal += shopSubtotal;
                deliveryFee += zone.DeliveryFee;
                checkoutLines.Add(new CheckoutLine(group.Key, selection, zone.DeliveryFee, group.ToList()));

                foreach (var item in group)
                {
                    var stock = await inventory.GetByBranchAndVariantAsync(selection.BranchId, item.ProductVariantId, cancellationToken)
                        ?? throw new ConflictAppException();
                    if (stock.QuantityOnHand < item.Quantity)
                        throw new ConflictAppException();
                }
            }

            var discount = 0m;
            Coupon? appliedCoupon = null;
            if (!string.IsNullOrWhiteSpace(request.CouponCode))
            {
                appliedCoupon = await coupons.ValidateAsync(request.CouponCode, subtotal, null, cancellationToken);
                discount = appliedCoupon.DiscountType == DiscountTypes.Percentage
                    ? RoundMoney(subtotal * appliedCoupon.DiscountValue / 100m)
                    : appliedCoupon.DiscountValue;
                discount = Math.Min(discount, subtotal);
            }

            var grandTotal = RoundMoney(subtotal + deliveryFee - discount);
            var orderId = idGenerator.NewId();
            createdOrderId = orderId;
            var orderNumber = CreateOrderNumber(orderId);
            var order = new OrderCreateCommand
            {
                Id = orderId,
                OrderNumber = orderNumber,
                UserId = actorUserId,
                AddressId = request.AddressId,
                OrderStatus = OrderStatuses.New,
                PaymentStatus = PaymentStatuses.Pending,
                Subtotal = subtotal,
                DeliveryFee = deliveryFee,
                DiscountAmount = discount,
                GrandTotal = grandTotal,
                CurrencyCode = currencyCodes[0].ToUpperInvariant()
            };

            _ = await orders.CreateAsync(order, actorUserId, cancellationToken);
            foreach (var line in checkoutLines)
            {
                var shopOrderId = await orders.CreateShopOrderAsync(new ShopOrderCreateCommand
                {
                    Id = idGenerator.NewId(),
                    OrderId = orderId,
                    ShopId = line.ShopId,
                    BranchId = line.Selection.BranchId,
                    DeliverySlotId = line.Selection.DeliverySlotId,
                    DeliveryFee = line.DeliveryFee
                }, actorUserId, cancellationToken);

                foreach (var item in line.Items)
                {
                    var stock = await inventory.GetByBranchAndVariantAsync(line.Selection.BranchId, item.ProductVariantId, cancellationToken)
                        ?? throw new ConflictAppException();
                    if (stock.QuantityOnHand < item.Quantity)
                        throw new ConflictAppException();

                    var lineTotal = RoundMoney(item.UnitPrice * item.Quantity);
                    await orders.CreateItemAsync(new OrderItemCreateCommand
                    {
                        Id = idGenerator.NewId(),
                        ShopOrderId = shopOrderId,
                        ProductVariantId = item.ProductVariantId,
                        ProductName = item.ProductName,
                        UnitName = item.UnitName,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        LineTotal = lineTotal
                    }, actorUserId, cancellationToken);

                    if (!await inventory.UpdateQuantityAsync(stock.Id, -item.Quantity, actorUserId, cancellationToken))
                        throw new ConflictAppException();

                    await inventory.CreateMovementAsync(idGenerator.NewId(), stock.Id, -item.Quantity, InventoryMovementReasons.Sale, orderNumber, actorUserId, cancellationToken);
                    if (!await carts.SoftDeleteItemAsync(cart.Id, item.ProductVariantId, actorUserId, cancellationToken))
                        throw new ConflictAppException();
                }
            }

            await orders.CreatePaymentAsync(new PaymentCreateCommand
            {
                Id = idGenerator.NewId(),
                OrderId = orderId,
                Method = PaymentMethods.CashOnDelivery,
                Status = PaymentStatuses.Pending,
                Amount = grandTotal
            }, actorUserId, cancellationToken);

            if (appliedCoupon is not null)
            {
                await orders.CreateCouponUsageAsync(new CouponUsageCreateCommand
                {
                    Id = idGenerator.NewId(),
                    CouponId = appliedCoupon.Id,
                    OrderId = orderId,
                    UserId = actorUserId,
                    DiscountAmount = discount
                }, actorUserId, cancellationToken);
            }

            await orders.CreateStatusHistoryAsync(idGenerator.NewId(), orderId, null, OrderStatuses.New, null, actorUserId, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);
        }
        catch
        {
            await unitOfWork.RollbackAsync(CancellationToken.None);
            throw;
        }

        return await orders.GetByIdAsync(createdOrderId, actorUserId, cancellationToken) ?? throw new NotFoundAppException();
    }

    public async Task<OrderSummary> GetByIdAsync(Guid orderId, Guid actorUserId, bool isPlatformAdmin, CancellationToken cancellationToken = default)
    {
        var order = await orders.GetByIdAsync(orderId, isPlatformAdmin ? null : actorUserId, cancellationToken);
        if (order is not null)
            return order;

        if (!isPlatformAdmin)
            throw new ForbiddenAppException();

        throw new NotFoundAppException();
    }

    public async Task<PaymentRecord> GetPaymentAsync(Guid orderId, Guid actorUserId, bool isPlatformAdmin, CancellationToken cancellationToken = default)
    {
        var order = await orders.GetByIdAsync(orderId, isPlatformAdmin ? null : actorUserId, cancellationToken);
        if (order is null)
        {
            if (!isPlatformAdmin)
                throw new ForbiddenAppException();
            throw new NotFoundAppException();
        }

        return await orders.GetPaymentByOrderIdAsync(orderId, cancellationToken)
            ?? throw new NotFoundAppException();
    }

    public async Task<PaymentRecord> ConfirmCashPaymentAsync(Guid orderId, CashPaymentConfirmationRequest request, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.ReceiptReference))
            throw new ValidationAppException();

        await unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        try
        {
            var order = await orders.GetByIdAsync(orderId, null, cancellationToken)
                ?? throw new NotFoundAppException();
            if (order.OrderStatus != OrderStatuses.Delivered || order.PaymentStatus != PaymentStatuses.Pending)
                throw new ConflictAppException();

            var payment = await orders.GetPaymentByOrderIdAsync(orderId, cancellationToken)
                ?? throw new NotFoundAppException();
            if (payment.Method != PaymentMethods.CashOnDelivery || payment.Status != PaymentStatuses.Pending)
                throw new ConflictAppException();

            var updatedPayment = await orders.ConfirmCashPaymentAsync(
                orderId,
                request.ReceiptReference.Trim(),
                actorUserId,
                cancellationToken);
            if (updatedPayment is null)
                throw new ConflictAppException();
            if (!await orders.SetOrderPaymentStatusAsync(orderId, PaymentStatuses.Paid, actorUserId, cancellationToken))
                throw new ConflictAppException();

            await unitOfWork.CommitAsync(cancellationToken);
            return updatedPayment;
        }
        catch
        {
            await unitOfWork.RollbackAsync(CancellationToken.None);
            throw;
        }
    }

    public async Task<IReadOnlyList<ShopOrderSummary>> GetShopOrdersAsync(Guid shopId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        await EnsureCanManageAsync(shopId, actorUserId, cancellationToken);
        return await orders.GetShopOrdersAsync(shopId, cancellationToken);
    }

    public async Task<ShopOrderSummary> UpdateShopOrderStatusAsync(Guid shopId, Guid shopOrderId, OrderStatusRequest request, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        await EnsureCanManageAsync(shopId, actorUserId, cancellationToken);
        await unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        try
        {
            var shopOrder = await orders.GetShopOrderAsync(shopOrderId, cancellationToken)
                ?? throw new NotFoundAppException();
            if (shopOrder.ShopId != shopId)
                throw new NotFoundAppException();
            if (!IsAllowedTransition(shopOrder.Status, request.Status))
                throw new ConflictAppException();

            var changed = await orders.UpdateShopOrderStatusAsync(shopOrderId, request.Status, actorUserId, cancellationToken);
            if (!changed)
                throw new ConflictAppException();

            if (request.Status == OrderStatuses.Cancelled)
            {
                var stockLines = await orders.GetStockLinesByShopOrderIdAsync(shopOrderId, cancellationToken);
                await RestoreStockAsync(stockLines, OrderNotes.ShopOrderCancelled, actorUserId, cancellationToken);
            }

            await orders.CreateStatusHistoryAsync(idGenerator.NewId(), shopOrder.OrderId, shopOrderId, request.Status, request.Note?.Trim(), actorUserId, cancellationToken);
            await SynchronizeOrderStatusAsync(shopOrder.OrderId, request.Note?.Trim(), actorUserId, cancellationToken);

            await unitOfWork.CommitAsync(cancellationToken);
        }
        catch
        {
            await unitOfWork.RollbackAsync(CancellationToken.None);
            throw;
        }

        return await orders.GetShopOrderAsync(shopOrderId, cancellationToken) ?? throw new NotFoundAppException();
    }

    public async Task<OrderSummary> CancelAsync(Guid orderId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        await unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        try
        {
            var order = await orders.GetByIdAsync(orderId, actorUserId, cancellationToken)
                ?? throw new NotFoundAppException();
            if (order.OrderStatus is not OrderStatuses.New and not OrderStatuses.Confirmed)
                throw new ConflictAppException();

            var shopOrders = await orders.GetShopOrdersByOrderIdAsync(orderId, cancellationToken);
            if (shopOrders.Any(shopOrder => shopOrder.Status is not OrderStatuses.New and not OrderStatuses.Confirmed))
                throw new ConflictAppException();

            if (!await orders.SetOrderStatusAsync(orderId, OrderStatuses.Cancelled, actorUserId, cancellationToken))
                throw new ConflictAppException();

            foreach (var shopOrder in shopOrders)
            {
                if (!await orders.UpdateShopOrderStatusAsync(shopOrder.Id, OrderStatuses.Cancelled, actorUserId, cancellationToken))
                    throw new ConflictAppException();
                await orders.CreateStatusHistoryAsync(idGenerator.NewId(), orderId, shopOrder.Id, OrderStatuses.Cancelled, OrderNotes.CancelledByCustomer, actorUserId, cancellationToken);
            }

            if (!await orders.SetPaymentStatusAsync(orderId, PaymentStatuses.Cancelled, actorUserId, cancellationToken)
                || !await orders.SetOrderPaymentStatusAsync(orderId, PaymentStatuses.Cancelled, actorUserId, cancellationToken))
                throw new ConflictAppException();

            var stockLines = await orders.GetStockLinesByOrderIdAsync(orderId, cancellationToken);
            await RestoreStockAsync(stockLines, OrderNotes.CancelledByCustomer, actorUserId, cancellationToken);
            await orders.CreateStatusHistoryAsync(idGenerator.NewId(), orderId, null, OrderStatuses.Cancelled, OrderNotes.CancelledByCustomer, actorUserId, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);
        }
        catch
        {
            await unitOfWork.RollbackAsync(CancellationToken.None);
            throw;
        }

        return await orders.GetByIdAsync(orderId, actorUserId, cancellationToken) ?? throw new NotFoundAppException();
    }

    private async Task RestoreStockAsync(IReadOnlyList<OrderStockLine> lines, string notes, Guid actorUserId, CancellationToken cancellationToken)
    {
        foreach (var line in lines)
        {
            var stock = await inventory.GetByBranchAndVariantAsync(line.ShopBranchId, line.ProductVariantId, cancellationToken)
                ?? throw new ConflictAppException();
            if (!await inventory.UpdateQuantityAsync(stock.Id, line.Quantity, actorUserId, cancellationToken))
                throw new ConflictAppException();

            await inventory.CreateMovementAsync(
                idGenerator.NewId(),
                stock.Id,
                line.Quantity,
                InventoryMovementReasons.Cancellation,
                notes,
                actorUserId,
                cancellationToken);
        }
    }

    private async Task SynchronizeOrderStatusAsync(Guid orderId, string? note, Guid actorUserId, CancellationToken cancellationToken)
    {
        var childOrders = await orders.GetShopOrdersByOrderIdAsync(orderId, cancellationToken);
        if (childOrders.Count == 0)
            return;

        byte? desiredStatus = null;
        if (childOrders.All(childOrder => childOrder.Status == OrderStatuses.Cancelled))
        {
            desiredStatus = OrderStatuses.Cancelled;
        }
        else if (childOrders.All(childOrder => childOrder.Status is OrderStatuses.Delivered or OrderStatuses.Cancelled)
            && childOrders.Any(childOrder => childOrder.Status == OrderStatuses.Delivered))
        {
            desiredStatus = OrderStatuses.Delivered;
        }
        else
        {
            desiredStatus = childOrders
                .Where(childOrder => childOrder.Status != OrderStatuses.Cancelled)
                .Select(childOrder => childOrder.Status)
                .Max();
        }

        var customerOrder = await orders.GetByIdAsync(orderId, null, cancellationToken)
            ?? throw new NotFoundAppException();
        if (customerOrder.OrderStatus == desiredStatus)
            return;

        var mayAdvance = desiredStatus == OrderStatuses.Cancelled
            || desiredStatus == OrderStatuses.Delivered
            || GetStatusRank(desiredStatus.Value) > GetStatusRank(customerOrder.OrderStatus);
        if (!mayAdvance)
            return;

        if (!await orders.SetOrderStatusAsync(orderId, desiredStatus.Value, actorUserId, cancellationToken))
            throw new ConflictAppException();

        if (desiredStatus == OrderStatuses.Cancelled
            && (!await orders.SetPaymentStatusAsync(orderId, PaymentStatuses.Cancelled, actorUserId, cancellationToken)
                || !await orders.SetOrderPaymentStatusAsync(orderId, PaymentStatuses.Cancelled, actorUserId, cancellationToken)))
            throw new ConflictAppException();

        await orders.CreateStatusHistoryAsync(idGenerator.NewId(), orderId, null, desiredStatus.Value, note, actorUserId, cancellationToken);
    }

    private static int GetStatusRank(byte status)
    {
        return status switch
        {
            OrderStatuses.New => 0,
            OrderStatuses.Confirmed => 1,
            OrderStatuses.Preparing => 2,
            OrderStatuses.Ready => 3,
            OrderStatuses.OutForDelivery => 4,
            OrderStatuses.Delivered => 5,
            _ => -1
        };
    }

    private async Task EnsureCanManageAsync(Guid shopId, Guid actorUserId, CancellationToken cancellationToken)
    {
        if (!await shops.CanManageAsync(shopId, actorUserId, cancellationToken))
            throw new ForbiddenAppException();
    }

    private static bool IsAllowedTransition(byte current, byte next)
    {
        return (current, next) switch
        {
            (OrderStatuses.New, OrderStatuses.Confirmed) => true,
            (OrderStatuses.New, OrderStatuses.Cancelled) => true,
            (OrderStatuses.Confirmed, OrderStatuses.Preparing) => true,
            (OrderStatuses.Confirmed, OrderStatuses.Cancelled) => true,
            (OrderStatuses.Preparing, OrderStatuses.Ready) => true,
            (OrderStatuses.Ready, OrderStatuses.OutForDelivery) => true,
            (OrderStatuses.OutForDelivery, OrderStatuses.Delivered) => true,
            _ => false
        };
    }

    private static void ValidateShopSelections(IEnumerable<Guid> shopIds, IReadOnlyList<ShopCheckoutSelection> selections)
    {
        if (selections is null || selections.Count == 0 || selections.Any(selection => selection.ShopId == Guid.Empty || selection.BranchId == Guid.Empty || selection.DeliverySlotId == Guid.Empty)
            || selections.Select(selection => selection.ShopId).Distinct().Count() != selections.Count
            || !shopIds.Order().SequenceEqual(selections.Select(selection => selection.ShopId).Order()))
            throw new ValidationAppException();
    }

    private static decimal RoundMoney(decimal amount)
    {
        return decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
    }

    private static string CreateOrderNumber(Guid orderId)
    {
        return OrderNumberSettings.Prefix + orderId.ToString("N")[..OrderNumberSettings.IdentifierLength].ToUpperInvariant();
    }

    private sealed class CheckoutLine(Guid shopId, ShopCheckoutSelection selection, decimal deliveryFee, IReadOnlyList<CartItem> items)
    {
        public Guid ShopId { get; } = shopId;

        public ShopCheckoutSelection Selection { get; } = selection;

        public decimal DeliveryFee { get; } = deliveryFee;

        public IReadOnlyList<CartItem> Items { get; } = items;
    }
}
