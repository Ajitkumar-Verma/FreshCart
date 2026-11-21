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

/// <summary>Manages the authenticated customer's shopping cart.</summary>
[ApiController]
[Authorize]
[Route(ApiRoutes.Cart)]
public sealed class CartController(
    ICartService carts,
    IErrorLocalizer localizer,
    ILogger<CartController> logger) : ApiControllerBase(localizer, logger)
{
    /// <summary>Returns the authenticated user's cart and current item totals.</summary>
    /// <param name="cancellationToken">Cancels the cart query.</param>
    /// <response code="200">Returns the user's cart.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="500">The server could not retrieve the cart.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<Cart>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await carts.GetAsync(GetActorId(), cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Adds a quantity of a currently visible product variant to the cart.</summary>
    /// <param name="productVariantId">GUID of the sellable product variant.</param>
    /// <param name="request">The quantity to add.</param>
    /// <param name="cancellationToken">Cancels the cart update.</param>
    /// <response code="200">Returns the updated cart.</response>
    /// <response code="400">The quantity or variant is invalid.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="404">The product variant is not publicly available.</response>
    /// <response code="500">The server could not update the cart.</response>
    [HttpPost(ApiRoutes.CartItemByVariant)]
    [ProducesResponseType(typeof(ApiResponse<Cart>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddItem([FromRoute] Guid productVariantId, [FromBody] CartItemRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await carts.AddItemAsync(GetActorId(), productVariantId, request, cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Sets the quantity for one variant already in the cart.</summary>
    /// <param name="productVariantId">GUID of the product variant.</param>
    /// <param name="request">The replacement quantity.</param>
    /// <param name="cancellationToken">Cancels the cart update.</param>
    /// <response code="200">Returns the updated cart.</response>
    /// <response code="400">The quantity is invalid.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="404">The cart item does not exist.</response>
    /// <response code="500">The server could not update the cart.</response>
    [HttpPut(ApiRoutes.CartItemByVariant)]
    [ProducesResponseType(typeof(ApiResponse<Cart>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateItem([FromRoute] Guid productVariantId, [FromBody] CartItemRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await carts.UpdateItemAsync(GetActorId(), productVariantId, request, cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Soft-deletes a product variant from the cart.</summary>
    /// <param name="productVariantId">GUID of the product variant to remove.</param>
    /// <param name="cancellationToken">Cancels the cart update.</param>
    /// <response code="200">Returns the updated cart.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="404">The cart item does not exist.</response>
    /// <response code="500">The server could not update the cart.</response>
    [HttpDelete(ApiRoutes.CartItemByVariant)]
    [ProducesResponseType(typeof(ApiResponse<Cart>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveItem([FromRoute] Guid productVariantId, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await carts.RemoveItemAsync(GetActorId(), productVariantId, cancellationToken));
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
