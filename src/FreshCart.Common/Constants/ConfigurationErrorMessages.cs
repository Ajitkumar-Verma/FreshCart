namespace FreshCart.Common.Constants;

public static class ConfigurationErrorMessages
{
    public const string JwtSettingsMissing = "JWT settings are missing.";
    public const string JwtSettingsInvalid = "JWT issuer, audience, signing key, or expiration configuration is invalid.";
    public const string DatabaseConnectionMissing = "Connection string 'FreshCart' is missing.";
}
