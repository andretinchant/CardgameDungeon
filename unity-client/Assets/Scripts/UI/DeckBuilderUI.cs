using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CardgameDungeon.Unity.Cards;
using CardgameDungeon.Unity.Core;
using CardgameDungeon.Unity.Network;

namespace CardgameDungeon.Unity.UI
{
    public class DeckBuilderUI : MonoBehaviour
    {
        private const int MaxAdventurers = 40;
        private const int MaxEnemies = 40;
        private const int MaxRooms = 5;
        private const int MaxBosses = 1;

        [Header("Layout")]
        [SerializeField] private Transform collectionGrid;
        [SerializeField] private Transform deckPanel;

        [Header("Counters")]
        [SerializeField] private TMP_Text adventurerCountText;
        [SerializeField] private TMP_Text enemyCountText;
        [SerializeField] private TMP_Text roomCountText;

        [Header("Boss")]
        [SerializeField] private Transform bossSlot;

        [Header("Actions")]
        [SerializeField] private Button saveButton;
        [SerializeField] private Button backButton;

        private string currentDeckId;
        private readonly List<CardData> selectedAdventurers = new List<CardData>();
        private readonly List<CardData> selectedEnemies = new List<CardData>();
        private readonly List<CardData> selectedRooms = new List<CardData>();
        private CardData selectedBoss;

        private void OnEnable()
        {
            saveButton.onClick.AddListener(SaveDeck);
            backButton.onClick.AddListener(OnBackClicked);
            LoadCollection();
        }

        private void OnDisable()
        {
            saveButton.onClick.RemoveListener(SaveDeck);
            backButton.onClick.RemoveListener(OnBackClicked);
        }

        public void LoadCollection()
        {
            StartCoroutine(LoadCollectionRoutine());
        }

        private IEnumerator LoadCollectionRoutine()
        {
            yield return StartCoroutine(ApiClient.GetCollection((cards, error) =>
            {
                if (!string.IsNullOrEmpty(error))
                {
                    Debug.LogError($"Failed to load collection: {error}");
                    return;
                }

                foreach (Transform child in collectionGrid)
                {
                    Destroy(child.gameObject);
                }

                foreach (var card in cards)
                {
                    var cardView = GameManager.Instance.CreateCardView(card, collectionGrid);
                    var button = cardView.GetComponent<Button>();
                    if (button != null)
                    {
                        var capturedCard = card;
                        button.onClick.AddListener(() => AddCardToDeck(capturedCard));
                    }
                }
            }));
        }

        public bool AddCardToDeck(CardData card)
        {
            switch (card.CardType)
            {
                case CardType.Ally:
                case CardType.Equipment:
                    if (selectedAdventurers.Count >= MaxAdventurers)
                    {
                        Debug.LogWarning($"Cannot add more adventurer cards. Maximum is {MaxAdventurers}.");
                        return false;
                    }
                    selectedAdventurers.Add(card);
                    break;

                case CardType.Monster:
                case CardType.Trap:
                    if (selectedEnemies.Count >= MaxEnemies)
                    {
                        Debug.LogWarning($"Cannot add more enemy cards. Maximum is {MaxEnemies}.");
                        return false;
                    }
                    selectedEnemies.Add(card);
                    break;

                case CardType.DungeonRoom:
                    if (selectedRooms.Count >= MaxRooms)
                    {
                        Debug.LogWarning($"Cannot add more room cards. Maximum is {MaxRooms}.");
                        return false;
                    }
                    selectedRooms.Add(card);
                    break;

                case CardType.Boss:
                    if (selectedBoss != null)
                    {
                        Debug.LogWarning("A boss is already selected. Remove it first.");
                        return false;
                    }
                    selectedBoss = card;
                    break;

                default:
                    Debug.LogWarning($"Unknown card type: {card.CardType}");
                    return false;
            }

            UpdateCounters();
            return true;
        }

        public void RemoveCardFromDeck(CardData card)
        {
            switch (card.CardType)
            {
                case CardType.Ally:
                case CardType.Equipment:
                    selectedAdventurers.Remove(card);
                    break;

                case CardType.Monster:
                case CardType.Trap:
                    selectedEnemies.Remove(card);
                    break;

                case CardType.DungeonRoom:
                    selectedRooms.Remove(card);
                    break;

                case CardType.Boss:
                    if (selectedBoss == card)
                    {
                        selectedBoss = null;
                    }
                    break;
            }

            UpdateCounters();
        }

        public void SaveDeck()
        {
            StartCoroutine(SaveDeckRoutine());
        }

        private IEnumerator SaveDeckRoutine()
        {
            var allCardIds = new List<string>();
            foreach (var card in selectedAdventurers) allCardIds.Add(card.CardId);
            foreach (var card in selectedEnemies) allCardIds.Add(card.CardId);
            foreach (var card in selectedRooms) allCardIds.Add(card.CardId);
            if (selectedBoss != null) allCardIds.Add(selectedBoss.CardId);

            if (string.IsNullOrEmpty(currentDeckId))
            {
                yield return StartCoroutine(ApiClient.CreateDeck(allCardIds, (deckId, error) =>
                {
                    if (!string.IsNullOrEmpty(error))
                    {
                        Debug.LogError($"Failed to create deck: {error}");
                        return;
                    }

                    currentDeckId = deckId;
                    Debug.Log($"Deck created with ID: {deckId}");
                }));
            }
            else
            {
                yield return StartCoroutine(ApiClient.UpdateDeck(currentDeckId, allCardIds, (success, error) =>
                {
                    if (!string.IsNullOrEmpty(error))
                    {
                        Debug.LogError($"Failed to update deck: {error}");
                        return;
                    }

                    Debug.Log($"Deck {currentDeckId} updated successfully.");
                }));
            }
        }

        public void UpdateCounters()
        {
            adventurerCountText.text = $"Adventurers: {selectedAdventurers.Count}/{MaxAdventurers}";
            enemyCountText.text = $"Enemies: {selectedEnemies.Count}/{MaxEnemies}";
            roomCountText.text = $"Rooms: {selectedRooms.Count}/{MaxRooms}";
        }

        public void ValidateDeck()
        {
            StartCoroutine(ValidateDeckRoutine());
        }

        private IEnumerator ValidateDeckRoutine()
        {
            if (string.IsNullOrEmpty(currentDeckId))
            {
                Debug.LogWarning("No deck to validate. Save the deck first.");
                yield break;
            }

            yield return StartCoroutine(ApiClient.ValidateDeck(currentDeckId, (isValid, errors, error) =>
            {
                if (!string.IsNullOrEmpty(error))
                {
                    Debug.LogError($"Failed to validate deck: {error}");
                    return;
                }

                if (isValid)
                {
                    Debug.Log("Deck is valid!");
                }
                else
                {
                    Debug.LogWarning($"Deck validation failed: {string.Join(", ", errors)}");
                }
            }));
        }

        private void OnBackClicked()
        {
            GameManager.Instance.LoadScene("MainMenu");
        }
    }
}
