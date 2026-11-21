using FreshCart.Common.Constants;
using FreshCart.Common.Contracts;
using FreshCart.Common.Exceptions;
using FreshCart.Common.Localization;
using FreshCart.Common.Shopping;
using FreshCart.IService.Shopping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FreshCart.Api.Controllers;

/// <summary>Manages delivery addresses for the authenticated customer.</summary>
[ApiController]
[Authorize]
[Route(ApiRoutes.Addresses)]
public sealed class AddressesController(
    IAddressService addresses,
    IErrorLocalizer localizer,
    ILogger<AddressesController> logger) : ApiControllerBase(localizer, logger)
{
    /// <summary>Lists addresses belonging to the authenticated user.</summary>
    /// <param name="cancellationToken">Cancels the address query.</param>
    /// <response code="200">Returns the user's delivery addresses.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="500">The server could not retrieve addresses.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<Address>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await addresses.GetAllAsync(GetActorId(), cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Gets one delivery address belonging to the authenticated user.</summary>
    /// <param name="addressId">GUID of the address.</param>
    /// <param name="cancellationToken">Cancels the address query.</param>
    /// <response code="200">Returns the address.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="404">The address does not exist for this user.</response>
    /// <response code="500">The server could not retrieve the address.</response>
    [HttpGet(ApiRoutes.AddressById)]
    [ProducesResponseType(typeof(ApiResponse<Address>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById([FromRoute] Guid addressId, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await addresses.GetByIdAsync(GetActorId(), addressId, cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Creates a delivery address for the authenticated user.</summary>
    /// <param name="request">Recipient and delivery location fields.</param>
    /// <param name="cancellationToken">Cancels address creation.</param>
    /// <response code="201">The address was created.</response>
    /// <response code="400">The address fields are invalid.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="500">The server could not create the address.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Address>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] AddressRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var created = await addresses.CreateAsync(GetActorId(), request, cancellationToken);
            return CreatedAtAction(
                nameof(GetById),
                new { addressId = created.Id },
                CreateSuccessEnvelope(created, StatusCodes.Status201Created));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Updates an address belonging to the authenticated user.</summary>
    /// <param name="addressId">GUID of the address to update.</param>
    /// <param name="request">Updated recipient and delivery location fields.</param>
    /// <param name="cancellationToken">Cancels address update.</param>
    /// <response code="200">The address was updated.</response>
    /// <response code="400">The address fields are invalid.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="404">The address does not exist for this user.</response>
    /// <response code="500">The server could not update the address.</response>
    [HttpPut(ApiRoutes.AddressById)]
    [ProducesResponseType(typeof(ApiResponse<Address>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update([FromRoute] Guid addressId, [FromBody] AddressRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await addresses.UpdateAsync(GetActorId(), addressId, request, cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Sets one of the user's addresses as the default delivery address.</summary>
    /// <param name="addressId">GUID of the address to make default.</param>
    /// <param name="cancellationToken">Cancels the default selection.</param>
    /// <response code="200">The default address was changed.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="404">The address does not exist for this user.</response>
    /// <response code="500">The server could not update the default address.</response>
    [HttpPut(ApiRoutes.AddressDefault)]
    [ProducesResponseType(typeof(ApiResponse<Address>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SetDefault([FromRoute] Guid addressId, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await addresses.SetDefaultAsync(GetActorId(), addressId, cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Soft-deletes an address belonging to the authenticated user.</summary>
    /// <param name="addressId">GUID of the address to remove.</param>
    /// <param name="cancellationToken">Cancels address deletion.</param>
    /// <response code="200">The address was soft-deleted.</response>
    /// <response code="401">Authentication is required.</response>
    /// <response code="404">The address does not exist for this user.</response>
    /// <response code="500">The server could not delete the address.</response>
    [HttpDelete(ApiRoutes.AddressById)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([FromRoute] Guid addressId, CancellationToken cancellationToken)
    {
        try
        {
            await addresses.DeleteAsync(GetActorId(), addressId, cancellationToken);
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
