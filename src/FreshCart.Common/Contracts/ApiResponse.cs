namespace FreshCart.Common.Contracts;

/// <summary>Consistent API response envelope for successful results and errors.</summary>
public sealed record ApiResponse<T>(
    int Status,
    bool Success,
    string? Code,
    string Message,
    T? Data,
    string? TraceId,
    string? Path);
