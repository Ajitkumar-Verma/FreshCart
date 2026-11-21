using FreshCart.Common.Constants;
using FreshCart.Common.Contracts;
using FreshCart.Common.Exceptions;
using FreshCart.Common.Inventory;
using FreshCart.IService.Inventory;
using FreshCart.Common.Localization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FreshCart.Api.Controllers;

/// <summary>Manages shop branch stock levels and movement records.</summary>
[ApiController]
[Authorize(Roles = AppRoles.ShopOwner + "," + AppRoles.ShopStaff + "," + AppRoles.PlatformAdmin)]
[Route(ApiRoutes.ShopInventory)]
public sealed class InventoryController(
    IInventoryService inventory,
    IErrorLocalizer localizer,
    ILogger<InventoryController> logger) : ApiControllerBase(localizer, logger)
{
    /// <summary>Lists current inventory for a shop's branches.</summary>
    /// <param name="shopId">Shop GUID.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="200">Returns inventory rows.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="403">The caller cannot manage the shop.</response>
    /// <response code="500">The inventory could not be read.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<InventoryItem>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByShop([FromRoute] Guid shopId, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await inventory.GetByShopAsync(shopId, GetActorId(), cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Creates an empty stock row for a branch and product variant.</summary>
    /// <param name="shopId">Owning shop GUID.</param>
    /// <param name="branchId">Branch GUID.</param>
    /// <param name="productVariantId">Variant GUID.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="201">Inventory row created.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="403">The caller cannot manage the shop.</response>
    /// <response code="404">The branch or variant does not exist in this shop.</response>
    /// <response code="409">Inventory already exists for this branch and variant.</response>
    /// <response code="500">The inventory row could not be created.</response>
    [HttpPost(ApiRoutes.InventoryCreateForVariant)]
    [ProducesResponseType(typeof(ApiResponse<InventoryItem>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromRoute] Guid shopId, [FromRoute] Guid branchId, [FromRoute] Guid productVariantId, CancellationToken cancellationToken)
    {
        try
        {
            var created = await inventory.CreateAsync(shopId, branchId, productVariantId, GetActorId(), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, CreateSuccessEnvelope(created, StatusCodes.Status201Created));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Applies a signed stock adjustment and records a stock movement; stock cannot fall below zero.</summary>
    /// <param name="shopId">Owning shop GUID.</param>
    /// <param name="inventoryId">Inventory row GUID.</param>
    /// <param name="request">Positive stock change, reason, and optional notes.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="200">Stock and movement were updated.</response>
    /// <response code="400">The adjustment is invalid.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="403">The caller cannot manage the shop.</response>
    /// <response code="404">Inventory was not found in the shop.</response>
    /// <response code="409">The adjustment conflicts with the current stock.</response>
    /// <response code="500">The stock could not be adjusted.</response>
    [HttpPost(ApiRoutes.ShopInventoryAdjust)]
    [ProducesResponseType(typeof(ApiResponse<InventoryItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Adjust([FromRoute] Guid shopId, [FromRoute] Guid inventoryId, [FromBody] InventoryAdjustmentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await inventory.AdjustAsync(shopId, inventoryId, request, GetActorId(), cancellationToken));
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
