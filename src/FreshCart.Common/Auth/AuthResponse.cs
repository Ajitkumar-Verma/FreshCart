namespace FreshCart.Common.Auth;

public sealed record AuthResponse(Guid UserId, string Email, string DisplayName, IReadOnlyList<string> Roles, string AccessToken, DateTime ExpiresAtUtc);
