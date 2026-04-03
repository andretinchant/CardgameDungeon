using System;
using System.Collections;
using UnityEngine;
using TMPro;

namespace CardgameDungeon.Unity.Cards
{
    public class CardAnimator : MonoBehaviour
    {
        [Header("Animation Curves")]
        [SerializeField] private AnimationCurve moveCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] private AnimationCurve flipCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Timing")]
        [SerializeField] private float drawDuration = 0.5f;
        [SerializeField] private float playDuration = 0.4f;
        [SerializeField] private float discardDuration = 0.3f;
        [SerializeField] private float exileDuration = 0.6f;
        [SerializeField] private float flipDuration = 0.4f;
        [SerializeField] private float shakeDuration = 0.3f;

        [Header("Draw Settings")]
        [SerializeField] private float drawArcHeight = 1.5f;

        [Header("Play Glow")]
        [SerializeField] private Color playGlowColor = new Color(1f, 0.9f, 0.5f, 0.8f);
        [SerializeField] private float playGlowDuration = 0.3f;

        [Header("Exile Dissolve")]
        [SerializeField] private Color exileDissolveColor = new Color(0.5f, 0f, 0.8f, 1f);
        [SerializeField] private ParticleSystem exileParticles;

        [Header("Combat Shake")]
        [SerializeField] private float shakeIntensity = 0.15f;

        [Header("Floating Damage")]
        [SerializeField] private GameObject damageNumberPrefab;
        [SerializeField] private float damageRiseSpeed = 1.5f;
        [SerializeField] private float damageLifetime = 1.0f;

        private Vector3 originalScale;
        private Vector3 originalLocalPosition;
        private Coroutine activeCoroutine;

        private void Awake()
        {
            originalScale = transform.localScale;
            originalLocalPosition = transform.localPosition;
        }

        // ── DrawCard: arc from deck to hand ──

        public Coroutine AnimateDraw(Transform target)
        {
            StopActive();
            activeCoroutine = StartCoroutine(DrawCoroutine(target));
            return activeCoroutine;
        }

        private IEnumerator DrawCoroutine(Transform target)
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = target.position;
            Quaternion startRot = transform.rotation;
            Quaternion endRot = target.rotation;
            Vector3 startScale = originalScale * 0.3f;

            transform.localScale = startScale;
            float elapsed = 0f;

            while (elapsed < drawDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / drawDuration);
                float ct = moveCurve.Evaluate(t);

                // Smooth arc trajectory
                Vector3 pos = Vector3.Lerp(startPos, endPos, ct);
                pos.y += Mathf.Sin(t * Mathf.PI) * drawArcHeight;
                transform.position = pos;

                transform.rotation = Quaternion.Slerp(startRot, endRot, ct);
                transform.localScale = Vector3.Lerp(startScale, originalScale, scaleCurve.Evaluate(t));

                yield return null;
            }

            transform.position = endPos;
            transform.rotation = endRot;
            transform.localScale = originalScale;
            activeCoroutine = null;
        }

        // ── PlayCard: hand to field with glow burst ──

        public Coroutine AnimatePlay(Transform target)
        {
            StopActive();
            activeCoroutine = StartCoroutine(PlayCoroutine(target));
            return activeCoroutine;
        }

        private IEnumerator PlayCoroutine(Transform target)
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = target.position;
            Quaternion startRot = transform.rotation;
            Quaternion endRot = target.rotation;
            float elapsed = 0f;

            // Get glow renderer for flash effect
            SpriteRenderer glowRenderer = GetGlowRenderer();

            while (elapsed < playDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / playDuration);
                float ct = moveCurve.Evaluate(t);

                transform.position = Vector3.Lerp(startPos, endPos, ct);
                transform.rotation = Quaternion.Slerp(startRot, endRot, ct);

                // Scale punch: briefly enlarge at midpoint
                float scalePunch = 1f + 0.15f * Mathf.Sin(t * Mathf.PI);
                transform.localScale = originalScale * scalePunch;

                yield return null;
            }

            transform.position = endPos;
            transform.rotation = endRot;
            transform.localScale = originalScale;

            // Glow burst on arrival
            if (glowRenderer != null)
            {
                yield return GlowBurst(glowRenderer, playGlowColor, playGlowDuration);
            }

            activeCoroutine = null;
        }

        private IEnumerator GlowBurst(SpriteRenderer renderer, Color color, float duration)
        {
            Color original = renderer.color;
            float elapsed = 0f;

            renderer.color = color;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                renderer.color = Color.Lerp(color, original, t);
                yield return null;
            }

            renderer.color = original;
        }

        // ── DiscardCard: move to discard pile with fade ──

        public Coroutine AnimateDiscard(Transform target)
        {
            StopActive();
            activeCoroutine = StartCoroutine(DiscardCoroutine(target));
            return activeCoroutine;
        }

        private IEnumerator DiscardCoroutine(Transform target)
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = target.position;
            Vector3 startScale = transform.localScale;
            Vector3 endScale = originalScale * 0.6f;
            float elapsed = 0f;

            SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
            Color[] origColors = CaptureColors(renderers);

            while (elapsed < discardDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / discardDuration);
                float ct = moveCurve.Evaluate(t);

                transform.position = Vector3.Lerp(startPos, endPos, ct);
                transform.localScale = Vector3.Lerp(startScale, endScale, scaleCurve.Evaluate(t));

                // Fade out
                float alpha = Mathf.Lerp(1f, 0.2f, t);
                ApplyAlpha(renderers, origColors, alpha);

                yield return null;
            }

            transform.position = endPos;
            transform.localScale = endScale;
            RestoreColors(renderers, origColors);
            activeCoroutine = null;
        }

        // ── ExileCard: purple dissolve effect ──

        public Coroutine AnimateExile()
        {
            StopActive();
            activeCoroutine = StartCoroutine(ExileCoroutine());
            return activeCoroutine;
        }

        private IEnumerator ExileCoroutine()
        {
            Vector3 startScale = transform.localScale;
            float elapsed = 0f;

            if (exileParticles != null)
            {
                exileParticles.transform.position = transform.position;
                var main = exileParticles.main;
                main.startColor = exileDissolveColor;
                exileParticles.Play();
            }

            SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
            Color[] origColors = CaptureColors(renderers);

            while (elapsed < exileDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / exileDuration);

                // Shrink with slight upward drift
                transform.localScale = Vector3.Lerp(startScale, Vector3.zero, scaleCurve.Evaluate(t));
                transform.position += Vector3.up * Time.deltaTime * 0.5f;

                // Tint purple and fade
                float alpha = Mathf.Lerp(1f, 0f, t);
                for (int i = 0; i < renderers.Length; i++)
                {
                    if (renderers[i] == null) continue;
                    Color tinted = Color.Lerp(origColors[i], exileDissolveColor, t * 0.7f);
                    tinted.a = alpha;
                    renderers[i].color = tinted;
                }

                yield return null;
            }

            transform.localScale = Vector3.zero;
            RestoreColors(renderers, origColors);
            activeCoroutine = null;
        }

        // ── FlipCard: 3D Y-axis rotation ──

        public Coroutine AnimateFlip(bool faceUp)
        {
            StopActive();
            activeCoroutine = StartCoroutine(FlipCoroutine(faceUp));
            return activeCoroutine;
        }

        private IEnumerator FlipCoroutine(bool faceUp)
        {
            float startAngle = transform.localEulerAngles.y;
            float targetAngle = faceUp ? 0f : 180f;

            if (Mathf.Abs(Mathf.DeltaAngle(startAngle, targetAngle)) < 1f)
            {
                activeCoroutine = null;
                yield break;
            }

            float elapsed = 0f;

            while (elapsed < flipDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / flipDuration);
                float angle = Mathf.Lerp(startAngle, targetAngle, flipCurve.Evaluate(t));

                Vector3 euler = transform.localEulerAngles;
                euler.y = angle;
                transform.localEulerAngles = euler;

                yield return null;
            }

            Vector3 final = transform.localEulerAngles;
            final.y = targetAngle;
            transform.localEulerAngles = final;
            activeCoroutine = null;
        }

        // ── CombatShake: shake on the unit receiving damage ──

        public Coroutine AnimateCombatShake()
        {
            StopActive();
            activeCoroutine = StartCoroutine(CombatShakeCoroutine());
            return activeCoroutine;
        }

        private IEnumerator CombatShakeCoroutine()
        {
            Vector3 basePos = transform.localPosition;
            float elapsed = 0f;

            while (elapsed < shakeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / shakeDuration;
                float decay = 1f - t;

                float offsetX = UnityEngine.Random.Range(-shakeIntensity, shakeIntensity) * decay;
                float offsetY = UnityEngine.Random.Range(-shakeIntensity, shakeIntensity) * decay;
                transform.localPosition = basePos + new Vector3(offsetX, offsetY, 0f);

                yield return null;
            }

            transform.localPosition = basePos;
            activeCoroutine = null;
        }

        // ── FloatingDamage: number rising above the hit unit ──

        public void ShowFloatingDamage(int amount, Color color)
        {
            StartCoroutine(FloatingDamageCoroutine(amount, color));
        }

        public void ShowFloatingDamage(int amount)
        {
            Color color = amount > 0 ? Color.red : Color.green;
            ShowFloatingDamage(amount, color);
        }

        private IEnumerator FloatingDamageCoroutine(int amount, Color color)
        {
            GameObject numObj;
            TextMeshPro tmp;

            if (damageNumberPrefab != null)
            {
                numObj = Instantiate(damageNumberPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
                tmp = numObj.GetComponent<TextMeshPro>() ?? numObj.GetComponentInChildren<TextMeshPro>();
            }
            else
            {
                numObj = new GameObject("FloatingDamage");
                numObj.transform.position = transform.position + Vector3.up * 0.5f;
                tmp = numObj.AddComponent<TextMeshPro>();
                tmp.fontSize = 8f;
                tmp.alignment = TextAlignmentOptions.Center;
                tmp.fontStyle = FontStyles.Bold;
                tmp.sortingOrder = 100;
            }

            if (tmp == null) { Destroy(numObj); yield break; }

            tmp.text = amount > 0 ? $"-{amount}" : $"+{Mathf.Abs(amount)}";
            tmp.color = color;

            // Punch scale on spawn
            numObj.transform.localScale = Vector3.one * 1.5f;

            Vector3 startPos = numObj.transform.position;
            float xOffset = UnityEngine.Random.Range(-0.2f, 0.2f);
            float elapsed = 0f;

            while (elapsed < damageLifetime)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / damageLifetime;

                // Rise
                numObj.transform.position = startPos + new Vector3(xOffset * t, damageRiseSpeed * t, 0f);

                // Scale down from punch in first 20%
                if (t < 0.2f)
                    numObj.transform.localScale = Vector3.Lerp(Vector3.one * 1.5f, Vector3.one, t / 0.2f);

                // Fade in last 40%
                if (t > 0.6f)
                {
                    float fadeT = (t - 0.6f) / 0.4f;
                    tmp.color = new Color(color.r, color.g, color.b, Mathf.Lerp(1f, 0f, fadeT));
                }

                yield return null;
            }

            Destroy(numObj);
        }

        // ── Helpers ──

        private void StopActive()
        {
            if (activeCoroutine != null)
            {
                StopCoroutine(activeCoroutine);
                activeCoroutine = null;
            }
        }

        private SpriteRenderer GetGlowRenderer()
        {
            Transform glow = transform.Find("Glow");
            return glow != null ? glow.GetComponent<SpriteRenderer>() : null;
        }

        private static Color[] CaptureColors(SpriteRenderer[] renderers)
        {
            Color[] colors = new Color[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
                colors[i] = renderers[i] != null ? renderers[i].color : Color.white;
            return colors;
        }

        private static void ApplyAlpha(SpriteRenderer[] renderers, Color[] originals, float alpha)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] == null) continue;
                Color c = originals[i];
                c.a = alpha;
                renderers[i].color = c;
            }
        }

        private static void RestoreColors(SpriteRenderer[] renderers, Color[] originals)
        {
            for (int i = 0; i < renderers.Length; i++)
                if (renderers[i] != null) renderers[i].color = originals[i];
        }
    }
}
