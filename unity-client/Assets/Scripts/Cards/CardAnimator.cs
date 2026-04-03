using System.Collections;
using UnityEngine;

namespace CardgameDungeon.Unity.Cards
{
    public class CardAnimator : MonoBehaviour
    {
        [Header("Animation Curves")]
        [SerializeField] private AnimationCurve moveCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] private AnimationCurve flipCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] private AnimationCurve combatLungeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        [Header("Timing")]
        [SerializeField] private float drawDuration = 0.5f;
        [SerializeField] private float playDuration = 0.4f;
        [SerializeField] private float discardDuration = 0.3f;
        [SerializeField] private float exileDuration = 0.6f;
        [SerializeField] private float combatDuration = 0.5f;
        [SerializeField] private float flipDuration = 0.4f;
        [SerializeField] private float hoverDuration = 0.15f;

        [Header("Combat Settings")]
        [SerializeField] private float lungeDistance = 0.6f;

        [Header("Hover Settings")]
        [SerializeField] private float hoverScaleMultiplier = 1.15f;

        [Header("Exile Effects")]
        [SerializeField] private ParticleSystem exileParticles;

        private Vector3 originalScale;
        private Coroutine activeCoroutine;

        private void Awake()
        {
            originalScale = transform.localScale;
        }

        public Coroutine AnimateDraw(Transform target)
        {
            StopActiveCoroutine();
            activeCoroutine = StartCoroutine(DrawCoroutine(target));
            return activeCoroutine;
        }

        public Coroutine AnimatePlay(Transform target)
        {
            StopActiveCoroutine();
            activeCoroutine = StartCoroutine(PlayCoroutine(target));
            return activeCoroutine;
        }

        public Coroutine AnimateDiscard(Transform target)
        {
            StopActiveCoroutine();
            activeCoroutine = StartCoroutine(DiscardCoroutine(target));
            return activeCoroutine;
        }

        public Coroutine AnimateExile(Transform target)
        {
            StopActiveCoroutine();
            activeCoroutine = StartCoroutine(ExileCoroutine(target));
            return activeCoroutine;
        }

        public Coroutine AnimateCombat(Transform attacker, Transform defender)
        {
            StopActiveCoroutine();
            activeCoroutine = StartCoroutine(CombatCoroutine(attacker, defender));
            return activeCoroutine;
        }

        public Coroutine AnimateFlip(bool faceUp)
        {
            StopActiveCoroutine();
            activeCoroutine = StartCoroutine(FlipCoroutine(faceUp));
            return activeCoroutine;
        }

        public Coroutine AnimateHover(bool hovering)
        {
            StopActiveCoroutine();
            activeCoroutine = StartCoroutine(HoverCoroutine(hovering));
            return activeCoroutine;
        }

        private IEnumerator DrawCoroutine(Transform target)
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = target.position;
            Quaternion startRot = transform.rotation;
            Quaternion endRot = target.rotation;
            Vector3 startScale = transform.localScale * 0.5f;
            Vector3 endScale = originalScale;

            transform.localScale = startScale;
            float elapsed = 0f;

            // Arc height for a nice draw animation
            float arcHeight = 1.5f;

            while (elapsed < drawDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / drawDuration);
                float curveT = moveCurve.Evaluate(t);

                // Position with arc
                Vector3 currentPos = Vector3.Lerp(startPos, endPos, curveT);
                currentPos.y += Mathf.Sin(t * Mathf.PI) * arcHeight;
                transform.position = currentPos;

                // Rotation
                transform.rotation = Quaternion.Slerp(startRot, endRot, curveT);

                // Scale up from small to normal
                float scaleT = scaleCurve.Evaluate(t);
                transform.localScale = Vector3.Lerp(startScale, endScale, scaleT);

                yield return null;
            }

            transform.position = endPos;
            transform.rotation = endRot;
            transform.localScale = endScale;
            activeCoroutine = null;
        }

        private IEnumerator PlayCoroutine(Transform target)
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = target.position;
            Quaternion startRot = transform.rotation;
            Quaternion endRot = target.rotation;
            float elapsed = 0f;

            while (elapsed < playDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / playDuration);
                float curveT = moveCurve.Evaluate(t);

                transform.position = Vector3.Lerp(startPos, endPos, curveT);
                transform.rotation = Quaternion.Slerp(startRot, endRot, curveT);

                yield return null;
            }

            transform.position = endPos;
            transform.rotation = endRot;
            activeCoroutine = null;
        }

        private IEnumerator DiscardCoroutine(Transform target)
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = target.position;
            Vector3 startScale = transform.localScale;
            Vector3 endScale = originalScale * 0.7f;
            float elapsed = 0f;

            while (elapsed < discardDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / discardDuration);
                float curveT = moveCurve.Evaluate(t);

                transform.position = Vector3.Lerp(startPos, endPos, curveT);

                float scaleT = scaleCurve.Evaluate(t);
                transform.localScale = Vector3.Lerp(startScale, endScale, scaleT);

                yield return null;
            }

            transform.position = endPos;
            transform.localScale = endScale;
            activeCoroutine = null;
        }

        private IEnumerator ExileCoroutine(Transform target)
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = target.position;
            Vector3 startScale = transform.localScale;
            float elapsed = 0f;

            // Trigger particle effect
            if (exileParticles != null)
            {
                exileParticles.transform.position = transform.position;
                exileParticles.Play();
            }

            // Get all renderers for fade
            SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
            Color[] originalColors = new Color[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                originalColors[i] = renderers[i].color;
            }

            while (elapsed < exileDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / exileDuration);
                float curveT = moveCurve.Evaluate(t);

                // Move toward exile position
                transform.position = Vector3.Lerp(startPos, endPos, curveT);

                // Shrink
                float scaleT = scaleCurve.Evaluate(t);
                transform.localScale = Vector3.Lerp(startScale, Vector3.zero, scaleT);

                // Fade out all renderers
                float alpha = Mathf.Lerp(1f, 0f, t);
                for (int i = 0; i < renderers.Length; i++)
                {
                    if (renderers[i] != null)
                    {
                        Color c = originalColors[i];
                        c.a = alpha;
                        renderers[i].color = c;
                    }
                }

                yield return null;
            }

            transform.position = endPos;
            transform.localScale = Vector3.zero;

            // Restore colors for potential reuse
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null)
                {
                    renderers[i].color = originalColors[i];
                }
            }

            activeCoroutine = null;
        }

        private IEnumerator CombatCoroutine(Transform attacker, Transform defender)
        {
            Vector3 startPos = attacker.position;
            Vector3 defenderPos = defender.position;

            // Calculate lunge target (move toward defender but not all the way)
            Vector3 direction = (defenderPos - startPos).normalized;
            Vector3 lungeTarget = Vector3.Lerp(startPos, defenderPos, lungeDistance);

            float halfDuration = combatDuration * 0.5f;

            // Phase 1: Lunge toward defender
            float elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / halfDuration);
                float curveT = combatLungeCurve.Evaluate(t);

                attacker.position = Vector3.Lerp(startPos, lungeTarget, curveT);
                yield return null;
            }

            attacker.position = lungeTarget;

            // Brief pause at impact
            yield return new WaitForSeconds(0.05f);

            // Phase 2: Return to original position
            elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / halfDuration);
                float curveT = moveCurve.Evaluate(t);

                attacker.position = Vector3.Lerp(lungeTarget, startPos, curveT);
                yield return null;
            }

            attacker.position = startPos;
            activeCoroutine = null;
        }

        private IEnumerator FlipCoroutine(bool faceUp)
        {
            float startAngle = transform.localEulerAngles.y;
            float targetAngle = faceUp ? 0f : 180f;

            // If already at target, no-op
            float angleDiff = Mathf.Abs(Mathf.DeltaAngle(startAngle, targetAngle));
            if (angleDiff < 1f)
            {
                activeCoroutine = null;
                yield break;
            }

            float elapsed = 0f;

            while (elapsed < flipDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / flipDuration);
                float curveT = flipCurve.Evaluate(t);

                float angle = Mathf.Lerp(startAngle, targetAngle, curveT);
                Vector3 euler = transform.localEulerAngles;
                euler.y = angle;
                transform.localEulerAngles = euler;

                yield return null;
            }

            Vector3 finalEuler = transform.localEulerAngles;
            finalEuler.y = targetAngle;
            transform.localEulerAngles = finalEuler;

            activeCoroutine = null;
        }

        private IEnumerator HoverCoroutine(bool hovering)
        {
            Vector3 startScale = transform.localScale;
            Vector3 targetScale = hovering ? originalScale * hoverScaleMultiplier : originalScale;

            float elapsed = 0f;

            while (elapsed < hoverDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / hoverDuration);
                float curveT = scaleCurve.Evaluate(t);

                transform.localScale = Vector3.Lerp(startScale, targetScale, curveT);
                yield return null;
            }

            transform.localScale = targetScale;
            activeCoroutine = null;
        }

        private void StopActiveCoroutine()
        {
            if (activeCoroutine != null)
            {
                StopCoroutine(activeCoroutine);
                activeCoroutine = null;
            }
        }
    }
}
