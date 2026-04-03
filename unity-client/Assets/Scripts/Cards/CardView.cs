using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using CardgameDungeon.Unity.Core;

namespace CardgameDungeon.Unity.Cards
{
    public class CardView : MonoBehaviour
    {
        [Header("Card Faces")]
        [SerializeField] private GameObject frontFace;
        [SerializeField] private GameObject backFace;

        [Header("Card UI Elements")]
        [SerializeField] private TextMeshPro nameText;
        [SerializeField] private TextMeshPro costText;
        [SerializeField] private TextMeshPro statsText;
        [SerializeField] private TextMeshPro effectText;
        [SerializeField] private SpriteRenderer artworkImage;
        [SerializeField] private SpriteRenderer rarityBorder;

        [Header("Glow")]
        [SerializeField] private SpriteRenderer glowRenderer;

        [Header("Tooltip")]
        [SerializeField] private GameObject tooltipPanel;
        [SerializeField] private TextMeshPro tooltipText;
        [SerializeField] private Vector3 tooltipOffset = new Vector3(2.5f, 0.5f, 0f);

        [Header("Hover Settings")]
        [SerializeField] private float hoverLiftY = 0.4f;
        [SerializeField] private float hoverDuration = 0.12f;
        [SerializeField] private float hoverScaleMultiplier = 1.1f;

        [Header("Drag Settings")]
        [SerializeField] private float dragZOffset = -1f;
        [SerializeField] private LayerMask fieldSlotMask;

        [Header("Highlight")]
        [SerializeField] private GameObject highlightOverlay;

        private CardData data;
        private bool isFaceUp = true;
        private bool isInteractable = true;
        private bool isHovering;
        private bool isDragging;
        private Vector3 originalScale;
        private Vector3 originalPosition;
        private int originalSortingOrder;
        private Coroutine hoverCoroutine;
        private Coroutine glowPulseCoroutine;
        private CardAnimator cardAnimator;
        private Camera mainCamera;

        public CardData Data => data;
        public bool IsFaceUp => isFaceUp;
        public bool IsDragging => isDragging;

        private void Awake()
        {
            originalScale = transform.localScale;
            cardAnimator = GetComponent<CardAnimator>();
            mainCamera = Camera.main;

            if (tooltipPanel != null) tooltipPanel.SetActive(false);
        }

        // ── Data Binding ──

        public void SetData(CardData cardData)
        {
            data = cardData;
            if (data == null) return;

            if (nameText != null) nameText.text = data.CardName;
            if (costText != null) costText.text = data.Cost.ToString();
            if (effectText != null) effectText.text = GetEffectText(data);
            if (statsText != null) statsText.text = GetStatsLine(data);

            UpdateRarityGlow(data.Rarity);
            UpdateFaceVisibility();
        }

        // ── Flip ──

        public void Flip()
        {
            if (cardAnimator != null)
                StartCoroutine(FlipWithAnimator());
            else
            {
                isFaceUp = !isFaceUp;
                UpdateFaceVisibility();
            }
        }

        private IEnumerator FlipWithAnimator()
        {
            bool targetFaceUp = !isFaceUp;
            float duration = 0.4f;
            float elapsed = 0f;
            float startAngle = transform.localEulerAngles.y;
            float midAngle = startAngle + 90f;
            float endAngle = startAngle + 180f;

            while (elapsed < duration * 0.5f)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / (duration * 0.5f));
                Vector3 e = transform.localEulerAngles;
                e.y = Mathf.Lerp(startAngle, midAngle, t);
                transform.localEulerAngles = e;
                yield return null;
            }

            isFaceUp = targetFaceUp;
            UpdateFaceVisibility();

            elapsed = 0f;
            while (elapsed < duration * 0.5f)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / (duration * 0.5f));
                Vector3 e = transform.localEulerAngles;
                e.y = Mathf.Lerp(midAngle, endAngle, t);
                transform.localEulerAngles = e;
                yield return null;
            }

            Vector3 final = transform.localEulerAngles;
            final.y = endAngle;
            transform.localEulerAngles = final;
        }

        // ── Hover: lift + scale + tooltip ──

        private void OnMouseEnter()
        {
            if (!isInteractable || isDragging) return;
            isHovering = true;

            originalPosition = transform.localPosition;
            if (hoverCoroutine != null) StopCoroutine(hoverCoroutine);
            hoverCoroutine = StartCoroutine(HoverLift(true));

            ShowTooltip(true);
            EventBus.Publish(new CardHoverEvent(this, true));
        }

        private void OnMouseExit()
        {
            if (!isInteractable) return;
            isHovering = false;

            if (!isDragging)
            {
                if (hoverCoroutine != null) StopCoroutine(hoverCoroutine);
                hoverCoroutine = StartCoroutine(HoverLift(false));
            }

            ShowTooltip(false);
            EventBus.Publish(new CardHoverEvent(this, false));
        }

        private IEnumerator HoverLift(bool lifting)
        {
            Vector3 startPos = transform.localPosition;
            Vector3 startScale = transform.localScale;

            Vector3 targetPos = lifting
                ? originalPosition + Vector3.up * hoverLiftY
                : originalPosition;
            Vector3 targetScale = lifting
                ? originalScale * hoverScaleMultiplier
                : originalScale;

            float elapsed = 0f;
            while (elapsed < hoverDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / hoverDuration);
                transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
                transform.localScale = Vector3.Lerp(startScale, targetScale, t);
                yield return null;
            }

            transform.localPosition = targetPos;
            transform.localScale = targetScale;
        }

        // ── Tooltip ──

        private void ShowTooltip(bool show)
        {
            if (tooltipPanel == null || data == null) return;

            tooltipPanel.SetActive(show);
            if (!show) return;

            tooltipPanel.transform.localPosition = tooltipOffset;

            if (tooltipText != null)
                tooltipText.text = BuildTooltipText(data);
        }

        private string BuildTooltipText(CardData card)
        {
            string header = $"<b>{card.CardName}</b>\n" +
                            $"<size=70%>{card.Rarity} {card.CardType}  |  Cost: {card.Cost}</size>\n";

            string body = card switch
            {
                AllyCardData a => $"STR: {a.Strength}  HP: {a.HitPoints}  INIT: {a.Initiative}\n" +
                                  (a.IsAmbusher ? "<color=#FF8800>Ambusher</color>\n" : "") +
                                  (a.Treasure > 0 ? $"Treasure: {a.Treasure}\n" : "") +
                                  (!string.IsNullOrEmpty(a.Effect) ? $"<i>{a.Effect}</i>" : ""),

                EquipmentCardData e => $"STR: {Signed(e.StrengthModifier)}  HP: {Signed(e.HitPointsModifier)}  " +
                                       $"INIT: {Signed(e.InitiativeModifier)}",

                MonsterCardData m => $"STR: {m.Strength}  HP: {m.HitPoints}  INIT: {m.Initiative}\n" +
                                     (m.Treasure > 0 ? $"Treasure: {m.Treasure}\n" : "") +
                                     (!string.IsNullOrEmpty(m.Effect) ? $"<i>{m.Effect}</i>" : ""),

                TrapCardData t => $"Damage: {t.Damage}\n" +
                                  (!string.IsNullOrEmpty(t.Effect) ? $"<i>{t.Effect}</i>" : ""),

                DungeonRoomCardData r => $"Room #{r.Order}  Budget: {r.MonsterCostBudget}\n" +
                                          (!string.IsNullOrEmpty(r.Effect) ? $"<i>{r.Effect}</i>" : ""),

                BossCardData b => $"STR: {b.Strength}  HP: {b.HitPoints}  INIT: {b.Initiative}\n" +
                                  (!string.IsNullOrEmpty(b.Effect) ? $"<i>{b.Effect}</i>" : ""),

                _ => ""
            };

            return header + body;
        }

        private static string Signed(int val) => val >= 0 ? $"+{val}" : val.ToString();

        // ── Drag and Drop ──

        private void OnMouseDown()
        {
            if (!isInteractable) return;

            isDragging = true;
            originalPosition = transform.localPosition;
            originalSortingOrder = GetSortingOrder();
            SetSortingOrder(1000);

            ShowTooltip(false);
            EventBus.Publish(new CardClickedEvent(this));
        }

        private void OnMouseDrag()
        {
            if (!isDragging || mainCamera == null) return;

            Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = transform.position.z + dragZOffset;
            transform.position = mouseWorld;
        }

        private void OnMouseUp()
        {
            if (!isDragging) return;
            isDragging = false;

            SetSortingOrder(originalSortingOrder);

            // Check if dropped on a valid field slot
            bool played = TryDropOnFieldSlot();

            if (!played)
            {
                // Return to original position
                if (hoverCoroutine != null) StopCoroutine(hoverCoroutine);
                hoverCoroutine = StartCoroutine(ReturnToPosition(originalPosition));
            }
        }

        private bool TryDropOnFieldSlot()
        {
            if (mainCamera == null) return false;

            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f, fieldSlotMask);

            if (hit.collider != null)
            {
                // Publish drop event — BoardManager handles the logic
                EventBus.Publish(new CardDroppedOnSlotEvent(this, hit.collider.gameObject));
                return true;
            }

            return false;
        }

        private IEnumerator ReturnToPosition(Vector3 target)
        {
            Vector3 start = transform.localPosition;
            float elapsed = 0f;
            float duration = 0.2f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                transform.localPosition = Vector3.Lerp(start, target, t);
                yield return null;
            }

            transform.localPosition = target;
            transform.localScale = originalScale;
        }

        // ── Rarity Glow ──

        private void UpdateRarityGlow(Rarity rarity)
        {
            if (glowRenderer == null && rarityBorder == null) return;

            Color glowColor;
            bool pulse = false;

            switch (rarity)
            {
                case Rarity.Common:
                    glowColor = new Color(0.85f, 0.75f, 0.4f, 0.3f); // dim gold
                    break;
                case Rarity.Uncommon:
                    glowColor = new Color(1f, 0.85f, 0.2f, 0.6f); // vivid gold
                    break;
                case Rarity.Rare:
                    glowColor = new Color(0.6f, 0.1f, 0.9f, 0.6f); // purple
                    break;
                case Rarity.Unique:
                    glowColor = new Color(0.7f, 0f, 1f, 0.8f); // bright purple
                    pulse = true;
                    break;
                default:
                    glowColor = Color.clear;
                    break;
            }

            if (glowRenderer != null) glowRenderer.color = glowColor;
            if (rarityBorder != null) rarityBorder.color = glowColor;

            if (pulse)
                StartGlowPulse(glowColor);
            else
                StopGlowPulse();
        }

        private void StartGlowPulse(Color baseColor)
        {
            StopGlowPulse();
            glowPulseCoroutine = StartCoroutine(GlowPulseLoop(baseColor));
        }

        private void StopGlowPulse()
        {
            if (glowPulseCoroutine != null)
            {
                StopCoroutine(glowPulseCoroutine);
                glowPulseCoroutine = null;
            }
        }

        private IEnumerator GlowPulseLoop(Color baseColor)
        {
            SpriteRenderer target = glowRenderer != null ? glowRenderer : rarityBorder;
            if (target == null) yield break;

            Color dim = baseColor;
            dim.a *= 0.4f;

            while (true)
            {
                float t = (Mathf.Sin(Time.time * 3f) + 1f) * 0.5f;
                target.color = Color.Lerp(dim, baseColor, t);
                yield return null;
            }
        }

        // ── Highlight ──

        public void SetHighlight(bool highlighted)
        {
            if (highlightOverlay != null)
                highlightOverlay.SetActive(highlighted);
        }

        public void SetInteractable(bool interactable) => isInteractable = interactable;

        // ── Helpers ──

        private void UpdateFaceVisibility()
        {
            if (frontFace != null) frontFace.SetActive(isFaceUp);
            if (backFace != null) backFace.SetActive(!isFaceUp);
        }

        private int GetSortingOrder()
        {
            var sr = GetComponent<SpriteRenderer>();
            return sr != null ? sr.sortingOrder : 0;
        }

        private void SetSortingOrder(int order)
        {
            foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
                sr.sortingOrder = order;
        }

        private string GetStatsLine(CardData cardData)
        {
            return cardData switch
            {
                AllyCardData a => $"{a.Strength}/{a.HitPoints}/{a.Initiative}",
                MonsterCardData m => $"{m.Strength}/{m.HitPoints}/{m.Initiative}",
                BossCardData b => $"{b.Strength}/{b.HitPoints}/{b.Initiative}",
                EquipmentCardData e => $"{Signed(e.StrengthModifier)}/{Signed(e.HitPointsModifier)}/{Signed(e.InitiativeModifier)}",
                TrapCardData t => $"DMG:{t.Damage}",
                _ => ""
            };
        }

        private string GetEffectText(CardData cardData)
        {
            return cardData switch
            {
                AllyCardData ally => ally.Effect,
                MonsterCardData monster => monster.Effect,
                TrapCardData trap => trap.Effect,
                DungeonRoomCardData room => room.Effect,
                BossCardData boss => boss.Effect,
                _ => string.Empty
            };
        }

        private void OnDestroy()
        {
            StopGlowPulse();
        }
    }

    // Event for drag-drop onto field slots
    public struct CardDroppedOnSlotEvent
    {
        public readonly CardView Card;
        public readonly GameObject SlotObject;

        public CardDroppedOnSlotEvent(CardView card, GameObject slotObject)
        {
            Card = card;
            SlotObject = slotObject;
        }
    }
}
