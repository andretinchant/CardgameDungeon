using UnityEngine;
using UnityEngine.UI;
using CardgameDungeon.Unity.Core;

namespace CardgameDungeon.Unity.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button collectionButton;
        [SerializeField] private Button deckBuilderButton;
        [SerializeField] private Button marketplaceButton;
        [SerializeField] private Button boosterShopButton;
        [SerializeField] private Button profileButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button logoutButton;
        [SerializeField] private Button quitButton;

        private void OnEnable()
        {
            if (playButton != null) playButton.onClick.AddListener(OnPlayClicked);
            if (collectionButton != null) collectionButton.onClick.AddListener(OnCollectionClicked);
            if (deckBuilderButton != null) deckBuilderButton.onClick.AddListener(OnDeckBuilderClicked);
            if (marketplaceButton != null) marketplaceButton.onClick.AddListener(OnMarketplaceClicked);
            if (boosterShopButton != null) boosterShopButton.onClick.AddListener(OnBoosterShopClicked);
            if (profileButton != null) profileButton.onClick.AddListener(OnProfileClicked);
            if (settingsButton != null) settingsButton.onClick.AddListener(OnSettingsClicked);
            if (logoutButton != null) logoutButton.onClick.AddListener(OnLogoutClicked);
            if (quitButton != null) quitButton.onClick.AddListener(OnQuitClicked);
        }

        private void OnDisable()
        {
            if (playButton != null) playButton.onClick.RemoveListener(OnPlayClicked);
            if (collectionButton != null) collectionButton.onClick.RemoveListener(OnCollectionClicked);
            if (deckBuilderButton != null) deckBuilderButton.onClick.RemoveListener(OnDeckBuilderClicked);
            if (marketplaceButton != null) marketplaceButton.onClick.RemoveListener(OnMarketplaceClicked);
            if (boosterShopButton != null) boosterShopButton.onClick.RemoveListener(OnBoosterShopClicked);
            if (profileButton != null) profileButton.onClick.RemoveListener(OnProfileClicked);
            if (settingsButton != null) settingsButton.onClick.RemoveListener(OnSettingsClicked);
            if (logoutButton != null) logoutButton.onClick.RemoveListener(OnLogoutClicked);
            if (quitButton != null) quitButton.onClick.RemoveListener(OnQuitClicked);
        }

        private void OnPlayClicked()
        {
            GameManager.Instance.GoToSceneWithLoading("Match");
        }

        private void OnCollectionClicked()
        {
            GameManager.Instance.GoToSceneWithLoading("Collection");
        }

        private void OnDeckBuilderClicked()
        {
            GameManager.Instance.GoToSceneWithLoading("DeckBuilder");
        }

        private void OnMarketplaceClicked()
        {
            GameManager.Instance.GoToSceneWithLoading("Marketplace");
        }

        private void OnBoosterShopClicked()
        {
            GameManager.Instance.GoToSceneWithLoading("BoosterShop");
        }

        private void OnProfileClicked()
        {
            GameManager.Instance.GoToSceneWithLoading("Profile");
        }

        private void OnSettingsClicked()
        {
            GameManager.Instance.GoToSceneWithLoading("Settings");
        }

        private void OnLogoutClicked()
        {
            GameManager.Instance.ClearAuth();
            GameManager.Instance.GoToScene("Login");
        }

        private void OnQuitClicked()
        {
            Application.Quit();
        }
    }
}
