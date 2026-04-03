using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CardgameDungeon.Unity.Cards;
using CardgameDungeon.Unity.Core;
using CardgameDungeon.Unity.Network;

namespace CardgameDungeon.Unity.UI
{
    public class MarketplaceUI : MonoBehaviour
    {
        [Header("Listings")]
        [SerializeField] private Transform listingsGrid;

        [Header("List Card")]
        [SerializeField] private GameObject listCardPanel;
        [SerializeField] private TMP_InputField priceInput;
        [SerializeField] private Button listButton;
        [SerializeField] private CardData selectedCardForSale;

        [Header("Filters")]
        [SerializeField] private TMP_Dropdown cardTypeDropdown;
        [SerializeField] private TMP_Dropdown rarityDropdown;
        [SerializeField] private Button searchButton;

        [Header("Balance")]
        [SerializeField] private TMP_Text balanceText;

        [Header("Navigation")]
        [SerializeField] private Button backButton;

        private void OnEnable()
        {
            searchButton.onClick.AddListener(ApplyFilters);
            listButton.onClick.AddListener(ListCard);
            backButton.onClick.AddListener(OnBackClicked);

            LoadListings();
            UpdateBalance();
        }

        private void OnDisable()
        {
            searchButton.onClick.RemoveListener(ApplyFilters);
            listButton.onClick.RemoveListener(ListCard);
            backButton.onClick.RemoveListener(OnBackClicked);
        }

        public void LoadListings()
        {
            string cardTypeFilter = cardTypeDropdown != null && cardTypeDropdown.value > 0
                ? cardTypeDropdown.options[cardTypeDropdown.value].text
                : null;

            string rarityFilter = rarityDropdown != null && rarityDropdown.value > 0
                ? rarityDropdown.options[rarityDropdown.value].text
                : null;

            StartCoroutine(LoadListingsRoutine(cardTypeFilter, rarityFilter));
        }

        private IEnumerator LoadListingsRoutine(string cardTypeFilter, string rarityFilter)
        {
            yield return StartCoroutine(ApiClient.GetMarketplace(cardTypeFilter, rarityFilter, (listings, error) =>
            {
                if (!string.IsNullOrEmpty(error))
                {
                    Debug.LogError($"Failed to load marketplace listings: {error}");
                    return;
                }

                foreach (Transform child in listingsGrid)
                {
                    Destroy(child.gameObject);
                }

                foreach (var listing in listings)
                {
                    var listingView = GameManager.Instance.CreateListingView(listing, listingsGrid);
                    var buyButton = listingView.GetComponentInChildren<Button>();
                    if (buyButton != null)
                    {
                        var capturedId = listing.ListingId;
                        buyButton.onClick.AddListener(() => BuyCard(capturedId));
                    }
                }
            }));
        }

        public void BuyCard(string listingId)
        {
            StartCoroutine(BuyCardRoutine(listingId));
        }

        private IEnumerator BuyCardRoutine(string listingId)
        {
            yield return StartCoroutine(ApiClient.BuyCard(listingId, (success, error) =>
            {
                if (!string.IsNullOrEmpty(error))
                {
                    Debug.LogError($"Failed to buy card: {error}");
                    return;
                }

                Debug.Log("Card purchased successfully.");
                LoadListings();
                UpdateBalance();
            }));
        }

        public void ListCard()
        {
            if (selectedCardForSale == null)
            {
                Debug.LogWarning("No card selected for listing.");
                return;
            }

            if (!int.TryParse(priceInput.text, out int price) || price <= 0)
            {
                Debug.LogWarning("Please enter a valid price.");
                return;
            }

            StartCoroutine(ListCardRoutine(selectedCardForSale.CardId, price));
        }

        private IEnumerator ListCardRoutine(string cardId, int price)
        {
            yield return StartCoroutine(ApiClient.ListCard(cardId, price, (success, error) =>
            {
                if (!string.IsNullOrEmpty(error))
                {
                    Debug.LogError($"Failed to list card: {error}");
                    return;
                }

                Debug.Log("Card listed on marketplace successfully.");
                selectedCardForSale = null;
                priceInput.text = "";
                listCardPanel.SetActive(false);
                LoadListings();
            }));
        }

        public void CancelListing(string listingId)
        {
            StartCoroutine(CancelListingRoutine(listingId));
        }

        private IEnumerator CancelListingRoutine(string listingId)
        {
            yield return StartCoroutine(ApiClient.CancelListing(listingId, (success, error) =>
            {
                if (!string.IsNullOrEmpty(error))
                {
                    Debug.LogError($"Failed to cancel listing: {error}");
                    return;
                }

                Debug.Log("Listing cancelled.");
                LoadListings();
            }));
        }

        public void UpdateBalance()
        {
            StartCoroutine(UpdateBalanceRoutine());
        }

        private IEnumerator UpdateBalanceRoutine()
        {
            yield return StartCoroutine(ApiClient.GetBalance((balance, error) =>
            {
                if (!string.IsNullOrEmpty(error))
                {
                    Debug.LogError($"Failed to get balance: {error}");
                    return;
                }

                balanceText.text = $"Balance: {balance}";
            }));
        }

        public void ApplyFilters()
        {
            LoadListings();
        }

        private void OnBackClicked()
        {
            GameManager.Instance.LoadScene("MainMenu");
        }
    }
}
