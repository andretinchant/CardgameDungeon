using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CardgameDungeon.Unity.Effects
{
    public class ScreenEffects : MonoBehaviour
    {
        [Header("Screen Flash")]
        [SerializeField] private Image screenFlashImage;

        [Header("Camera Shake")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private float defaultShakeIntensity = 0.15f;
        [SerializeField] private float defaultShakeDuration = 0.3f;

        [Header("Fade")]
        [SerializeField] private Image fadeOverlay;
        [SerializeField] private float defaultFadeDuration = 0.5f;

        [Header("Damage Numbers")]
        [SerializeField] private Canvas worldSpaceCanvas;
        [SerializeField] private GameObject damageNumberPrefab;
        [SerializeField] private float damageNumberLifetime = 1.2f;
        [SerializeField] private float damageNumberRiseSpeed = 1.5f;
        [SerializeField] private float damageNumberFontSize = 36f;

        private Vector3 cameraOriginalPosition;
        private Coroutine shakeCoroutine;
        private Coroutine flashCoroutine;

        private void Awake()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            if (mainCamera != null)
            {
                cameraOriginalPosition = mainCamera.transform.localPosition;
            }

            // Initialize flash overlay as transparent
            if (screenFlashImage != null)
            {
                screenFlashImage.color = Color.clear;
                screenFlashImage.raycastTarget = false;
            }

            // Initialize fade overlay as transparent
            if (fadeOverlay != null)
            {
                fadeOverlay.color = Color.clear;
                fadeOverlay.raycastTarget = false;
            }
        }

        // ----------------------------------------------------------------
        // Screen Flash
        // ----------------------------------------------------------------

        public void FlashScreen(Color color, float duration)
        {
            if (screenFlashImage == null)
            {
                Debug.LogWarning("[ScreenEffects] No screen flash image assigned.");
                return;
            }

            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
            }

            flashCoroutine = StartCoroutine(FlashRoutine(color, duration));
        }

        private IEnumerator FlashRoutine(Color color, float duration)
        {
            screenFlashImage.color = color;

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float alpha = Mathf.Lerp(color.a, 0f, t);
                screenFlashImage.color = new Color(color.r, color.g, color.b, alpha);
                yield return null;
            }

            screenFlashImage.color = Color.clear;
            flashCoroutine = null;
        }

        // ----------------------------------------------------------------
        // Camera Shake
        // ----------------------------------------------------------------

        public void ShakeCamera(float intensity, float duration)
        {
            if (mainCamera == null)
            {
                Debug.LogWarning("[ScreenEffects] No camera assigned for shake.");
                return;
            }

            if (shakeCoroutine != null)
            {
                StopCoroutine(shakeCoroutine);
                mainCamera.transform.localPosition = cameraOriginalPosition;
            }

            shakeCoroutine = StartCoroutine(ShakeRoutine(intensity, duration));
        }

        private IEnumerator ShakeRoutine(float intensity, float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // Decreasing intensity over time
                float currentIntensity = intensity * (1f - t);

                float offsetX = UnityEngine.Random.Range(-currentIntensity, currentIntensity);
                float offsetY = UnityEngine.Random.Range(-currentIntensity, currentIntensity);

                mainCamera.transform.localPosition = cameraOriginalPosition +
                    new Vector3(offsetX, offsetY, 0f);

                yield return null;
            }

            mainCamera.transform.localPosition = cameraOriginalPosition;
            shakeCoroutine = null;
        }

        // ----------------------------------------------------------------
        // Fade Effects
        // ----------------------------------------------------------------

        public void FadeToBlack(float duration, Action onComplete)
        {
            if (fadeOverlay == null)
            {
                Debug.LogWarning("[ScreenEffects] No fade overlay assigned.");
                onComplete?.Invoke();
                return;
            }

            StartCoroutine(FadeRoutine(Color.clear, Color.black, duration, onComplete));
        }

        public void FadeFromBlack(float duration)
        {
            if (fadeOverlay == null)
            {
                Debug.LogWarning("[ScreenEffects] No fade overlay assigned.");
                return;
            }

            StartCoroutine(FadeRoutine(Color.black, Color.clear, duration, null));
        }

        private IEnumerator FadeRoutine(Color from, Color to, float duration, Action onComplete)
        {
            fadeOverlay.raycastTarget = true;
            fadeOverlay.color = from;

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                fadeOverlay.color = Color.Lerp(from, to, t);
                yield return null;
            }

            fadeOverlay.color = to;

            // Disable raycast blocking if fully transparent
            if (to.a <= 0f)
            {
                fadeOverlay.raycastTarget = false;
            }

            onComplete?.Invoke();
        }

        // ----------------------------------------------------------------
        // Damage Numbers
        // ----------------------------------------------------------------

        public void ShowDamageNumber(Vector3 position, int amount, Color color)
        {
            StartCoroutine(DamageNumberRoutine(position, amount, color));
        }

        private IEnumerator DamageNumberRoutine(Vector3 position, int amount, Color color)
        {
            GameObject numberObj;
            TextMeshProUGUI textComponent;

            if (damageNumberPrefab != null && worldSpaceCanvas != null)
            {
                numberObj = Instantiate(damageNumberPrefab, worldSpaceCanvas.transform);
                textComponent = numberObj.GetComponent<TextMeshProUGUI>();
                if (textComponent == null)
                {
                    textComponent = numberObj.GetComponentInChildren<TextMeshProUGUI>();
                }
            }
            else
            {
                // Create a simple damage number if no prefab is assigned
                numberObj = new GameObject("DamageNumber");

                if (worldSpaceCanvas != null)
                {
                    numberObj.transform.SetParent(worldSpaceCanvas.transform, false);
                }

                textComponent = numberObj.AddComponent<TextMeshProUGUI>();
                textComponent.fontSize = damageNumberFontSize;
                textComponent.alignment = TextAlignmentOptions.Center;
                textComponent.fontStyle = FontStyles.Bold;

                RectTransform rect = numberObj.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(200f, 50f);
            }

            if (textComponent == null)
            {
                Destroy(numberObj);
                yield break;
            }

            // Set text and color
            string prefix = amount >= 0 ? "-" : "+";
            textComponent.text = $"{prefix}{Mathf.Abs(amount)}";
            textComponent.color = color;

            // Position the number
            numberObj.transform.position = position;

            // Add slight random horizontal offset
            float xOffset = UnityEngine.Random.Range(-0.3f, 0.3f);
            numberObj.transform.position += new Vector3(xOffset, 0f, 0f);

            // Animate: rise up and fade out
            float elapsed = 0f;
            Vector3 startPos = numberObj.transform.position;
            Color startColor = color;

            // Initial punch scale
            Vector3 punchScale = Vector3.one * 1.5f;
            numberObj.transform.localScale = punchScale;

            while (elapsed < damageNumberLifetime)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / damageNumberLifetime;

                // Rise upward
                numberObj.transform.position = startPos +
                    Vector3.up * damageNumberRiseSpeed * t;

                // Scale down from punch to normal in first 20%
                if (t < 0.2f)
                {
                    float scaleT = t / 0.2f;
                    numberObj.transform.localScale =
                        Vector3.Lerp(punchScale, Vector3.one, scaleT);
                }

                // Fade out in last 40%
                if (t > 0.6f)
                {
                    float fadeT = (t - 0.6f) / 0.4f;
                    float alpha = Mathf.Lerp(startColor.a, 0f, fadeT);
                    textComponent.color = new Color(
                        startColor.r,
                        startColor.g,
                        startColor.b,
                        alpha
                    );
                }

                yield return null;
            }

            Destroy(numberObj);
        }
    }
}
