namespace FreshCart.Common.Auth;

public sealed record ManagedUserRecord(
    Guid Id,
    string Email,
    string DisplayName,
    string? PhoneNumber,
    DateTime CreatedOn,
    DateTime? UpdatedOn);
