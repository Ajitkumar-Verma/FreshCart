using FreshCart.Common.Constants;
using FreshCart.Common.Contracts;
using FreshCart.Common.Exceptions;
using FreshCart.Common.Localization;
using FreshCart.Common.Notifications;
using FreshCart.IService.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FreshCart.Api.Controllers;

/// <summary>Manages notifications for the authenticated user.</summary>
[ApiController]
[Authorize]
[Route(ApiRoutes.Notifications)]
public sealed class NotificationsController(
    INotificationService notifications,
    IErrorLocalizer localizer,
    ILogger<NotificationsController> logger) : ApiControllerBase(localizer, logger)
{
    /// <summary>Lists notifications owned by the current user.</summary>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="200">Returns the user's active notifications.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="500">Notifications could not be retrieved.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<UserNotification>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMine(CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await notifications.GetMineAsync(GetActorId(), cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Marks an owned notification as read.</summary>
    /// <param name="notificationId">Notification GUID.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="200">Notification marked as read.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="404">An owned notification was not found.</response>
    /// <response code="500">Notification could not be updated.</response>
    [HttpPut(ApiRoutes.NotificationRead)]
    [ProducesResponseType(typeof(ApiResponse<UserNotification>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> MarkRead([FromRoute] Guid notificationId, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await notifications.MarkReadAsync(notificationId, GetActorId(), cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Soft-deletes an owned notification.</summary>
    /// <param name="notificationId">Notification GUID.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="200">Notification deleted.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="404">An owned notification was not found.</response>
    /// <response code="500">Notification could not be deleted.</response>
    [HttpDelete(ApiRoutes.NotificationById)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([FromRoute] Guid notificationId, CancellationToken cancellationToken)
    {
        try
        {
            await notifications.DeleteAsync(notificationId, GetActorId(), cancellationToken);
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
