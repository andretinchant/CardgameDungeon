using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CardgameDungeon.Unity.Core;
using CardgameDungeon.Unity.Network;

namespace CardgameDungeon.Unity.UI
{
    public class MatchHUD : MonoBehaviour
    {
        [Header("Player Panels")]
        [SerializeField] private PlayerArea player1Panel;
        [SerializeField] private PlayerArea player2Panel;

        [Header("Match Info")]
        [SerializeField] private TMP_Text phaseText;
        [SerializeField] private TMP_Text roomText;

        [Header("Initiative")]
        [SerializeField] private GameObject initiativePanel;

        [Header("Betting")]
        [SerializeField] private GameObject betPanel;
        [SerializeField] private TMP_InputField betAmountInput;
        [SerializeField] private Toggle betExileToggle;
        [SerializeField] private Button placeBetButton;

        [Header("Actions")]
        [SerializeField] private Button endTurnButton;
        [SerializeField] private Button concedeButton;

        private string currentMatchId;

        private void OnEnable()
        {
            endTurnButton.onClick.AddListener(OnEndTurnClicked);
            concedeButton.onClick.AddListener(OnConcedeClicked);
            placeBetButton.onClick.AddListener(OnPlaceBetClicked);
        }

        private void OnDisable()
        {
            endTurnButton.onClick.RemoveListener(OnEndTurnClicked);
            concedeButton.onClick.RemoveListener(OnConcedeClicked);
            placeBetButton.onClick.RemoveListener(OnPlaceBetClicked);
        }

        public void SetMatchId(string matchId)
        {
            currentMatchId = matchId;
        }

        public void UpdateFromMatchState(MatchResponse matchState)
        {
            if (matchState == null) return;

            phaseText.text = $"Phase: {matchState.Phase}";
            roomText.text = $"Room: {matchState.CurrentRoom}";

            if (player1Panel != null && matchState.Player1 != null)
            {
                player1Panel.UpdateFromState(matchState.Player1);
            }

            if (player2Panel != null && matchState.Player2 != null)
            {
                player2Panel.UpdateFromState(matchState.Player2);
            }
        }

        public void ShowInitiativeResult(InitiativeResponse result)
        {
            if (result == null) return;

            initiativePanel.SetActive(true);

            var resultText = initiativePanel.GetComponentInChildren<TMP_Text>();
            if (resultText != null)
            {
                if (result.IsTied)
                {
                    resultText.text = $"Initiative Tied! ({result.Player1Total} vs {result.Player2Total})";
                }
                else
                {
                    resultText.text = $"Initiative Winner: Player {(result.WinnerId.HasValue ? result.WinnerId.Value.ToString() : "Unknown")}\n" +
                                     $"({result.Player1Total} vs {result.Player2Total})";
                }
            }
        }

        public void ShowBetPanel(bool show)
        {
            betPanel.SetActive(show);

            if (show)
            {
                betAmountInput.text = "";
                betExileToggle.isOn = false;
            }
        }

        public void ShowCombatResult(ResolveCombatRoundResponse combatResult)
        {
            if (combatResult == null) return;

            Debug.Log($"Combat resolved - Outcome: {combatResult.OverallOutcome}, " +
                      $"Results: {combatResult.Results.Count}, " +
                      $"Simultaneous Elimination: {combatResult.SimultaneousElimination}, " +
                      $"Phase: {combatResult.Phase}");
        }

        private void OnEndTurnClicked()
        {
            if (string.IsNullOrEmpty(currentMatchId)) return;

            StartCoroutine(EndTurnRoutine());
        }

        private IEnumerator EndTurnRoutine()
        {
            yield return StartCoroutine(ApiClient.EndTurn(currentMatchId, (matchState, error) =>
            {
                if (!string.IsNullOrEmpty(error))
                {
                    Debug.LogError($"Failed to end turn: {error}");
                    return;
                }

                UpdateFromMatchState(matchState);
            }));
        }

        private void OnConcedeClicked()
        {
            if (string.IsNullOrEmpty(currentMatchId)) return;

            StartCoroutine(ConcedeRoutine());
        }

        private IEnumerator ConcedeRoutine()
        {
            yield return StartCoroutine(ApiClient.ConcedeMatch(currentMatchId, (success, error) =>
            {
                if (!string.IsNullOrEmpty(error))
                {
                    Debug.LogError($"Failed to concede: {error}");
                    return;
                }

                Debug.Log("Match conceded.");
                GameManager.Instance.LoadScene("MainMenu");
            }));
        }

        private void OnPlaceBetClicked()
        {
            if (string.IsNullOrEmpty(currentMatchId)) return;

            if (!int.TryParse(betAmountInput.text, out int betAmount) || betAmount <= 0)
            {
                Debug.LogWarning("Please enter a valid bet amount.");
                return;
            }

            bool exile = betExileToggle.isOn;

            StartCoroutine(PlaceBetRoutine(betAmount, exile));
        }

        private IEnumerator PlaceBetRoutine(int amount, bool exile)
        {
            yield return StartCoroutine(ApiClient.PlaceBet(currentMatchId, amount, exile, (matchState, error) =>
            {
                if (!string.IsNullOrEmpty(error))
                {
                    Debug.LogError($"Failed to place bet: {error}");
                    return;
                }

                ShowBetPanel(false);
                UpdateFromMatchState(matchState);
            }));
        }
    }
}
