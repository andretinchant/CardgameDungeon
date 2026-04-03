using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardgameDungeon.Unity.Core;
using CardgameDungeon.Unity.Network;
using CardgameDungeon.Unity.Board;
using CardgameDungeon.Unity.UI;

namespace CardgameDungeon.Unity.Combat
{
    public enum MatchPhase
    {
        Setup,
        RoomReveal,
        Initiative,
        Combat,
        RoomResolution,
        BossRoom,
        Finished
    }

    [Serializable]
    public class CombatPairing
    {
        public string AttackerId;
        public string DefenderId;

        public CombatPairing(string attackerId, string defenderId)
        {
            AttackerId = attackerId;
            DefenderId = defenderId;
        }
    }

    public class CombatManager : MonoBehaviour
    {
        [SerializeField] private BoardManager boardManager;
        [SerializeField] private MatchHUD matchHud;

        private string currentMatchId;
        private MatchPhase currentPhase;
        private bool isPlayerTurn;

        public string CurrentMatchId => currentMatchId;
        public MatchPhase CurrentPhase => currentPhase;
        public bool IsPlayerTurn => isPlayerTurn;

        public event Action<MatchPhase> OnPhaseChanged;

        private void OnEnable()
        {
            EventBus.Subscribe<StartMatchRequestEvent>(OnStartMatchRequest);
            EventBus.Subscribe<SubmitTeamRequestEvent>(OnSubmitTeamRequest);
            EventBus.Subscribe<PlaceBetRequestEvent>(OnPlaceBetRequest);
            EventBus.Subscribe<AssignCombatRequestEvent>(OnAssignCombatRequest);
            EventBus.Subscribe<RetargetRequestEvent>(OnRetargetRequest);
            EventBus.Subscribe<AdvanceRoomRequestEvent>(OnAdvanceRoomRequest);
            EventBus.Subscribe<ConcedeRoomRequestEvent>(OnConcedeRoomRequest);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<StartMatchRequestEvent>(OnStartMatchRequest);
            EventBus.Unsubscribe<SubmitTeamRequestEvent>(OnSubmitTeamRequest);
            EventBus.Unsubscribe<PlaceBetRequestEvent>(OnPlaceBetRequest);
            EventBus.Unsubscribe<AssignCombatRequestEvent>(OnAssignCombatRequest);
            EventBus.Unsubscribe<RetargetRequestEvent>(OnRetargetRequest);
            EventBus.Unsubscribe<AdvanceRoomRequestEvent>(OnAdvanceRoomRequest);
            EventBus.Unsubscribe<ConcedeRoomRequestEvent>(OnConcedeRoomRequest);
        }

        private void SetPhase(MatchPhase phase)
        {
            currentPhase = phase;
            OnPhaseChanged?.Invoke(currentPhase);
            if (matchHud != null)
            {
                matchHud.UpdatePhaseDisplay(currentPhase.ToString());
            }
        }

        // ----------------------------------------------------------------
        // Match flow methods
        // ----------------------------------------------------------------

        public void StartMatch(string player1Id, string player2Id, string deck1Id, string deck2Id)
        {
            StartCoroutine(StartMatchRoutine(player1Id, player2Id, deck1Id, deck2Id));
        }

        private IEnumerator StartMatchRoutine(string player1Id, string player2Id, string deck1Id, string deck2Id)
        {
            SetPhase(MatchPhase.Setup);

            var request = new StartMatchRequest
            {
                Player1Id = player1Id,
                Player2Id = player2Id,
                Deck1Id = deck1Id,
                Deck2Id = deck2Id
            };

            var task = ApiClient.Instance.Post<StartMatchRequest, StartMatchResponse>(
                "/api/matches", request);
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.Result != null)
            {
                currentMatchId = task.Result.MatchId;
                isPlayerTurn = true;

                if (matchHud != null)
                {
                    matchHud.ShowMessage("Match started! Select your team.");
                }

                if (boardManager != null)
                {
                    boardManager.InitializeBoard(task.Result);
                }

                Debug.Log($"[CombatManager] Match created: {currentMatchId}");
            }
            else
            {
                Debug.LogError("[CombatManager] Failed to start match.");
                if (matchHud != null)
                {
                    matchHud.ShowMessage("Failed to start match.");
                }
            }
        }

        public void SubmitSetupTeam(List<string> allyIds)
        {
            StartCoroutine(SubmitSetupTeamRoutine(allyIds));
        }

        private IEnumerator SubmitSetupTeamRoutine(List<string> allyIds)
        {
            var request = new SubmitTeamRequest { AllyIds = allyIds };

            var task = ApiClient.Instance.Post<SubmitTeamRequest, SubmitTeamResponse>(
                $"/api/matches/{currentMatchId}/setup-team", request);
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.Result != null)
            {
                isPlayerTurn = false;

                if (matchHud != null)
                {
                    matchHud.ShowMessage("Team submitted. Waiting for opponent...");
                }

                Debug.Log("[CombatManager] Team submitted successfully.");
            }
            else
            {
                Debug.LogError("[CombatManager] Failed to submit team.");
            }
        }

        public void RevealTeams()
        {
            StartCoroutine(RevealTeamsRoutine());
        }

        private IEnumerator RevealTeamsRoutine()
        {
            var task = ApiClient.Instance.Get<RevealTeamsResponse>(
                $"/api/matches/{currentMatchId}/reveal-teams");
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.Result != null)
            {
                SetPhase(MatchPhase.RoomReveal);

                if (boardManager != null)
                {
                    boardManager.RevealTeams(task.Result);
                }

                if (matchHud != null)
                {
                    matchHud.ShowMessage("Teams revealed!");
                }

                Debug.Log("[CombatManager] Teams revealed.");
            }
            else
            {
                Debug.LogError("[CombatManager] Failed to reveal teams.");
            }
        }

        public void ResolveInitiative()
        {
            StartCoroutine(ResolveInitiativeRoutine());
        }

        private IEnumerator ResolveInitiativeRoutine()
        {
            SetPhase(MatchPhase.Initiative);

            var task = ApiClient.Instance.Post<EmptyRequest, InitiativeResponse>(
                $"/api/matches/{currentMatchId}/initiative", new EmptyRequest());
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.Result != null)
            {
                isPlayerTurn = task.Result.IsLocalPlayerFirst;

                EventBus.Publish(new InitiativeResolvedEvent(task.Result));

                if (matchHud != null)
                {
                    string turnLabel = isPlayerTurn ? "You go first!" : "Opponent goes first.";
                    matchHud.ShowMessage(turnLabel);
                }

                Debug.Log($"[CombatManager] Initiative resolved. Player first: {isPlayerTurn}");
            }
            else
            {
                Debug.LogError("[CombatManager] Failed to resolve initiative.");
            }
        }

        public void PlaceBet(int amount, bool exile)
        {
            StartCoroutine(PlaceBetRoutine(amount, exile));
        }

        private IEnumerator PlaceBetRoutine(int amount, bool exile)
        {
            var request = new PlaceBetRequest { Amount = amount, Exile = exile };

            var task = ApiClient.Instance.Post<PlaceBetRequest, PlaceBetResponse>(
                $"/api/matches/{currentMatchId}/bet", request);
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.Result != null)
            {
                isPlayerTurn = task.Result.IsLocalPlayerFirst;

                if (matchHud != null)
                {
                    matchHud.UpdateTreasure(task.Result.RemainingTreasure);
                    matchHud.ShowMessage(task.Result.IsTied ? "Still tied! Bet again." : "Bet placed.");
                }

                Debug.Log($"[CombatManager] Bet placed: {amount}, exile: {exile}");
            }
            else
            {
                Debug.LogError("[CombatManager] Failed to place bet.");
            }
        }

        public void AssignCombat(List<CombatPairing> pairings)
        {
            StartCoroutine(AssignCombatRoutine(pairings));
        }

        private IEnumerator AssignCombatRoutine(List<CombatPairing> pairings)
        {
            SetPhase(MatchPhase.Combat);

            var request = new AssignCombatRequest { Pairings = pairings };

            var task = ApiClient.Instance.Post<AssignCombatRequest, AssignCombatResponse>(
                $"/api/matches/{currentMatchId}/assign-combat", request);
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.Result != null)
            {
                if (boardManager != null)
                {
                    boardManager.ShowCombatPairings(task.Result);
                }

                if (matchHud != null)
                {
                    matchHud.ShowMessage("Combat assignments confirmed.");
                }

                Debug.Log("[CombatManager] Combat assignments submitted.");
            }
            else
            {
                Debug.LogError("[CombatManager] Failed to assign combat.");
            }
        }

        public void ResolveCombatRound()
        {
            StartCoroutine(ResolveCombatRoundRoutine());
        }

        private IEnumerator ResolveCombatRoundRoutine()
        {
            var task = ApiClient.Instance.Post<EmptyRequest, CombatRoundResponse>(
                $"/api/matches/{currentMatchId}/resolve-combat", new EmptyRequest());
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.Result != null)
            {
                if (boardManager != null)
                {
                    boardManager.ApplyCombatResults(task.Result);
                }

                if (matchHud != null)
                {
                    matchHud.UpdateCombatLog(task.Result.LogEntries);
                }

                if (task.Result.IsCombatOver)
                {
                    SetPhase(MatchPhase.RoomResolution);
                    if (matchHud != null)
                    {
                        matchHud.ShowMessage("Combat resolved!");
                    }
                }

                Debug.Log("[CombatManager] Combat round resolved.");
            }
            else
            {
                Debug.LogError("[CombatManager] Failed to resolve combat round.");
            }
        }

        public void Retarget(string allyId, string newDefenderId, int cost, bool exileCost)
        {
            StartCoroutine(RetargetRoutine(allyId, newDefenderId, cost, exileCost));
        }

        private IEnumerator RetargetRoutine(string allyId, string newDefenderId, int cost, bool exileCost)
        {
            var request = new RetargetRequest
            {
                AllyId = allyId,
                NewDefenderId = newDefenderId,
                Cost = cost,
                ExileCost = exileCost
            };

            var task = ApiClient.Instance.Post<RetargetRequest, RetargetResponse>(
                $"/api/matches/{currentMatchId}/retarget", request);
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.Result != null)
            {
                if (boardManager != null)
                {
                    boardManager.UpdateTargetLines(task.Result);
                }

                if (matchHud != null)
                {
                    matchHud.UpdateTreasure(task.Result.RemainingTreasure);
                    matchHud.ShowMessage("Retarget successful.");
                }

                Debug.Log($"[CombatManager] Retargeted {allyId} to {newDefenderId}.");
            }
            else
            {
                Debug.LogError("[CombatManager] Failed to retarget.");
            }
        }

        public void OpportunityAttack(string attackerAllyId, string fleeingAllyId)
        {
            StartCoroutine(OpportunityAttackRoutine(attackerAllyId, fleeingAllyId));
        }

        private IEnumerator OpportunityAttackRoutine(string attackerAllyId, string fleeingAllyId)
        {
            var request = new OpportunityAttackRequest
            {
                AttackerAllyId = attackerAllyId,
                FleeingAllyId = fleeingAllyId
            };

            var task = ApiClient.Instance.Post<OpportunityAttackRequest, OpportunityAttackResponse>(
                $"/api/matches/{currentMatchId}/opportunity-attack", request);
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.Result != null)
            {
                if (boardManager != null)
                {
                    boardManager.PlayOpportunityAttack(task.Result);
                }

                if (matchHud != null)
                {
                    matchHud.ShowMessage($"Opportunity attack! {task.Result.DamageDealt} damage dealt.");
                }

                Debug.Log($"[CombatManager] Opportunity attack: {attackerAllyId} -> {fleeingAllyId}");
            }
            else
            {
                Debug.LogError("[CombatManager] Failed to execute opportunity attack.");
            }
        }

        public void AdvanceRoom()
        {
            StartCoroutine(AdvanceRoomRoutine());
        }

        private IEnumerator AdvanceRoomRoutine()
        {
            var task = ApiClient.Instance.Post<EmptyRequest, AdvanceRoomResponse>(
                $"/api/matches/{currentMatchId}/advance-room", new EmptyRequest());
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.Result != null)
            {
                if (task.Result.IsBossRoom)
                {
                    SetPhase(MatchPhase.BossRoom);
                }
                else if (task.Result.IsMatchOver)
                {
                    SetPhase(MatchPhase.Finished);
                }
                else
                {
                    SetPhase(MatchPhase.RoomReveal);
                }

                if (boardManager != null)
                {
                    boardManager.SetupRoom(task.Result);
                }

                if (matchHud != null)
                {
                    matchHud.ShowMessage($"Entering Room {task.Result.RoomNumber}...");
                }

                Debug.Log($"[CombatManager] Advanced to room {task.Result.RoomNumber}.");
            }
            else
            {
                Debug.LogError("[CombatManager] Failed to advance room.");
            }
        }

        public void ConcedeRoom()
        {
            StartCoroutine(ConcedeRoomRoutine());
        }

        private IEnumerator ConcedeRoomRoutine()
        {
            var task = ApiClient.Instance.Post<EmptyRequest, ConcedeRoomResponse>(
                $"/api/matches/{currentMatchId}/concede-room", new EmptyRequest());
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.Result != null)
            {
                SetPhase(MatchPhase.RoomResolution);

                if (matchHud != null)
                {
                    matchHud.ShowMessage("Room conceded. Opponent claims treasure.");
                }

                Debug.Log("[CombatManager] Room conceded.");
            }
            else
            {
                Debug.LogError("[CombatManager] Failed to concede room.");
            }
        }

        public void RefreshMatchState()
        {
            StartCoroutine(RefreshMatchStateRoutine());
        }

        private IEnumerator RefreshMatchStateRoutine()
        {
            var task = ApiClient.Instance.Get<MatchStateResponse>(
                $"/api/matches/{currentMatchId}");
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.Result != null)
            {
                currentMatchId = task.Result.MatchId;
                isPlayerTurn = task.Result.IsLocalPlayerTurn;

                MatchPhase parsedPhase;
                if (Enum.TryParse(task.Result.Phase, out parsedPhase))
                {
                    SetPhase(parsedPhase);
                }

                if (boardManager != null)
                {
                    boardManager.SyncBoardState(task.Result);
                }

                if (matchHud != null)
                {
                    matchHud.RefreshAll(task.Result);
                }

                Debug.Log("[CombatManager] Match state refreshed.");
            }
            else
            {
                Debug.LogError("[CombatManager] Failed to refresh match state.");
            }
        }

        // ----------------------------------------------------------------
        // EventBus handlers
        // ----------------------------------------------------------------

        private void OnStartMatchRequest(StartMatchRequestEvent evt)
        {
            StartMatch(evt.Player1Id, evt.Player2Id, evt.Deck1Id, evt.Deck2Id);
        }

        private void OnSubmitTeamRequest(SubmitTeamRequestEvent evt)
        {
            SubmitSetupTeam(evt.AllyIds);
        }

        private void OnPlaceBetRequest(PlaceBetRequestEvent evt)
        {
            PlaceBet(evt.Amount, evt.Exile);
        }

        private void OnAssignCombatRequest(AssignCombatRequestEvent evt)
        {
            AssignCombat(evt.Pairings);
        }

        private void OnRetargetRequest(RetargetRequestEvent evt)
        {
            Retarget(evt.AllyId, evt.NewDefenderId, evt.Cost, evt.ExileCost);
        }

        private void OnAdvanceRoomRequest(AdvanceRoomRequestEvent evt)
        {
            AdvanceRoom();
        }

        private void OnConcedeRoomRequest(ConcedeRoomRequestEvent evt)
        {
            ConcedeRoom();
        }
    }

    // ----------------------------------------------------------------
    // Event Bus event types for UI-to-CombatManager communication
    // ----------------------------------------------------------------

    public class StartMatchRequestEvent
    {
        public string Player1Id;
        public string Player2Id;
        public string Deck1Id;
        public string Deck2Id;
    }

    public class SubmitTeamRequestEvent
    {
        public List<string> AllyIds;
    }

    public class PlaceBetRequestEvent
    {
        public int Amount;
        public bool Exile;
    }

    public class AssignCombatRequestEvent
    {
        public List<CombatPairing> Pairings;
    }

    public class RetargetRequestEvent
    {
        public string AllyId;
        public string NewDefenderId;
        public int Cost;
        public bool ExileCost;
    }

    public class AdvanceRoomRequestEvent { }

    public class ConcedeRoomRequestEvent { }

    public class InitiativeResolvedEvent
    {
        public InitiativeResponse Response;

        public InitiativeResolvedEvent(InitiativeResponse response)
        {
            Response = response;
        }
    }

    // ----------------------------------------------------------------
    // API request/response DTOs
    // ----------------------------------------------------------------

    [Serializable]
    public class EmptyRequest { }

    [Serializable]
    public class StartMatchRequest
    {
        public string Player1Id;
        public string Player2Id;
        public string Deck1Id;
        public string Deck2Id;
    }

    [Serializable]
    public class StartMatchResponse
    {
        public string MatchId;
        public List<string> Player1Hand;
        public List<string> Player2Hand;
    }

    [Serializable]
    public class SubmitTeamRequest
    {
        public List<string> AllyIds;
    }

    [Serializable]
    public class SubmitTeamResponse
    {
        public bool Success;
    }

    [Serializable]
    public class RevealTeamsResponse
    {
        public List<string> Player1Team;
        public List<string> Player2Team;
    }

    [Serializable]
    public class InitiativeResponse
    {
        public int Player1Total;
        public int Player2Total;
        public string WinnerId;
        public bool IsTied;
        public bool IsLocalPlayerFirst;
    }

    [Serializable]
    public class PlaceBetRequest
    {
        public int Amount;
        public bool Exile;
    }

    [Serializable]
    public class PlaceBetResponse
    {
        public bool IsTied;
        public bool IsLocalPlayerFirst;
        public int RemainingTreasure;
    }

    [Serializable]
    public class AssignCombatRequest
    {
        public List<CombatPairing> Pairings;
    }

    [Serializable]
    public class AssignCombatResponse
    {
        public bool Success;
        public List<CombatPairing> ConfirmedPairings;
    }

    [Serializable]
    public class CombatRoundResponse
    {
        public bool IsCombatOver;
        public List<string> LogEntries;
        public List<CombatResultEntry> Results;
    }

    [Serializable]
    public class CombatResultEntry
    {
        public string AttackerId;
        public string DefenderId;
        public int DamageDealt;
        public bool DefenderEliminated;
    }

    [Serializable]
    public class RetargetRequest
    {
        public string AllyId;
        public string NewDefenderId;
        public int Cost;
        public bool ExileCost;
    }

    [Serializable]
    public class RetargetResponse
    {
        public bool Success;
        public int RemainingTreasure;
    }

    [Serializable]
    public class OpportunityAttackRequest
    {
        public string AttackerAllyId;
        public string FleeingAllyId;
    }

    [Serializable]
    public class OpportunityAttackResponse
    {
        public int DamageDealt;
        public bool TargetEliminated;
    }

    [Serializable]
    public class AdvanceRoomResponse
    {
        public int RoomNumber;
        public bool IsBossRoom;
        public bool IsMatchOver;
        public string RoomCardId;
    }

    [Serializable]
    public class ConcedeRoomResponse
    {
        public bool Success;
        public int OpponentTreasureGained;
    }

    [Serializable]
    public class MatchStateResponse
    {
        public string MatchId;
        public string Phase;
        public bool IsLocalPlayerTurn;
        public int Player1Treasure;
        public int Player2Treasure;
        public int CurrentRoom;
        public List<string> Player1Team;
        public List<string> Player2Team;
    }
}
