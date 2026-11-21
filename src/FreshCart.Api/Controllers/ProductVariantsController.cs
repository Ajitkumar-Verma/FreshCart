using FreshCart.Common.Catalog;
using FreshCart.Common.Constants;
using FreshCart.Common.Contracts;
using FreshCart.Common.Exceptions;
using FreshCart.Common.Localization;
using FreshCart.IService.Catalog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FreshCart.Api.Controllers;

/// <summary>Lists and manages the sellable size and price variants of products.</summary>
[ApiController]
[Route(ApiRoutes.ProductVariants)]
public sealed class ProductVariantsController(
    IProductService products,
    IErrorLocalizer localizer,
    ILogger<ProductVariantsController> logger) : ApiControllerBase(localizer, logger)
{
    /// <summary>Lists active variants for an approved product.</summary>
    /// <param name="productId">GUID of the product.</param>
    /// <param name="cancellationToken">Cancels the variant query.</param>
    /// <response code="200">Returns active sale variants.</response>
    /// <response code="404">The product is not available.</response>
    /// <response code="500">The server could not retrieve variants.</response>
    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ProductVariant>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll([FromRoute] Guid productId, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await products.GetVariantsAsync(productId, cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Adds a size and price option to a product.</summary>
    /// <param name="productId">GUID of the product that receives the variant.</param>
    /// <param name="request">SKU, sale unit, quantity, price, currency, and default selection.</param>
    /// <param name="cancellationToken">Cancels variant creation.</param>
    /// <response code="201">The variant was created.</response>
    /// <response code="400">The variant fields are invalid.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="403">The caller cannot manage this product's shop.</response>
    /// <response code="404">The product does not exist.</response>
    /// <response code="500">The server could not create the variant.</response>
    [Authorize(Roles = AppRoles.ShopOwner + "," + AppRoles.ShopStaff + "," + AppRoles.PlatformAdmin)]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProductVariant>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromRoute] Guid productId, [FromBody] ProductVariantRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var created = await products.CreateVariantAsync(productId, request, GetActorId(), cancellationToken);
            return CreatedAtAction(
                nameof(GetById),
                new { productId, variantId = created.Id },
                CreateSuccessEnvelope(created, StatusCodes.Status201Created));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Gets a variant from the manager view for its parent product.</summary>
    /// <param name="productId">GUID of the parent product.</param>
    /// <param name="variantId">GUID of the variant.</param>
    /// <param name="cancellationToken">Cancels the variant query.</param>
    /// <response code="200">Returns the requested variant.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="403">The caller cannot manage the product's shop.</response>
    /// <response code="404">The product variant does not exist.</response>
    /// <response code="500">The server could not retrieve the variant.</response>
    [Authorize(Roles = AppRoles.ShopOwner + "," + AppRoles.ShopStaff + "," + AppRoles.PlatformAdmin)]
    [HttpGet(ApiRoutes.ProductVariantById)]
    [ProducesResponseType(typeof(ApiResponse<ProductVariant>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById([FromRoute] Guid productId, [FromRoute] Guid variantId, CancellationToken cancellationToken)
    {
        try
        {
            var variant = await products.GetVariantForManagementAsync(variantId, GetActorId(), cancellationToken);
            if (variant.ProductId != productId) throw new NotFoundAppException();
            return CreateSuccessResponse(variant);
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Updates a product variant and records any price change.</summary>
    /// <param name="productId">GUID of the parent product.</param>
    /// <param name="variantId">GUID of the variant to update.</param>
    /// <param name="request">Updated SKU, sale unit, quantity, price, currency, and active state.</param>
    /// <param name="cancellationToken">Cancels the update request.</param>
    /// <response code="200">The variant was updated.</response>
    /// <response code="400">The variant fields are invalid.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="403">The caller cannot manage the product's shop.</response>
    /// <response code="404">The product variant does not exist.</response>
    /// <response code="409">The existing default variant cannot be unset without selecting a replacement.</response>
    /// <response code="500">The server could not update the variant.</response>
    [Authorize(Roles = AppRoles.ShopOwner + "," + AppRoles.ShopStaff + "," + AppRoles.PlatformAdmin)]
    [HttpPut(ApiRoutes.ProductVariantById)]
    [ProducesResponseType(typeof(ApiResponse<ProductVariant>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update([FromRoute] Guid productId, [FromRoute] Guid variantId, [FromBody] ProductVariantRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var existing = await products.GetVariantForManagementAsync(variantId, GetActorId(), cancellationToken);
            if (existing.ProductId != productId) throw new NotFoundAppException();
            return CreateSuccessResponse(await products.UpdateVariantAsync(variantId, request, GetActorId(), cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Soft-deletes a non-default product variant.</summary>
    /// <param name="productId">GUID of the parent product.</param>
    /// <param name="variantId">GUID of the variant to remove.</param>
    /// <param name="cancellationToken">Cancels the delete request.</param>
    /// <response code="200">The variant was soft-deleted.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="403">The caller cannot manage the product's shop.</response>
    /// <response code="404">The variant does not exist.</response>
    /// <response code="409">The default variant cannot be deleted.</response>
    /// <response code="500">The server could not delete the variant.</response>
    [Authorize(Roles = AppRoles.ShopOwner + "," + AppRoles.ShopStaff + "," + AppRoles.PlatformAdmin)]
    [HttpDelete(ApiRoutes.ProductVariantById)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([FromRoute] Guid productId, [FromRoute] Guid variantId, CancellationToken cancellationToken)
    {
        try
        {
            var existing = await products.GetVariantForManagementAsync(variantId, GetActorId(), cancellationToken);
            if (existing.ProductId != productId) throw new NotFoundAppException();
            await products.DeleteVariantAsync(variantId, GetActorId(), cancellationToken);
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
