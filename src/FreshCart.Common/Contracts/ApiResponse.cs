namespace FreshCart.Common.Contracts;

/// <summary>Consistent API response envelope for successful results and errors.</summary>
public sealed class ApiResponse<T>
{
    public ApiResponse(
        int status,
        bool success,
        string? code,
        string message,
        T? data,
        string? traceId,
        string? path)
    {
        Status = status;
        Success = success;
        Code = code;
        Message = message;
        Data = data;
        TraceId = traceId;
        Path = path;
    }

    public int Status { get; }

    public bool Success { get; }

    public string? Code { get; }

    public string Message { get; }

    public T? Data { get; }

    public string? TraceId { get; }

    public string? Path { get; }
}
