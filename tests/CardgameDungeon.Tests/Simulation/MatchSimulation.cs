using System.Text;
using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Domain.Services;
using CardgameDungeon.Domain.ValueObjects;

namespace CardgameDungeon.Tests.Simulation;

public class MatchSimulation
{
    private readonly StringBuilder _log = new();
    private readonly CombatResolver _resolver = new();

    [Fact]
    public void FullMatchSimulation()
    {
        Log("╔════════════════════════════════════════════════════════════════╗");
        Log("║     CARDGAME DUNGEON — FULL MATCH SIMULATION (80-card deck) ║");
        Log("╚════════════════════════════════════════════════════════════════╝");
        Log("");

        var p1Id = Guid.NewGuid();
        var p2Id = Guid.NewGuid();

        // Build 80-card decks: 40 adventurer + 40 enemy, all mixed
        var p1AllCards = BuildMixedDeck("P1");
        var p2AllCards = BuildMixedDeck("P2");

        // Dungeon with monsters in every room
        var monsterIds = Enumerable.Range(0, 10).Select(_ => Guid.NewGuid()).ToList();
        var dungeonRooms = new List<DungeonRoomCard>
        {
            new(Guid.NewGuid(), "Goblin Cave", Rarity.Common, 1, monsterIds.Take(2)),
            new(Guid.NewGuid(), "Spider Nest", Rarity.Common, 2, monsterIds.Skip(2).Take(2)),
            new(Guid.NewGuid(), "Undead Crypt", Rarity.Common, 3, monsterIds.Skip(4).Take(2)),
            new(Guid.NewGuid(), "Dragon Lair", Rarity.Common, 4, monsterIds.Skip(6).Take(2)),
            new(Guid.NewGuid(), "Throne Room", Rarity.Common, 5, monsterIds.Skip(8).Take(2)),
        };
        var boss = new BossCard(Guid.NewGuid(), "Strahd von Zarovich", Rarity.Unique, 5, 7, 10, 1,
            "Necrotic Aura — drains 1 HP from each ally at the start of boss combat.",
            race: Race.Undead);

        var player1 = new PlayerState(p1Id, 20, Shuffle(p1AllCards));
        var player2 = new PlayerState(p2Id, 20, Shuffle(p2AllCards));

        player1.RefillHand();
        player2.RefillHand();

        var match = new MatchState(Guid.NewGuid(), player1, player2, dungeonRooms, boss);

        Log($"P1 Deck: {player1.Deck.Count} cards | Hand: {player1.Hand.Count}");
        Log($"P2 Deck: {player2.Deck.Count} cards | Hand: {player2.Hand.Count}");
        LogHandComposition("P1", player1);
        LogHandComposition("P2", player2);
        Log("");

        // ── SETUP (search DECK for allies, not hand) ──
        LogPhase("SETUP — search deck for allies (cost ≤ 5)");

        var p1Available = player1.GetAlliesInDeck().OrderByDescending(a => a.Strength).ToList();
        var p2Available = player2.GetAlliesInDeck().OrderByDescending(a => a.Strength).ToList();
        Log($"P1 has {p1Available.Count} allies in deck to choose from");
        Log($"P2 has {p2Available.Count} allies in deck to choose from");

        // Pick best team that fits cost ≤ 5
        var p1Team = PickSetupTeam(p1Available);
        var p2Team = PickSetupTeam(p2Available);

        // Extract chosen allies from deck
        var p1Extracted = player1.ExtractAlliesFromDeck(p1Team.Select(a => a.Id));
        var p2Extracted = player2.ExtractAlliesFromDeck(p2Team.Select(a => a.Id));

        LogTeam("P1", p1Extracted.ToList());
        LogTeam("P2", p2Extracted.ToList());
        Log($"P1 deck after setup: {player1.Deck.Count} cards");
        Log($"P2 deck after setup: {player2.Deck.Count} cards");

        if (p1Extracted.Count > 0) match.SubmitSetupTeam(p1Id, p1Extracted);
        if (p2Extracted.Count > 0) match.SubmitSetupTeam(p2Id, p2Extracted);

        if (match.BothTeamsSubmitted)
        {
            match.RevealTeams();
            Log("Teams revealed!");
        }
        Log("");

        // ── TURN LOOP (new flow) ──
        int maxRounds = 25;
        int round = 0;

        while (!match.IsFinished && round < maxRounds)
        {
            round++;
            var activeId = match.ActivePlayerId;
            var activeName = activeId == p1Id ? "P1" : "P2";
            Log($"══════════════ ROUND {round} ({activeName}'s turn) ══════════════");
            Log($"Room: {match.CurrentRoom} | Phase: {match.Phase} | Boss: {match.IsBossRoom}");

            // PlayCards phase — active player plays allies/equipment from hand
            if (match.Phase == MatchPhase.PlayCards)
            {
                LogPhase($"PLAY CARDS ({activeName})");
                var active = match.GetActivePlayer();

                // Play allies from hand (up to max 5 in play)
                var alliesInHand = active.Hand.OfType<AllyCard>().OrderByDescending(a => a.Strength).ToList();
                foreach (var ally in alliesInHand)
                {
                    if (active.AlliesInPlay.Count >= PlayerState.MaxAlliesInPlay) break;
                    active.PlayAlly(ally);
                    Log($"  {activeName} plays ALLY: {ally.Name} (STR:{ally.Strength} HP:{ally.HitPoints} INIT:{ally.Initiative} Class:{ally.Class})");
                }

                // Equip equipment on allies (simple: equip weapons on strongest ally)
                var equipInHand = active.Hand.OfType<EquipmentCard>()
                    .Where(e => e.Slot.IsGear())
                    .OrderByDescending(e => e.StrengthModifier + e.HitPointsModifier)
                    .Take(2).ToList();
                foreach (var equip in equipInHand)
                {
                    var targetAlly = active.AlliesInPlay.FirstOrDefault(a =>
                        !active.HasEquipmentSlot(a.Id, equip.Slot));
                    if (targetAlly != null)
                    {
                        try
                        {
                            active.EquipItem(targetAlly.Id, equip);
                            Log($"  {activeName} equips {equip.Name} (+{equip.StrengthModifier}S/+{equip.HitPointsModifier}H) on {targetAlly.Name}");
                        }
                        catch { /* slot occupied or class restriction */ }
                    }
                }

                if (active.AlliesInPlay.Count == 0)
                    Log($"  {activeName} has NO ALLIES to play!");
                else
                    Log($"  {activeName} total field: {active.AlliesInPlay.Count} allies (STR:{active.AlliesInPlay.Sum(a => a.Strength)} HP:{active.AlliesInPlay.Sum(a => a.HitPoints)})");

                match.FinishPlayingCards();
                Log($"→ Phase: {match.Phase}");
            }

            // DefenderSetup phase — defender places monsters and traps
            if (match.Phase == MatchPhase.DefenderSetup)
            {
                var defender = match.GetInactivePlayer();
                var defenderName = activeId == p1Id ? "P2" : "P1";

                LogPhase($"DEFENDER SETUP ({defenderName})");

                if (match.IsBossRoom)
                {
                    Log($"  BOSS ROOM — {match.Boss.Name} awaits! (STR:{match.Boss.Strength} HP:{match.Boss.HitPoints})");
                    Log($"  Boss effect: {match.Boss.Effect}");
                }

                // Defender also plays monsters and traps (even in boss room — they assist the boss)
                {
                    // Defender plays ALL available monsters from hand (up to 3)
                    var monstersInHand = defender.Hand.OfType<MonsterCard>().OrderByDescending(m => m.Strength).ToList();
                    if (monstersInHand.Count == 0)
                    {
                        Log($"  {defenderName} has NO MONSTERS in hand! Room is undefended by monsters.");
                    }
                    else
                    {
                        foreach (var monster in monstersInHand.Take(3))
                        {
                            if (defender.MonstersInPlay.Count >= PlayerState.MaxMonstersInPlay) break;
                            defender.PlayMonster(monster);
                            Log($"  {defenderName} plays MONSTER: {monster.Name} (STR:{monster.Strength} HP:{monster.HitPoints})");
                        }
                    }

                    // Defender sets ALL available traps from hand (up to 2)
                    var trapsInHand = defender.Hand.OfType<TrapCard>().OrderByDescending(t => t.Damage).ToList();
                    if (trapsInHand.Count == 0)
                    {
                        Log($"  {defenderName} has no traps to set.");
                    }
                    else
                    {
                        foreach (var trap in trapsInHand.Take(2))
                        {
                            defender.SetTrap(trap);
                            Log($"  {defenderName} sets TRAP: {trap.Name} (DMG:{trap.Damage})");
                        }
                    }
                }

                match.FinishDefenderSetup();
                Log($"→ Phase: {match.Phase}");
            }

            // Combat phase
            if (match.Phase is MatchPhase.Combat or MatchPhase.BossRoom)
            {
                var attacker = match.GetAttacker();
                var defender = match.GetInactivePlayer();
                var attackerName = activeName;
                var defenderName = activeId == p1Id ? "P2" : "P1";

                // Activate traps against attacker (including boss room)
                while (defender.TrapsSet.Count > 0)
                {
                    var trap = defender.ActivateTrap()!;
                    Log($"  *** TRAP ACTIVATES: {trap.Name} deals {trap.Damage} damage! ***");
                    if (attacker.AlliesInPlay.Count > 0)
                    {
                        var target = attacker.AlliesInPlay.OrderBy(a => a.HitPoints).First();
                        Log($"    -> {target.Name} takes {trap.Damage} trap damage");
                    }
                }

                if (match.IsBossRoom)
                {
                    // ── BOSS COMBAT (boss + defending monsters) ──
                    LogPhase("BOSS COMBAT");
                    var bossCard = match.Boss;
                    var bossMonsters = defender.MonstersInPlay.ToList();
                    var totalDefStr = bossCard.Strength + bossMonsters.Sum(m => m.Strength);
                    var totalDefHp = bossCard.HitPoints + bossMonsters.Sum(m => m.HitPoints);

                    Log($"Attacker ({attackerName}): {FormatAllies(attacker)}");
                    Log($"BOSS: {bossCard.Name} (STR:{bossCard.Strength} HP:{bossCard.HitPoints})");
                    if (bossMonsters.Count > 0)
                        Log($"Boss minions: {string.Join(", ", bossMonsters.Select(m => $"{m.Name}(S{m.Strength}/H{m.HitPoints})"))}");
                    Log($"Total defense: STR:{totalDefStr} HP:{totalDefHp}");
                    Log($"Boss effect: {bossCard.Effect}");

                    if (attacker.AlliesInPlay.Count == 0)
                    {
                        Log("Attacker has no allies — boss wins by default!");
                        foreach (var m in bossMonsters) defender.RemoveMonster(m);
                        match.ResolveCombat(0, 0, false);
                    }
                    else
                    {
                        var atkGroup = attacker.AlliesInPlay.ToList();
                        var result = _resolver.ResolveCombat(atkGroup, bossCard);

                        Log($"  {atkGroup.Count} allies (STR:{result.AttackerStrength}) vs BOSS (STR:{result.DefenderStrength})");
                        Log($"  Result: {result.Outcome}");
                        Log($"  Damage: {result.DamageToAttacker} to attacker, {result.DamageToDefender} to boss");
                        Log($"  Advantage: ATK={result.Advantage.AttackerState} DEF={result.Advantage.DefenderState}");

                        bool atkElim = result.Outcome is CombatOutcome.AttackerEliminated or CombatOutcome.SimultaneousElimination;

                        if (result.Outcome is CombatOutcome.DefenderEliminated or CombatOutcome.SimultaneousElimination)
                        {
                            Log($"  *** BOSS {bossCard.Name} DEFEATED! ***");
                            foreach (var m in bossMonsters)
                                Log($"  *** MINION {m.Name} falls with the boss ***");
                        }
                        if (atkElim)
                        {
                            foreach (var a in atkGroup.ToList())
                                if (attacker.AlliesInPlay.Contains(a))
                                {
                                    Log($"  *** ALLY {a.Name} ELIMINATED by boss ***");
                                    attacker.EliminateAlly(a);
                                }
                        }

                        // Clear any leftover defender monsters
                        foreach (var m in defender.MonstersInPlay.ToList()) defender.RemoveMonster(m);

                        match.ResolveCombat(result.DamageToAttacker, result.DamageToDefender,
                            result.Outcome == CombatOutcome.SimultaneousElimination);
                    }
                }
                else
                {
                    // ── NORMAL ROOM COMBAT ──
                    LogPhase($"COMBAT (Room {match.CurrentRoom})");
                    Log($"Attacker ({attackerName}): {FormatAllies(attacker)}");
                    Log($"Defender ({defenderName}) monsters: {FormatMonsters(defender)}");

                    if (attacker.AlliesInPlay.Count == 0 || defender.MonstersInPlay.Count == 0)
                    {
                        if (attacker.AlliesInPlay.Count == 0)
                            Log("Attacker has no allies — defender wins room!");
                        else
                            Log("Defender has no monsters — room undefended, attacker wins!");
                        foreach (var m in defender.MonstersInPlay.ToList()) defender.RemoveMonster(m);
                        match.ResolveCombat(0, 0, false);
                    }
                    else
                    {
                        var atkGroup = attacker.AlliesInPlay.ToList();
                        var defGroup = defender.MonstersInPlay.ToList();
                        var result = _resolver.ResolveCombat(atkGroup, defGroup, false);

                        Log($"  {atkGroup.Count} allies (STR:{result.AttackerStrength}) vs {defGroup.Count} monsters (STR:{result.DefenderStrength})");
                        Log($"  Result: {result.Outcome}");
                        Log($"  Damage: {result.DamageToAttacker} to attacker, {result.DamageToDefender} to defender");
                        Log($"  Advantage: ATK={result.Advantage.AttackerState} DEF={result.Advantage.DefenderState}");

                        bool defElim = result.Outcome is CombatOutcome.DefenderEliminated or CombatOutcome.SimultaneousElimination;
                        bool atkElim = result.Outcome is CombatOutcome.AttackerEliminated or CombatOutcome.SimultaneousElimination;

                        if (defElim)
                        {
                            foreach (var m in defGroup)
                            {
                                Log($"  *** MONSTER {m.Name} ELIMINATED ***");
                                if (defender.MonstersInPlay.Contains(m))
                                    defender.EliminateMonster(m);
                            }
                        }
                        if (atkElim)
                        {
                            foreach (var a in atkGroup.ToList())
                                if (attacker.AlliesInPlay.Contains(a))
                                {
                                    Log($"  *** ALLY {a.Name} ELIMINATED ***");
                                    attacker.EliminateAlly(a);
                                }
                        }

                        foreach (var m in defender.MonstersInPlay.ToList()) defender.RemoveMonster(m);

                        match.ResolveCombat(result.DamageToAttacker, result.DamageToDefender,
                            result.Outcome == CombatOutcome.SimultaneousElimination);
                    }
                }

                Log($"  Phase: {match.Phase}");
            }

            // RoomCleared — choose to advance or stop
            if (match.Phase == MatchPhase.RoomCleared)
            {
                if (match.CanAdvance)
                {
                    // Defender decides: reshuffle if few monsters in hand, otherwise just draw
                    var def = match.GetInactivePlayer();
                    var defName = activeId == p1Id ? "P2" : "P1";
                    var monstersInDefHand = def.Hand.OfType<MonsterCard>().Count();
                    bool shouldReshuffle = monstersInDefHand < 2;

                    match.AdvanceToNextRoom(defenderReshuffles: shouldReshuffle);

                    if (shouldReshuffle)
                        Log($"ADVANCE → Room {match.CurrentRoom} | {defName} RESHUFFLES hand ({monstersInDefHand} monsters was too few) and draws 8 fresh cards");
                    else
                        Log($"ADVANCE → Room {match.CurrentRoom} | {defName} draws up to 8 (had {monstersInDefHand} monsters)");

                    LogHandComposition(defName, def);
                }
                else
                {
                    match.StopAndHeal();
                    Log($"STOP & HEAL → Cure 1 HP all allies, pass turn");
                }
                Log($"Phase: {match.Phase}");
                LogState(player1, player2);
            }

            Log("");
            if (match.IsFinished) break;
        }

        // ── RESULT ──
        Log("╔════════════════════════════════════════════════════════════════╗");
        Log("║                        MATCH RESULT                          ║");
        Log("╚════════════════════════════════════════════════════════════════╝");
        Log($"Finished: {match.IsFinished} | Phase: {match.Phase}");
        Log($"Winner: {(match.WinnerId.HasValue ? (match.WinnerId == p1Id ? "PLAYER 1" : "PLAYER 2") : "NONE")}");
        Log($"Rounds: {round} | Final room: {match.CurrentRoom}");
        Log($"P1 — Allies:{player1.AlliesInPlay.Count} Deck:{player1.Deck.Count} Hand:{player1.Hand.Count} Discard:{player1.Discard.Count} Exile:{player1.ExileCount}");
        Log($"P2 — Allies:{player2.AlliesInPlay.Count} Deck:{player2.Deck.Count} Hand:{player2.Hand.Count} Discard:{player2.Discard.Count} Exile:{player2.ExileCount}");

        var reportPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "match_simulation_report.txt");
        File.WriteAllText(reportPath, _log.ToString());

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
        var cost = team.Sum(a => a.Cost);
        Log($"{player} team (cost {cost}): {string.Join(", ", team.Select(a => $"{a.Name}(C{a.Cost} S{a.Strength} H{a.HitPoints} I{a.Initiative})"))}");
    }

    private void LogHandComposition(string player, PlayerState ps)
    {
        var allies = ps.Hand.OfType<AllyCard>().Count();
        var monsters = ps.Hand.OfType<MonsterCard>().Count();
        var traps = ps.Hand.OfType<TrapCard>().Count();
        var equip = ps.Hand.OfType<EquipmentCard>().Count();
        Log($"{player} hand composition: {allies} allies, {monsters} monsters, {traps} traps, {equip} equipment");
    }

    private void LogState(PlayerState p1, PlayerState p2)
    {
        Log($"P1: Allies={p1.AlliesInPlay.Count} Deck={p1.Deck.Count} Hand={p1.Hand.Count} Discard={p1.Discard.Count} Exile={p1.ExileCount}");
        Log($"P2: Allies={p2.AlliesInPlay.Count} Deck={p2.Deck.Count} Hand={p2.Hand.Count} Discard={p2.Discard.Count} Exile={p2.ExileCount}");
    }

    private string FormatAllies(PlayerState ps)
    {
        if (ps.AlliesInPlay.Count == 0) return "(none)";
        return string.Join(", ", ps.AlliesInPlay.Select(a => $"{a.Name}(S{a.Strength}/H{a.HitPoints})"));
    }

    private string FormatMonsters(PlayerState ps)
    {
        if (ps.MonstersInPlay.Count == 0) return "(none)";
        return string.Join(", ", ps.MonstersInPlay.Select(m => $"{m.Name}(S{m.Strength}/H{m.HitPoints})"));
    }

    private void Log(string line) => _log.AppendLine(line);

    /// <summary>
    /// Build a realistic 80-card deck:
    /// Adventurer (40): 10 allies + 10 equipment + 20 consumables
    /// Enemy (40): 32 monsters + 8 traps
    /// </summary>
    private static List<Card> BuildMixedDeck(string prefix)
    {
        var cards = new List<Card>();

        // ═══ ADVENTURER (40) ═══

        // ── 10 Allies ──
        // 2 Rare
        cards.Add(new AllyCard(Guid.NewGuid(), $"{prefix}-Paladin Kael", Rarity.Rare, 4, 5, 5, 2, allyClass: AllyClass.Paladin, effect: "Divine shield protects adjacent allies"));
        cards.Add(new AllyCard(Guid.NewGuid(), $"{prefix}-Archmage Vex", Rarity.Rare, 4, 4, 4, 3, allyClass: AllyClass.Mage, effect: "Chain lightning hits all enemies"));

        // 10 Uncommon allies (cost 2-3, STR 3-4, HP 3-4, various classes)
        cards.Add(new AllyCard(Guid.NewGuid(), $"{prefix}-Knight Errant", Rarity.Uncommon, 2, 3, 4, 1, allyClass: AllyClass.Warrior));
        cards.Add(new AllyCard(Guid.NewGuid(), $"{prefix}-Shieldbearer", Rarity.Uncommon, 2, 3, 4, 1, allyClass: AllyClass.Warrior));
        cards.Add(new AllyCard(Guid.NewGuid(), $"{prefix}-Battle Mage", Rarity.Uncommon, 3, 4, 3, 2, allyClass: AllyClass.Mage));
        cards.Add(new AllyCard(Guid.NewGuid(), $"{prefix}-Evoker", Rarity.Uncommon, 2, 3, 3, 2, allyClass: AllyClass.Mage));
        cards.Add(new AllyCard(Guid.NewGuid(), $"{prefix}-War Cleric", Rarity.Uncommon, 2, 3, 4, 1, allyClass: AllyClass.Cleric));
        cards.Add(new AllyCard(Guid.NewGuid(), $"{prefix}-Acolyte", Rarity.Uncommon, 2, 3, 3, 1, allyClass: AllyClass.Cleric));
        cards.Add(new AllyCard(Guid.NewGuid(), $"{prefix}-Cutthroat", Rarity.Uncommon, 2, 4, 3, 2, isAmbusher: true, allyClass: AllyClass.Rogue));
        cards.Add(new AllyCard(Guid.NewGuid(), $"{prefix}-Scout", Rarity.Uncommon, 2, 3, 3, 3, allyClass: AllyClass.Ranger));
        cards.Add(new AllyCard(Guid.NewGuid(), $"{prefix}-Druid of the Grove", Rarity.Uncommon, 3, 3, 4, 2, allyClass: AllyClass.Druid));
        cards.Add(new AllyCard(Guid.NewGuid(), $"{prefix}-Berserker", Rarity.Uncommon, 2, 4, 3, 1, allyClass: AllyClass.Warrior));

        // 10 Common allies (cost 1-2, STR 2-3, HP 2-3)
        for (int i = 0; i < 3; i++)
            cards.Add(new AllyCard(Guid.NewGuid(), $"{prefix}-Militia-{i}", Rarity.Common, 1, 2, 3, 1, allyClass: AllyClass.Warrior));
        for (int i = 0; i < 2; i++)
            cards.Add(new AllyCard(Guid.NewGuid(), $"{prefix}-Apprentice-{i}", Rarity.Common, 1, 2, 2, 2, allyClass: AllyClass.Mage));
        for (int i = 0; i < 2; i++)
            cards.Add(new AllyCard(Guid.NewGuid(), $"{prefix}-Novice Healer-{i}", Rarity.Common, 1, 2, 3, 1, allyClass: AllyClass.Cleric));
        for (int i = 0; i < 2; i++)
            cards.Add(new AllyCard(Guid.NewGuid(), $"{prefix}-Pickpocket-{i}", Rarity.Common, 1, 2, 2, 2, isAmbusher: true, allyClass: AllyClass.Rogue));
        cards.Add(new AllyCard(Guid.NewGuid(), $"{prefix}-Trapper", Rarity.Common, 1, 2, 2, 2, allyClass: AllyClass.Ranger));

        // 5 Uncommon equipment (Weapon, Armor, Shield with +2 mods)
        cards.Add(new EquipmentCard(Guid.NewGuid(), $"{prefix}-Flamebrand Sword", Rarity.Uncommon, 2, 2, 0, 0, EquipmentSlot.Weapon, effect: "Burns enemies on hit"));
        cards.Add(new EquipmentCard(Guid.NewGuid(), $"{prefix}-Enchanted Longsword", Rarity.Uncommon, 2, 2, 0, 1, EquipmentSlot.Weapon));
        cards.Add(new EquipmentCard(Guid.NewGuid(), $"{prefix}-Mithril Chainmail", Rarity.Uncommon, 2, 0, 2, 0, EquipmentSlot.Armor));
        cards.Add(new EquipmentCard(Guid.NewGuid(), $"{prefix}-Tower Shield", Rarity.Uncommon, 2, 0, 2, -1, EquipmentSlot.Shield));
        cards.Add(new EquipmentCard(Guid.NewGuid(), $"{prefix}-Winged Boots", Rarity.Uncommon, 2, 0, 0, 2, EquipmentSlot.Boots));

        // 5 Common equipment (basic +1 mods)
        cards.Add(new EquipmentCard(Guid.NewGuid(), $"{prefix}-Iron Sword", Rarity.Common, 1, 1, 0, 0, EquipmentSlot.Weapon));
        cards.Add(new EquipmentCard(Guid.NewGuid(), $"{prefix}-Wooden Shield", Rarity.Common, 1, 0, 1, 0, EquipmentSlot.Shield));
        cards.Add(new EquipmentCard(Guid.NewGuid(), $"{prefix}-Leather Armor", Rarity.Common, 1, 0, 1, 0, EquipmentSlot.Armor));
        cards.Add(new EquipmentCard(Guid.NewGuid(), $"{prefix}-Iron Helm", Rarity.Common, 1, 0, 1, 0, EquipmentSlot.Helmet));
        cards.Add(new EquipmentCard(Guid.NewGuid(), $"{prefix}-Lucky Charm", Rarity.Common, 1, 0, 0, 1, EquipmentSlot.Accessory));

        // 5 Consumables (Scrolls, Potions — using equipment with Accessory slot)
        cards.Add(new EquipmentCard(Guid.NewGuid(), $"{prefix}-Scroll of Fireball", Rarity.Uncommon, 2, 2, 0, 0, EquipmentSlot.Accessory, effect: "Deals fire damage to all enemies"));
        cards.Add(new EquipmentCard(Guid.NewGuid(), $"{prefix}-Potion of Might", Rarity.Common, 1, 1, 0, 0, EquipmentSlot.Accessory, effect: "Temporary STR boost"));
        cards.Add(new EquipmentCard(Guid.NewGuid(), $"{prefix}-Healing Potion", Rarity.Common, 1, 0, 1, 0, EquipmentSlot.Accessory, effect: "Restores HP"));
        cards.Add(new EquipmentCard(Guid.NewGuid(), $"{prefix}-Scroll of Haste", Rarity.Common, 1, 0, 0, 1, EquipmentSlot.Accessory, effect: "Grants extra initiative"));
        cards.Add(new EquipmentCard(Guid.NewGuid(), $"{prefix}-Smoke Bomb", Rarity.Uncommon, 1, 0, 1, 1, EquipmentSlot.Accessory, effect: "Grants evasion"));

        // ═══ ENEMY (40): 32 monsters + 8 traps ═══

        // ── 32 Monsters ──
        // 4 Rare (STR 5-6, HP 6-7)
        cards.Add(new MonsterCard(Guid.NewGuid(), $"{prefix}-Ancient Wyvern", Rarity.Rare, 4, 6, 7, 1, effect: "Breath attack hits all allies"));
        cards.Add(new MonsterCard(Guid.NewGuid(), $"{prefix}-Death Knight", Rarity.Rare, 4, 5, 7, 2, effect: "Drains life on hit"));
        cards.Add(new MonsterCard(Guid.NewGuid(), $"{prefix}-Beholder", Rarity.Rare, 4, 6, 6, 1, effect: "Antimagic cone"));
        cards.Add(new MonsterCard(Guid.NewGuid(), $"{prefix}-Lich", Rarity.Rare, 4, 5, 6, 2, effect: "Raises dead as undead"));
        // 12 Uncommon (STR 3-4, HP 4-5)
        cards.Add(new MonsterCard(Guid.NewGuid(), $"{prefix}-Ogre Chieftain", Rarity.Uncommon, 3, 4, 5, 1));
        cards.Add(new MonsterCard(Guid.NewGuid(), $"{prefix}-Troll Berserker", Rarity.Uncommon, 3, 4, 5, 1));
        cards.Add(new MonsterCard(Guid.NewGuid(), $"{prefix}-Wraith", Rarity.Uncommon, 2, 3, 4, 2));
        cards.Add(new MonsterCard(Guid.NewGuid(), $"{prefix}-Minotaur", Rarity.Uncommon, 3, 4, 5, 1));
        cards.Add(new MonsterCard(Guid.NewGuid(), $"{prefix}-Harpy", Rarity.Uncommon, 2, 3, 4, 3));
        cards.Add(new MonsterCard(Guid.NewGuid(), $"{prefix}-Phase Spider", Rarity.Uncommon, 2, 3, 4, 2));
        cards.Add(new MonsterCard(Guid.NewGuid(), $"{prefix}-Orc Warlord", Rarity.Uncommon, 3, 4, 4, 1));
        cards.Add(new MonsterCard(Guid.NewGuid(), $"{prefix}-Drow Assassin", Rarity.Uncommon, 2, 3, 4, 3));
        cards.Add(new MonsterCard(Guid.NewGuid(), $"{prefix}-Ghoul Pack", Rarity.Uncommon, 2, 3, 5, 1));
        cards.Add(new MonsterCard(Guid.NewGuid(), $"{prefix}-Manticore", Rarity.Uncommon, 3, 4, 5, 2));
        cards.Add(new MonsterCard(Guid.NewGuid(), $"{prefix}-Basilisk", Rarity.Uncommon, 3, 4, 5, 1));
        cards.Add(new MonsterCard(Guid.NewGuid(), $"{prefix}-Displacer Beast", Rarity.Uncommon, 2, 3, 4, 2));
        // 16 Common (STR 2-3, HP 2-3)
        for (int i = 0; i < 4; i++)
            cards.Add(new MonsterCard(Guid.NewGuid(), $"{prefix}-Goblin Raider-{i}", Rarity.Common, 1, 2, 3, 1));
        for (int i = 0; i < 4; i++)
            cards.Add(new MonsterCard(Guid.NewGuid(), $"{prefix}-Skeleton Warrior-{i}", Rarity.Common, 1, 2, 2, 1));
        for (int i = 0; i < 4; i++)
            cards.Add(new MonsterCard(Guid.NewGuid(), $"{prefix}-Kobold-{i}", Rarity.Common, 1, 2, 2, 2));
        for (int i = 0; i < 4; i++)
            cards.Add(new MonsterCard(Guid.NewGuid(), $"{prefix}-Orc Grunt-{i}", Rarity.Common, 1, 3, 3, 1));

        // ── 8 Traps ──
        // 3 Uncommon (damage 3)
        cards.Add(new TrapCard(Guid.NewGuid(), $"{prefix}-Flame Geyser", Rarity.Uncommon, 2, 3, "Fire erupts"));
        cards.Add(new TrapCard(Guid.NewGuid(), $"{prefix}-Poison Dart Wall", Rarity.Uncommon, 2, 3, "Venomous darts"));
        cards.Add(new TrapCard(Guid.NewGuid(), $"{prefix}-Lightning Rune", Rarity.Uncommon, 2, 3, "Electric discharge"));
        // 5 Common (damage 1-2)
        for (int i = 0; i < 3; i++)
            cards.Add(new TrapCard(Guid.NewGuid(), $"{prefix}-Spike Pit-{i}", Rarity.Common, 1, 2, "Iron spikes"));
        for (int i = 0; i < 2; i++)
            cards.Add(new TrapCard(Guid.NewGuid(), $"{prefix}-Tripwire-{i}", Rarity.Common, 1, 1, "Falling net"));

        // Total: 10 allies + 10 equip + 20 consumables + 32 monsters + 8 traps = 80
        return cards;
    }

    /// <summary>Pick the best team from available allies that fits cost ≤ 5.</summary>
    private static List<AllyCard> PickSetupTeam(List<AllyCard> available)
    {
        var team = new List<AllyCard>();
        var budget = 5;

        foreach (var ally in available.OrderByDescending(a => a.Strength))
        {
            if (ally.Cost <= budget && ally.Cost >= 1)
            {
                team.Add(ally);
                budget -= ally.Cost;
                if (budget <= 0 || team.Count >= 5) break;
            }
        }

        return team;
    }

    private static List<Card> Shuffle(List<Card> cards)
    {
        var rng = new Random(42);
        var list = new List<Card>(cards);
        for (var i = list.Count - 1; i > 0; i--)
        {
            var j = rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
        return list;
    }
}
