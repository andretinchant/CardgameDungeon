using System.Collections.Generic;
using UnityEngine;
using CardgameDungeon.Unity.Cards;
using CardgameDungeon.Unity.Core;
using CardgameDungeon.Unity.Network;

namespace CardgameDungeon.Unity.Board
{
    public class BoardManager : MonoBehaviour
    {
        [Header("Board Areas")]
        [SerializeField] private Transform playerArea;
        [SerializeField] private Transform opponentArea;
        [SerializeField] private Transform dungeonTrack;

        [Header("Pile Positions")]
        [SerializeField] private Transform deckPosition;
        [SerializeField] private Transform discardPosition;
        [SerializeField] private Transform exilePosition;

        [Header("Layout Settings")]
        [SerializeField] private int maxAllySlots = 5;
        [SerializeField] private int maxDungeonRooms = 6;

        [Header("References")]
        [SerializeField] private DungeonLayout dungeonLayout;
        [SerializeField] private PlayerArea playerAreaController;
        [SerializeField] private PlayerArea opponentAreaController;

        private readonly CardView[] playerAllySlots = new CardView[5];
        private readonly CardView[] opponentAllySlots = new CardView[5];
        private readonly CardView[] dungeonRoomSlots = new CardView[6];

        public Transform DeckPosition => deckPosition;
        public Transform DiscardPosition => discardPosition;
        public Transform ExilePosition => exilePosition;
        public Transform PlayerAreaTransform => playerArea;
        public Transform OpponentAreaTransform => opponentArea;

        private void OnEnable()
        {
            EventBus.Subscribe<CardPlacedEvent>(OnCardPlaced);
            EventBus.Subscribe<AllyRemovedEvent>(OnAllyRemoved);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<CardPlacedEvent>(OnCardPlaced);
            EventBus.Unsubscribe<AllyRemovedEvent>(OnAllyRemoved);
        }

        public void SetupBoard(MatchResponse matchData)
        {
            if (matchData == null)
            {
                Debug.LogError("[BoardManager] SetupBoard called with null match data.");
                return;
            }

            ClearBoard();

            // Initialize dungeon track
            if (dungeonLayout != null && matchData.dungeonRooms != null)
            {
                dungeonLayout.Initialize(matchData.dungeonRooms);
            }

            // Update player areas
            if (playerAreaController != null && matchData.player != null)
            {
                playerAreaController.UpdateStats(matchData.player);
            }

            if (opponentAreaController != null && matchData.opponent != null)
            {
                opponentAreaController.UpdateStats(matchData.opponent);
            }

            // Set current dungeon room
            UpdateDungeonTrack(matchData.currentRoom);

            Debug.Log($"[BoardManager] Board setup complete for match {matchData.matchId}");
        }

        public bool PlaceAlly(CardView card, int slot)
        {
            if (slot < 0 || slot >= maxAllySlots)
            {
                Debug.LogWarning($"[BoardManager] Invalid ally slot: {slot}");
                return false;
            }

            if (playerAllySlots[slot] != null)
            {
                Debug.LogWarning($"[BoardManager] Ally slot {slot} is already occupied.");
                return false;
            }

            playerAllySlots[slot] = card;

            // Position the card in the slot
            Transform slotTransform = GetPlayerAllySlotTransform(slot);
            if (slotTransform != null)
            {
                CardAnimator animator = card.GetComponent<CardAnimator>();
                if (animator != null)
                {
                    animator.AnimatePlay(slotTransform);
                }
                else
                {
                    card.transform.position = slotTransform.position;
                    card.transform.rotation = slotTransform.rotation;
                }
            }

            return true;
        }

        public CardView RemoveAlly(int slot)
        {
            if (slot < 0 || slot >= maxAllySlots)
            {
                Debug.LogWarning($"[BoardManager] Invalid ally slot: {slot}");
                return null;
            }

            CardView card = playerAllySlots[slot];
            playerAllySlots[slot] = null;
            return card;
        }

        public bool PlaceOpponentAlly(CardView card, int slot)
        {
            if (slot < 0 || slot >= maxAllySlots)
            {
                Debug.LogWarning($"[BoardManager] Invalid opponent ally slot: {slot}");
                return false;
            }

            if (opponentAllySlots[slot] != null)
            {
                Debug.LogWarning($"[BoardManager] Opponent ally slot {slot} is already occupied.");
                return false;
            }

            opponentAllySlots[slot] = card;

            Transform slotTransform = GetOpponentAllySlotTransform(slot);
            if (slotTransform != null)
            {
                card.transform.position = slotTransform.position;
                card.transform.rotation = slotTransform.rotation;
            }

            return true;
        }

        public void UpdateDungeonTrack(int currentRoom)
        {
            if (dungeonLayout != null)
            {
                dungeonLayout.AdvanceRoom(currentRoom);
            }
        }

        public void ClearBoard()
        {
            // Clear player ally slots
            for (int i = 0; i < playerAllySlots.Length; i++)
            {
                if (playerAllySlots[i] != null)
                {
                    Destroy(playerAllySlots[i].gameObject);
                    playerAllySlots[i] = null;
                }
            }

            // Clear opponent ally slots
            for (int i = 0; i < opponentAllySlots.Length; i++)
            {
                if (opponentAllySlots[i] != null)
                {
                    Destroy(opponentAllySlots[i].gameObject);
                    opponentAllySlots[i] = null;
                }
            }

            // Clear dungeon room slots
            for (int i = 0; i < dungeonRoomSlots.Length; i++)
            {
                if (dungeonRoomSlots[i] != null)
                {
                    Destroy(dungeonRoomSlots[i].gameObject);
                    dungeonRoomSlots[i] = null;
                }
            }

            EventBus.Publish(new BoardClearedEvent());
        }

        public CardView GetPlayerAlly(int slot)
        {
            if (slot < 0 || slot >= maxAllySlots) return null;
            return playerAllySlots[slot];
        }

        public CardView GetOpponentAlly(int slot)
        {
            if (slot < 0 || slot >= maxAllySlots) return null;
            return opponentAllySlots[slot];
        }

        public int GetFirstEmptyPlayerSlot()
        {
            for (int i = 0; i < playerAllySlots.Length; i++)
            {
                if (playerAllySlots[i] == null) return i;
            }
            return -1;
        }

        private void OnCardPlaced(CardPlacedEvent evt)
        {
            if (evt.IsPlayerCard)
            {
                PlaceAlly(evt.Card, evt.SlotIndex);
            }
            else
            {
                PlaceOpponentAlly(evt.Card, evt.SlotIndex);
            }
        }

        private void OnAllyRemoved(AllyRemovedEvent evt)
        {
            if (evt.IsPlayerCard)
            {
                RemoveAlly(evt.SlotIndex);
            }
            else
            {
                if (evt.SlotIndex >= 0 && evt.SlotIndex < opponentAllySlots.Length)
                {
                    opponentAllySlots[evt.SlotIndex] = null;
                }
            }
        }

        private Transform GetPlayerAllySlotTransform(int slot)
        {
            if (playerArea == null || slot < 0) return null;

            // Look for child named "AllySlot_X"
            Transform slotTransform = playerArea.Find($"AllySlot_{slot}");
            if (slotTransform != null) return slotTransform;

            // Fallback: calculate position based on slot index
            float spacing = 2.0f;
            float startX = -(maxAllySlots - 1) * spacing * 0.5f;
            Vector3 position = playerArea.position + new Vector3(startX + slot * spacing, 0f, 0f);

            // Create a temporary transform target
            GameObject temp = new GameObject($"AllySlot_{slot}");
            temp.transform.SetParent(playerArea);
            temp.transform.position = position;
            return temp.transform;
        }

        private Transform GetOpponentAllySlotTransform(int slot)
        {
            if (opponentArea == null || slot < 0) return null;

            Transform slotTransform = opponentArea.Find($"AllySlot_{slot}");
            if (slotTransform != null) return slotTransform;

            float spacing = 2.0f;
            float startX = -(maxAllySlots - 1) * spacing * 0.5f;
            Vector3 position = opponentArea.position + new Vector3(startX + slot * spacing, 0f, 0f);

            GameObject temp = new GameObject($"AllySlot_{slot}");
            temp.transform.SetParent(opponentArea);
            temp.transform.position = position;
            return temp.transform;
        }
    }
}
