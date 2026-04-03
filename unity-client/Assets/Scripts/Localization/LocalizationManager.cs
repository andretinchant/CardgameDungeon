using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace CardgameDungeon.Unity.Localization
{
    /// <summary>
    /// Singleton that loads locale JSON files from Resources/Locales
    /// and provides localized strings by key.
    /// </summary>
    public class LocalizationManager : MonoBehaviour
    {
        private static LocalizationManager instance;
        public static LocalizationManager Instance => instance;

        [SerializeField] private string defaultLocale = "en-US";

        private string currentLocale;
        private Dictionary<string, string> currentStrings = new();
        private Dictionary<string, string> fallbackStrings = new();

        private static readonly string[] SupportedLocales = { "en-US", "pt-BR" };

        public string CurrentLocale => currentLocale;
        public static event Action OnLocaleChanged;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);

            string systemLocale = MapSystemLanguage(Application.systemLanguage);
            SetLocale(systemLocale);
        }

        public void SetLocale(string locale)
        {
            currentLocale = ResolveLocale(locale);
            currentStrings = LoadLocale(currentLocale);
            fallbackStrings = currentLocale != "en-US" ? LoadLocale("en-US") : currentStrings;

            Debug.Log($"[Localization] Locale set to: {currentLocale}");
            OnLocaleChanged?.Invoke();
        }

        public string Get(string key)
        {
            if (currentStrings.TryGetValue(key, out var value)) return value;
            if (fallbackStrings.TryGetValue(key, out var fallback)) return fallback;
            return key;
        }

        public string Get(string key, params object[] args)
        {
            var template = Get(key);
            try { return string.Format(template, args); }
            catch { return template; }
        }

        public static string L(string key)
            => instance != null ? instance.Get(key) : key;

        public static string L(string key, params object[] args)
            => instance != null ? instance.Get(key, args) : key;

        public string[] GetSupportedLocales() => SupportedLocales;

        private static Dictionary<string, string> LoadLocale(string locale)
        {
            var textAsset = Resources.Load<TextAsset>($"Locales/{locale}");
            if (textAsset == null)
            {
                Debug.LogWarning($"[Localization] Locale file not found: {locale}");
                return new Dictionary<string, string>();
            }

            return ParseFlatJson(textAsset.text);
        }

        /// <summary>
        /// Parses a flat JSON object {"key": "value", ...} into a Dictionary.
        /// Avoids dependency on JsonUtility (which can't deserialize Dictionary)
        /// and on System.Text.Json (not available in Unity by default).
        /// </summary>
        private static Dictionary<string, string> ParseFlatJson(string json)
        {
            var dict = new Dictionary<string, string>();

            // Match "key": "value" pairs, handling escaped quotes
            var pattern = @"""([^""\\]*(?:\\.[^""\\]*)*)""[\s]*:[\s]*""([^""\\]*(?:\\.[^""\\]*)*)""";
            var matches = Regex.Matches(json, pattern);

            foreach (Match match in matches)
            {
                if (match.Groups.Count >= 3)
                {
                    string key = UnescapeJson(match.Groups[1].Value);
                    string value = UnescapeJson(match.Groups[2].Value);
                    dict[key] = value;
                }
            }

            return dict;
        }

        private static string UnescapeJson(string s)
        {
            return s.Replace("\\\"", "\"")
                    .Replace("\\\\", "\\")
                    .Replace("\\n", "\n")
                    .Replace("\\t", "\t");
        }

        private static string ResolveLocale(string requested)
        {
            if (string.IsNullOrEmpty(requested)) return "en-US";

            foreach (var loc in SupportedLocales)
                if (loc.Equals(requested, StringComparison.OrdinalIgnoreCase))
                    return loc;

            var lang = requested.Split('-')[0].ToLowerInvariant();
            foreach (var loc in SupportedLocales)
                if (loc.StartsWith(lang, StringComparison.OrdinalIgnoreCase))
                    return loc;

            return "en-US";
        }

        private static string MapSystemLanguage(SystemLanguage lang)
        {
            return lang switch
            {
                SystemLanguage.Portuguese => "pt-BR",
                SystemLanguage.English => "en-US",
                _ => "en-US"
            };
        }
    }
}
