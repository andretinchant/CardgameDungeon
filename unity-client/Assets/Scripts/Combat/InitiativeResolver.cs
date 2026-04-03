using System.Collections;
using UnityEngine;
using TMPro;
using CardgameDungeon.Unity.Core;
using CardgameDungeon.Unity.UI;

namespace CardgameDungeon.Unity.Combat
{
    public class InitiativeResolver : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject initiativePanel;
        [SerializeField] private TextMeshProUGUI player1TotalText;
        [SerializeField] private TextMeshProUGUI player2TotalText;
        [SerializeField] private TextMeshProUGUI resultText;
        [SerializeField] private GameObject player1Highlight;
        [SerializeField] private GameObject player2Highlight;
        [SerializeField] private GameObject betPromptPanel;

        [Header("Animation Settings")]
        [SerializeField] private float revealDelay = 0.8f;
        [SerializeField] private float winnerHighlightDuration = 1.5f;
        [SerializeField] private float numberRollDuration = 0.5f;
        [SerializeField] private int rollSteps = 10;

        private void OnEnable()
        {
            EventBus.Subscribe<InitiativeResolvedEvent>(OnInitiativeResolved);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<InitiativeResolvedEvent>(OnInitiativeResolved);
        }

        private void OnInitiativeResolved(InitiativeResolvedEvent evt)
        {
            ShowInitiativeRoll(evt.Response);
        }

        public void ShowInitiativeRoll(InitiativeResponse response)
        {
            StartCoroutine(InitiativeRollRoutine(response));
        }

        private IEnumerator InitiativeRollRoutine(InitiativeResponse response)
        {
            // Show the initiative panel
            if (initiativePanel != null)
            {
                initiativePanel.SetActive(true);
            }

            // Reset visuals
            if (player1Highlight != null) player1Highlight.SetActive(false);
            if (player2Highlight != null) player2Highlight.SetActive(false);
            if (betPromptPanel != null) betPromptPanel.SetActive(false);
            if (resultText != null) resultText.text = "";
            if (player1TotalText != null) player1TotalText.text = "?";
            if (player2TotalText != null) player2TotalText.text = "?";

            yield return new WaitForSeconds(revealDelay * 0.5f);

            // Animate Player 1 total rolling
            yield return StartCoroutine(AnimateNumberRoll(player1TotalText, response.Player1Total));

            yield return new WaitForSeconds(revealDelay);

            // Animate Player 2 total rolling
            yield return StartCoroutine(AnimateNumberRoll(player2TotalText, response.Player2Total));

            yield return new WaitForSeconds(revealDelay);

            // Reveal result
            if (response.IsTied)
            {
                if (resultText != null)
                {
                    resultText.text = "TIED! Place your bets!";
                }

                ShowBetPrompt();
            }
            else
            {
                AnimateInitiativeWin(response.WinnerId);
            }
        }

        private IEnumerator AnimateNumberRoll(TextMeshProUGUI targetText, int finalValue)
        {
            if (targetText == null) yield break;

            float stepDuration = numberRollDuration / rollSteps;

            for (int i = 0; i < rollSteps; i++)
            {
                int randomValue = Random.Range(
                    Mathf.Max(1, finalValue - 10),
                    finalValue + 10
                );
                targetText.text = randomValue.ToString();
                yield return new WaitForSeconds(stepDuration);
            }

            // Set final value
            targetText.text = finalValue.ToString();

            // Punch scale for impact
            Vector3 originalScale = targetText.transform.localScale;
            Vector3 punchScale = originalScale * 1.3f;
            float punchDuration = 0.15f;
            float elapsed = 0f;

            while (elapsed < punchDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / punchDuration;
                targetText.transform.localScale = Vector3.Lerp(punchScale, originalScale, t);
                yield return null;
            }

            targetText.transform.localScale = originalScale;
        }

        public void ShowBetPrompt()
        {
            if (betPromptPanel != null)
            {
                betPromptPanel.SetActive(true);
            }

            Debug.Log("[InitiativeResolver] Bet prompt displayed.");
        }

        public void HideBetPrompt()
        {
            if (betPromptPanel != null)
            {
                betPromptPanel.SetActive(false);
            }
        }

        public void AnimateInitiativeWin(string winnerId)
        {
            StartCoroutine(InitiativeWinRoutine(winnerId));
        }

        private IEnumerator InitiativeWinRoutine(string winnerId)
        {
            if (betPromptPanel != null)
            {
                betPromptPanel.SetActive(false);
            }

            if (resultText != null)
            {
                resultText.text = "Initiative Won!";
            }

            // Determine which side to highlight
            // Convention: if winnerId matches local player, highlight player1 side
            bool isPlayer1 = IsLocalPlayer(winnerId);

            GameObject winnerObj = isPlayer1 ? player1Highlight : player2Highlight;
            TextMeshProUGUI winnerText = isPlayer1 ? player1TotalText : player2TotalText;

            // Activate winner highlight
            if (winnerObj != null)
            {
                winnerObj.SetActive(true);
            }

            // Pulse the winner's total text
            if (winnerText != null)
            {
                yield return StartCoroutine(PulseText(winnerText, winnerHighlightDuration));
            }
            else
            {
                yield return new WaitForSeconds(winnerHighlightDuration);
            }

            yield return new WaitForSeconds(0.5f);

            // Hide initiative panel after a brief pause
            if (initiativePanel != null)
            {
                initiativePanel.SetActive(false);
            }

            Debug.Log($"[InitiativeResolver] Winner highlighted: {winnerId}");
        }

        private IEnumerator PulseText(TextMeshProUGUI text, float duration)
        {
            Vector3 originalScale = text.transform.localScale;
            float elapsed = 0f;
            int pulseCount = 3;
            float pulseDuration = duration / pulseCount;

            for (int i = 0; i < pulseCount; i++)
            {
                elapsed = 0f;
                // Scale up
                while (elapsed < pulseDuration * 0.5f)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / (pulseDuration * 0.5f);
                    float scale = Mathf.Lerp(1f, 1.25f, t);
                    text.transform.localScale = originalScale * scale;
                    yield return null;
                }

                elapsed = 0f;
                // Scale down
                while (elapsed < pulseDuration * 0.5f)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / (pulseDuration * 0.5f);
                    float scale = Mathf.Lerp(1.25f, 1f, t);
                    text.transform.localScale = originalScale * scale;
                    yield return null;
                }
            }

            text.transform.localScale = originalScale;
        }

        private bool IsLocalPlayer(string playerId)
        {
            return GameManager.Instance != null &&
                   GameManager.Instance.LocalPlayerId == playerId;
        }
    }
}
