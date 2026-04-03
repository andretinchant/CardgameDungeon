using CardgameDungeon.Features.Localization;
using Microsoft.AspNetCore.Http;

namespace CardgameDungeon.Infrastructure.Localization;

/// <summary>
/// Scoped ILocalizer that reads the locale from the current HTTP request
/// via Accept-Language header or ?lang= query parameter.
/// </summary>
public class RequestLocalizer : ILocalizer
{
    private readonly JsonLocalizer _inner;

    public RequestLocalizer(IHttpContextAccessor httpContextAccessor)
    {
        var locale = ResolveFromRequest(httpContextAccessor.HttpContext);
        _inner = new JsonLocalizer(locale);
    }

    public string this[string key] => _inner[key];
    public string this[string key, params object[] args] => _inner[key, args];
    public string CurrentLocale => _inner.CurrentLocale;

    private static string ResolveFromRequest(HttpContext? context)
    {
        if (context == null) return "en-US";

        // 1. Query param takes priority
        if (context.Request.Query.TryGetValue("lang", out var langParam) && !string.IsNullOrEmpty(langParam))
            return langParam.ToString();

        // 2. Accept-Language header
        var acceptLanguage = context.Request.Headers.AcceptLanguage.FirstOrDefault();
        if (!string.IsNullOrEmpty(acceptLanguage))
        {
            // Parse first language from "pt-BR,pt;q=0.9,en-US;q=0.8"
            var first = acceptLanguage.Split(',')[0].Split(';')[0].Trim();
            if (!string.IsNullOrEmpty(first)) return first;
        }

        return "en-US";
    }
}
