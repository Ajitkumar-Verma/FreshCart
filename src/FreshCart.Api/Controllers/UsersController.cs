using FreshCart.Common.Auth;
using FreshCart.Common.Constants;
using FreshCart.Common.Contracts;
using FreshCart.Common.Exceptions;
using FreshCart.Common.Localization;
using FreshCart.IService.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FreshCart.Api.Controllers;

/// <summary>Manages user accounts for platform administrators.</summary>
[ApiController]
[Authorize(Roles = AppRoles.PlatformAdmin)]
[Route(ApiRoutes.Users)]
public sealed class UsersController(
    IUserManagementService users,
    IErrorLocalizer localizer,
    ILogger<UsersController> logger) : ApiControllerBase(localizer, logger)
{
    /// <summary>Lists active user accounts and their roles.</summary>
    /// <param name="cancellationToken">Cancels the request.</param>
    /// <response code="200">Returns the active user accounts.</response>
    /// <response code="401">The caller is not authenticated.</response>
    /// <response code="403">The caller is not a platform administrator.</response>
    /// <response code="500">The server could not retrieve users.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ManagedUserResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await users.GetAllAsync(cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Gets one active user account.</summary>
    /// <param name="userId">GUID of the account to retrieve.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    /// <response code="200">Returns the matching user.</response>
    /// <response code="401">The caller is not authenticated.</response>
    /// <response code="403">The caller is not a platform administrator.</response>
    /// <response code="404">No active user has that identifier.</response>
    /// <response code="500">The server could not retrieve the user.</response>
    [HttpGet(ApiRoutes.UserById)]
    [ProducesResponseType(typeof(ApiResponse<ManagedUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await users.GetByIdAsync(userId, cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Creates a user account and assigns Customer by default.</summary>
    /// <param name="request">Account fields and an optional allowed role.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    /// <response code="201">The account was created.</response>
    /// <response code="400">The request is invalid or requests an unsupported role.</response>
    /// <response code="401">The caller is not authenticated.</response>
    /// <response code="403">The caller is not a platform administrator.</response>
    /// <response code="409">The email address is already registered.</response>
    /// <response code="500">The server could not create the user.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ManagedUserResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateManagedUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var created = await users.CreateAsync(request, cancellationToken);
            return CreatedAtAction(
                nameof(GetById),
                new { userId = created.Id },
                CreateSuccessEnvelope(created, StatusCodes.Status201Created));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Updates a user's email, display name, and phone number.</summary>
    /// <param name="userId">GUID of the account to update.</param>
    /// <param name="request">The updated profile values.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    /// <response code="200">The account was updated.</response>
    /// <response code="400">The request fields are invalid.</response>
    /// <response code="401">The caller is not authenticated.</response>
    /// <response code="403">The caller is not a platform administrator.</response>
    /// <response code="404">No active user has that identifier.</response>
    /// <response code="409">The email address is already registered.</response>
    /// <response code="500">The server could not update the user.</response>
    [HttpPut(ApiRoutes.UserById)]
    [ProducesResponseType(typeof(ApiResponse<ManagedUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update([FromRoute] Guid userId, [FromBody] UpdateManagedUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await users.UpdateAsync(userId, request, GetActorId(), cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Soft-deletes a user account.</summary>
    /// <param name="userId">GUID of the account to deactivate.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    /// <response code="200">The account was soft-deleted.</response>
    /// <response code="401">The caller is not authenticated.</response>
    /// <response code="403">The caller is not a platform administrator.</response>
    /// <response code="404">No active user has that identifier.</response>
    /// <response code="409">The caller cannot delete their own account, or this is the last platform administrator.</response>
    /// <response code="500">The server could not delete the user.</response>
    [HttpDelete(ApiRoutes.UserById)]
    [ProducesResponseType(typeof(ApiResponse<UserDeletionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            await users.DeleteAsync(userId, GetActorId(), cancellationToken);
            return CreateSuccessResponse(new UserDeletionResponse(userId, true));
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
