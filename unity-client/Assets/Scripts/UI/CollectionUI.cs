using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CardgameDungeon.Unity.Core;
using CardgameDungeon.Unity.Network;

namespace CardgameDungeon.Unity.UI
{
    public class CollectionUI : MonoBehaviour
    {
        [Header("Grid")]
        [SerializeField] private Transform cardGrid;
        [SerializeField] private GameObject cardRowPrefab;

        [Header("Stats")]
        [SerializeField] private TMP_Text totalCardsText;
        [SerializeField] private TMP_Text availableCardsText;
        [SerializeField] private TMP_Text statusText;

        [Header("Detail")]
        [SerializeField] private GameObject cardDetailPanel;

        [Header("Collection Actions")]
        [SerializeField] private TMP_InputField salePriceInput;
        [SerializeField] private Button listForSaleButton;
        [SerializeField] private Button openInMarketplaceButton;
        [SerializeField] private string marketplaceSceneName = "Marketplace";

        [Header("Filters")]
        [SerializeField] private TMP_InputField searchInput;
        [SerializeField] private TMP_Dropdown typeDropdown;
        [SerializeField] private TMP_Dropdown rarityDropdown;
        [SerializeField] private Toggle availableOnlyToggle;
        [SerializeField] private Button refreshButton;

        [Header("Navigation")]
        [SerializeField] private Button backButton;

        private readonly List<OwnedCardDto> _allOwnedCards = new();
        private List<CardCollectionEntry> _visibleEntries = new();
        private CardCollectionEntry _selectedEntry;

        private Guid PlayerId => GameManager.Instance.CurrentPlayerId;

        private sealed class CardCollectionEntry
        {
            public string CardId { get; set; } = string.Empty;
            public string CardName { get; set; } = "Unknown Card";
            public string CardType { get; set; } = "Unknown";
            public string Rarity { get; set; } = "Unknown";
            public int Cost { get; set; }
            public string DetailText { get; set; } = string.Empty;
            public int TotalCopies { get; set; }
            public int AvailableCopies { get; set; }
            public string FirstAvailableOwnedCardId { get; set; } = string.Empty;
            public int ReservedCopies => TotalCopies - AvailableCopies;
        }

        private void OnEnable()
        {
            if (backButton != null) backButton.onClick.AddListener(OnBackClicked);
            if (refreshButton != null) refreshButton.onClick.AddListener(LoadCollection);

            if (searchInput != null) searchInput.onValueChanged.AddListener(OnFilterChanged);
            if (typeDropdown != null) typeDropdown.onValueChanged.AddListener(OnDropdownFilterChanged);
            if (rarityDropdown != null) rarityDropdown.onValueChanged.AddListener(OnDropdownFilterChanged);
            if (availableOnlyToggle != null) availableOnlyToggle.onValueChanged.AddListener(OnAvailableOnlyChanged);
            if (listForSaleButton != null) listForSaleButton.onClick.AddListener(OnListSelectedForSaleClicked);
            if (openInMarketplaceButton != null) openInMarketplaceButton.onClick.AddListener(OnOpenSelectedInMarketplaceClicked);

            if (cardDetailPanel != null) cardDetailPanel.SetActive(false);
            LoadCollection();
        }

        private void OnDisable()
        {
            if (backButton != null) backButton.onClick.RemoveListener(OnBackClicked);
            if (refreshButton != null) refreshButton.onClick.RemoveListener(LoadCollection);

            if (searchInput != null) searchInput.onValueChanged.RemoveListener(OnFilterChanged);
            if (typeDropdown != null) typeDropdown.onValueChanged.RemoveListener(OnDropdownFilterChanged);
            if (rarityDropdown != null) rarityDropdown.onValueChanged.RemoveListener(OnDropdownFilterChanged);
            if (availableOnlyToggle != null) availableOnlyToggle.onValueChanged.RemoveListener(OnAvailableOnlyChanged);
            if (listForSaleButton != null) listForSaleButton.onClick.RemoveListener(OnListSelectedForSaleClicked);
            if (openInMarketplaceButton != null) openInMarketplaceButton.onClick.RemoveListener(OnOpenSelectedInMarketplaceClicked);
        }

        public void LoadCollection()
        {
            StartCoroutine(LoadCollectionRoutine());
        }

        private IEnumerator LoadCollectionRoutine()
        {
            yield return GameManager.Instance.Api.GetCollection(this, PlayerId, response =>
            {
                _allOwnedCards.Clear();
                if (response?.cards != null)
                    _allOwnedCards.AddRange(response.cards);

                BindDynamicFilters();
                ApplyFiltersAndRender();

                if (totalCardsText != null)
                    totalCardsText.text = $"Total: {response?.totalCards ?? 0}";
                if (availableCardsText != null)
                    availableCardsText.text = $"Disponíveis: {response?.availableCards ?? 0}";
                SetStatus($"Coleção carregada com {_allOwnedCards.Count} cópias.");
            }, error =>
            {
                SetStatus($"Erro ao carregar coleção: {error}");
                Debug.LogError($"[CollectionUI] Failed to load collection: {error}");
            });
        }

        private void BindDynamicFilters()
        {
            if (typeDropdown != null)
                PopulateDropdown(typeDropdown, _allOwnedCards.Select(c => c.cardType));

            if (rarityDropdown != null)
                PopulateDropdown(rarityDropdown, _allOwnedCards.Select(c => c.rarity));
        }

        private static void PopulateDropdown(TMP_Dropdown dropdown, IEnumerable<string> sourceValues)
        {
            var currentLabel = dropdown.options.Count > 0
                ? dropdown.options[Mathf.Clamp(dropdown.value, 0, dropdown.options.Count - 1)].text
                : "All";

            var options = sourceValues
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => value.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(value => value)
                .ToList();

            options.Insert(0, "All");
            dropdown.ClearOptions();
            dropdown.AddOptions(options);

            var selectedIndex = options.FindIndex(option =>
                option.Equals(currentLabel, StringComparison.OrdinalIgnoreCase));
            dropdown.value = selectedIndex >= 0 ? selectedIndex : 0;
            dropdown.RefreshShownValue();
        }

        private void OnFilterChanged(string _)
        {
            ApplyFiltersAndRender();
        }

        private void OnDropdownFilterChanged(int _)
        {
            ApplyFiltersAndRender();
        }

        private void OnAvailableOnlyChanged(bool _)
        {
            ApplyFiltersAndRender();
        }

        private void ApplyFiltersAndRender()
        {
            var searchTerm = searchInput != null
                ? (searchInput.text ?? string.Empty).Trim()
                : string.Empty;
            var selectedType = GetDropdownValue(typeDropdown);
            var selectedRarity = GetDropdownValue(rarityDropdown);
            var onlyAvailable = availableOnlyToggle != null && availableOnlyToggle.isOn;

            _visibleEntries = _allOwnedCards
                .GroupBy(card => card.cardId)
                .Select(group =>
                {
                    var first = group.First();
                    return new CardCollectionEntry
                    {
                        CardId = first.cardId,
                        CardName = first.cardName,
                        CardType = first.cardType,
                        Rarity = first.rarity,
                        Cost = first.cost,
                        DetailText = first.detailText,
                        TotalCopies = group.Count(),
                        AvailableCopies = group.Count(copy => !copy.isReserved),
                        FirstAvailableOwnedCardId = group
                            .FirstOrDefault(copy => !copy.isReserved)?.ownedCardId ?? string.Empty
                    };
                })
                .Where(entry =>
                    (string.IsNullOrEmpty(searchTerm) ||
                     entry.CardName.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) &&
                    (string.IsNullOrEmpty(selectedType) ||
                     entry.CardType.Equals(selectedType, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrEmpty(selectedRarity) ||
                     entry.Rarity.Equals(selectedRarity, StringComparison.OrdinalIgnoreCase)) &&
                    (!onlyAvailable || entry.AvailableCopies > 0))
                .OrderBy(entry => entry.CardName)
                .ThenBy(entry => entry.Rarity)
                .ToList();

            RenderEntries();
            SetStatus($"Exibindo {_visibleEntries.Count} cartas únicas.");
        }

        private static string GetDropdownValue(TMP_Dropdown dropdown)
        {
            if (dropdown == null || dropdown.options.Count == 0)
                return string.Empty;

            var option = dropdown.options[Mathf.Clamp(dropdown.value, 0, dropdown.options.Count - 1)].text;
            return option.Equals("All", StringComparison.OrdinalIgnoreCase) ? string.Empty : option;
        }

        private void RenderEntries()
        {
            if (cardGrid == null) return;

            foreach (Transform child in cardGrid)
                Destroy(child.gameObject);

            if (_visibleEntries.Count == 0)
            {
                CreateRow("Nenhuma carta encontrada para os filtros selecionados.", null);
                return;
            }

            foreach (var entry in _visibleEntries)
            {
                var label = $"{entry.CardName} | {entry.Rarity} {entry.CardType} | " +
                            $"Custo {entry.Cost} | {entry.AvailableCopies}/{entry.TotalCopies} disponíveis";
                CreateRow(label, () => ShowCardDetail(entry));
            }
        }

        private void CreateRow(string text, Action onClick)
        {
            if (cardGrid == null) return;

            GameObject rowObject;
            if (cardRowPrefab != null)
            {
                rowObject = Instantiate(cardRowPrefab, cardGrid);
            }
            else
            {
                rowObject = new GameObject("CollectionRow", typeof(RectTransform), typeof(Image), typeof(Button), typeof(TextMeshProUGUI));
                rowObject.transform.SetParent(cardGrid, false);

                var image = rowObject.GetComponent<Image>();
                image.color = new Color(0.16f, 0.12f, 0.08f, 0.6f);

                var layout = rowObject.AddComponent<LayoutElement>();
                layout.preferredHeight = 38f;
            }

            var label = rowObject.GetComponentInChildren<TMP_Text>();
            if (label == null)
            {
                label = rowObject.GetComponent<TMP_Text>();
            }
            if (label != null) label.text = text;

            var button = rowObject.GetComponent<Button>();
            if (button == null) button = rowObject.GetComponentInChildren<Button>();
            if (button != null && onClick != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => onClick.Invoke());
            }
        }

        private void ShowCardDetail(CardCollectionEntry entry)
        {
            if (entry == null || cardDetailPanel == null) return;

            _selectedEntry = entry;
            cardDetailPanel.SetActive(true);

            var nameText = cardDetailPanel.transform.Find("CardName")?.GetComponent<TMP_Text>();
            var typeText = cardDetailPanel.transform.Find("CardType")?.GetComponent<TMP_Text>();
            var rarityText = cardDetailPanel.transform.Find("CardRarity")?.GetComponent<TMP_Text>();
            var costText = cardDetailPanel.transform.Find("CardCost")?.GetComponent<TMP_Text>();
            var statsText = cardDetailPanel.transform.Find("CardStats")?.GetComponent<TMP_Text>();

            if (nameText != null) nameText.text = entry.CardName;
            if (typeText != null) typeText.text = $"Type: {entry.CardType}";
            if (rarityText != null) rarityText.text = $"Rarity: {entry.Rarity}";
            if (costText != null) costText.text = $"Cost: {entry.Cost}";
            if (statsText != null)
            {
                statsText.text =
                    $"CardId: {entry.CardId}\n" +
                    $"Copies: {entry.TotalCopies} (Disponíveis: {entry.AvailableCopies} | Reservadas: {entry.ReservedCopies})\n" +
                    $"{entry.DetailText}";
            }

            if (salePriceInput == null)
                salePriceInput = cardDetailPanel.transform.Find("SalePriceInput")?.GetComponent<TMP_InputField>();
            if (listForSaleButton == null)
                listForSaleButton = cardDetailPanel.transform.Find("ListForSaleButton")?.GetComponent<Button>();
            if (openInMarketplaceButton == null)
                openInMarketplaceButton = cardDetailPanel.transform.Find("OpenInMarketplaceButton")?.GetComponent<Button>();

            if (salePriceInput != null && string.IsNullOrWhiteSpace(salePriceInput.text))
                salePriceInput.text = "50";
            if (listForSaleButton != null)
            {
                listForSaleButton.onClick.RemoveListener(OnListSelectedForSaleClicked);
                listForSaleButton.onClick.AddListener(OnListSelectedForSaleClicked);
                listForSaleButton.interactable = entry.AvailableCopies > 0;
            }
            if (openInMarketplaceButton != null)
            {
                openInMarketplaceButton.onClick.RemoveListener(OnOpenSelectedInMarketplaceClicked);
                openInMarketplaceButton.onClick.AddListener(OnOpenSelectedInMarketplaceClicked);
                openInMarketplaceButton.interactable = true;
            }

            var closeButton = cardDetailPanel.transform.Find("CloseButton")?.GetComponent<Button>();
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(() => cardDetailPanel.SetActive(false));
            }
        }

        private void SetStatus(string message)
        {
            if (statusText != null) statusText.text = message;
            Debug.Log($"[CollectionUI] {message}");
        }

        private void OnListSelectedForSaleClicked()
        {
            if (_selectedEntry == null)
            {
                SetStatus("Selecione uma carta na coleção antes de listar.");
                return;
            }

            if (_selectedEntry.AvailableCopies <= 0 || string.IsNullOrWhiteSpace(_selectedEntry.FirstAvailableOwnedCardId))
            {
                SetStatus("Nenhuma cópia disponível para venda.");
                return;
            }

            if (!Guid.TryParse(_selectedEntry.FirstAvailableOwnedCardId, out var ownedCardId))
            {
                SetStatus("Falha ao identificar a cópia da carta para venda.");
                return;
            }

            var price = 50;
            if (salePriceInput != null && !string.IsNullOrWhiteSpace(salePriceInput.text))
            {
                if (!int.TryParse(salePriceInput.text, out price) || price <= 0)
                {
                    SetStatus("Preço inválido. Informe um valor inteiro maior que zero.");
                    return;
                }
            }

            if (listForSaleButton != null) listForSaleButton.interactable = false;
            SetStatus($"Listando {_selectedEntry.CardName} por {price}...");

            var request = new ListCardRequest
            {
                sellerId = PlayerId.ToString(),
                ownedCardId = ownedCardId.ToString(),
                price = price
            };

            GameManager.Instance.Api.ListCard(this, request, listing =>
            {
                SetStatus($"Carta listada com sucesso. Listing: {listing.listingId}");
                LoadCollection();
            }, error =>
            {
                SetStatus($"Erro ao listar carta: {error}");
                if (listForSaleButton != null)
                    listForSaleButton.interactable = _selectedEntry.AvailableCopies > 0;
            });
        }

        private void OnOpenSelectedInMarketplaceClicked()
        {
            if (_selectedEntry == null)
            {
                SetStatus("Selecione uma carta para abrir o marketplace filtrado.");
                return;
            }

            GameManager.Instance.SetMarketplaceFocus(
                _selectedEntry.CardId,
                _selectedEntry.CardName,
                _selectedEntry.CardType,
                _selectedEntry.Rarity);

            GameManager.Instance.GoToScene(marketplaceSceneName);
        }

        private void OnBackClicked()
        {
            GameManager.Instance.GoToScene("MainMenu");
        }
    }
}
