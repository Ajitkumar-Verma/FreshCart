using FreshCart.Common.Constants;
using FreshCart.Common.Contracts;
using FreshCart.Common.Exceptions;
using FreshCart.Common.Localization;
using FreshCart.Common.Shopping;
using FreshCart.IService.Shopping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FreshCart.Api.Controllers;

/// <summary>Validates coupons for customers and manages coupon records.</summary>
[ApiController]
[Route(ApiRoutes.Coupons)]
public sealed class CouponsController(
    ICouponService coupons,
    IErrorLocalizer localizer,
    ILogger<CouponsController> logger) : ApiControllerBase(localizer, logger)
{
    /// <summary>Lists coupon records for platform administration.</summary>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="200">Returns active coupons.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="403">Platform administrator role is required.</response>
    /// <response code="500">Coupons could not be retrieved.</response>
    [HttpGet]
    [Authorize(Roles = AppRoles.PlatformAdmin)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<Coupon>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await coupons.GetAllAsync(cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Validates a coupon for a subtotal and optional shop.</summary>
    /// <param name="code">Coupon code.</param>
    /// <param name="subtotal">Current order subtotal.</param>
    /// <param name="shopId">Optional applicable shop GUID.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="200">Coupon can be used for this subtotal.</response>
    /// <response code="400">Subtotal or code is invalid.</response>
    /// <response code="404">Coupon was not found.</response>
    /// <response code="409">Coupon is expired, inactive, below minimum, or for another shop.</response>
    /// <response code="500">Coupon could not be validated.</response>
    [HttpGet(ApiRoutes.CouponValidate)]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<Coupon>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Validate([FromQuery] string code, [FromQuery] decimal subtotal, [FromQuery] Guid? shopId, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await coupons.ValidateAsync(code, subtotal, shopId, cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Creates a platform coupon or a shop coupon for a managed shop.</summary>
    /// <param name="request">Coupon rules and optional shop scope.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="201">Coupon created.</response>
    /// <response code="400">Coupon rules are invalid.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="403">The caller cannot manage the requested coupon scope.</response>
    /// <response code="409">Coupon code already exists.</response>
    /// <response code="500">Coupon could not be created.</response>
    [HttpPost]
    [Authorize(Roles = AppRoles.ShopOwner + "," + AppRoles.PlatformAdmin)]
    [ProducesResponseType(typeof(ApiResponse<Coupon>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CouponRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var created = await coupons.CreateAsync(request, GetActorId(), User.IsInRole(AppRoles.PlatformAdmin), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, CreateSuccessEnvelope(created, StatusCodes.Status201Created));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Updates a coupon record.</summary>
    /// <param name="couponId">Coupon GUID.</param>
    /// <param name="request">Updated coupon rules.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="200">Coupon updated.</response>
    /// <response code="400">Coupon rules are invalid.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="403">The caller cannot manage this coupon.</response>
    /// <response code="404">Coupon was not found.</response>
    /// <response code="409">Coupon code already exists.</response>
    /// <response code="500">Coupon could not be updated.</response>
    [HttpPut(ApiRoutes.CouponById)]
    [Authorize(Roles = AppRoles.ShopOwner + "," + AppRoles.PlatformAdmin)]
    [ProducesResponseType(typeof(ApiResponse<Coupon>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update([FromRoute] Guid couponId, [FromBody] CouponRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await coupons.UpdateAsync(couponId, request, GetActorId(), User.IsInRole(AppRoles.PlatformAdmin), cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Soft-deletes a coupon.</summary>
    /// <param name="couponId">Coupon GUID.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="200">Coupon deleted.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="403">The caller cannot manage this coupon.</response>
    /// <response code="404">Coupon was not found.</response>
    /// <response code="500">Coupon could not be deleted.</response>
    [HttpDelete(ApiRoutes.CouponById)]
    [Authorize(Roles = AppRoles.ShopOwner + "," + AppRoles.PlatformAdmin)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([FromRoute] Guid couponId, CancellationToken cancellationToken)
    {
        try
        {
            await coupons.DeleteAsync(couponId, GetActorId(), User.IsInRole(AppRoles.PlatformAdmin), cancellationToken);
            return CreateSuccessResponse(true);
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
