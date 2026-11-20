using System.Reflection;
using System.Text.Json;

namespace FreshCart.Common.Localization;

/// <summary>Reads embedded error catalogs and falls back to English, then to the error code.</summary>
public sealed class ErrorLocalizer : IErrorLocalizer
{
    private readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> _catalogs;

    public ErrorLocalizer()
    {
        _catalogs = new Dictionary<string, IReadOnlyDictionary<string, string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["en"] = Load("FreshCart.Common.Resources.Errors.en.json"),
            ["fr"] = Load("FreshCart.Common.Resources.Errors.fr.json")
        };
    }

    public string GetMessage(string errorCode, string? language = null)
    {
        var culture = string.IsNullOrWhiteSpace(language) ? "en" : language.Split('-', 2)[0];
        if (_catalogs.TryGetValue(culture, out var selected) && selected.TryGetValue(errorCode, out var localized)) return localized;
        return _catalogs["en"].TryGetValue(errorCode, out var english) ? english : errorCode;
    }

    private static IReadOnlyDictionary<string, string> Load(string resourceName)
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded error catalog '{resourceName}' was not found.");
        return JsonSerializer.Deserialize<Dictionary<string, string>>(stream)
            ?? throw new InvalidOperationException($"Embedded error catalog '{resourceName}' is invalid.");
    }
}
