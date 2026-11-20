using FreshCart.Common.Contracts;
using FreshCart.Common.Exceptions;
using FreshCart.Common.Localization;
using FreshCart.Common.Constants;
using Microsoft.AspNetCore.Mvc;

namespace FreshCart.Api.Controllers;

public abstract class ApiControllerBase : ControllerBase
{
    private readonly IErrorLocalizer _localizer;
    private readonly ILogger _logger;

    protected ApiControllerBase(IErrorLocalizer localizer, ILogger logger)
    {
        _localizer = localizer;
        _logger = logger;
    }

    protected IActionResult CreateErrorResponse(AppException exception)
    {
        var language = Request.Headers[HttpHeaderNames.AcceptLanguage].FirstOrDefault();
        var response = new ApiErrorResponse(
            exception.ErrorCode,
            _localizer.GetMessage(exception.ErrorCode, language),
            HttpContext.TraceIdentifier,
            Request.Path);

        return StatusCode(exception.StatusCode, response);
    }

    protected IActionResult CreateInternalErrorResponse(Exception exception)
    {
        _logger.LogError(exception, "Unhandled controller error. TraceId: {TraceId}", HttpContext.TraceIdentifier);
        var language = Request.Headers[HttpHeaderNames.AcceptLanguage].FirstOrDefault();
        var response = new ApiErrorResponse(
            AppErrorCodes.InternalError,
            _localizer.GetMessage(AppErrorCodes.InternalError, language),
            HttpContext.TraceIdentifier,
            Request.Path);

        return StatusCode(StatusCodes.Status500InternalServerError, response);
    }
}
