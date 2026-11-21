using FreshCart.Common.Constants;
using FreshCart.Common.Contracts;
using FreshCart.Common.Exceptions;
using FreshCart.Common.Localization;
using FreshCart.Common.Reviews;
using FreshCart.IService.Reviews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FreshCart.Api.Controllers;

/// <summary>Reads product reviews and handles customer and moderator review actions.</summary>
[ApiController]
[Route(ApiRoutes.ProductReviews)]
public sealed class ReviewsController(
    IReviewService reviews,
    IErrorLocalizer localizer,
    ILogger<ReviewsController> logger) : ApiControllerBase(localizer, logger)
{
    /// <summary>Lists public reviews for a product.</summary>
    /// <param name="productId">Product GUID.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="200">Returns published reviews.</response>
    /// <response code="500">Reviews could not be retrieved.</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ProductReview>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByProduct([FromRoute] Guid productId, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await reviews.GetByProductAsync(productId, cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Creates or updates a review for an item from a delivered order.</summary>
    /// <param name="productId">Product GUID.</param>
    /// <param name="orderItemId">Purchased order item GUID.</param>
    /// <param name="request">Rating and optional review comment.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="200">Review saved.</response>
    /// <response code="400">Rating or comment is invalid.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="403">The order item was not purchased by this user or was not delivered.</response>
    /// <response code="500">The review could not be saved.</response>
    [HttpPut(ApiRoutes.ProductReviewByOrderItem)]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<ProductReview>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Submit([FromRoute] Guid productId, [FromRoute] Guid orderItemId, [FromBody] ProductReviewRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await reviews.SubmitAsync(productId, orderItemId, request, GetActorId(), cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Changes review visibility for platform moderation.</summary>
    /// <param name="reviewId">Review GUID.</param>
    /// <param name="isPublished">Whether the review is visible publicly.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="200">Review visibility updated.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="403">Platform administrator role is required.</response>
    /// <response code="404">Review was not found.</response>
    /// <response code="500">Review could not be moderated.</response>
    [HttpPatch(ApiRoutes.ReviewModeration)]
    [Authorize(Roles = AppRoles.PlatformAdmin)]
    [ProducesResponseType(typeof(ApiResponse<ProductReview>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SetPublished([FromRoute] Guid reviewId, [FromQuery] bool isPublished, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await reviews.SetPublishedAsync(reviewId, isPublished, GetActorId(), cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Soft-deletes the authenticated customer's review.</summary>
    /// <param name="reviewId">Review GUID.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="200">Review deleted.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="404">An owned review was not found.</response>
    /// <response code="500">Review could not be deleted.</response>
    [HttpDelete(ApiRoutes.ReviewMineById)]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteMine([FromRoute] Guid reviewId, CancellationToken cancellationToken)
    {
        try
        {
            await reviews.DeleteMineAsync(reviewId, GetActorId(), cancellationToken);
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
