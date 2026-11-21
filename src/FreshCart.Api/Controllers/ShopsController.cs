using FreshCart.Common.Constants;
using FreshCart.Common.Contracts;
using FreshCart.Common.Exceptions;
using FreshCart.Common.Localization;
using FreshCart.Common.Marketplace;
using FreshCart.IService.Marketplace;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FreshCart.Api.Controllers;

/// <summary>Provides storefront shop discovery and shop-owner administration.</summary>
[ApiController]
[Route(ApiRoutes.Shops)]
public sealed class ShopsController(
    IShopService shops,
    IErrorLocalizer localizer,
    ILogger<ShopsController> logger) : ApiControllerBase(localizer, logger)
{
    /// <summary>Lists active approved shops.</summary>
    /// <param name="cancellationToken">Cancels the shop query.</param>
    /// <response code="200">Returns approved shops.</response>
    /// <response code="500">The server could not retrieve shops.</response>
    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<Shop>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetApproved(CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await shops.GetApprovedAsync(cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Lists shops the authenticated user owns or manages.</summary>
    /// <param name="cancellationToken">Cancels the shop query.</param>
    /// <response code="200">Returns the user's managed shops.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="500">The server could not retrieve shops.</response>
    [Authorize(Roles = AppRoles.ShopOwner + "," + AppRoles.ShopStaff + "," + AppRoles.PlatformAdmin)]
    [HttpGet(ApiRoutes.ShopsMine)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<Shop>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetManaged(CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await shops.GetManagedAsync(GetActorId(), cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Gets a shop managed by the authenticated user.</summary>
    /// <param name="shopId">GUID of the shop to retrieve.</param>
    /// <param name="cancellationToken">Cancels the shop query.</param>
    /// <response code="200">Returns the requested shop.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="403">The caller cannot manage this shop.</response>
    /// <response code="404">The shop does not exist.</response>
    /// <response code="500">The server could not retrieve the shop.</response>
    [Authorize(Roles = AppRoles.ShopOwner + "," + AppRoles.ShopStaff + "," + AppRoles.PlatformAdmin)]
    [HttpGet(ApiRoutes.ShopById)]
    [ProducesResponseType(typeof(ApiResponse<Shop>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById([FromRoute] Guid shopId, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await shops.GetManagedByIdAsync(shopId, GetActorId(), cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Creates a shop that requires platform approval before appearing in storefronts.</summary>
    /// <param name="request">Shop name, URL slug, and optional description.</param>
    /// <param name="cancellationToken">Cancels shop creation.</param>
    /// <response code="201">The shop was created in pending approval state.</response>
    /// <response code="400">The request is invalid.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="403">The caller is not permitted to create a shop.</response>
    /// <response code="409">The shop slug is already in use.</response>
    /// <response code="500">The server could not create the shop.</response>
    [Authorize(Roles = AppRoles.ShopOwner + "," + AppRoles.PlatformAdmin)]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Shop>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] ShopRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var created = await shops.CreateAsync(request, GetActorId(), cancellationToken);
            return CreatedAtAction(
                nameof(GetById),
                new { shopId = created.Id },
                CreateSuccessEnvelope(created, StatusCodes.Status201Created));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Updates the profile of a shop managed by the authenticated user.</summary>
    /// <param name="shopId">GUID of the shop to update.</param>
    /// <param name="request">The updated shop fields.</param>
    /// <param name="cancellationToken">Cancels shop update.</param>
    /// <response code="200">The shop was updated.</response>
    /// <response code="400">The request is invalid.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="403">The caller cannot manage this shop.</response>
    /// <response code="404">The shop does not exist.</response>
    /// <response code="409">The shop slug is already in use.</response>
    /// <response code="500">The server could not update the shop.</response>
    [Authorize(Roles = AppRoles.ShopOwner + "," + AppRoles.ShopStaff + "," + AppRoles.PlatformAdmin)]
    [HttpPut(ApiRoutes.ShopById)]
    [ProducesResponseType(typeof(ApiResponse<Shop>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update([FromRoute] Guid shopId, [FromBody] ShopRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await shops.UpdateAsync(shopId, request, GetActorId(), cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Approves, suspends, or returns a shop to pending approval.</summary>
    /// <param name="shopId">GUID of the shop whose status is changing.</param>
    /// <param name="request">The desired shop status.</param>
    /// <param name="cancellationToken">Cancels the status update.</param>
    /// <response code="200">The status was updated.</response>
    /// <response code="400">The requested status is invalid.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="403">The caller is not a platform administrator.</response>
    /// <response code="404">The shop does not exist.</response>
    /// <response code="500">The server could not update the status.</response>
    [Authorize(Roles = AppRoles.PlatformAdmin)]
    [HttpPut(ApiRoutes.ShopStatus)]
    [ProducesResponseType(typeof(ApiResponse<Shop>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SetStatus([FromRoute] Guid shopId, [FromBody] ShopStatusRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await shops.SetStatusAsync(shopId, request.Status, GetActorId(), cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Soft-deletes a shop managed by the authenticated user.</summary>
    /// <param name="shopId">GUID of the shop to deactivate.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    /// <response code="200">The shop was soft-deleted.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="403">The caller cannot manage this shop.</response>
    /// <response code="404">The shop does not exist.</response>
    /// <response code="500">The server could not delete the shop.</response>
    [Authorize(Roles = AppRoles.ShopOwner + "," + AppRoles.ShopStaff + "," + AppRoles.PlatformAdmin)]
    [HttpDelete(ApiRoutes.ShopById)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([FromRoute] Guid shopId, CancellationToken cancellationToken)
    {
        try
        {
            await shops.DeleteAsync(shopId, GetActorId(), cancellationToken);
            return CreateSuccessResponse(true);
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    private Guid GetActorId()
    {
        var subject = User.FindFirstValue(TokenClaimNames.Subject);
        return Guid.TryParse(subject, out var actorId)
            ? actorId
            : throw new UnauthorizedAppException();
    }
}
