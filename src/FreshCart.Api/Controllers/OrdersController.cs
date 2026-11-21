using FreshCart.Common.Constants;
using FreshCart.Common.Contracts;
using FreshCart.Common.Exceptions;
using FreshCart.Common.Localization;
using FreshCart.Common.Orders;
using FreshCart.IService.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FreshCart.Api.Controllers;

/// <summary>Provides customer order history and cancellation workflows.</summary>
[ApiController]
[Authorize]
[Route(ApiRoutes.Orders)]
public sealed class OrdersController(
    IOrderService orders,
    IErrorLocalizer localizer,
    ILogger<OrdersController> logger) : ApiControllerBase(localizer, logger)
{
    /// <summary>Gets payment status for an order owned by the current customer or visible to a platform administrator.</summary>
    /// <param name="orderId">Order GUID.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="200">Returns payment method, status, amount, and receipt reference.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="403">The current customer does not own the order.</response>
    /// <response code="404">Order or payment was not found.</response>
    /// <response code="500">Payment status could not be retrieved.</response>
    [HttpGet(ApiRoutes.OrderPayment)]
    [ProducesResponseType(typeof(ApiResponse<PaymentRecord>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPayment([FromRoute] Guid orderId, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await orders.GetPaymentAsync(orderId, GetActorId(), User.IsInRole(AppRoles.PlatformAdmin), cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Records externally verified cash-on-delivery collection after an order is delivered.</summary>
    /// <param name="orderId">Order GUID.</param>
    /// <param name="request">Receipt identifier recorded by the platform administrator.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="200">Payment reconciled as paid.</response>
    /// <response code="400">Receipt reference is invalid.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="403">Platform administrator role is required.</response>
    /// <response code="404">Order or payment was not found.</response>
    /// <response code="409">The order is not delivered or payment is no longer pending COD.</response>
    /// <response code="500">Payment could not be reconciled.</response>
    [HttpPost(ApiRoutes.OrderCashPaymentConfirm)]
    [Authorize(Roles = AppRoles.PlatformAdmin)]
    [ProducesResponseType(typeof(ApiResponse<PaymentRecord>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ConfirmCashPayment([FromRoute] Guid orderId, [FromBody] CashPaymentConfirmationRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await orders.ConfirmCashPaymentAsync(orderId, request, GetActorId(), cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Creates an order from the authenticated customer's cart.</summary>
    /// <param name="request">Address, one branch and slot per shop, optional global coupon, and COD payment method.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="201">Order created with server-calculated totals and stock deducted.</response>
    /// <response code="400">Checkout details or payment method are invalid.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="404">The delivery address does not exist for this customer.</response>
    /// <response code="409">Cart, stock, delivery coverage, coupon, or slot is no longer valid.</response>
    /// <response code="500">The order could not be created.</response>
    [HttpPost(ApiRoutes.Checkout)]
    [ProducesResponseType(typeof(ApiResponse<OrderSummary>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var created = await orders.CheckoutAsync(request, GetActorId(), cancellationToken);
            return CreatedAtAction(
                nameof(GetById),
                new { orderId = created.Id },
                CreateSuccessEnvelope(created, StatusCodes.Status201Created));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Lists orders placed by the authenticated customer.</summary>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="200">Returns the customer's orders.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="500">Orders could not be retrieved.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<OrderSummary>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMine(CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await orders.GetMineAsync(GetActorId(), cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Gets an order owned by the current customer or visible to a platform administrator.</summary>
    /// <param name="orderId">Order GUID.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="200">Returns the order.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="403">The current customer does not own the order.</response>
    /// <response code="404">The order does not exist.</response>
    /// <response code="500">The order could not be retrieved.</response>
    [HttpGet(ApiRoutes.OrderById)]
    [ProducesResponseType(typeof(ApiResponse<OrderSummary>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById([FromRoute] Guid orderId, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await orders.GetByIdAsync(orderId, GetActorId(), User.IsInRole(AppRoles.PlatformAdmin), cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Cancels an order while it is new or confirmed.</summary>
    /// <param name="orderId">Order GUID.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="200">The order was cancelled.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="404">The order does not exist for this customer.</response>
    /// <response code="409">The order is too far along to cancel.</response>
    /// <response code="500">The order could not be cancelled.</response>
    [HttpPost(ApiRoutes.OrderById + "/cancel")]
    [ProducesResponseType(typeof(ApiResponse<OrderSummary>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Cancel([FromRoute] Guid orderId, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await orders.CancelAsync(orderId, GetActorId(), cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    private Guid GetActorId()
    {
        var subject = User.FindFirstValue(TokenClaimNames.Subject);
        return Guid.TryParse(subject, out var actorId) ? actorId : throw new UnauthorizedAppException();
    }
}
