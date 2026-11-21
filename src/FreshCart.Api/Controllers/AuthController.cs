using FreshCart.Common.Auth;
using FreshCart.Common.Constants;
using FreshCart.Common.Contracts;
using FreshCart.Common.Exceptions;
using FreshCart.Common.Localization;
using FreshCart.IService.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreshCart.Api.Controllers;

/// <summary>Provides public registration and login endpoints.</summary>
[ApiController]
[Route(ApiRoutes.Auth)]
public sealed class AuthController(
    IAuthService authService,
    IErrorLocalizer localizer,
    ILogger<AuthController> logger) : ApiControllerBase(localizer, logger)
{
    /// <summary>Registers a customer account and returns a JWT access token.</summary>
    /// <param name="request">The email, password, display name, and optional phone number for the new account.</param>
    /// <param name="cancellationToken">Cancels the registration request.</param>
    /// <response code="200">The account was created and an access token was issued.</response>
    /// <response code="400">The request fields failed validation.</response>
    /// <response code="409">An account already uses this email address.</response>
    /// <response code="500">The server could not complete registration.</response>
    [AllowAnonymous]
    [HttpPost(ApiRoutes.AuthRegister)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await authService.RegisterAsync(request, cancellationToken));
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

    /// <summary>Authenticates a customer and returns a JWT access token.</summary>
    /// <param name="request">The email address and password to verify.</param>
    /// <param name="cancellationToken">Cancels the login request.</param>
    /// <response code="200">The credentials are valid and a token was issued.</response>
    /// <response code="400">The request fields failed validation.</response>
    /// <response code="401">The email or password is invalid.</response>
    /// <response code="500">The server could not complete login.</response>
    [AllowAnonymous]
    [HttpPost(ApiRoutes.AuthLogin)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await authService.LoginAsync(request, cancellationToken));
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
