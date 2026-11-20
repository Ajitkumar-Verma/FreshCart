using FreshCart.Common.Constants;

namespace FreshCart.Common.Exceptions;

/// <summary>Base type for application errors that can safely be returned to API clients.</summary>
public abstract class AppException : Exception
{
    protected AppException(string errorCode, int statusCode, Exception? innerException = null)
        : base(errorCode, innerException)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }

    public string ErrorCode { get; }
    public int StatusCode { get; }
}

/// <summary>Represents invalid request data or a failed business validation rule.</summary>
public sealed class ValidationAppException(string errorCode = AppErrorCodes.Validation) : AppException(errorCode, 400);

/// <summary>Represents a request without valid authentication.</summary>
public sealed class UnauthorizedAppException(string errorCode = AppErrorCodes.Unauthorized) : AppException(errorCode, 401);

/// <summary>Represents an authenticated caller who lacks required permission.</summary>
public sealed class ForbiddenAppException(string errorCode = AppErrorCodes.Forbidden) : AppException(errorCode, 403);

/// <summary>Represents a requested resource that does not exist or is unavailable.</summary>
public sealed class NotFoundAppException(string errorCode = AppErrorCodes.NotFound) : AppException(errorCode, 404);

/// <summary>Represents a conflict with the current state of a resource.</summary>
public sealed class ConflictAppException(string errorCode = AppErrorCodes.Conflict) : AppException(errorCode, 409);

/// <summary>Represents a required server-side application configuration that is missing.</summary>
public sealed class ConfigurationAppException(string errorCode = AppErrorCodes.InternalError) : AppException(errorCode, 500);
