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
        [SerializeField] private Button quitButton;

        private void OnEnable()
        {
            playButton.onClick.AddListener(OnPlayClicked);
            collectionButton.onClick.AddListener(OnCollectionClicked);
            deckBuilderButton.onClick.AddListener(OnDeckBuilderClicked);
            marketplaceButton.onClick.AddListener(OnMarketplaceClicked);
            quitButton.onClick.AddListener(OnQuitClicked);
        }

        private void OnDisable()
        {
            playButton.onClick.RemoveListener(OnPlayClicked);
            collectionButton.onClick.RemoveListener(OnCollectionClicked);
            deckBuilderButton.onClick.RemoveListener(OnDeckBuilderClicked);
            marketplaceButton.onClick.RemoveListener(OnMarketplaceClicked);
            quitButton.onClick.RemoveListener(OnQuitClicked);
        }

        private void OnPlayClicked()
        {
            GameManager.Instance.LoadScene("Match");
        }

        private void OnCollectionClicked()
        {
            GameManager.Instance.LoadScene("Collection");
        }

        private void OnDeckBuilderClicked()
        {
            GameManager.Instance.LoadScene("DeckBuilder");
        }

        private void OnMarketplaceClicked()
        {
            GameManager.Instance.LoadScene("Marketplace");
        }

        private void OnQuitClicked()
        {
            Application.Quit();
        }
    }
}
