namespace FreshCart.Common.Constants;

public static class AuthPolicy
{
    public const int MinimumPasswordLength = 12;
    public const int MaximumPasswordLength = 128;
    public const int MinimumJwtSigningKeyBytes = 32;
}
