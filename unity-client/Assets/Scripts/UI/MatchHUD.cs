using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CardgameDungeon.Unity.Core;
using CardgameDungeon.Unity.Network;

namespace CardgameDungeon.Unity.UI
{
    public enum TurnPhase
    {
        Setup,
        Initiative,
        Betting,
        Combat,
        Advance,
        Finished
    }

    public class MatchHUD : MonoBehaviour
    {
        [Header("Dungeon Progress")]
        [SerializeField] private TMP_Text playerRoomText;
        [SerializeField] private TMP_Text opponentRoomText;
        [SerializeField] private Image[] roomIcons = new Image[6]; // 5 rooms + boss

        [Header("Exile Counts (public)")]
        [SerializeField] private TMP_Text playerExileText;
        [SerializeField] private TMP_Text opponentExileText;

        [Header("Deck & Discard Counts")]
        [SerializeField] private TMP_Text playerDeckText;
        [SerializeField] private TMP_Text playerDiscardText;
        [SerializeField] private TMP_Text opponentDeckText;
        [SerializeField] private TMP_Text opponentDiscardText;

        [Header("Phase Display")]
        [SerializeField] private TMP_Text phaseText;
        [SerializeField] private Image phaseIcon;
        [SerializeField] private Color setupColor = new Color(0.3f, 0.7f, 1f);
        [SerializeField] private Color initiativeColor = new Color(1f, 0.8f, 0.2f);
        [SerializeField] private Color bettingColor = new Color(1f, 0.5f, 0.1f);
        [SerializeField] private Color combatColor = new Color(1f, 0.2f, 0.2f);
        [SerializeField] private Color advanceColor = new Color(0.3f, 1f, 0.3f);

        [Header("Action Buttons")]
        [SerializeField] private GameObject setupButtonGroup;
        [SerializeField] private Button confirmTeamButton;
        [SerializeField] private GameObject initiativeButtonGroup;
        [SerializeField] private Button resolveInitiativeButton;
        [SerializeField] private GameObject bettingButtonGroup;
        [SerializeField] private TMP_InputField betAmountInput;
        [SerializeField] private Toggle betExileToggle;
        [SerializeField] private Button placeBetButton;
        [SerializeField] private GameObject combatButtonGroup;
        [SerializeField] private Button confirmCombatButton;
        [SerializeField] private Button concedeRoomButton;
        [SerializeField] private GameObject advanceButtonGroup;
        [SerializeField] private Button advanceRoomButton;

        [Header("Combat Result Panel")]
        [SerializeField] private GameObject combatResultPanel;
        [SerializeField] private TMP_Text combatResultText;
        [SerializeField] private float combatResultDisplayTime = 3f;

        [Header("Match Info")]
        [SerializeField] private TMP_Text matchIdText;

        private string currentMatchId;
        private TurnPhase currentPhase;
        private Coroutine combatResultCoroutine;

        public TurnPhase CurrentPhase => currentPhase;

        public event Action OnConfirmTeam;
        public event Action OnResolveInitiative;
        public event Action<int, bool> OnPlaceBet; // amount, exile
        public event Action OnConfirmCombat;
        public event Action OnConcedeRoom;
        public event Action OnAdvanceRoom;

        // ── Lifecycle ──

        private void OnEnable()
        {
            if (confirmTeamButton != null) confirmTeamButton.onClick.AddListener(HandleConfirmTeam);
            if (resolveInitiativeButton != null) resolveInitiativeButton.onClick.AddListener(HandleResolveInitiative);
            if (placeBetButton != null) placeBetButton.onClick.AddListener(HandlePlaceBet);
            if (confirmCombatButton != null) confirmCombatButton.onClick.AddListener(HandleConfirmCombat);
            if (concedeRoomButton != null) concedeRoomButton.onClick.AddListener(HandleConcedeRoom);
            if (advanceRoomButton != null) advanceRoomButton.onClick.AddListener(HandleAdvanceRoom);
        }

        private void OnDisable()
        {
            if (confirmTeamButton != null) confirmTeamButton.onClick.RemoveListener(HandleConfirmTeam);
            if (resolveInitiativeButton != null) resolveInitiativeButton.onClick.RemoveListener(HandleResolveInitiative);
            if (placeBetButton != null) placeBetButton.onClick.RemoveListener(HandlePlaceBet);
            if (confirmCombatButton != null) confirmCombatButton.onClick.RemoveListener(HandleConfirmCombat);
            if (concedeRoomButton != null) concedeRoomButton.onClick.RemoveListener(HandleConcedeRoom);
            if (advanceRoomButton != null) advanceRoomButton.onClick.RemoveListener(HandleAdvanceRoom);
        }

        // ── Public API ──

        public void SetMatchId(string matchId)
        {
            currentMatchId = matchId;
            if (matchIdText != null)
                matchIdText.text = $"Match: {matchId[..Mathf.Min(8, matchId.Length)]}...";
        }

        public void UpdateFromMatchState(MatchResponse state)
        {
            if (state == null) return;

            // Room progress
            UpdateRoomProgress(state.CurrentRoom);

            // Exile counts (public, contents hidden)
            UpdateExileCounts(
                state.Player1?.ExileCount ?? 0,
                state.Player2?.ExileCount ?? 0);

            // Deck and discard counts
            UpdateDeckDiscardCounts(state);

            // Phase
            TurnPhase phase = MapPhase(state.Phase);
            SetPhase(phase);
        }

        // ── Room Progress ──

        public void UpdateRoomProgress(int currentRoom)
        {
            string roomLabel = currentRoom > 5 ? "BOSS" : $"Room {currentRoom}/5";
            if (playerRoomText != null) playerRoomText.text = roomLabel;

            // Highlight completed rooms
            if (roomIcons != null)
            {
                for (int i = 0; i < roomIcons.Length; i++)
                {
                    if (roomIcons[i] == null) continue;

                    if (i < currentRoom - 1)
                        roomIcons[i].color = Color.green; // completed
                    else if (i == currentRoom - 1)
                        roomIcons[i].color = Color.yellow; // current
                    else
                        roomIcons[i].color = Color.gray; // upcoming
                }
            }
        }

        // ── Exile Counts ──

        public void UpdateExileCounts(int playerExile, int opponentExile)
        {
            if (playerExileText != null)
                playerExileText.text = $"Exile: {playerExile}";
            if (opponentExileText != null)
                opponentExileText.text = $"Exile: {opponentExile}";
        }

        // ── Deck/Discard Counts ──

        private void UpdateDeckDiscardCounts(MatchResponse state)
        {
            if (playerDeckText != null)
                playerDeckText.text = $"Deck: {state.Player1?.DeckCount ?? 0}";
            if (playerDiscardText != null)
                playerDiscardText.text = $"Discard: {state.Player1?.DiscardCount ?? 0}";
            if (opponentDeckText != null)
                opponentDeckText.text = $"Deck: {state.Player2?.DeckCount ?? 0}";
            if (opponentDiscardText != null)
                opponentDiscardText.text = $"Discard: {state.Player2?.DiscardCount ?? 0}";
        }

        // ── Phase Display + Contextual Buttons ──

        public void SetPhase(TurnPhase phase)
        {
            currentPhase = phase;

            // Phase label and color
            string label;
            Color color;
            switch (phase)
            {
                case TurnPhase.Setup:
                    label = "SETUP"; color = setupColor; break;
                case TurnPhase.Initiative:
                    label = "INITIATIVE"; color = initiativeColor; break;
                case TurnPhase.Betting:
                    label = "BID WAR"; color = bettingColor; break;
                case TurnPhase.Combat:
                    label = "COMBAT"; color = combatColor; break;
                case TurnPhase.Advance:
                    label = "ADVANCE"; color = advanceColor; break;
                case TurnPhase.Finished:
                    label = "FINISHED"; color = Color.white; break;
                default:
                    label = phase.ToString(); color = Color.white; break;
            }

            if (phaseText != null)
            {
                phaseText.text = label;
                phaseText.color = color;
            }
            if (phaseIcon != null)
                phaseIcon.color = color;

            // Show/hide button groups
            SetActive(setupButtonGroup, phase == TurnPhase.Setup);
            SetActive(initiativeButtonGroup, phase == TurnPhase.Initiative);
            SetActive(bettingButtonGroup, phase == TurnPhase.Betting);
            SetActive(combatButtonGroup, phase == TurnPhase.Combat);
            SetActive(advanceButtonGroup, phase == TurnPhase.Advance);
        }

        // ── Combat Result Display ──

        public void ShowCombatResult(string outcome, List<CombatResultEntry> results)
        {
            if (combatResultPanel == null) return;

            string text = $"<b>{outcome}</b>\n";
            if (results != null)
            {
                foreach (var r in results)
                {
                    text += $"  {r.AttackerName} vs {r.DefenderName}: ";
                    if (r.AttackerEliminated && r.DefenderEliminated)
                        text += "<color=red>Both eliminated</color>\n";
                    else if (r.AttackerEliminated)
                        text += $"<color=red>Attacker eliminated</color> ({r.DamageToAttacker} dmg)\n";
                    else if (r.DefenderEliminated)
                        text += $"<color=green>Defender eliminated</color> ({r.DamageToDefender} dmg)\n";
                    else
                        text += $"{r.DamageToAttacker} / {r.DamageToDefender} dmg\n";
                }
            }

            if (combatResultText != null)
                combatResultText.text = text;

            combatResultPanel.SetActive(true);

            if (combatResultCoroutine != null)
                StopCoroutine(combatResultCoroutine);
            combatResultCoroutine = StartCoroutine(HideCombatResultAfterDelay());
        }

        public void ShowMatchFinished(string winnerId)
        {
            SetPhase(TurnPhase.Finished);

            if (combatResultPanel != null && combatResultText != null)
            {
                combatResultText.text = $"<size=150%><b>MATCH OVER</b></size>\n\nWinner: {winnerId[..Mathf.Min(8, winnerId.Length)]}...";
                combatResultPanel.SetActive(true);
            }
        }

        private IEnumerator HideCombatResultAfterDelay()
        {
            yield return new WaitForSeconds(combatResultDisplayTime);
            if (combatResultPanel != null)
                combatResultPanel.SetActive(false);
            combatResultCoroutine = null;
        }

        // ── Button Handlers ──

        private void HandleConfirmTeam() => OnConfirmTeam?.Invoke();
        private void HandleResolveInitiative() => OnResolveInitiative?.Invoke();
        private void HandleConfirmCombat() => OnConfirmCombat?.Invoke();
        private void HandleConcedeRoom() => OnConcedeRoom?.Invoke();
        private void HandleAdvanceRoom() => OnAdvanceRoom?.Invoke();

        private void HandlePlaceBet()
        {
            if (betAmountInput == null) return;
            if (!int.TryParse(betAmountInput.text, out int amount) || amount <= 0)
            {
                Debug.LogWarning("[MatchHUD] Invalid bet amount.");
                return;
            }
            bool exile = betExileToggle != null && betExileToggle.isOn;
            OnPlaceBet?.Invoke(amount, exile);

            betAmountInput.text = "";
            if (betExileToggle != null) betExileToggle.isOn = false;
        }

        // ── Helpers ──

        private static TurnPhase MapPhase(string serverPhase)
        {
            return serverPhase switch
            {
                "Setup" => TurnPhase.Setup,
                "Initiative" => TurnPhase.Initiative,
                "Combat" or "BossRoom" => TurnPhase.Combat,
                "RoomReveal" or "RoomResolution" => TurnPhase.Advance,
                "Finished" => TurnPhase.Finished,
                _ => TurnPhase.Setup
            };
        }

        private static void SetActive(GameObject go, bool active)
        {
            if (go != null) go.SetActive(active);
        }
    }

    [Serializable]
    public class CombatResultEntry
    {
        public string AttackerName;
        public string DefenderName;
        public int DamageToAttacker;
        public int DamageToDefender;
        public bool AttackerEliminated;
        public bool DefenderEliminated;
    }
}
