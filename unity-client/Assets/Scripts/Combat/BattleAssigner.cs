using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CardgameDungeon.Unity.Core;
using CardgameDungeon.Unity.Cards;
using CardgameDungeon.Unity.Board;

namespace CardgameDungeon.Unity.Combat
{
    public class BattleAssigner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CombatManager combatManager;
        [SerializeField] private BoardManager boardManager;

        [Header("Line Rendering")]
        [SerializeField] private Material assignmentLineMaterial;
        [SerializeField] private Color lineColor = Color.red;
        [SerializeField] private float lineWidth = 0.05f;

        [Header("Highlight Colors")]
        [SerializeField] private Color attackerHighlightColor = new Color(1f, 0.4f, 0.2f);
        [SerializeField] private Color defenderHighlightColor = new Color(0.2f, 0.4f, 1f);
        [SerializeField] private Color ambusherColor = new Color(0.8f, 0.2f, 0.8f);

        private List<CombatPairing> assignments = new List<CombatPairing>();
        private string selectedAttacker;
        private bool isAssignmentMode;
        private Dictionary<string, LineRenderer> assignmentLines = new Dictionary<string, LineRenderer>();
        private Dictionary<string, CardView> attackerCards = new Dictionary<string, CardView>();
        private Dictionary<string, CardView> defenderCards = new Dictionary<string, CardView>();

        public List<CombatPairing> Assignments => assignments;
        public string SelectedAttacker => selectedAttacker;
        public bool IsAssignmentMode => isAssignmentMode;

        private void OnEnable()
        {
            EventBus.Subscribe<CardClickedEvent>(OnCardClicked);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<CardClickedEvent>(OnCardClicked);
        }

        public void EnterAssignmentMode()
        {
            isAssignmentMode = true;
            selectedAttacker = null;
            assignments.Clear();
            ClearAllLines();

            // Collect attacker and defender card references from the board
            if (boardManager != null)
            {
                attackerCards = boardManager.GetPlayerAllyCards();
                defenderCards = boardManager.GetOpponentAllyCards();
            }

            // Enable interaction on attacker cards
            foreach (var kvp in attackerCards)
            {
                kvp.Value.SetInteractable(true);
                kvp.Value.SetHighlight(false);
            }

            // Enable interaction on defender cards (as targets)
            foreach (var kvp in defenderCards)
            {
                kvp.Value.SetInteractable(true);
                kvp.Value.SetHighlight(false);
            }

            Debug.Log("[BattleAssigner] Assignment mode entered.");
        }

        public void ExitAssignmentMode()
        {
            isAssignmentMode = false;
            selectedAttacker = null;
            ClearAllLines();

            // Disable highlights
            foreach (var kvp in attackerCards)
            {
                kvp.Value.SetHighlight(false);
            }
            foreach (var kvp in defenderCards)
            {
                kvp.Value.SetHighlight(false);
            }

            Debug.Log("[BattleAssigner] Assignment mode exited.");
        }

        public void SelectAttacker(string allyId)
        {
            if (!isAssignmentMode) return;

            // Deselect previous
            if (!string.IsNullOrEmpty(selectedAttacker) && attackerCards.ContainsKey(selectedAttacker))
            {
                attackerCards[selectedAttacker].SetHighlight(false);
            }

            selectedAttacker = allyId;

            // Highlight selected attacker
            if (attackerCards.ContainsKey(allyId))
            {
                attackerCards[allyId].SetHighlight(true);

                // Check if this is an ambusher and tint accordingly
                CardData data = attackerCards[allyId].Data;
                if (data is AllyCardData allyData && allyData.IsAmbusher)
                {
                    Debug.Log($"[BattleAssigner] Selected ambusher: {allyId}");
                }
            }

            // Highlight valid defender targets
            foreach (var kvp in defenderCards)
            {
                kvp.Value.SetHighlight(true);
            }

            Debug.Log($"[BattleAssigner] Attacker selected: {allyId}");
        }

        public void SelectDefender(string allyId)
        {
            if (!isAssignmentMode) return;
            if (string.IsNullOrEmpty(selectedAttacker)) return;

            // Validate ambusher rule: ambushers cannot be assigned to defenders
            // that are already being attacked by non-ambushers (game-specific rule)
            if (!ValidateAmbusherRule(selectedAttacker, allyId))
            {
                Debug.LogWarning($"[BattleAssigner] Ambusher rule violation: {selectedAttacker} -> {allyId}");
                return;
            }

            // Remove any existing assignment for this attacker
            RemoveAssignment(selectedAttacker);

            // Create new pairing
            CombatPairing pairing = new CombatPairing(selectedAttacker, allyId);
            assignments.Add(pairing);

            // Draw visual line
            CreateAssignmentLine(selectedAttacker, allyId);

            // Reset highlights on defender cards
            foreach (var kvp in defenderCards)
            {
                kvp.Value.SetHighlight(false);
            }

            // Clear attacker selection
            if (attackerCards.ContainsKey(selectedAttacker))
            {
                attackerCards[selectedAttacker].SetHighlight(false);
            }

            Debug.Log($"[BattleAssigner] Pairing created: {selectedAttacker} -> {allyId}");

            selectedAttacker = null;
        }

        public void RemoveAssignment(string attackerId)
        {
            CombatPairing existing = assignments.Find(p => p.AttackerId == attackerId);
            if (existing != null)
            {
                assignments.Remove(existing);
                DestroyAssignmentLine(attackerId);
                Debug.Log($"[BattleAssigner] Assignment removed for attacker: {attackerId}");
            }
        }

        public void ConfirmAssignments()
        {
            if (!isAssignmentMode) return;

            if (assignments.Count == 0)
            {
                Debug.LogWarning("[BattleAssigner] No assignments to confirm.");
                return;
            }

            // Submit to combat manager
            if (combatManager != null)
            {
                combatManager.AssignCombat(new List<CombatPairing>(assignments));
            }

            ExitAssignmentMode();

            Debug.Log($"[BattleAssigner] Confirmed {assignments.Count} assignments.");
        }

        public void DrawAssignmentLines()
        {
            ClearAllLines();

            foreach (var pairing in assignments)
            {
                CreateAssignmentLine(pairing.AttackerId, pairing.DefenderId);
            }
        }

        private void CreateAssignmentLine(string attackerId, string defenderId)
        {
            if (!attackerCards.ContainsKey(attackerId) || !defenderCards.ContainsKey(defenderId))
                return;

            // Destroy existing line for this attacker if present
            DestroyAssignmentLine(attackerId);

            Vector3 startPos = attackerCards[attackerId].transform.position;
            Vector3 endPos = defenderCards[defenderId].transform.position;

            GameObject lineObj = new GameObject($"AssignmentLine_{attackerId}");
            lineObj.transform.SetParent(transform);

            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.SetPosition(0, startPos);
            lr.SetPosition(1, endPos);
            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;
            lr.startColor = lineColor;
            lr.endColor = lineColor;

            if (assignmentLineMaterial != null)
            {
                lr.material = assignmentLineMaterial;
            }
            else
            {
                lr.material = new Material(Shader.Find("Sprites/Default"));
                lr.startColor = lineColor;
                lr.endColor = lineColor;
            }

            lr.sortingOrder = 100;
            assignmentLines[attackerId] = lr;
        }

        private void DestroyAssignmentLine(string attackerId)
        {
            if (assignmentLines.TryGetValue(attackerId, out LineRenderer lr))
            {
                if (lr != null)
                {
                    Destroy(lr.gameObject);
                }
                assignmentLines.Remove(attackerId);
            }
        }

        private void ClearAllLines()
        {
            foreach (var kvp in assignmentLines)
            {
                if (kvp.Value != null)
                {
                    Destroy(kvp.Value.gameObject);
                }
            }
            assignmentLines.Clear();
        }

        private bool ValidateAmbusherRule(string attackerId, string defenderId)
        {
            if (!attackerCards.ContainsKey(attackerId)) return false;

            CardData attackerData = attackerCards[attackerId].Data;

            // If the attacker is an ambusher, it can only target defenders that
            // are not already being targeted by other non-ambusher attackers
            if (attackerData is AllyCardData allyData && allyData.IsAmbusher)
            {
                foreach (var existing in assignments)
                {
                    if (existing.DefenderId == defenderId &&
                        existing.AttackerId != attackerId)
                    {
                        // Check if the other attacker targeting this defender is a non-ambusher
                        if (attackerCards.ContainsKey(existing.AttackerId))
                        {
                            CardData otherData = attackerCards[existing.AttackerId].Data;
                            if (otherData is AllyCardData otherAlly && !otherAlly.IsAmbusher)
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        private void Update()
        {
            if (!isAssignmentMode) return;

            // Update line positions to follow card transforms
            foreach (var pairing in assignments)
            {
                if (assignmentLines.TryGetValue(pairing.AttackerId, out LineRenderer lr) && lr != null)
                {
                    if (attackerCards.ContainsKey(pairing.AttackerId))
                    {
                        lr.SetPosition(0, attackerCards[pairing.AttackerId].transform.position);
                    }
                    if (defenderCards.ContainsKey(pairing.DefenderId))
                    {
                        lr.SetPosition(1, defenderCards[pairing.DefenderId].transform.position);
                    }
                }
            }
        }

        private void OnCardClicked(CardClickedEvent evt)
        {
            if (!isAssignmentMode) return;

            CardView card = evt.Card;
            if (card == null || card.Data == null) return;

            string cardId = card.Data.CardId;

            // Determine if clicked card is an attacker or defender
            if (attackerCards.ContainsKey(cardId))
            {
                SelectAttacker(cardId);
            }
            else if (defenderCards.ContainsKey(cardId))
            {
                SelectDefender(cardId);
            }
        }
    }
}
