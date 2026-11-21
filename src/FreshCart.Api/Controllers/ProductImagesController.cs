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

/// <summary>Manages image URL metadata for products.</summary>
[ApiController]
[Route(ApiRoutes.ProductImages)]
public sealed class ProductImagesController(
    IProductImageService images,
    IErrorLocalizer localizer,
    ILogger<ProductImagesController> logger) : ApiControllerBase(localizer, logger)
{
    /// <summary>Lists visible image metadata for a public product.</summary>
    /// <param name="productId">Product GUID.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="200">Returns product images.</response>
    /// <response code="404">Product not found.</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ProductImage>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByProduct(Guid productId, CancellationToken cancellationToken)
    {
        try { return CreateSuccessResponse(await images.GetByProductAsync(productId, cancellationToken)); }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Adds image URL metadata to a managed product.</summary>
    /// <param name="productId">Product GUID.</param>
    /// <param name="request">Image URL, alt text, and display order.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="201">Image metadata created.</response>
    /// <response code="400">Image metadata invalid.</response>
    /// <response code="401">Authentication required.</response>
    /// <response code="403">Product access denied.</response>
    /// <response code="404">Product not found.</response>
    [HttpPost]
    [Authorize(Roles = AppRoles.ShopOwner + "," + AppRoles.ShopStaff + "," + AppRoles.PlatformAdmin)]
    [ProducesResponseType(typeof(ApiResponse<ProductImage>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(Guid productId, [FromBody] ProductImageRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var created = await images.CreateAsync(productId, request, GetActorId(), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, CreateSuccessEnvelope(created, StatusCodes.Status201Created));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Updates image URL metadata for a managed product.</summary>
    /// <param name="productId">Product GUID.</param>
    /// <param name="imageId">Image GUID.</param>
    /// <param name="request">Updated image URL and display details.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="200">Image updated.</response>
    /// <response code="400">Image metadata invalid.</response>
    /// <response code="401">Authentication required.</response>
    /// <response code="403">Product access denied.</response>
    /// <response code="404">Product image not found.</response>
    [HttpPut(ApiRoutes.ProductImageById)]
    [Authorize(Roles = AppRoles.ShopOwner + "," + AppRoles.ShopStaff + "," + AppRoles.PlatformAdmin)]
    [ProducesResponseType(typeof(ApiResponse<ProductImage>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(Guid productId, Guid imageId, [FromBody] ProductImageRequest request, CancellationToken cancellationToken)
    {
        try { return CreateSuccessResponse(await images.UpdateAsync(productId, imageId, request, GetActorId(), cancellationToken)); }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Soft-deletes product image URL metadata.</summary>
    /// <param name="productId">Product GUID.</param>
    /// <param name="imageId">Image GUID.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="200">Image deleted.</response>
    /// <response code="401">Authentication required.</response>
    /// <response code="403">Product access denied.</response>
    /// <response code="404">Product image not found.</response>
    [HttpDelete(ApiRoutes.ProductImageById)]
    [Authorize(Roles = AppRoles.ShopOwner + "," + AppRoles.ShopStaff + "," + AppRoles.PlatformAdmin)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(Guid productId, Guid imageId, CancellationToken cancellationToken)
    {
        try { await images.DeleteAsync(productId, imageId, GetActorId(), cancellationToken); return CreateSuccessResponse(true); }
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
