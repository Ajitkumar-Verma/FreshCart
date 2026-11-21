using FreshCart.Common.Constants;
using FreshCart.Common.Contracts;
using FreshCart.Common.Exceptions;
using FreshCart.Common.Localization;
using FreshCart.Common.Marketplace;
using FreshCart.IService.Marketplace;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FreshCart.Api.Controllers;

/// <summary>Manages shop delivery coverage and recurring slot templates.</summary>
[ApiController]
[Authorize(Roles = AppRoles.ShopOwner + "," + AppRoles.ShopStaff + "," + AppRoles.PlatformAdmin)]
[Route(ApiRoutes.ShopBranchRoute)]
public sealed class DeliveryConfigurationController(
    IDeliveryConfigurationService delivery,
    IErrorLocalizer localizer,
    ILogger<DeliveryConfigurationController> logger) : ApiControllerBase(localizer, logger)
{
    /// <summary>Lists delivery zones configured for a managed branch.</summary>
    /// <param name="shopId">Shop GUID.</param>
    /// <param name="branchId">Branch GUID.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="200">Returns branch zones.</response>
    /// <response code="401">Authentication required.</response>
    /// <response code="403">Shop access denied.</response>
    /// <response code="404">Branch not found.</response>
    [HttpGet(ApiRoutes.BranchDeliveryZones)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<DeliveryZone>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetZones(Guid shopId, Guid branchId, CancellationToken cancellationToken)
    {
        try { return CreateSuccessResponse(await delivery.GetZonesForManagementAsync(shopId, branchId, GetActorId(), cancellationToken)); }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Creates a delivery zone for a managed branch.</summary>
    /// <param name="shopId">Shop GUID.</param>
    /// <param name="branchId">Branch GUID.</param>
    /// <param name="request">Zone name, postal code, fee, and minimum order.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="201">Zone created.</response>
    /// <response code="400">Zone data invalid.</response>
    /// <response code="401">Authentication required.</response>
    /// <response code="403">Shop access denied.</response>
    /// <response code="404">Branch not found.</response>
    [HttpPost(ApiRoutes.BranchDeliveryZones)]
    [ProducesResponseType(typeof(ApiResponse<DeliveryZone>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateZone(Guid shopId, Guid branchId, [FromBody] DeliveryZoneRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var created = await delivery.CreateZoneAsync(shopId, branchId, request, GetActorId(), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, CreateSuccessEnvelope(created, StatusCodes.Status201Created));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Updates a delivery zone.</summary>
    /// <param name="shopId">Shop GUID.</param>
    /// <param name="branchId">Branch GUID.</param>
    /// <param name="zoneId">Zone GUID.</param>
    /// <param name="request">Updated zone details.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="200">Zone updated.</response>
    /// <response code="400">Zone data invalid.</response>
    /// <response code="401">Authentication required.</response>
    /// <response code="403">Shop access denied.</response>
    /// <response code="404">Zone not found.</response>
    [HttpPut(ApiRoutes.BranchDeliveryZoneById)]
    [ProducesResponseType(typeof(ApiResponse<DeliveryZone>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateZone(Guid shopId, Guid branchId, Guid zoneId, [FromBody] DeliveryZoneRequest request, CancellationToken cancellationToken)
    {
        try { return CreateSuccessResponse(await delivery.UpdateZoneAsync(shopId, branchId, zoneId, request, GetActorId(), cancellationToken)); }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Soft-deletes a delivery zone.</summary>
    /// <param name="shopId">Shop GUID.</param>
    /// <param name="branchId">Branch GUID.</param>
    /// <param name="zoneId">Zone GUID.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="200">Zone deleted.</response>
    /// <response code="401">Authentication required.</response>
    /// <response code="403">Shop access denied.</response>
    /// <response code="404">Zone not found.</response>
    [HttpDelete(ApiRoutes.BranchDeliveryZoneById)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteZone(Guid shopId, Guid branchId, Guid zoneId, CancellationToken cancellationToken)
    {
        try { await delivery.DeleteZoneAsync(shopId, branchId, zoneId, GetActorId(), cancellationToken); return CreateSuccessResponse(true); }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Lists recurring delivery slot templates for a branch.</summary>
    /// <param name="shopId">Shop GUID.</param>
    /// <param name="branchId">Branch GUID.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="200">Returns active slot templates.</response>
    /// <response code="401">Authentication required.</response>
    /// <response code="403">Shop access denied.</response>
    /// <response code="404">Branch not found.</response>
    [HttpGet(ApiRoutes.BranchDeliverySlots)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<DeliverySlot>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSlots(Guid shopId, Guid branchId, CancellationToken cancellationToken)
    {
        try { return CreateSuccessResponse(await delivery.GetSlotsAsync(shopId, branchId, GetActorId(), cancellationToken)); }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Creates a recurring slot template for a branch.</summary>
    /// <param name="shopId">Shop GUID.</param>
    /// <param name="branchId">Branch GUID.</param>
    /// <param name="request">Start/end times and slot capacity.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="201">Slot template created.</response>
    /// <response code="400">Slot data invalid.</response>
    /// <response code="401">Authentication required.</response>
    /// <response code="403">Shop access denied.</response>
    /// <response code="404">Branch not found.</response>
    [HttpPost(ApiRoutes.BranchDeliverySlots)]
    [ProducesResponseType(typeof(ApiResponse<DeliverySlot>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateSlot(Guid shopId, Guid branchId, [FromBody] DeliverySlotRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var created = await delivery.CreateSlotAsync(shopId, branchId, request, GetActorId(), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, CreateSuccessEnvelope(created, StatusCodes.Status201Created));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Updates a recurring delivery slot template.</summary>
    /// <param name="shopId">Shop GUID.</param>
    /// <param name="branchId">Branch GUID.</param>
    /// <param name="slotId">Delivery slot GUID.</param>
    /// <param name="request">Updated slot details.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="200">Slot updated.</response>
    /// <response code="400">Slot data invalid.</response>
    /// <response code="401">Authentication required.</response>
    /// <response code="403">Shop access denied.</response>
    /// <response code="404">Slot not found.</response>
    [HttpPut(ApiRoutes.BranchDeliverySlotById)]
    [ProducesResponseType(typeof(ApiResponse<DeliverySlot>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateSlot(Guid shopId, Guid branchId, Guid slotId, [FromBody] DeliverySlotRequest request, CancellationToken cancellationToken)
    {
        try { return CreateSuccessResponse(await delivery.UpdateSlotAsync(shopId, branchId, slotId, request, GetActorId(), cancellationToken)); }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }

    /// <summary>Soft-deletes a recurring delivery slot template.</summary>
    /// <param name="shopId">Shop GUID.</param>
    /// <param name="branchId">Branch GUID.</param>
    /// <param name="slotId">Delivery slot GUID.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="200">Slot deleted.</response>
    /// <response code="401">Authentication required.</response>
    /// <response code="403">Shop access denied.</response>
    /// <response code="404">Slot not found.</response>
    [HttpDelete(ApiRoutes.BranchDeliverySlotById)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteSlot(Guid shopId, Guid branchId, Guid slotId, CancellationToken cancellationToken)
    {
        try { await delivery.DeleteSlotAsync(shopId, branchId, slotId, GetActorId(), cancellationToken); return CreateSuccessResponse(true); }
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

