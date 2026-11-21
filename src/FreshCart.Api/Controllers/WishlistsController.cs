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

/// <summary>Manages favorites and named wishlists for the authenticated user.</summary>
[ApiController]
[Authorize]
[Route(ApiRoutes.Wishlists)]
public sealed class WishlistsController(
    IWishlistService wishlists,
    IErrorLocalizer localizer,
    ILogger<WishlistsController> logger) : ApiControllerBase(localizer, logger)
{
    /// <summary>Lists wishlists owned by the authenticated user.</summary>
    /// <param name="cancellationToken">Cancels the wishlist query.</param>
    /// <response code="200">Returns the user's wishlists.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="500">The server could not retrieve wishlists.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<Wishlist>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await wishlists.GetAllAsync(GetActorId(), cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Creates a named wishlist.</summary>
    /// <param name="request">The wishlist name.</param>
    /// <param name="cancellationToken">Cancels wishlist creation.</param>
    /// <response code="201">The wishlist was created.</response>
    /// <response code="400">The wishlist name is invalid.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="409">The user already has a wishlist with this name.</response>
    /// <response code="500">The server could not create the wishlist.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Wishlist>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] WishlistRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var created = await wishlists.CreateAsync(GetActorId(), request, cancellationToken);
            return CreatedAtAction(
                nameof(GetById),
                new { wishlistId = created.Id },
                CreateSuccessEnvelope(created, StatusCodes.Status201Created));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Gets one wishlist owned by the authenticated user.</summary>
    /// <param name="wishlistId">GUID of the wishlist.</param>
    /// <param name="cancellationToken">Cancels the wishlist query.</param>
    /// <response code="200">Returns the wishlist.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="404">The wishlist does not exist for this user.</response>
    /// <response code="500">The server could not retrieve the wishlist.</response>
    [HttpGet(ApiRoutes.WishlistById)]
    [ProducesResponseType(typeof(ApiResponse<Wishlist>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById([FromRoute] Guid wishlistId, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await wishlists.GetByIdAsync(GetActorId(), wishlistId, cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Renames a wishlist owned by the authenticated user.</summary>
    /// <param name="wishlistId">GUID of the wishlist to rename.</param>
    /// <param name="request">The new wishlist name.</param>
    /// <param name="cancellationToken">Cancels the update.</param>
    /// <response code="200">The wishlist was renamed.</response>
    /// <response code="400">The name is invalid.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="404">The wishlist does not exist for this user.</response>
    /// <response code="409">The user already has a wishlist with this name.</response>
    /// <response code="500">The server could not update the wishlist.</response>
    [HttpPut(ApiRoutes.WishlistById)]
    [ProducesResponseType(typeof(ApiResponse<Wishlist>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update([FromRoute] Guid wishlistId, [FromBody] WishlistRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await wishlists.UpdateAsync(GetActorId(), wishlistId, request, cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Soft-deletes a wishlist owned by the authenticated user.</summary>
    /// <param name="wishlistId">GUID of the wishlist to deactivate.</param>
    /// <param name="cancellationToken">Cancels deletion.</param>
    /// <response code="200">The wishlist was soft-deleted.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="404">The wishlist does not exist for this user.</response>
    /// <response code="500">The server could not delete the wishlist.</response>
    [HttpDelete(ApiRoutes.WishlistById)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([FromRoute] Guid wishlistId, CancellationToken cancellationToken)
    {
        try
        {
            await wishlists.DeleteAsync(GetActorId(), wishlistId, cancellationToken);
            return CreateSuccessResponse(true);
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Lists currently visible products saved in the wishlist.</summary>
    /// <param name="wishlistId">GUID of the wishlist.</param>
    /// <param name="cancellationToken">Cancels the wishlist item query.</param>
    /// <response code="200">Returns saved products.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="404">The wishlist does not exist for this user.</response>
    /// <response code="500">The server could not retrieve wishlist items.</response>
    [HttpGet(ApiRoutes.WishlistItems)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<WishlistItem>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetItems([FromRoute] Guid wishlistId, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await wishlists.GetItemsAsync(GetActorId(), wishlistId, cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Adds a currently visible product to a user's wishlist.</summary>
    /// <param name="wishlistId">GUID of the wishlist.</param>
    /// <param name="productId">GUID of the product to save.</param>
    /// <param name="cancellationToken">Cancels the wishlist update.</param>
    /// <response code="200">The wishlist item was saved.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="404">The wishlist or product does not exist.</response>
    /// <response code="500">The server could not save the wishlist item.</response>
    [HttpPost(ApiRoutes.WishlistItemByProduct)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddItem([FromRoute] Guid wishlistId, [FromRoute] Guid productId, CancellationToken cancellationToken)
    {
        try
        {
            await wishlists.AddItemAsync(GetActorId(), wishlistId, productId, cancellationToken);
            return CreateSuccessResponse(true);
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Soft-deletes a product entry from a wishlist.</summary>
    /// <param name="wishlistId">GUID of the wishlist.</param>
    /// <param name="productId">GUID of the product to remove.</param>
    /// <param name="cancellationToken">Cancels the wishlist update.</param>
    /// <response code="200">The product was removed from the wishlist.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="404">The wishlist item does not exist.</response>
    /// <response code="500">The server could not remove the wishlist item.</response>
    [HttpDelete(ApiRoutes.WishlistItemByProduct)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveItem([FromRoute] Guid wishlistId, [FromRoute] Guid productId, CancellationToken cancellationToken)
    {
        try
        {
            await wishlists.RemoveItemAsync(GetActorId(), wishlistId, productId, cancellationToken);
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
