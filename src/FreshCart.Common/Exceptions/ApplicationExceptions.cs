namespace FreshCart.Common.Exceptions;

/// <summary>Base type for application errors that can safely be returned to API clients.</summary>
public abstract class AppException : Exception
{
    protected AppException(string errorCode, string message, int statusCode, Exception? innerException = null)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }

    public string ErrorCode { get; }
    public int StatusCode { get; }
}

/// <summary>Represents invalid request data or a failed business validation rule.</summary>
public sealed class ValidationAppException(string message, string errorCode = "validation_error") : AppException(errorCode, message, 400);

/// <summary>Represents a request without valid authentication.</summary>
public sealed class UnauthorizedAppException(string message = "Authentication is required.") : AppException("unauthorized", message, 401);

/// <summary>Represents an authenticated caller who lacks required permission.</summary>
public sealed class ForbiddenAppException(string message = "You do not have permission to perform this action.") : AppException("forbidden", message, 403);

/// <summary>Represents a requested resource that does not exist or is unavailable.</summary>
public sealed class NotFoundAppException(string resource = "Resource") : AppException("not_found", $"{resource} was not found.", 404);

/// <summary>Represents a conflict with the current state of a resource.</summary>
public sealed class ConflictAppException(string message = "The request conflicts with the current resource state.") : AppException("conflict", message, 409);
