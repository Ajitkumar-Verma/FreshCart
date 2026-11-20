namespace FreshCart.Common.Configuration;

/// <summary>Typed database connection configuration shared across application layers.</summary>
public sealed class DatabaseOptions
{
    public const string SectionName = "ConnectionStrings";
    public string FreshCart { get; set; } = string.Empty;
}
