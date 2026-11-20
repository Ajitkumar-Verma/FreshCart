using FreshCart.Common.Auth;
using FreshCart.Common.Constants;
using FreshCart.Common.Contracts;
using FreshCart.Common.Exceptions;
using FreshCart.Common.Localization;
using FreshCart.IService.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreshCart.Api.Controllers;

/// <summary>Creates platform administrator accounts.</summary>
[ApiController]
[Route(ApiRoutes.SuperUsers)]
public sealed class SuperUsersController(
    IAuthService authService,
    IErrorLocalizer localizer,
    ILogger<SuperUsersController> logger) : ApiControllerBase(localizer, logger)
{
    /// <summary>Creates another platform administrator account.</summary>
    /// <param name="request">The email, password, and display name for the new administrator.</param>
    /// <param name="cancellationToken">Cancels the create request.</param>
    /// <response code="200">The administrator was created and a token was issued.</response>
    /// <response code="400">The request fields failed validation.</response>
    /// <response code="401">The caller is not authenticated.</response>
    /// <response code="403">The authenticated caller lacks the platform administrator role.</response>
    /// <response code="409">The email address is already in use.</response>
    /// <response code="500">The server could not complete the request.</response>
    [Authorize(Roles = AppRoles.PlatformAdmin)]
    [HttpPost]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateSuperUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await authService.CreatePlatformAdminAsync(request, cancellationToken));
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

    /// <summary>Creates the initial platform administrator during deployment bootstrap.</summary>
    /// <param name="request">The email, password, and display name for the initial administrator.</param>
    /// <param name="bootstrapKey">The deployment bootstrap secret supplied in the configured request header.</param>
    /// <param name="cancellationToken">Cancels the bootstrap request.</param>
    /// <response code="200">The initial administrator was created and a token was issued.</response>
    /// <response code="400">The request fields failed validation.</response>
    /// <response code="403">The bootstrap key is missing, invalid, or not configured.</response>
    /// <response code="409">Bootstrap has already been completed, or the email is in use.</response>
    /// <response code="500">The server could not complete bootstrap.</response>
    [AllowAnonymous]
    [HttpPost(ApiRoutes.BootstrapSuperUser)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Bootstrap(
        [FromBody] CreateSuperUserRequest request,
        [FromHeader(Name = HttpHeaderNames.BootstrapKey)] string? bootstrapKey,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await authService.BootstrapPlatformAdminAsync(request, bootstrapKey, cancellationToken));
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
