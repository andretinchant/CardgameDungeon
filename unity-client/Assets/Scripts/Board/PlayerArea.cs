using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CardgameDungeon.Unity.Cards;
using CardgameDungeon.Unity.Core;
using CardgameDungeon.Unity.Network;

namespace CardgameDungeon.Unity.Board
{
    public class PlayerArea : MonoBehaviour
    {
        [Header("Player Info UI")]
        [SerializeField] private TextMeshPro playerNameText;
        [SerializeField] private TextMeshPro hpText;
        [SerializeField] private TextMeshPro deckCountText;
        [SerializeField] private TextMeshPro discardCountText;
        [SerializeField] private TextMeshPro exileCountText;

        [Header("Hand Area")]
        [SerializeField] private Transform handArea;
        [SerializeField] private float handCardSpacing = 1.5f;
        [SerializeField] private float handArcHeight = 0.3f;
        [SerializeField] private float handArcAngle = 5f;
        [SerializeField] private int maxHandSize = 10;

        [Header("Field Area")]
        [SerializeField] private Transform fieldArea;
        [SerializeField] private float fieldSlotSpacing = 2.0f;
        [SerializeField] private int maxFieldSlots = 5;

        private readonly List<CardView> handCards = new List<CardView>();
        private readonly CardView[] fieldSlots = new CardView[5];

        public IReadOnlyList<CardView> HandCards => handCards;
        public int HandCount => handCards.Count;
        public int FieldCount
        {
            get
            {
                int count = 0;
                foreach (CardView slot in fieldSlots)
                {
                    if (slot != null) count++;
                }
                return count;
            }
        }

        public void UpdateStats(PlayerStateDto playerState)
        {
            if (playerState == null) return;

            if (playerNameText != null)
                playerNameText.text = playerState.playerName ?? "";

            if (hpText != null)
                hpText.text = $"HP: {playerState.hitPoints}/{playerState.maxHitPoints}";

            if (deckCountText != null)
                deckCountText.text = $"Deck: {playerState.deckCount}";

            if (discardCountText != null)
                discardCountText.text = $"Discard: {playerState.discardCount}";

            if (exileCountText != null)
                exileCountText.text = $"Exile: {playerState.exileCount}";
        }

        public void AddCardToHand(CardView card)
        {
            if (card == null)
            {
                Debug.LogWarning("[PlayerArea] Attempted to add null card to hand.");
                return;
            }

            if (handCards.Count >= maxHandSize)
            {
                Debug.LogWarning("[PlayerArea] Hand is full, cannot add more cards.");
                return;
            }

            handCards.Add(card);
            card.transform.SetParent(handArea);
            ArrangeHand();
        }

        public bool RemoveCardFromHand(CardView card)
        {
            if (card == null) return false;

            bool removed = handCards.Remove(card);
            if (removed)
            {
                ArrangeHand();
            }
            return removed;
        }

        public void PlayAllyToField(CardView card, int slot)
        {
            if (card == null)
            {
                Debug.LogWarning("[PlayerArea] Attempted to play null card to field.");
                return;
            }

            if (slot < 0 || slot >= maxFieldSlots)
            {
                Debug.LogWarning($"[PlayerArea] Invalid field slot: {slot}");
                return;
            }

            if (fieldSlots[slot] != null)
            {
                Debug.LogWarning($"[PlayerArea] Field slot {slot} is already occupied.");
                return;
            }

            // Remove from hand if present
            RemoveCardFromHand(card);

            // Place on field
            fieldSlots[slot] = card;
            card.transform.SetParent(fieldArea);

            Transform slotTransform = GetFieldSlotTransform(slot);

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

            EventBus.Publish(new CardPlayedEvent(card, slot));
        }

        public CardView RemoveAllyFromField(int slot)
        {
            if (slot < 0 || slot >= maxFieldSlots)
            {
                Debug.LogWarning($"[PlayerArea] Invalid field slot: {slot}");
                return null;
            }

            CardView card = fieldSlots[slot];
            fieldSlots[slot] = null;
            return card;
        }

        public int GetFirstEmptyFieldSlot()
        {
            for (int i = 0; i < fieldSlots.Length; i++)
            {
                if (fieldSlots[i] == null) return i;
            }
            return -1;
        }

        public CardView GetFieldAlly(int slot)
        {
            if (slot < 0 || slot >= maxFieldSlots) return null;
            return fieldSlots[slot];
        }

        private void ArrangeHand()
        {
            if (handArea == null) return;

            int count = handCards.Count;
            if (count == 0) return;

            float totalWidth = (count - 1) * handCardSpacing;
            float startX = -totalWidth * 0.5f;

            for (int i = 0; i < count; i++)
            {
                CardView card = handCards[i];
                if (card == null) continue;

                // Calculate position along an arc
                float t = count > 1 ? (float)i / (count - 1) : 0.5f;
                float normalizedT = t * 2f - 1f; // Range: -1 to 1

                float xPos = startX + i * handCardSpacing;
                float yPos = -Mathf.Abs(normalizedT) * handArcHeight + handArcHeight;
                float zPos = -i * 0.01f; // Slight z-offset for layering

                card.transform.localPosition = new Vector3(xPos, yPos, zPos);

                // Slight rotation for fan effect
                float angle = -normalizedT * handArcAngle;
                card.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
            }
        }

        private Transform GetFieldSlotTransform(int slot)
        {
            if (fieldArea == null) return transform;

            // Look for named slot
            Transform slotTransform = fieldArea.Find($"FieldSlot_{slot}");
            if (slotTransform != null) return slotTransform;

            // Calculate position
            float totalWidth = (maxFieldSlots - 1) * fieldSlotSpacing;
            float startX = -totalWidth * 0.5f;
            Vector3 position = fieldArea.position + new Vector3(startX + slot * fieldSlotSpacing, 0f, 0f);

            GameObject slotObj = new GameObject($"FieldSlot_{slot}");
            slotObj.transform.SetParent(fieldArea);
            slotObj.transform.position = position;

            return slotObj.transform;
        }

        public void ClearHand()
        {
            foreach (CardView card in handCards)
            {
                if (card != null)
                {
                    Destroy(card.gameObject);
                }
            }
            handCards.Clear();
        }

        public void ClearField()
        {
            for (int i = 0; i < fieldSlots.Length; i++)
            {
                if (fieldSlots[i] != null)
                {
                    Destroy(fieldSlots[i].gameObject);
                    fieldSlots[i] = null;
                }
            }
        }
    }
}
