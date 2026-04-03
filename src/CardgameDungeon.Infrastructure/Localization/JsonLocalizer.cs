using System.Collections.Concurrent;
using System.Text.Json;
using CardgameDungeon.Features.Localization;

namespace CardgameDungeon.Infrastructure.Localization;

public class JsonLocalizer : ILocalizer
{
    private static readonly ConcurrentDictionary<string, Dictionary<string, string>> Cache = new();
    private static readonly string[] SupportedLocales = ["en-US", "pt-BR"];
    private const string DefaultLocale = "en-US";
    private const string FallbackLocale = "en-US";

    private readonly Dictionary<string, string> _strings;
    private readonly Dictionary<string, string> _fallback;

    public string CurrentLocale { get; }

    public JsonLocalizer(string locale)
    {
        CurrentLocale = ResolveLocale(locale);
        _strings = LoadLocale(CurrentLocale);
        _fallback = CurrentLocale != FallbackLocale ? LoadLocale(FallbackLocale) : _strings;
    }

    public string this[string key]
    {
        get
        {
            if (_strings.TryGetValue(key, out var value)) return value;
            if (_fallback.TryGetValue(key, out var fallback)) return fallback;
            return key;
        }
    }

    public string this[string key, params object[] args]
    {
        get
        {
            var template = this[key];
            try { return string.Format(template, args); }
            catch { return template; }
        }
    }

    private static string ResolveLocale(string? requested)
    {
        if (string.IsNullOrEmpty(requested)) return DefaultLocale;

        // Exact match
        foreach (var locale in SupportedLocales)
            if (locale.Equals(requested, StringComparison.OrdinalIgnoreCase))
                return locale;

        // Language-only match (e.g. "pt" → "pt-BR")
        var lang = requested.Split('-')[0].ToLowerInvariant();
        foreach (var locale in SupportedLocales)
            if (locale.StartsWith(lang, StringComparison.OrdinalIgnoreCase))
                return locale;

        return DefaultLocale;
    }

    private static Dictionary<string, string> LoadLocale(string locale)
    {
        return Cache.GetOrAdd(locale, loc =>
        {
            var assembly = typeof(JsonLocalizer).Assembly;
            var resourceName = $"CardgameDungeon.Infrastructure.Localization.Locales.{loc}.json";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
                return new Dictionary<string, string>();

            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();

            var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                ?? new Dictionary<string, string>();

            return dict;
        });
    }
}
