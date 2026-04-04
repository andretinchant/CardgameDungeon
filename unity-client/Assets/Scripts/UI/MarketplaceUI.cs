using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CardgameDungeon.Unity.Core;
using CardgameDungeon.Unity.Network;

namespace CardgameDungeon.Unity.UI
{
    public class MarketplaceUI : MonoBehaviour
    {
        [Header("Listings")]
        [SerializeField] private Transform listingsGrid;
        [SerializeField] private GameObject listingRowPrefab;

        [Header("Filters")]
        [SerializeField] private TMP_Dropdown cardTypeDropdown;
        [SerializeField] private TMP_Dropdown rarityDropdown;
        [SerializeField] private Button searchButton;
        [SerializeField] private Button clearFocusButton;

        [Header("Status")]
        [SerializeField] private TMP_Text balanceText;
        [SerializeField] private TMP_Text focusCardText;
        [SerializeField] private TMP_Text statusText;

        [Header("Navigation")]
        [SerializeField] private Button backButton;

        private readonly List<ListingDto> _allListings = new();
        private string _focusedCardId = string.Empty;
        private string _focusedCardName = string.Empty;

        private Guid PlayerId => GameManager.Instance.CurrentPlayerId;

        private void OnEnable()
        {
            if (searchButton != null) searchButton.onClick.AddListener(LoadListings);
            if (clearFocusButton != null) clearFocusButton.onClick.AddListener(ClearFocusedCard);
            if (backButton != null) backButton.onClick.AddListener(OnBackClicked);
            if (cardTypeDropdown != null) cardTypeDropdown.onValueChanged.AddListener(OnFilterChanged);
            if (rarityDropdown != null) rarityDropdown.onValueChanged.AddListener(OnFilterChanged);

            ApplyPendingFocus();
            UpdateBalance();
            LoadListings();
        }

        private void OnDisable()
        {
            if (searchButton != null) searchButton.onClick.RemoveListener(LoadListings);
            if (clearFocusButton != null) clearFocusButton.onClick.RemoveListener(ClearFocusedCard);
            if (backButton != null) backButton.onClick.RemoveListener(OnBackClicked);
            if (cardTypeDropdown != null) cardTypeDropdown.onValueChanged.RemoveListener(OnFilterChanged);
            if (rarityDropdown != null) rarityDropdown.onValueChanged.RemoveListener(OnFilterChanged);
        }

        private void ApplyPendingFocus()
        {
            var focus = GameManager.Instance.ConsumeMarketplaceFocus();
            if (focus == null)
            {
                RefreshFocusLabel();
                return;
            }

            _focusedCardId = focus.CardId ?? string.Empty;
            _focusedCardName = focus.CardName ?? string.Empty;

            SelectDropdownValue(cardTypeDropdown, focus.CardType);
            SelectDropdownValue(rarityDropdown, focus.Rarity);
            RefreshFocusLabel();
        }

        private static void SelectDropdownValue(TMP_Dropdown dropdown, string value)
        {
            if (dropdown == null || string.IsNullOrWhiteSpace(value) || dropdown.options == null)
                return;

            var index = dropdown.options.FindIndex(option =>
                option != null &&
                option.text.Equals(value, StringComparison.OrdinalIgnoreCase));

            if (index >= 0)
            {
                dropdown.value = index;
                dropdown.RefreshShownValue();
            }
        }

        public void LoadListings()
        {
            var cardTypeFilter = GetDropdownFilterValue(cardTypeDropdown);
            var rarityFilter = GetDropdownFilterValue(rarityDropdown);

            SetStatus("Carregando listagens...");
            GameManager.Instance.Api.GetMarketplace(this, cardTypeFilter, rarityFilter, response =>
            {
                _allListings.Clear();
                if (response?.listings != null)
                    _allListings.AddRange(response.listings.Where(l => l != null));

                RenderFilteredListings();
                SetStatus($"Marketplace carregado: {_allListings.Count} listagens.");
            }, error =>
            {
                SetStatus($"Erro ao carregar marketplace: {error}");
                Debug.LogError($"[MarketplaceUI] Failed to load listings: {error}");
            });
        }

        private void RenderFilteredListings()
        {
            if (listingsGrid == null) return;

            foreach (Transform child in listingsGrid)
                Destroy(child.gameObject);

            var listings = _allListings.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(_focusedCardId))
            {
                listings = listings.Where(listing =>
                    string.Equals(listing.cardId, _focusedCardId, StringComparison.OrdinalIgnoreCase));
            }

            var filtered = listings.ToList();
            if (filtered.Count == 0)
            {
                CreateInfoRow("Nenhuma listagem encontrada para os filtros selecionados.");
                return;
            }

            foreach (var listing in filtered)
            {
                CreateListingRow(listing);
            }
        }

        private void CreateListingRow(ListingDto listing)
        {
            var isOwnListing = string.Equals(listing.sellerId, PlayerId.ToString(), StringComparison.OrdinalIgnoreCase);
            var priceText = $"Preço {listing.price} (+ taxa {listing.fee})";
            var rowText = $"{listing.cardId} | {priceText} | Seller: {listing.sellerId}";

            if (listingRowPrefab != null)
            {
                var row = Instantiate(listingRowPrefab, listingsGrid);
                var text = row.GetComponentInChildren<TMP_Text>();
                if (text != null) text.text = rowText;

                var actionButton = row.GetComponentInChildren<Button>();
                if (actionButton != null)
                {
                    actionButton.onClick.RemoveAllListeners();
                    var label = actionButton.GetComponentInChildren<TMP_Text>();
                    if (label != null) label.text = isOwnListing ? "Cancelar" : "Comprar";
                    actionButton.onClick.AddListener(() =>
                    {
                        if (isOwnListing) CancelListing(listing);
                        else BuyListing(listing);
                    });
                }

                return;
            }

            var rowObject = new GameObject("ListingRow", typeof(RectTransform), typeof(Image), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
            rowObject.transform.SetParent(listingsGrid, false);

            var image = rowObject.GetComponent<Image>();
            image.color = new Color(0.14f, 0.11f, 0.08f, 0.7f);

            var layout = rowObject.GetComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(10, 10, 6, 6);
            layout.spacing = 8f;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;

            var rowLayout = rowObject.GetComponent<LayoutElement>();
            rowLayout.preferredHeight = 42f;

            var labelGo = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI), typeof(LayoutElement));
            labelGo.transform.SetParent(rowObject.transform, false);
            var label = labelGo.GetComponent<TextMeshProUGUI>();
            label.text = rowText;
            label.fontSize = 20;
            label.alignment = TextAlignmentOptions.Left;
            var labelLayout = labelGo.GetComponent<LayoutElement>();
            labelLayout.flexibleWidth = 1f;

            var actionButton = CreateActionButton(rowObject.transform, isOwnListing ? "Cancelar" : "Comprar");
            actionButton.onClick.AddListener(() =>
            {
                if (isOwnListing) CancelListing(listing);
                else BuyListing(listing);
            });
        }

        private static Button CreateActionButton(Transform parent, string caption)
        {
            var buttonGo = new GameObject("ActionButton", typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
            buttonGo.transform.SetParent(parent, false);

            var layout = buttonGo.GetComponent<LayoutElement>();
            layout.preferredWidth = 120f;
            layout.preferredHeight = 30f;

            var image = buttonGo.GetComponent<Image>();
            image.color = new Color(0.32f, 0.24f, 0.12f, 0.95f);

            var labelGo = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            labelGo.transform.SetParent(buttonGo.transform, false);
            var rect = labelGo.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var label = labelGo.GetComponent<TextMeshProUGUI>();
            label.text = caption;
            label.fontSize = 19f;
            label.alignment = TextAlignmentOptions.Center;

            return buttonGo.GetComponent<Button>();
        }

        private void BuyListing(ListingDto listing)
        {
            if (!Guid.TryParse(listing.listingId, out var listingId))
            {
                SetStatus("Listing inválido para compra.");
                return;
            }

            SetStatus("Comprando carta...");
            var request = new BuyCardRequest { buyerId = PlayerId.ToString() };
            GameManager.Instance.Api.BuyCard(this, listingId, request, _ =>
            {
                SetStatus("Compra realizada com sucesso.");
                UpdateBalance();
                LoadListings();
            }, error => SetStatus($"Erro ao comprar carta: {error}"));
        }

        private void CancelListing(ListingDto listing)
        {
            if (!Guid.TryParse(listing.listingId, out var listingId))
            {
                SetStatus("Listing inválido para cancelamento.");
                return;
            }

            SetStatus("Cancelando listagem...");
            GameManager.Instance.Api.CancelListing(this, listingId, PlayerId, _ =>
            {
                SetStatus("Listagem cancelada.");
                LoadListings();
            }, error => SetStatus($"Erro ao cancelar listagem: {error}"));
        }

        private void UpdateBalance()
        {
            GameManager.Instance.Api.GetBalance(this, PlayerId, response =>
            {
                if (balanceText != null)
                    balanceText.text = $"Saldo: {response.balance}";
            }, error => SetStatus($"Erro ao carregar saldo: {error}"));
        }

        private void ClearFocusedCard()
        {
            _focusedCardId = string.Empty;
            _focusedCardName = string.Empty;
            RefreshFocusLabel();
            RenderFilteredListings();
            SetStatus("Filtro de carta específica removido.");
        }

        private static string GetDropdownFilterValue(TMP_Dropdown dropdown)
        {
            if (dropdown == null || dropdown.options == null || dropdown.options.Count == 0)
                return null;

            if (dropdown.value <= 0)
                return null;

            var option = dropdown.options[dropdown.value]?.text;
            return string.IsNullOrWhiteSpace(option) ? null : option;
        }

        private void OnFilterChanged(int _)
        {
            LoadListings();
        }

        private void RefreshFocusLabel()
        {
            if (focusCardText == null) return;

            if (string.IsNullOrWhiteSpace(_focusedCardId))
            {
                focusCardText.text = "Carta foco: nenhuma";
                return;
            }

            var label = string.IsNullOrWhiteSpace(_focusedCardName) ? _focusedCardId : _focusedCardName;
            focusCardText.text = $"Carta foco: {label}";
        }

        private void SetStatus(string message)
        {
            if (statusText != null) statusText.text = message;
            Debug.Log($"[MarketplaceUI] {message}");
        }

        private void CreateInfoRow(string text)
        {
            var row = new GameObject("MarketplaceInfoRow", typeof(RectTransform), typeof(TextMeshProUGUI));
            row.transform.SetParent(listingsGrid, false);
            var label = row.GetComponent<TextMeshProUGUI>();
            label.text = text;
            label.fontSize = 24f;
        }

        private void OnBackClicked()
        {
            GameManager.Instance.GoToScene("MainMenu");
        }
    }
}
