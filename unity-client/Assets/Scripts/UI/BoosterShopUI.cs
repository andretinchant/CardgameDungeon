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
    public class BoosterShopUI : MonoBehaviour
    {
        [Header("Set Filter")]
        [SerializeField] private TMP_InputField setFilterInput;
        [SerializeField] private TMP_Dropdown setDropdown;
        [SerializeField] private Button refreshSetsButton;

        [Header("Actions")]
        [SerializeField] private Button buyAndOpenButton;
        [SerializeField] private Button goToCollectionButton;
        [SerializeField] private Button backButton;
        [SerializeField] private bool navigateToCollectionScene = true;
        [SerializeField] private string collectionSceneName = "Collection";

        [Header("Status")]
        [SerializeField] private TMP_Text walletBalanceText;
        [SerializeField] private TMP_Text collectionCountText;
        [SerializeField] private TMP_Text statusText;

        [Header("Opened Cards")]
        [SerializeField] private Transform openedCardsContainer;
        [SerializeField] private GameObject openedCardRowPrefab;

        private readonly List<BoosterSetDto> _allSets = [];
        private List<BoosterSetDto> _filteredSets = [];
        private bool _hasPendingCollectionRefresh;
        private int _openedBoostersPending;

        private Guid PlayerId => GameManager.Instance.CurrentPlayerId;

        private void OnEnable()
        {
            if (setFilterInput != null) setFilterInput.onValueChanged.AddListener(OnFilterChanged);
            if (setDropdown != null) setDropdown.onValueChanged.AddListener(OnSetChanged);
            if (refreshSetsButton != null) refreshSetsButton.onClick.AddListener(RefreshAll);
            if (buyAndOpenButton != null) buyAndOpenButton.onClick.AddListener(BuyAndOpenSelectedSet);
            if (goToCollectionButton != null) goToCollectionButton.onClick.AddListener(ConfirmAndGoToCollection);
            if (backButton != null) backButton.onClick.AddListener(OnBackClicked);

            if (goToCollectionButton != null) goToCollectionButton.interactable = false;
            RefreshAll();
        }

        private void OnDisable()
        {
            if (setFilterInput != null) setFilterInput.onValueChanged.RemoveListener(OnFilterChanged);
            if (setDropdown != null) setDropdown.onValueChanged.RemoveListener(OnSetChanged);
            if (refreshSetsButton != null) refreshSetsButton.onClick.RemoveListener(RefreshAll);
            if (buyAndOpenButton != null) buyAndOpenButton.onClick.RemoveListener(BuyAndOpenSelectedSet);
            if (goToCollectionButton != null) goToCollectionButton.onClick.RemoveListener(ConfirmAndGoToCollection);
            if (backButton != null) backButton.onClick.RemoveListener(OnBackClicked);
        }

        private void RefreshAll()
        {
            LoadBoosterSets();
            LoadBalance();
            LoadCollection();
            ResetRevealSession(clearVisuals: true);
        }

        private void LoadBoosterSets()
        {
            GameManager.Instance.Api.GetBoosterSets(
                this,
                response =>
                {
                    _allSets.Clear();
                    if (response?.sets != null)
                        _allSets.AddRange(response.sets);

                    ApplySetFilter(setFilterInput != null ? setFilterInput.text : string.Empty);
                    SetStatus($"Sets carregados: {_allSets.Count}.");
                },
                error => SetStatus($"Erro ao carregar sets: {error}"));
        }

        private void LoadBalance()
        {
            GameManager.Instance.Api.GetBalance(
                this,
                PlayerId,
                response =>
                {
                    if (walletBalanceText != null)
                        walletBalanceText.text = $"Saldo: {response.balance}";
                },
                error => SetStatus($"Erro ao carregar saldo: {error}"));
        }

        private void LoadCollection()
        {
            GameManager.Instance.Api.GetCollection(
                this,
                PlayerId,
                response =>
                {
                    if (collectionCountText != null)
                        collectionCountText.text = $"Coleção: {response.totalCards} cartas ({response.availableCards} disponíveis)";
                },
                error => SetStatus($"Erro ao carregar coleção: {error}"));
        }

        private void OnFilterChanged(string filterText)
        {
            ApplySetFilter(filterText);
        }

        private void OnSetChanged(int _)
        {
            var selected = GetSelectedSet();
            if (selected == null) return;
            SetStatus($"Set selecionado: {selected.setCode} | Booster: {selected.boosterPrice}");
        }

        private void ApplySetFilter(string filterText)
        {
            var filter = (filterText ?? string.Empty).Trim();
            _filteredSets = _allSets
                .Where(set =>
                    string.IsNullOrEmpty(filter) ||
                    set.setCode.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    set.setName.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            if (setDropdown == null) return;

            setDropdown.ClearOptions();
            var options = _filteredSets
                .Select(set => $"{set.setCode} - {set.setName} ({set.totalCards})")
                .ToList();

            if (options.Count == 0)
                options.Add("Nenhum set encontrado");

            setDropdown.AddOptions(options);
            setDropdown.value = 0;
            setDropdown.RefreshShownValue();
        }

        private BoosterSetDto GetSelectedSet()
        {
            if (setDropdown == null || _filteredSets.Count == 0) return null;
            var index = Mathf.Clamp(setDropdown.value, 0, _filteredSets.Count - 1);
            return _filteredSets[index];
        }

        private void BuyAndOpenSelectedSet()
        {
            var selected = GetSelectedSet();
            if (selected == null)
            {
                SetStatus("Selecione um set válido.");
                return;
            }

            if (buyAndOpenButton != null) buyAndOpenButton.interactable = false;
            SetStatus($"Comprando booster de {selected.setCode}...");

            var request = new OpenBoosterRequest
            {
                playerId = PlayerId.ToString(),
                boosterPrice = selected.boosterPrice,
                setCode = selected.setCode
            };

            GameManager.Instance.Api.OpenBooster(
                this,
                request,
                response =>
                {
                    RenderOpenedCards(response.cards, response.setCode, append: true);
                    LoadBalance();
                    _hasPendingCollectionRefresh = true;
                    _openedBoostersPending++;
                    if (goToCollectionButton != null) goToCollectionButton.interactable = true;

                    SetStatus(
                        $"Booster {_openedBoostersPending} aberto ({response.setCode}). " +
                        $"{response.cards?.Count ?? 0} cartas reveladas. " +
                        "Abra mais boosters ou toque em 'Ir para Coleção'.");
                    if (buyAndOpenButton != null) buyAndOpenButton.interactable = true;
                },
                error =>
                {
                    SetStatus($"Erro ao abrir booster: {error}");
                    if (buyAndOpenButton != null) buyAndOpenButton.interactable = true;
                });
        }

        private void RenderOpenedCards(List<BoosterCardDto> cards, string setCode, bool append)
        {
            if (openedCardsContainer == null) return;

            if (!append)
            {
                foreach (Transform child in openedCardsContainer)
                    Destroy(child.gameObject);
            }

            if (cards == null || cards.Count == 0)
            {
                CreateCardRow($"[{setCode}] Nenhuma carta recebida.");
                return;
            }

            CreateCardRow($"=== Booster {_openedBoostersPending + 1} ({setCode}) ===");
            foreach (var card in cards)
            {
                var text = $"[{card.setCode}] {card.name} | {card.rarity} | {card.type}";
                CreateCardRow(text);
            }
        }

        private void CreateCardRow(string text)
        {
            if (openedCardsContainer == null) return;

            if (openedCardRowPrefab != null)
            {
                var row = Instantiate(openedCardRowPrefab, openedCardsContainer);
                var label = row.GetComponentInChildren<TMP_Text>();
                if (label != null) label.text = text;
                return;
            }

            var go = new GameObject("BoosterCardRow", typeof(RectTransform), typeof(TextMeshProUGUI));
            go.transform.SetParent(openedCardsContainer, false);
            var tmp = go.GetComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 24;
        }

        private void ConfirmAndGoToCollection()
        {
            if (_hasPendingCollectionRefresh)
            {
                LoadCollection();
                SetStatus("Coleção atualizada com as novas cartas.");
            }

            ResetRevealSession(clearVisuals: true);

            if (navigateToCollectionScene && !string.IsNullOrWhiteSpace(collectionSceneName))
                GameManager.Instance.GoToScene(collectionSceneName);
        }

        private void ResetRevealSession(bool clearVisuals)
        {
            _hasPendingCollectionRefresh = false;
            _openedBoostersPending = 0;

            if (goToCollectionButton != null)
                goToCollectionButton.interactable = false;

            if (!clearVisuals || openedCardsContainer == null)
                return;

            foreach (Transform child in openedCardsContainer)
                Destroy(child.gameObject);
        }

        private void SetStatus(string message)
        {
            if (statusText != null)
                statusText.text = message;
            Debug.Log($"[BoosterShopUI] {message}");
        }

        private void OnBackClicked()
        {
            GameManager.Instance.GoToScene("MainMenu");
        }
    }
}
