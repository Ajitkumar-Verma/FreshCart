using FreshCart.Common.Constants;
using FreshCart.Common.Contracts;
using FreshCart.Common.Exceptions;
using FreshCart.Common.Localization;
using FreshCart.Common.Marketplace;
using FreshCart.IService.Marketplace;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreshCart.Api.Controllers;

/// <summary>Finds delivery coverage for a customer postal code.</summary>
[ApiController]
[Route(ApiRoutes.DeliveryZonesPublic)]
public sealed class DeliveryZonesController(
    IDeliveryConfigurationService delivery,
    IErrorLocalizer localizer,
    ILogger<DeliveryZonesController> logger) : ApiControllerBase(localizer, logger)
{
    /// <summary>Lists approved shop zones that serve the supplied postal code.</summary>
    /// <param name="postalCode">Postal code to check.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="200">Returns matching delivery zones.</response>
    /// <response code="400">Postal code is invalid.</response>
    /// <response code="500">Coverage could not be retrieved.</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<DeliveryZone>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByPostalCode([FromQuery] string postalCode, CancellationToken cancellationToken)
    {
        try
        {
            return CreateSuccessResponse(await delivery.GetZonesByPostalCodeAsync(postalCode, cancellationToken));
        }
        catch (OperationCanceledException) { throw; }
        catch (AppException exception) { return CreateErrorResponse(exception); }
        catch (Exception exception) { return CreateInternalErrorResponse(exception); }
    }
}
