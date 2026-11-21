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

/// <summary>Manages shop locations and exposes active approved branches for delivery selection.</summary>
[ApiController]
[Route(ApiRoutes.ShopBranches)]
public sealed class ShopBranchesController(
    IShopBranchService branches,
    IErrorLocalizer localizer,
    ILogger<ShopBranchesController> logger) : ApiControllerBase(localizer, logger)
{
    /// <summary>Lists active branches for an approved shop.</summary>
    /// <param name="shopId">GUID of the shop.</param>
    /// <param name="cancellationToken">Cancels the branch query.</param>
    /// <response code="200">Returns active public branches.</response>
    /// <response code="500">The server could not retrieve branches.</response>
    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ShopBranch>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPublic([FromRoute] Guid shopId, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await branches.GetPublicAsync(shopId, cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Gets a branch for a user who manages its shop.</summary>
    /// <param name="shopId">GUID of the shop.</param>
    /// <param name="branchId">GUID of the branch.</param>
    /// <param name="cancellationToken">Cancels the branch query.</param>
    /// <response code="200">Returns the branch.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="403">The caller cannot manage this shop.</response>
    /// <response code="404">The branch does not exist.</response>
    /// <response code="500">The server could not retrieve the branch.</response>
    [Authorize(Roles = AppRoles.ShopOwner + "," + AppRoles.ShopStaff + "," + AppRoles.PlatformAdmin)]
    [HttpGet(ApiRoutes.ShopBranchById)]
    [ProducesResponseType(typeof(ApiResponse<ShopBranch>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById([FromRoute] Guid shopId, [FromRoute] Guid branchId, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await branches.GetByIdAsync(shopId, branchId, GetActorId(), cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Creates a branch in a shop managed by the caller.</summary>
    /// <param name="shopId">GUID of the shop receiving the branch.</param>
    /// <param name="request">Branch name, address, postal code, and optional coordinates.</param>
    /// <param name="cancellationToken">Cancels branch creation.</param>
    /// <response code="201">The branch was created.</response>
    /// <response code="400">The branch fields are invalid.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="403">The caller cannot manage this shop.</response>
    /// <response code="500">The server could not create the branch.</response>
    [Authorize(Roles = AppRoles.ShopOwner + "," + AppRoles.ShopStaff + "," + AppRoles.PlatformAdmin)]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ShopBranch>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromRoute] Guid shopId, [FromBody] ShopBranchRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var created = await branches.CreateAsync(shopId, request, GetActorId(), cancellationToken);
            return CreatedAtAction(
                nameof(GetById),
                new { shopId, branchId = created.Id },
                CreateSuccessEnvelope(created, StatusCodes.Status201Created));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Updates a branch managed by the caller.</summary>
    /// <param name="shopId">GUID of the shop.</param>
    /// <param name="branchId">GUID of the branch.</param>
    /// <param name="request">Updated branch fields.</param>
    /// <param name="cancellationToken">Cancels the update.</param>
    /// <response code="200">The branch was updated.</response>
    /// <response code="400">The branch fields are invalid.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="403">The caller cannot manage this shop.</response>
    /// <response code="404">The branch does not exist.</response>
    /// <response code="500">The server could not update the branch.</response>
    [Authorize(Roles = AppRoles.ShopOwner + "," + AppRoles.ShopStaff + "," + AppRoles.PlatformAdmin)]
    [HttpPut(ApiRoutes.ShopBranchById)]
    [ProducesResponseType(typeof(ApiResponse<ShopBranch>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update([FromRoute] Guid shopId, [FromRoute] Guid branchId, [FromBody] ShopBranchRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await branches.UpdateAsync(shopId, branchId, request, GetActorId(), cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Soft-deletes a branch to preserve inventory and order history.</summary>
    /// <param name="shopId">GUID of the shop.</param>
    /// <param name="branchId">GUID of the branch to deactivate.</param>
    /// <param name="cancellationToken">Cancels deletion.</param>
    /// <response code="200">The branch was soft-deleted.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="403">The caller cannot manage this shop.</response>
    /// <response code="404">The branch does not exist.</response>
    /// <response code="500">The server could not delete the branch.</response>
    [Authorize(Roles = AppRoles.ShopOwner + "," + AppRoles.ShopStaff + "," + AppRoles.PlatformAdmin)]
    [HttpDelete(ApiRoutes.ShopBranchById)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([FromRoute] Guid shopId, [FromRoute] Guid branchId, CancellationToken cancellationToken)
    {
        try
        {
            await branches.DeleteAsync(shopId, branchId, GetActorId(), cancellationToken);
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
