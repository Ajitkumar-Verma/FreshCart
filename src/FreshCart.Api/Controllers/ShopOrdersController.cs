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

/// <summary>Allows shop staff to view and fulfill orders assigned to their shop.</summary>
[ApiController]
[Authorize(Roles = AppRoles.ShopOwner + "," + AppRoles.ShopStaff + "," + AppRoles.PlatformAdmin)]
[Route(ApiRoutes.ShopOrders)]
public sealed class ShopOrdersController(
    IOrderService orders,
    IErrorLocalizer localizer,
    ILogger<ShopOrdersController> logger) : ApiControllerBase(localizer, logger)
{
    /// <summary>Lists the orders assigned to the selected shop.</summary>
    /// <param name="shopId">Shop GUID.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="200">Returns shop orders.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="403">The caller cannot manage this shop.</response>
    /// <response code="500">Shop orders could not be retrieved.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ShopOrderSummary>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByShop([FromRoute] Guid shopId, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await orders.GetShopOrdersAsync(shopId, GetActorId(), cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Updates fulfillment state for one shop order.</summary>
    /// <param name="shopId">Shop GUID.</param>
    /// <param name="shopOrderId">Shop order GUID.</param>
    /// <param name="request">Next status and optional note.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="200">The shop order status changed.</response>
    /// <response code="400">The status request is invalid.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="403">The caller cannot manage this shop.</response>
    /// <response code="404">The shop order does not exist.</response>
    /// <response code="409">The requested transition is not allowed.</response>
    /// <response code="500">The status could not be changed.</response>
    [HttpPut(ApiRoutes.ShopOrderStatus)]
    [ProducesResponseType(typeof(ApiResponse<ShopOrderSummary>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateStatus([FromRoute] Guid shopId, [FromRoute] Guid shopOrderId, [FromBody] OrderStatusRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await orders.UpdateShopOrderStatusAsync(shopId, shopOrderId, request, GetActorId(), cancellationToken));
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
