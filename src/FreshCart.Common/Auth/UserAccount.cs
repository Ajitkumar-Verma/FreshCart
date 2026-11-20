namespace FreshCart.Common.Auth;

public sealed class UserAccount
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string NormalizedEmail { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public string NormalizedUserName { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public string PasswordHash { get; init; } = string.Empty;
}
