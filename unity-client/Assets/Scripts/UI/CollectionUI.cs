using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CardgameDungeon.Unity.Cards;
using CardgameDungeon.Unity.Core;
using CardgameDungeon.Unity.Network;

namespace CardgameDungeon.Unity.UI
{
    public class CollectionUI : MonoBehaviour
    {
        [Header("Grid")]
        [SerializeField] private Transform cardGrid;

        [Header("Stats")]
        [SerializeField] private TMP_Text totalCardsText;
        [SerializeField] private TMP_Text availableCardsText;

        [Header("Detail")]
        [SerializeField] private GameObject cardDetailPanel;

        [Header("Navigation")]
        [SerializeField] private Button backButton;

        private void OnEnable()
        {
            backButton.onClick.AddListener(OnBackClicked);
            LoadCollection();
        }

        private void OnDisable()
        {
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

                foreach (Transform child in cardGrid)
                {
                    Destroy(child.gameObject);
                }

                int totalCount = 0;
                int availableCount = 0;

                foreach (var card in cards)
                {
                    totalCount++;
                    availableCount++;

                    var cardView = GameManager.Instance.CreateCardView(card, cardGrid);
                    var button = cardView.GetComponent<Button>();
                    if (button != null)
                    {
                        var capturedCard = card;
                        button.onClick.AddListener(() => ShowCardDetail(capturedCard));
                    }
                }

                totalCardsText.text = $"Total Cards: {totalCount}";
                availableCardsText.text = $"Available: {availableCount}";
            }));
        }

        public void ShowCardDetail(CardData card)
        {
            if (card == null) return;

            cardDetailPanel.SetActive(true);

            var nameText = cardDetailPanel.transform.Find("CardName")?.GetComponent<TMP_Text>();
            var typeText = cardDetailPanel.transform.Find("CardType")?.GetComponent<TMP_Text>();
            var rarityText = cardDetailPanel.transform.Find("CardRarity")?.GetComponent<TMP_Text>();
            var costText = cardDetailPanel.transform.Find("CardCost")?.GetComponent<TMP_Text>();
            var statsText = cardDetailPanel.transform.Find("CardStats")?.GetComponent<TMP_Text>();

            if (nameText != null) nameText.text = card.CardName;
            if (typeText != null) typeText.text = $"Type: {card.CardType}";
            if (rarityText != null) rarityText.text = $"Rarity: {card.Rarity}";
            if (costText != null) costText.text = $"Cost: {card.Cost}";

            if (statsText != null)
            {
                string stats = "";

                if (card is AllyCardData ally)
                {
                    stats = $"STR: {ally.Strength}  HP: {ally.HitPoints}  INIT: {ally.Initiative}\n" +
                            $"Treasure: {ally.Treasure}  Ambusher: {ally.IsAmbusher}\n" +
                            $"{ally.Effect}";
                }
                else if (card is MonsterCardData monster)
                {
                    stats = $"STR: {monster.Strength}  HP: {monster.HitPoints}  INIT: {monster.Initiative}\n" +
                            $"Treasure: {monster.Treasure}\n" +
                            $"{monster.Effect}";
                }
                else if (card is EquipmentCardData equipment)
                {
                    stats = $"STR Mod: {equipment.StrengthModifier}  HP Mod: {equipment.HitPointsModifier}\n" +
                            $"INIT Mod: {equipment.InitiativeModifier}";
                }
                else if (card is TrapCardData trap)
                {
                    stats = $"Damage: {trap.Damage}\n{trap.Effect}";
                }
                else if (card is DungeonRoomCardData room)
                {
                    stats = $"Order: {room.Order}  Monster Budget: {room.MonsterCostBudget}\n" +
                            $"{room.Effect}";
                }
                else if (card is BossCardData boss)
                {
                    stats = $"STR: {boss.Strength}  HP: {boss.HitPoints}  INIT: {boss.Initiative}\n" +
                            $"{boss.Effect}";
                }

                statsText.text = stats;
            }

            var closeButton = cardDetailPanel.transform.Find("CloseButton")?.GetComponent<Button>();
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(() => cardDetailPanel.SetActive(false));
            }
        }

        public void OpenBooster()
        {
            StartCoroutine(OpenBoosterRoutine());
        }

        private IEnumerator OpenBoosterRoutine()
        {
            yield return StartCoroutine(ApiClient.OpenBooster((cards, error) =>
            {
                if (!string.IsNullOrEmpty(error))
                {
                    Debug.LogError($"Failed to open booster: {error}");
                    return;
                }

                StartCoroutine(AnimateBoosterReveal(cards));
            }));
        }

        private IEnumerator AnimateBoosterReveal(CardData[] cards)
        {
            foreach (var card in cards)
            {
                var cardView = GameManager.Instance.CreateCardView(card, cardGrid);

                var canvasGroup = cardView.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = cardView.AddComponent<CanvasGroup>();
                }

                canvasGroup.alpha = 0f;
                float elapsed = 0f;
                float duration = 0.4f;

                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    canvasGroup.alpha = Mathf.Clamp01(elapsed / duration);
                    yield return null;
                }

                canvasGroup.alpha = 1f;
                yield return new WaitForSeconds(0.2f);
            }

            LoadCollection();
        }

        private void OnBackClicked()
        {
            GameManager.Instance.LoadScene("MainMenu");
        }
    }
}
