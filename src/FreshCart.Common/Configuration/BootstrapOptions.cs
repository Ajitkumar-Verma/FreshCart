namespace FreshCart.Common.Configuration;

public sealed class BootstrapOptions
{
    public const string SectionName = "Bootstrap";
    public string SuperUserKey { get; set; } = string.Empty;
}
