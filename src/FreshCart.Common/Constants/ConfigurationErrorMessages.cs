namespace FreshCart.Common.Constants;

public static class ConfigurationErrorMessages
{
    public const string JwtSettingsMissing = "JWT settings are missing.";
    public const string JwtSettingsInvalid = "JWT issuer, audience, signing key, or expiration configuration is invalid.";
    public const string JwtSigningKeyMissing = "JWT signing key is missing or shorter than 32 UTF-8 bytes. Configure Jwt:SigningKey using User Secrets, an environment variable, or local development settings.";
    public const string DatabaseConnectionMissing = "Connection string 'FreshCart' is missing.";
}
