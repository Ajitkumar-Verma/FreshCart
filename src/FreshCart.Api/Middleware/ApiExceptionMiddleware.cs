using FreshCart.Common.Localization;
using FreshCart.Common.Exceptions;
using FreshCart.Common.Contracts;

namespace FreshCart.Api.Middleware;

public sealed class ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, IErrorLocalizer localizer)
    {
        try { await next(context); }
        catch (AppException exception)
        {
            context.Response.StatusCode = exception.StatusCode;
            context.Response.ContentType = "application/json";
            var language = context.Request.Headers["Accept-Language"].FirstOrDefault();
            await context.Response.WriteAsJsonAsync(new ApiErrorResponse(exception.ErrorCode, localizer.GetMessage(exception.ErrorCode, language), context.TraceIdentifier, context.Request.Path), context.RequestAborted);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled request error. TraceId: {TraceId}", context.TraceIdentifier);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            var language = context.Request.Headers["Accept-Language"].FirstOrDefault();
            await context.Response.WriteAsJsonAsync(new ApiErrorResponse("internal_error", localizer.GetMessage("internal_error", language), context.TraceIdentifier, context.Request.Path), context.RequestAborted);
        }
    }
}

