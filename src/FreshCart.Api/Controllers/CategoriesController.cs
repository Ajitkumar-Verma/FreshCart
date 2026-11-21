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

/// <summary>Provides public category browsing and administrator category management.</summary>
[ApiController]
[Route(ApiRoutes.Categories)]
public sealed class CategoriesController(
    ICategoryService categories,
    IErrorLocalizer localizer,
    ILogger<CategoriesController> logger) : ApiControllerBase(localizer, logger)
{
    /// <summary>Returns active categories in display order.</summary>
    /// <param name="cancellationToken">Cancels the category query.</param>
    /// <response code="200">Returns active categories.</response>
    /// <response code="500">The server could not retrieve categories.</response>
    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<Category>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await categories.GetAllAsync(cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Returns one active category by identifier.</summary>
    /// <param name="categoryId">GUID of the category to retrieve.</param>
    /// <param name="cancellationToken">Cancels the category query.</param>
    /// <response code="200">Returns the requested category.</response>
    /// <response code="404">No active category has the supplied identifier.</response>
    /// <response code="500">The server could not retrieve the category.</response>
    [AllowAnonymous]
    [HttpGet(ApiRoutes.CategoryById)]
    [ProducesResponseType(typeof(ApiResponse<Category>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById([FromRoute] Guid categoryId, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await categories.GetByIdAsync(categoryId, cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Creates a category for the catalog.</summary>
    /// <param name="request">The category name, slug, optional parent, visibility, and display order.</param>
    /// <param name="cancellationToken">Cancels the category create request.</param>
    /// <response code="201">The category was created.</response>
    /// <response code="400">The category input or parent is invalid.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="403">The caller is not a platform administrator.</response>
    /// <response code="409">The category slug is already in use.</response>
    /// <response code="500">The server could not create the category.</response>
    [Authorize(Roles = AppRoles.PlatformAdmin)]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Category>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CategoryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var created = await categories.CreateAsync(request, GetActorId(), cancellationToken);
            return CreatedAtAction(
                nameof(GetById),
                new { categoryId = created.Id },
                CreateSuccessEnvelope(created, StatusCodes.Status201Created));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Updates an existing category.</summary>
    /// <param name="categoryId">GUID of the category to update.</param>
    /// <param name="request">The updated category fields.</param>
    /// <param name="cancellationToken">Cancels the category update request.</param>
    /// <response code="200">The category was updated.</response>
    /// <response code="400">The category input or parent is invalid.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="403">The caller is not a platform administrator.</response>
    /// <response code="404">The category does not exist.</response>
    /// <response code="409">The category slug is already in use.</response>
    /// <response code="500">The server could not update the category.</response>
    [Authorize(Roles = AppRoles.PlatformAdmin)]
    [HttpPut(ApiRoutes.CategoryById)]
    [ProducesResponseType(typeof(ApiResponse<Category>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update([FromRoute] Guid categoryId, [FromBody] CategoryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await categories.UpdateAsync(categoryId, request, GetActorId(), cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Soft-deletes a category without removing historical data.</summary>
    /// <param name="categoryId">GUID of the category to deactivate.</param>
    /// <param name="cancellationToken">Cancels the category delete request.</param>
    /// <response code="200">The category was soft-deleted.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="403">The caller is not a platform administrator.</response>
    /// <response code="404">The category does not exist.</response>
    /// <response code="500">The server could not delete the category.</response>
    [Authorize(Roles = AppRoles.PlatformAdmin)]
    [HttpDelete(ApiRoutes.CategoryById)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([FromRoute] Guid categoryId, CancellationToken cancellationToken)
    {
        try
        {
            await categories.DeleteAsync(categoryId, GetActorId(), cancellationToken);
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
