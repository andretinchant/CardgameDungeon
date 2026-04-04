using UnityEngine;
using UnityEngine.UI;
using CardgameDungeon.Unity.Core;

namespace CardgameDungeon.Unity.UI
{
    public class SettingsUI : MonoBehaviour
    {
        [SerializeField] private Button backButton;
        [SerializeField] private string returnSceneName = "MainMenu";

        private void OnEnable()
        {
            if (backButton != null) backButton.onClick.AddListener(OnBackClicked);
        }

        private void OnDisable()
        {
            if (backButton != null) backButton.onClick.RemoveListener(OnBackClicked);
        }

        private void OnBackClicked()
        {
            GameManager.Instance.GoToSceneWithLoading(returnSceneName);
        }
    }
}
