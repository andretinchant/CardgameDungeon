using System.Text;
using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Domain.Services;
using CardgameDungeon.Domain.Effects;
using CardgameDungeon.Domain.ValueObjects;

namespace CardgameDungeon.Tests.Simulation;

public class MatchSimulation
{
    private readonly StringBuilder _log = new();
    private readonly CombatResolver _resolver = new();

    [Fact]
    public void FullMatchSimulation()
    {
        Log("╔════════════════════════════════════════════════════════════╗");
        Log("║          CARDGAME DUNGEON — FULL MATCH SIMULATION        ║");
        Log("╚════════════════════════════════════════════════════════════╝");
        Log("");

        // ── BUILD DECKS ──
        var p1Id = Guid.NewGuid();
        var p2Id = Guid.NewGuid();

        var p1Adventurers = BuildAdventurers("P1");
        var p2Adventurers = BuildAdventurers("P2");

        // Monster IDs for rooms
        var monsterIds = new List<Guid>();
        for (int i = 0; i < 10; i++) monsterIds.Add(Guid.NewGuid());

        var dungeonRooms = new List<DungeonRoomCard>
        {
            new(Guid.NewGuid(), "Goblin Cave", Rarity.Common, 1, monsterIds.Take(2).ToList(), effect: "Dark cave with goblins"),
            new(Guid.NewGuid(), "Spider Nest", Rarity.Common, 2, monsterIds.Skip(2).Take(2).ToList(), effect: "Webs everywhere"),
            new(Guid.NewGuid(), "Undead Crypt", Rarity.Common, 3, monsterIds.Skip(4).Take(2).ToList(), effect: "Cold and silent"),
            new(Guid.NewGuid(), "Dragon Lair Entrance", Rarity.Common, 4, monsterIds.Skip(6).Take(2).ToList(), effect: "Heat intensifies"),
            new(Guid.NewGuid(), "Throne Room", Rarity.Common, 5, monsterIds.Skip(8).Take(2).ToList(), effect: "The final chamber"),
        };

        var boss = new BossCard(Guid.NewGuid(), "Strahd von Zarovich", Rarity.Unique, 5, 7, 10, 1,
            "Castelo Sombrio. Restaura HP = aliados mortos no descarte.");

        var player1 = new PlayerState(p1Id, 20, Shuffle(p1Adventurers));
        var player2 = new PlayerState(p2Id, 20, Shuffle(p2Adventurers));

        player1.RefillHand();
        player2.RefillHand();

        var match = new MatchState(Guid.NewGuid(), player1, player2, dungeonRooms, boss);

        Log($"Match ID: {match.Id}");
        Log($"Player 1 (P1): Warrior/Barbarian focus");
        Log($"Player 2 (P2): Rogue/Mage focus");
        Log($"Dungeon: 5 rooms + Boss (Strahd)");
        Log($"P1 Hand: {player1.Hand.Count} | Deck: {player1.Deck.Count}");
        Log($"P2 Hand: {player2.Hand.Count} | Deck: {player2.Deck.Count}");
        Log("");

        // ── SETUP PHASE ──
        LogPhase("SETUP (cost ≤ 5)");

        var p1Team = player1.Hand.OfType<AllyCard>().OrderByDescending(a => a.Strength).Take(3).ToList();
        var p2Team = player2.Hand.OfType<AllyCard>().OrderByDescending(a => a.Initiative).Take(3).ToList();

        LogTeam("P1", p1Team);
        LogTeam("P2", p2Team);

        match.SubmitSetupTeam(p1Id, p1Team);
        match.SubmitSetupTeam(p2Id, p2Team);
        Log("Both teams submitted.");

        match.RevealTeams();
        Log("Teams revealed!");
        LogState(player1, player2, "P1", "P2");
        Log("");

        // ── ROOM LOOP ──
        int maxRounds = 20;
        int round = 0;

        while (!match.IsFinished && round < maxRounds)
        {
            round++;
            Log($"────────────────────── Round {round} ──────────────────────");
            Log($"Current Room: {match.CurrentRoom} | Phase: {match.Phase} | IsBoss: {match.IsBossRoom}");

            // Handle phase transitions
            if (match.Phase == MatchPhase.RoomReveal)
            {
                var roomName = match.CurrentDungeonRoom?.Name ?? "Boss Room";
                Log($"Revealing: {roomName}");
                match.RevealRoom();
                Log($"After reveal → Phase: {match.Phase}");

                if (match.Phase == MatchPhase.RoomReveal)
                {
                    Log("Room has no monsters — auto-advancing...");
                    continue;
                }
            }

            if (match.Phase == MatchPhase.Initiative)
            {
                LogPhase($"INITIATIVE (Room {match.CurrentRoom})");
                var initResult = _resolver.ResolveInitiative(
                    player1.AlliesInPlay, p1Id, player2.AlliesInPlay, p2Id);

                Log($"P1 Total INIT: {initResult.Player1Total} | P2 Total INIT: {initResult.Player2Total}");
                match.ResolveInitiative(initResult.Player1Total, initResult.Player2Total);

                if (initResult.IsTied)
                {
                    Log("TIED — P1 bets 1 (discard)");
                    if (player1.Deck.Count > 0)
                    {
                        match.PlaceBet(p1Id, 1, false);
                        match.TryResolveBets();
                    }
                }

                Log($"Winner: {(match.InitiativeWinnerId == p1Id ? "P1" : "P2")} → Phase: {match.Phase}");
            }

            if (match.Phase == MatchPhase.RoleSelection)
            {
                var winnerId = match.InitiativeWinnerId!.Value;
                bool choosesAttack = true; // AI: always attack
                match.ChooseRole(winnerId, choosesAttack);
                var winner = winnerId == p1Id ? "P1" : "P2";
                Log($"{winner} chooses to {(choosesAttack ? "ATTACK" : "DEFEND")}");
                Log($"Attacker: {(match.AttackerId == p1Id ? "P1" : "P2")}");
            }

            if (match.Phase is MatchPhase.Combat or MatchPhase.BossRoom)
            {
                var isBoss = match.Phase == MatchPhase.BossRoom;
                LogPhase(isBoss ? "BOSS COMBAT" : $"COMBAT (Room {match.CurrentRoom})");

                var attacker = match.GetAttacker();
                var defender = match.GetDefender();

                Log($"Attacker ({(match.AttackerId == p1Id ? "P1" : "P2")}): {FormatAllies(attacker)}");
                Log($"Defender ({(match.AttackerId == p1Id ? "P2" : "P1")}): {FormatAllies(defender)}");

                if (attacker.AlliesInPlay.Count == 0 || defender.AlliesInPlay.Count == 0)
                {
                    Log("One side has no allies — skipping combat");
                    match.ResolveCombat(0, 0, false);
                }
                else
                {
                    // Assign: all attackers → first defender
                    var defTarget = defender.AlliesInPlay[0];
                    foreach (var atkAlly in attacker.AlliesInPlay)
                    {
                        match.CombatBoard.Assign(atkAlly, defTarget, defender.AlliesInPlay);
                        Log($"  Assign: {atkAlly.Name}(S{atkAlly.Strength}) → {defTarget.Name}(H{defTarget.HitPoints})");
                    }

                    var atkGroup = attacker.AlliesInPlay.ToList();
                    var defGroup = new[] { defTarget };
                    var advantage = CombatAdvantage.Calculate(atkGroup.Count, defGroup.Length);
                    var result = _resolver.ResolveCombat(atkGroup, defGroup, isBoss);

                    Log($"  Result: {result.Outcome}");
                    Log($"  ATK STR: {result.AttackerStrength} → {result.DamageToDefender} dmg to defender");
                    Log($"  DEF STR: {result.DefenderStrength} → {result.DamageToAttacker} dmg to attacker");
                    Log($"  Advantage: ATK={advantage.AttackerState} DEF={advantage.DefenderState}");

                    bool defElim = result.Outcome is CombatOutcome.DefenderEliminated or CombatOutcome.SimultaneousElimination;
                    bool atkElim = result.Outcome is CombatOutcome.AttackerEliminated or CombatOutcome.SimultaneousElimination;

                    if (defElim)
                    {
                        Log($"  *** {defTarget.Name} ELIMINATED ***");
                        defender.EliminateAlly(defTarget);
                    }
                    if (atkElim)
                    {
                        foreach (var a in atkGroup.ToList())
                        {
                            if (attacker.AlliesInPlay.Contains(a))
                            {
                                Log($"  *** {a.Name} ELIMINATED ***");
                                attacker.EliminateAlly(a);
                            }
                        }
                    }

                    match.ResolveCombat(result.DamageToAttacker, result.DamageToDefender,
                        result.Outcome == CombatOutcome.SimultaneousElimination);

                    Log($"  Post-combat phase: {match.Phase}");
                }
            }

            if (match.Phase == MatchPhase.RoomResolution)
            {
                match.AdvanceRoom();
                Log($"ADVANCE → Room {match.CurrentRoom} | Phase: {match.Phase}");
                LogState(player1, player2, "P1", "P2");
            }

            Log("");

            if (match.IsFinished) break;
        }

        // ── FINAL REPORT ──
        Log("╔════════════════════════════════════════════════════════════╗");
        Log("║                     MATCH RESULT                         ║");
        Log("╚════════════════════════════════════════════════════════════╝");
        Log($"Phase: {match.Phase}");
        Log($"Finished: {match.IsFinished}");
        Log($"Winner: {(match.WinnerId.HasValue ? (match.WinnerId == p1Id ? "PLAYER 1" : "PLAYER 2") : "NONE (match incomplete)")}");
        Log($"Rounds played: {round}");
        Log($"Final room: {match.CurrentRoom}");
        Log($"");
        Log($"P1 — Allies: {player1.AlliesInPlay.Count}, Deck: {player1.Deck.Count}, Hand: {player1.Hand.Count}, Discard: {player1.Discard.Count}, Exile: {player1.ExileCount}, Dead allies: {player1.DeadAlliesInDiscard}");
        Log($"P2 — Allies: {player2.AlliesInPlay.Count}, Deck: {player2.Deck.Count}, Hand: {player2.Hand.Count}, Discard: {player2.Discard.Count}, Exile: {player2.ExileCount}, Dead allies: {player2.DeadAlliesInDiscard}");

        // Write report
        var reportPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "match_simulation_report.txt");
        File.WriteAllText(reportPath, _log.ToString());

        // Simulation always passes — it's a report generator, not a correctness test
        Assert.True(true, "Simulation completed. See match_simulation_report.txt");
    }

    // ── Helpers ──

    private void LogPhase(string name)
    {
        Log($"═══════════════════════════════════════");
        Log($"  {name}");
        Log($"═══════════════════════════════════════");
    }

    private void LogTeam(string player, List<AllyCard> team)
    {
        var total = team.Sum(a => a.Cost);
        Log($"{player} team (cost {total}): {string.Join(", ", team.Select(a => $"{a.Name}(C{a.Cost} S{a.Strength} H{a.HitPoints} I{a.Initiative})"))}");
    }

    private void LogState(PlayerState p1, PlayerState p2, string n1, string n2)
    {
        Log($"{n1}: Allies={p1.AlliesInPlay.Count} Deck={p1.Deck.Count} Hand={p1.Hand.Count} Discard={p1.Discard.Count} Exile={p1.ExileCount}");
        Log($"{n2}: Allies={p2.AlliesInPlay.Count} Deck={p2.Deck.Count} Hand={p2.Hand.Count} Discard={p2.Discard.Count} Exile={p2.ExileCount}");
    }

    private string FormatAllies(PlayerState ps)
    {
        if (ps.AlliesInPlay.Count == 0) return "(none)";
        return string.Join(", ", ps.AlliesInPlay.Select(a => $"{a.Name}(S{a.Strength}/H{a.HitPoints}/I{a.Initiative})"));
    }

    private void Log(string line) => _log.AppendLine(line);

    private static List<Card> BuildAdventurers(string prefix)
    {
        var cards = new List<Card>();
        for (int i = 0; i < 15; i++)
            cards.Add(new AllyCard(Guid.NewGuid(), $"{prefix}-Fighter-{i}", Rarity.Common, 1, 2, 3, 1, allyClass: AllyClass.Warrior));
        for (int i = 0; i < 10; i++)
            cards.Add(new AllyCard(Guid.NewGuid(), $"{prefix}-Veteran-{i}", Rarity.Common, 2, 3, 4, 1, allyClass: AllyClass.Warrior));
        for (int i = 0; i < 15; i++)
            cards.Add(new AllyCard(Guid.NewGuid(), $"{prefix}-Recruit-{i}", Rarity.Common, 1, 1, 2, 1));
        return cards;
    }

    private static List<Card> Shuffle(List<Card> cards)
    {
        var rng = new Random(42); // Fixed seed for reproducibility
        var list = new List<Card>(cards);
        for (var i = list.Count - 1; i > 0; i--)
        {
            var j = rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
        return list;
    }
}
