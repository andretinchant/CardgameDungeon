using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CardgameDungeon.Unity.Cards;
using CardgameDungeon.Unity.Core;
using CardgameDungeon.Unity.Network;

namespace CardgameDungeon.Unity.Board
{
    public class BoardManager : MonoBehaviour
    {
        [Header("Player Ally Slots (5)")]
        [SerializeField] private Transform playerFieldArea;
        [SerializeField] private float allySlotSpacing = 2.0f;

        [Header("Opponent/Monster Slots (5)")]
        [SerializeField] private Transform opponentFieldArea;

        [Header("Pile Positions")]
        [SerializeField] private Transform deckPosition;
        [SerializeField] private Transform discardPosition;
        [SerializeField] private Transform opponentDeckPosition;
        [SerializeField] private Transform opponentDiscardPosition;

        [Header("Pile Counters")]
        [SerializeField] private TextMeshPro deckCountText;
        [SerializeField] private TextMeshPro discardCountText;
        [SerializeField] private TextMeshPro opponentDeckCountText;
        [SerializeField] private TextMeshPro opponentDiscardCountText;

        [Header("Exile Indicators")]
        [SerializeField] private TextMeshPro playerExileCountText;
        [SerializeField] private TextMeshPro opponentExileCountText;

        [Header("Dungeon Room Indicator")]
        [SerializeField] private Transform dungeonTrackArea;
        [SerializeField] private TextMeshPro currentRoomText;
        [SerializeField] private DungeonLayout dungeonLayout;
        [SerializeField] private int totalRooms = 5;

        [Header("HP Labels per Slot")]
        [SerializeField] private TextMeshPro[] playerAllyHpLabels = new TextMeshPro[5];
        [SerializeField] private TextMeshPro[] opponentAllyHpLabels = new TextMeshPro[5];

        [Header("Slot Highlight")]
        [SerializeField] private SpriteRenderer[] playerSlotHighlights = new SpriteRenderer[5];
        [SerializeField] private Color validSlotColor = new Color(0.2f, 1f, 0.3f, 0.35f);
        [SerializeField] private Color invalidSlotColor = new Color(1f, 0f, 0f, 0.15f);

        [Header("References")]
        [SerializeField] private PlayerArea playerAreaController;
        [SerializeField] private PlayerArea opponentAreaController;

        private readonly CardView[] playerAllySlots = new CardView[5];
        private readonly CardView[] opponentAllySlots = new CardView[5];

        private int currentRoom = 1;

        public Transform DeckPosition => deckPosition;
        public Transform DiscardPosition => discardPosition;
        public Transform PlayerFieldArea => playerFieldArea;
        public Transform OpponentFieldArea => opponentFieldArea;

        // ── Lifecycle ──

        private void OnEnable()
        {
            EventBus.Subscribe<CardPlacedEvent>(OnCardPlaced);
            EventBus.Subscribe<AllyRemovedEvent>(OnAllyRemoved);
            EventBus.Subscribe<CardDroppedOnSlotEvent>(OnCardDroppedOnSlot);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<CardPlacedEvent>(OnCardPlaced);
            EventBus.Unsubscribe<AllyRemovedEvent>(OnAllyRemoved);
            EventBus.Unsubscribe<CardDroppedOnSlotEvent>(OnCardDroppedOnSlot);
        }

        // ── Setup ──

        public void SetupBoard(MatchResponse matchData)
        {
            if (matchData == null)
            {
                Debug.LogError("[BoardManager] SetupBoard called with null match data.");
                return;
            }

            ClearBoard();

            if (dungeonLayout != null && matchData.dungeonRooms != null)
                dungeonLayout.Initialize(matchData.dungeonRooms);

            if (playerAreaController != null && matchData.player != null)
                playerAreaController.UpdateStats(matchData.player);
            if (opponentAreaController != null && matchData.opponent != null)
                opponentAreaController.UpdateStats(matchData.opponent);

            UpdateDungeonRoom(matchData.currentRoom);
            UpdatePileCounts(matchData);
            UpdateExileCounts(matchData);

            Debug.Log($"[BoardManager] Board setup for match {matchData.matchId}");
        }

        // ── Ally Slots (player) ──

        public bool PlaceAlly(CardView card, int slot)
        {
            if (!IsValidSlot(slot) || playerAllySlots[slot] != null) return false;

            playerAllySlots[slot] = card;
            Transform target = GetSlotTransform(playerFieldArea, slot);

            CardAnimator animator = card.GetComponent<CardAnimator>();
            if (animator != null)
                animator.AnimatePlay(target);
            else
            {
                card.transform.position = target.position;
                card.transform.rotation = target.rotation;
            }

            UpdateAllyHpLabel(slot, true, card.Data);
            return true;
        }

        public CardView RemoveAlly(int slot)
        {
            if (!IsValidSlot(slot)) return null;
            CardView card = playerAllySlots[slot];
            playerAllySlots[slot] = null;
            ClearHpLabel(slot, true);
            return card;
        }

        // ── Opponent/Monster Slots ──

        public bool PlaceOpponentAlly(CardView card, int slot)
        {
            if (!IsValidSlot(slot) || opponentAllySlots[slot] != null) return false;

            opponentAllySlots[slot] = card;
            Transform target = GetSlotTransform(opponentFieldArea, slot);
            card.transform.position = target.position;
            card.transform.rotation = target.rotation;

            UpdateAllyHpLabel(slot, false, card.Data);
            return true;
        }

        public CardView RemoveOpponentAlly(int slot)
        {
            if (!IsValidSlot(slot)) return null;
            CardView card = opponentAllySlots[slot];
            opponentAllySlots[slot] = null;
            ClearHpLabel(slot, false);
            return card;
        }

        // ── Individual HP per slot ──

        public void UpdateAllyHp(int slot, bool isPlayer, int currentHp, int maxHp)
        {
            TextMeshPro[] labels = isPlayer ? playerAllyHpLabels : opponentAllyHpLabels;
            if (!IsValidSlot(slot) || labels == null || slot >= labels.Length || labels[slot] == null) return;

            labels[slot].text = $"{currentHp}/{maxHp}";

            float ratio = maxHp > 0 ? (float)currentHp / maxHp : 0f;
            labels[slot].color = ratio > 0.5f ? Color.white :
                                 ratio > 0.25f ? Color.yellow : Color.red;
        }

        private void UpdateAllyHpLabel(int slot, bool isPlayer, CardData data)
        {
            if (data is AllyCardData ally)
                UpdateAllyHp(slot, isPlayer, ally.HitPoints, ally.HitPoints);
            else if (data is MonsterCardData monster)
                UpdateAllyHp(slot, isPlayer, monster.HitPoints, monster.HitPoints);
            else if (data is BossCardData boss)
                UpdateAllyHp(slot, isPlayer, boss.HitPoints, boss.HitPoints);
        }

        private void ClearHpLabel(int slot, bool isPlayer)
        {
            TextMeshPro[] labels = isPlayer ? playerAllyHpLabels : opponentAllyHpLabels;
            if (IsValidSlot(slot) && labels != null && slot < labels.Length && labels[slot] != null)
                labels[slot].text = "";
        }

        // ── Exile Indicators (count public, contents hidden) ──

        public void UpdateExileCounts(int playerExile, int opponentExile)
        {
            if (playerExileCountText != null)
                playerExileCountText.text = $"Exile: {playerExile}";
            if (opponentExileCountText != null)
                opponentExileCountText.text = $"Exile: {opponentExile}";
        }

        private void UpdateExileCounts(MatchResponse data)
        {
            int pExile = data?.player?.exileCount ?? 0;
            int oExile = data?.opponent?.exileCount ?? 0;
            UpdateExileCounts(pExile, oExile);
        }

        // ── Pile Counters ──

        public void UpdatePileCounts(int deckCount, int discardCount, int oppDeck, int oppDiscard)
        {
            if (deckCountText != null) deckCountText.text = deckCount.ToString();
            if (discardCountText != null) discardCountText.text = discardCount.ToString();
            if (opponentDeckCountText != null) opponentDeckCountText.text = oppDeck.ToString();
            if (opponentDiscardCountText != null) opponentDiscardCountText.text = oppDiscard.ToString();
        }

        private void UpdatePileCounts(MatchResponse data)
        {
            UpdatePileCounts(
                data?.player?.deckCount ?? 0,
                data?.player?.discardCount ?? 0,
                data?.opponent?.deckCount ?? 0,
                data?.opponent?.discardCount ?? 0);
        }

        // ── Dungeon Room Indicator ──

        public void UpdateDungeonRoom(int room)
        {
            currentRoom = room;

            if (currentRoomText != null)
            {
                if (room > totalRooms)
                    currentRoomText.text = "BOSS";
                else
                    currentRoomText.text = $"Room {room}/{totalRooms}";
            }

            if (dungeonLayout != null)
                dungeonLayout.AdvanceRoom(room);
        }

        // ── Slot Highlights (for drag-drop targeting) ──

        public void ShowValidSlotHighlights(bool show)
        {
            if (playerSlotHighlights == null) return;

            for (int i = 0; i < playerSlotHighlights.Length; i++)
            {
                if (playerSlotHighlights[i] == null) continue;

                if (!show)
                {
                    playerSlotHighlights[i].color = Color.clear;
                    continue;
                }

                bool empty = playerAllySlots[i] == null;
                playerSlotHighlights[i].color = empty ? validSlotColor : invalidSlotColor;
            }
        }

        // ── Accessors ──

        public CardView GetPlayerAlly(int slot) => IsValidSlot(slot) ? playerAllySlots[slot] : null;
        public CardView GetOpponentAlly(int slot) => IsValidSlot(slot) ? opponentAllySlots[slot] : null;
        public int CurrentRoom => currentRoom;

        public int GetFirstEmptyPlayerSlot()
        {
            for (int i = 0; i < playerAllySlots.Length; i++)
                if (playerAllySlots[i] == null) return i;
            return -1;
        }

        public int GetFirstEmptyOpponentSlot()
        {
            for (int i = 0; i < opponentAllySlots.Length; i++)
                if (opponentAllySlots[i] == null) return i;
            return -1;
        }

        // ── Clear ──

        public void ClearBoard()
        {
            for (int i = 0; i < 5; i++)
            {
                DestroySlot(playerAllySlots, i);
                DestroySlot(opponentAllySlots, i);
                ClearHpLabel(i, true);
                ClearHpLabel(i, false);
            }

            ShowValidSlotHighlights(false);
            EventBus.Publish(new BoardClearedEvent());
        }

        private void DestroySlot(CardView[] slots, int i)
        {
            if (slots[i] != null)
            {
                Destroy(slots[i].gameObject);
                slots[i] = null;
            }
        }

        // ── Event Handlers ──

        private void OnCardPlaced(CardPlacedEvent evt)
        {
            if (evt.IsPlayerCard)
                PlaceAlly(evt.Card, evt.SlotIndex);
            else
                PlaceOpponentAlly(evt.Card, evt.SlotIndex);
        }

        private void OnAllyRemoved(AllyRemovedEvent evt)
        {
            if (evt.IsPlayerCard)
                RemoveAlly(evt.SlotIndex);
            else
                RemoveOpponentAlly(evt.SlotIndex);
        }

        private void OnCardDroppedOnSlot(CardDroppedOnSlotEvent evt)
        {
            for (int i = 0; i < 5; i++)
            {
                Transform slotTransform = GetSlotTransform(playerFieldArea, i);
                if (slotTransform == null) continue;

                float dist = Vector3.Distance(evt.SlotObject.transform.position, slotTransform.position);
                if (dist < 1.5f && playerAllySlots[i] == null)
                {
                    PlaceAlly(evt.Card, i);
                    ShowValidSlotHighlights(false);
                    EventBus.Publish(new CardPlayedEvent(evt.Card, i));
                    return;
                }
            }

            ShowValidSlotHighlights(false);
        }

        // ── Helpers ──

        private bool IsValidSlot(int slot) => slot >= 0 && slot < 5;

        private Transform GetSlotTransform(Transform area, int slot)
        {
            if (area == null || slot < 0) return null;

            Transform existing = area.Find($"AllySlot_{slot}");
            if (existing != null) return existing;

            float startX = -2f * allySlotSpacing;
            Vector3 pos = area.position + new Vector3(startX + slot * allySlotSpacing, 0f, 0f);

            GameObject go = new GameObject($"AllySlot_{slot}");
            go.transform.SetParent(area);
            go.transform.position = pos;
            return go.transform;
        }
    }
}
