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

/// <summary>Returns products available to shoppers.</summary>
[ApiController]
[Route(ApiRoutes.Products)]
public sealed class ProductsController(
    IProductService products,
    IErrorLocalizer localizer,
    ILogger<ProductsController> logger) : ApiControllerBase(localizer, logger)
{
    /// <summary>Lists active products, optionally filtered by shop, category, and search text.</summary>
    /// <param name="shopId">Optional shop GUID used to limit results.</param>
    /// <param name="categoryId">Optional category GUID used to limit results.</param>
    /// <param name="search">Optional text searched against product names and descriptions.</param>
    /// <param name="cancellationToken">Cancels the catalog query.</param>
    /// <response code="200">Returns the matching active products.</response>
    /// <response code="400">A supplied filter is invalid.</response>
    /// <response code="500">The server could not load the product catalog.</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<Product>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAvailable(
        [FromQuery] Guid? shopId,
        [FromQuery] Guid? categoryId,
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await products.GetAvailableAsync(shopId, categoryId, search, cancellationToken);
            return CreateSuccessResponse(result);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (AppException exception)
        {
            return CreateErrorResponse(exception);
        }
        catch (Exception exception)
        {
            return CreateInternalErrorResponse(exception);
        }
    }

    /// <summary>Returns one active product by its GUID.</summary>
    /// <param name="productId">GUID of the requested product.</param>
    /// <param name="cancellationToken">Cancels the catalog query.</param>
    /// <response code="200">Returns the matching product.</response>
    /// <response code="400">The product ID is invalid.</response>
    /// <response code="404">No active product has the supplied ID.</response>
    /// <response code="500">The server could not load the product.</response>
    [HttpGet(ApiRoutes.ProductById)]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<Product>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById([FromRoute] Guid productId, CancellationToken cancellationToken)
    {
        try
        {
            var product = await products.GetByIdAsync(productId, cancellationToken);
            return product is null
                ? throw new FreshCart.Common.Exceptions.NotFoundAppException()
                : CreateSuccessResponse(product);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (AppException exception)
        {
            return CreateErrorResponse(exception);
        }
        catch (Exception exception)
        {
            return CreateInternalErrorResponse(exception);
        }
    }

    /// <summary>Creates a product with its initial sellable size and price.</summary>
    /// <param name="request">Shop, category, product details, and initial variant fields.</param>
    /// <param name="cancellationToken">Cancels product creation.</param>
    /// <response code="201">The product and its initial variant were created.</response>
    /// <response code="400">The product input is invalid.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="403">The caller cannot manage the requested shop.</response>
    /// <response code="404">The category or shop does not exist.</response>
    /// <response code="500">The server could not create the product.</response>
    [Authorize(Roles = AppRoles.ShopOwner + "," + AppRoles.ShopStaff + "," + AppRoles.PlatformAdmin)]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Product>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] ProductCreateRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var created = await products.CreateAsync(request, GetActorId(), cancellationToken);
            return CreatedAtAction(
                nameof(GetManagedById),
                new { productId = created.ProductId },
                CreateSuccessEnvelope(created, StatusCodes.Status201Created));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Gets a product from its manager view, including products in pending shops.</summary>
    /// <param name="productId">GUID of the product to retrieve.</param>
    /// <param name="cancellationToken">Cancels the product query.</param>
    /// <response code="200">Returns the product for its shop manager.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="403">The caller cannot manage the product's shop.</response>
    /// <response code="404">The product does not exist.</response>
    /// <response code="500">The server could not retrieve the product.</response>
    [Authorize(Roles = AppRoles.ShopOwner + "," + AppRoles.ShopStaff + "," + AppRoles.PlatformAdmin)]
    [HttpGet(ApiRoutes.ProductManageById)]
    [ProducesResponseType(typeof(ApiResponse<Product>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetManagedById([FromRoute] Guid productId, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await products.GetManagedByIdAsync(productId, GetActorId(), cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Updates product name, category, description, and active state.</summary>
    /// <param name="productId">GUID of the product to update.</param>
    /// <param name="request">Updated product fields.</param>
    /// <param name="cancellationToken">Cancels product update.</param>
    /// <response code="200">The product was updated.</response>
    /// <response code="400">The product input is invalid.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="403">The caller cannot manage the product's shop.</response>
    /// <response code="404">The product or category does not exist.</response>
    /// <response code="500">The server could not update the product.</response>
    [Authorize(Roles = AppRoles.ShopOwner + "," + AppRoles.ShopStaff + "," + AppRoles.PlatformAdmin)]
    [HttpPut(ApiRoutes.ProductById)]
    [ProducesResponseType(typeof(ApiResponse<Product>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update([FromRoute] Guid productId, [FromBody] ProductUpdateRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await products.UpdateAsync(productId, request, GetActorId(), cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Soft-deletes a product from the active catalog.</summary>
    /// <param name="productId">GUID of the product to deactivate.</param>
    /// <param name="cancellationToken">Cancels product deletion.</param>
    /// <response code="200">The product was soft-deleted.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="403">The caller cannot manage the product's shop.</response>
    /// <response code="404">The product does not exist.</response>
    /// <response code="500">The server could not delete the product.</response>
    [Authorize(Roles = AppRoles.ShopOwner + "," + AppRoles.ShopStaff + "," + AppRoles.PlatformAdmin)]
    [HttpDelete(ApiRoutes.ProductById)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([FromRoute] Guid productId, CancellationToken cancellationToken)
    {
        try
        {
            await products.DeleteAsync(productId, GetActorId(), cancellationToken);
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
