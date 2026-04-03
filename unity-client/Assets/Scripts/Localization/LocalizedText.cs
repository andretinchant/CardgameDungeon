using UnityEngine;
using TMPro;

namespace CardgameDungeon.Unity.Localization
{
    /// <summary>
    /// Attach to any GameObject with TMP_Text or TextMeshPro.
    /// Set the localizationKey in the Inspector and the text
    /// auto-updates when the locale changes.
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField] private string localizationKey;

        private TMP_Text textComponent;

        private void Awake()
        {
            textComponent = GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            LocalizationManager.OnLocaleChanged += Refresh;
            Refresh();
        }

        private void OnDisable()
        {
            LocalizationManager.OnLocaleChanged -= Refresh;
        }

        public void SetKey(string key)
        {
            localizationKey = key;
            Refresh();
        }

        private void Refresh()
        {
            if (textComponent == null || string.IsNullOrEmpty(localizationKey)) return;
            textComponent.text = LocalizationManager.L(localizationKey);
        }
    }
}
