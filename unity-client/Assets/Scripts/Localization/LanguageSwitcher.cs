using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CardgameDungeon.Unity.Localization
{
    /// <summary>
    /// Simple UI dropdown/button for switching languages.
    /// Attach to a TMP_Dropdown or call SwitchTo(locale) directly.
    /// </summary>
    public class LanguageSwitcher : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown languageDropdown;

        private string[] locales;

        private void Start()
        {
            if (LocalizationManager.Instance == null || languageDropdown == null) return;

            locales = LocalizationManager.Instance.GetSupportedLocales();
            languageDropdown.ClearOptions();

            var options = new System.Collections.Generic.List<string>();
            int currentIndex = 0;

            for (int i = 0; i < locales.Length; i++)
            {
                options.Add(GetDisplayName(locales[i]));
                if (locales[i] == LocalizationManager.Instance.CurrentLocale)
                    currentIndex = i;
            }

            languageDropdown.AddOptions(options);
            languageDropdown.value = currentIndex;
            languageDropdown.onValueChanged.AddListener(OnLanguageSelected);
        }

        private void OnLanguageSelected(int index)
        {
            if (index >= 0 && index < locales.Length)
                SwitchTo(locales[index]);
        }

        public void SwitchTo(string locale)
        {
            if (LocalizationManager.Instance != null)
                LocalizationManager.Instance.SetLocale(locale);
        }

        private static string GetDisplayName(string locale)
        {
            return locale switch
            {
                "en-US" => "English",
                "pt-BR" => "Portugues (BR)",
                _ => locale
            };
        }

        private void OnDestroy()
        {
            if (languageDropdown != null)
                languageDropdown.onValueChanged.RemoveListener(OnLanguageSelected);
        }
    }
}
