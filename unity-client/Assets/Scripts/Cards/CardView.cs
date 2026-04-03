using System.Collections;
using UnityEngine;
using UnityEngine.UI;
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
        [SerializeField] private GameObject statsPanel;
        [SerializeField] private TextMeshPro effectText;
        [SerializeField] private SpriteRenderer artworkImage;
        [SerializeField] private SpriteRenderer rarityBorder;

        [Header("Interaction Settings")]
        [SerializeField] private float hoverScaleMultiplier = 1.15f;
        [SerializeField] private float hoverScaleDuration = 0.15f;

        [Header("Highlight")]
        [SerializeField] private GameObject highlightOverlay;
        [SerializeField] private Color highlightColor = Color.yellow;

        private CardData data;
        private bool isFaceUp = true;
        private bool isInteractable = true;
        private bool isHovering;
        private Vector3 originalScale;
        private Coroutine hoverCoroutine;
        private CardAnimator cardAnimator;

        public CardData Data => data;

        public bool IsFaceUp
        {
            get => isFaceUp;
            private set => isFaceUp = value;
        }

        private void Awake()
        {
            originalScale = transform.localScale;
            cardAnimator = GetComponent<CardAnimator>();
        }

        public void SetData(CardData cardData)
        {
            data = cardData;
            if (data == null) return;

            if (nameText != null)
                nameText.text = data.CardName;

            if (effectText != null)
                effectText.text = GetEffectText(data);

            UpdateRarityBorder(data.Rarity);
            UpdateFaceVisibility();
        }

        public void Flip()
        {
            if (cardAnimator != null)
            {
                StartCoroutine(FlipWithAnimator());
            }
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

            // First half: rotate to 90 degrees (card becomes invisible edge-on)
            while (elapsed < duration * 0.5f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (duration * 0.5f);
                float smoothT = Mathf.SmoothStep(0f, 1f, t);
                float angle = Mathf.Lerp(startAngle, midAngle, smoothT);
                transform.localEulerAngles = new Vector3(
                    transform.localEulerAngles.x,
                    angle,
                    transform.localEulerAngles.z
                );
                yield return null;
            }

            // Swap face visibility at midpoint
            isFaceUp = targetFaceUp;
            UpdateFaceVisibility();

            // Second half: rotate from 90 to 180 degrees
            elapsed = 0f;
            while (elapsed < duration * 0.5f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (duration * 0.5f);
                float smoothT = Mathf.SmoothStep(0f, 1f, t);
                float angle = Mathf.Lerp(midAngle, endAngle, smoothT);
                transform.localEulerAngles = new Vector3(
                    transform.localEulerAngles.x,
                    angle,
                    transform.localEulerAngles.z
                );
                yield return null;
            }

            // Snap to final rotation
            transform.localEulerAngles = new Vector3(
                transform.localEulerAngles.x,
                endAngle,
                transform.localEulerAngles.z
            );
        }

        public void SetHighlight(bool highlighted)
        {
            if (highlightOverlay != null)
            {
                highlightOverlay.SetActive(highlighted);
            }

            if (rarityBorder != null && highlighted)
            {
                rarityBorder.color = highlightColor;
            }
        }

        public void SetInteractable(bool interactable)
        {
            isInteractable = interactable;
        }

        private void OnMouseEnter()
        {
            if (!isInteractable) return;

            isHovering = true;
            if (hoverCoroutine != null) StopCoroutine(hoverCoroutine);
            hoverCoroutine = StartCoroutine(ScaleTo(originalScale * hoverScaleMultiplier));

            EventBus.Publish(new CardHoverEvent(this, true));
        }

        private void OnMouseExit()
        {
            if (!isInteractable) return;

            isHovering = false;
            if (hoverCoroutine != null) StopCoroutine(hoverCoroutine);
            hoverCoroutine = StartCoroutine(ScaleTo(originalScale));

            EventBus.Publish(new CardHoverEvent(this, false));
        }

        private void OnMouseDown()
        {
            if (!isInteractable) return;

            EventBus.Publish(new CardClickedEvent(this));
        }

        private IEnumerator ScaleTo(Vector3 targetScale)
        {
            Vector3 startScale = transform.localScale;
            float elapsed = 0f;

            while (elapsed < hoverScaleDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / hoverScaleDuration);
                transform.localScale = Vector3.Lerp(startScale, targetScale, t);
                yield return null;
            }

            transform.localScale = targetScale;
        }

        private void UpdateFaceVisibility()
        {
            if (frontFace != null) frontFace.SetActive(isFaceUp);
            if (backFace != null) backFace.SetActive(!isFaceUp);
        }

        private void UpdateRarityBorder(Rarity rarity)
        {
            if (rarityBorder == null) return;

            rarityBorder.color = rarity switch
            {
                Rarity.Common => Color.gray,
                Rarity.Uncommon => Color.green,
                Rarity.Rare => Color.blue,
                Rarity.Unique => new Color(1f, 0.65f, 0f), // orange
                _ => Color.white
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
    }
}
