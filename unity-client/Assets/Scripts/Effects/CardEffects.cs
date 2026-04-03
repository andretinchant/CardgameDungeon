using System.Collections;
using UnityEngine;
using CardgameDungeon.Unity.Cards;

namespace CardgameDungeon.Unity.Effects
{
    public class CardEffects : MonoBehaviour
    {
        [Header("Effect Settings")]
        [SerializeField] private float defaultGlowDuration = 0.5f;
        [SerializeField] private float defaultShakeIntensity = 0.1f;
        [SerializeField] private float shakeDuration = 0.3f;
        [SerializeField] private float defaultPulseScale = 1.2f;
        [SerializeField] private float pulseDuration = 0.4f;
        [SerializeField] private float dissolveDuration = 1.0f;

        [Header("Glow Settings")]
        [SerializeField] private Material glowMaterial;
        [SerializeField] private string glowColorProperty = "_GlowColor";
        [SerializeField] private string glowIntensityProperty = "_GlowIntensity";

        [Header("Highlight")]
        [SerializeField] private Color highlightColor = new Color(1f, 0.9f, 0.3f, 0.6f);

        [Header("Particle References")]
        [SerializeField] private ParticleManager particleManager;

        public void PlayGlow(CardView card, Color color, float duration)
        {
            if (card == null) return;
            StartCoroutine(GlowRoutine(card, color, duration));
        }

        private IEnumerator GlowRoutine(CardView card, Color color, float duration)
        {
            SpriteRenderer[] renderers = card.GetComponentsInChildren<SpriteRenderer>();
            Dictionary<SpriteRenderer, Material> originalMaterials =
                new Dictionary<SpriteRenderer, Material>();

            // Apply glow material
            foreach (var renderer in renderers)
            {
                originalMaterials[renderer] = renderer.material;

                if (glowMaterial != null)
                {
                    Material glowInstance = new Material(glowMaterial);
                    glowInstance.SetColor(glowColorProperty, color);
                    renderer.material = glowInstance;
                }
                else
                {
                    renderer.color = color;
                }
            }

            // Animate glow intensity
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // Fade in then fade out
                float intensity;
                if (t < 0.5f)
                {
                    intensity = Mathf.Lerp(0f, 1f, t * 2f);
                }
                else
                {
                    intensity = Mathf.Lerp(1f, 0f, (t - 0.5f) * 2f);
                }

                foreach (var renderer in renderers)
                {
                    if (renderer.material.HasProperty(glowIntensityProperty))
                    {
                        renderer.material.SetFloat(glowIntensityProperty, intensity);
                    }
                }

                yield return null;
            }

            // Restore original materials
            foreach (var kvp in originalMaterials)
            {
                if (kvp.Key != null)
                {
                    kvp.Key.material = kvp.Value;
                }
            }
        }

        public void PlayShake(CardView card, float intensity)
        {
            if (card == null) return;
            StartCoroutine(ShakeRoutine(card, intensity));
        }

        private IEnumerator ShakeRoutine(CardView card, float intensity)
        {
            Transform cardTransform = card.transform;
            Vector3 originalPosition = cardTransform.localPosition;
            float elapsed = 0f;

            while (elapsed < shakeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / shakeDuration;

                // Decreasing intensity over time
                float currentIntensity = intensity * (1f - t);

                float offsetX = Random.Range(-currentIntensity, currentIntensity);
                float offsetY = Random.Range(-currentIntensity, currentIntensity);

                cardTransform.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0f);

                yield return null;
            }

            // Snap back to original position
            cardTransform.localPosition = originalPosition;
        }

        public void PlayPulse(CardView card, float scale)
        {
            if (card == null) return;
            StartCoroutine(PulseRoutine(card, scale));
        }

        private IEnumerator PulseRoutine(CardView card, float scale)
        {
            Transform cardTransform = card.transform;
            Vector3 originalScale = cardTransform.localScale;
            Vector3 targetScale = originalScale * scale;
            float halfDuration = pulseDuration * 0.5f;

            // Scale up
            float elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / halfDuration);
                cardTransform.localScale = Vector3.Lerp(originalScale, targetScale, t);
                yield return null;
            }

            // Scale down
            elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / halfDuration);
                cardTransform.localScale = Vector3.Lerp(targetScale, originalScale, t);
                yield return null;
            }

            cardTransform.localScale = originalScale;
        }

        public void PlayDissolve(CardView card)
        {
            if (card == null) return;
            StartCoroutine(DissolveRoutine(card));
        }

        private IEnumerator DissolveRoutine(CardView card)
        {
            Transform cardTransform = card.transform;
            SpriteRenderer[] renderers = card.GetComponentsInChildren<SpriteRenderer>();

            // Store original colors
            Dictionary<SpriteRenderer, Color> originalColors =
                new Dictionary<SpriteRenderer, Color>();
            foreach (var renderer in renderers)
            {
                originalColors[renderer] = renderer.color;
            }

            // Spawn exile particles
            if (particleManager != null)
            {
                particleManager.PlayExile(cardTransform.position);
            }

            // Fade out and shrink
            float elapsed = 0f;
            Vector3 originalScale = cardTransform.localScale;

            while (elapsed < dissolveDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / dissolveDuration;

                // Fade alpha
                foreach (var renderer in renderers)
                {
                    if (renderer == null) continue;

                    Color original = originalColors.ContainsKey(renderer)
                        ? originalColors[renderer]
                        : Color.white;

                    renderer.color = new Color(
                        original.r,
                        original.g,
                        original.b,
                        Mathf.Lerp(original.a, 0f, t)
                    );
                }

                // Slight scale down
                float scaleT = Mathf.Lerp(1f, 0.7f, t);
                cardTransform.localScale = originalScale * scaleT;

                // Add slight vertical drift
                cardTransform.localPosition += Vector3.up * Time.deltaTime * 0.3f;

                yield return null;
            }

            // Deactivate the card
            card.gameObject.SetActive(false);

            // Restore original state (in case it gets reused)
            cardTransform.localScale = originalScale;
            foreach (var kvp in originalColors)
            {
                if (kvp.Key != null)
                {
                    kvp.Key.color = kvp.Value;
                }
            }
        }

        public void PlayHighlight(CardView card, bool on)
        {
            if (card == null) return;

            card.SetHighlight(on);

            if (on)
            {
                // Apply a subtle glow on highlight
                SpriteRenderer[] renderers = card.GetComponentsInChildren<SpriteRenderer>();
                foreach (var renderer in renderers)
                {
                    if (renderer.material.HasProperty(glowColorProperty))
                    {
                        renderer.material.SetColor(glowColorProperty, highlightColor);
                        renderer.material.SetFloat(glowIntensityProperty, 0.5f);
                    }
                }
            }
            else
            {
                // Remove glow
                SpriteRenderer[] renderers = card.GetComponentsInChildren<SpriteRenderer>();
                foreach (var renderer in renderers)
                {
                    if (renderer.material.HasProperty(glowIntensityProperty))
                    {
                        renderer.material.SetFloat(glowIntensityProperty, 0f);
                    }
                }
            }
        }
    }
}
