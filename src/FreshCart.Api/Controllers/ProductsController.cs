using FreshCart.Common.Catalog;
using FreshCart.Common.Constants;
using FreshCart.Common.Contracts;
using FreshCart.Common.Exceptions;
using FreshCart.Common.Localization;
using FreshCart.IService.Catalog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreshCart.Api.Controllers;

/// <summary>Returns products available to shoppers.</summary>
[AllowAnonymous]
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
}
