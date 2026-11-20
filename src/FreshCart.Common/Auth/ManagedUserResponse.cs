namespace FreshCart.Common.Auth;

public sealed record ManagedUserResponse(
    Guid Id,
    string Email,
    string DisplayName,
    string? PhoneNumber,
    DateTime CreatedOn,
    DateTime? UpdatedOn,
    IReadOnlyList<string> Roles);
