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

/// <summary>Manages staff assigned to a shop.</summary>
[ApiController]
[Authorize(Roles = AppRoles.ShopOwner + "," + AppRoles.ShopStaff + "," + AppRoles.PlatformAdmin)]
[Route(ApiRoutes.ShopMemberships)]
public sealed class ShopMembershipsController(
    IShopMembershipService memberships,
    IErrorLocalizer localizer,
    ILogger<ShopMembershipsController> logger) : ApiControllerBase(localizer, logger)
{
    /// <summary>Lists active shop memberships.</summary>
    /// <param name="shopId">Shop GUID.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="200">Returns active memberships.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="403">Only the shop owner, a shop manager, or platform admin can manage staff.</response>
    /// <response code="500">Memberships could not be read.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ShopMembership>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByShop([FromRoute] Guid shopId, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await memberships.GetByShopAsync(shopId, GetActorId(), cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Adds a user as a shop staff member.</summary>
    /// <param name="shopId">Shop GUID.</param>
    /// <param name="request">User GUID and either Staff or Manager role.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="201">Membership created.</response>
    /// <response code="400">Role or user is invalid.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="403">Only the shop owner, a shop manager, or platform admin can manage staff.</response>
    /// <response code="409">The user is already assigned to this shop.</response>
    /// <response code="500">Membership could not be created.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ShopMembership>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Add([FromRoute] Guid shopId, [FromBody] ShopMembershipRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var created = await memberships.AddAsync(shopId, request, GetActorId(), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, CreateSuccessEnvelope(created, StatusCodes.Status201Created));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Changes the role of one shop member.</summary>
    /// <param name="shopId">Shop GUID.</param>
    /// <param name="membershipId">Membership GUID.</param>
    /// <param name="request">New Staff or Manager role.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="200">Membership updated.</response>
    /// <response code="400">Role is invalid.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="403">Only the shop owner, a shop manager, or platform admin can manage staff.</response>
    /// <response code="404">Membership does not exist.</response>
    /// <response code="500">Membership could not be updated.</response>
    [HttpPut(ApiRoutes.ShopMembershipById)]
    [ProducesResponseType(typeof(ApiResponse<ShopMembership>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update([FromRoute] Guid shopId, [FromRoute] Guid membershipId, [FromBody] ShopMembershipRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await memberships.UpdateAsync(shopId, membershipId, request, GetActorId(), cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Deactivates a shop member while preserving its audit history.</summary>
    /// <param name="shopId">Shop GUID.</param>
    /// <param name="membershipId">Membership GUID.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="200">Membership deactivated.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="403">Only the shop owner, a shop manager, or platform admin can manage staff.</response>
    /// <response code="404">Membership does not exist.</response>
    /// <response code="500">Membership could not be deactivated.</response>
    [HttpDelete(ApiRoutes.ShopMembershipById)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([FromRoute] Guid shopId, [FromRoute] Guid membershipId, CancellationToken cancellationToken)
    {
        try
        {
            await memberships.DeleteAsync(shopId, membershipId, GetActorId(), cancellationToken);
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
