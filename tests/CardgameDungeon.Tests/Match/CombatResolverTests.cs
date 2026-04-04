using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Domain.Services;

namespace CardgameDungeon.Tests.Match;

public class CombatResolverTests
{
    private readonly CombatResolver _resolver = new();
    private readonly Guid _player1Id = Guid.NewGuid();
    private readonly Guid _player2Id = Guid.NewGuid();

    private static AllyCard MakeAlly(
        int attack = 3, int hp = 5, int initiative = 2,
        bool isAmbusher = false, int cost = 1, string? name = null)
        => new(Guid.NewGuid(), name ?? $"Ally-{Guid.NewGuid():N}"[..12],
            Rarity.Common, cost, attack, hp, initiative, isAmbusher);

    private static MonsterCard MakeMonster(int attack = 3, int hp = 5, int initiative = 2)
        => new(Guid.NewGuid(), $"Monster-{Guid.NewGuid():N}"[..12],
            Rarity.Common, 1, attack, hp, initiative);

    private static BossCard MakeBoss(int attack = 10, int hp = 20, int initiative = 5)
        => new(Guid.NewGuid(), "DungeonBoss", Rarity.Unique, 1, attack, hp, initiative);

    #region Initiative

    [Fact]
    public void ResolveInitiative_HigherTotal_Wins()
    {
        var p1Allies = new[] { MakeAlly(initiative: 5), MakeAlly(initiative: 3) };
        var p2Allies = new[] { MakeAlly(initiative: 2), MakeAlly(initiative: 4) };

        var result = _resolver.ResolveInitiative(p1Allies, _player1Id, p2Allies, _player2Id);

        Assert.False(result.IsTied);
        Assert.Equal(_player1Id, result.WinnerId);
        Assert.Equal(8, result.Player1Total);
        Assert.Equal(6, result.Player2Total);
    }

    [Fact]
    public void ResolveInitiative_EqualTotals_ReturnsTied()
    {
        var p1Allies = new[] { MakeAlly(initiative: 4) };
        var p2Allies = new[] { MakeAlly(initiative: 4) };

        var result = _resolver.ResolveInitiative(p1Allies, _player1Id, p2Allies, _player2Id);

        Assert.True(result.IsTied);
        Assert.Null(result.WinnerId);
    }

    [Fact]
    public void ResolveInitiative_Player2Wins_WhenHigher()
    {
        var p1Allies = new[] { MakeAlly(initiative: 1) };
        var p2Allies = new[] { MakeAlly(initiative: 7) };

        var result = _resolver.ResolveInitiative(p1Allies, _player1Id, p2Allies, _player2Id);

        Assert.Equal(_player2Id, result.WinnerId);
    }

    [Fact]
    public void ResolveInitiative_MultipleAllies_SumsInitiative()
    {
        var p1Allies = new[] { MakeAlly(initiative: 1), MakeAlly(initiative: 2), MakeAlly(initiative: 3) };
        var p2Allies = new[] { MakeAlly(initiative: 5) };

        var result = _resolver.ResolveInitiative(p1Allies, _player1Id, p2Allies, _player2Id);

        Assert.Equal(6, result.Player1Total);
        Assert.Equal(5, result.Player2Total);
        Assert.Equal(_player1Id, result.WinnerId);
    }

    [Fact]
    public void ResolveInitiative_EmptyAllies_ZeroInitiative()
    {
        AllyCard[] empty = [];
        var p2Allies = new[] { MakeAlly(initiative: 1) };

        var result = _resolver.ResolveInitiative(empty, _player1Id, p2Allies, _player2Id);

        Assert.Equal(0, result.Player1Total);
        Assert.Equal(_player2Id, result.WinnerId);
    }

    [Fact]
    public void ResolveInitiative_BothEmpty_Tied()
    {
        AllyCard[] empty1 = [];
        AllyCard[] empty2 = [];

        var result = _resolver.ResolveInitiative(empty1, _player1Id, empty2, _player2Id);

        Assert.True(result.IsTied);
        Assert.Equal(0, result.Player1Total);
        Assert.Equal(0, result.Player2Total);
    }

    #endregion

    #region Combat — Allies vs Allies (1 HP per ATK comparison win)

    [Fact]
    public void ResolveCombat_AttackerStronger_DefenderEliminatedAfterRounds()
    {
        // ATK 7 vs ATK 5 → attacker wins each round, defender loses 1 HP per round
        // Defender has 6 HP → eliminated in 6 rounds
        var attackers = new[] { MakeAlly(attack: 7, hp: 20) };
        var defenders = new[] { MakeAlly(attack: 5, hp: 6) };

        var result = _resolver.ResolveCombat(attackers, defenders, isBossRoom: false);

        Assert.Equal(CombatOutcome.DefenderEliminated, result.Outcome);
        Assert.Equal(6, result.DamageToDefender);
        Assert.Equal(0, result.DamageToAttacker); // attacker never took damage
        Assert.Equal(6, result.Rounds);
    }

    [Fact]
    public void ResolveCombat_EqualAttack_BothEliminatedSimultaneously()
    {
        // ATK 5 vs ATK 5 → tie each round, both lose 1 HP
        // Both have 3 HP → eliminated in 3 rounds
        var attackers = new[] { MakeAlly(attack: 5, hp: 3) };
        var defenders = new[] { MakeAlly(attack: 5, hp: 3) };

        var result = _resolver.ResolveCombat(attackers, defenders, isBossRoom: false);

        Assert.Equal(CombatOutcome.SimultaneousElimination, result.Outcome);
        Assert.Equal(3, result.DamageToAttacker);
        Assert.Equal(3, result.DamageToDefender);
        Assert.Equal(3, result.Rounds);
    }

    [Fact]
    public void ResolveCombat_EqualAttack_UnequalHp_LowerHpDiesFirst()
    {
        // ATK 5 vs ATK 5, both lose 1 HP per round
        // Attacker HP 3, Defender HP 5 → attacker eliminated in 3 rounds
        var attackers = new[] { MakeAlly(attack: 5, hp: 3) };
        var defenders = new[] { MakeAlly(attack: 5, hp: 5) };

        var result = _resolver.ResolveCombat(attackers, defenders, isBossRoom: false);

        Assert.Equal(CombatOutcome.AttackerEliminated, result.Outcome);
        Assert.Equal(3, result.DamageToAttacker);
        Assert.Equal(3, result.DamageToDefender); // defender took 3 HP too but survived
        Assert.Equal(3, result.Rounds);
    }

    [Fact]
    public void ResolveCombat_DoubleAttack_InstantElimination()
    {
        // ATK 10 vs ATK 5 → 10 >= 5*2 → instant elimination
        var attackers = new[] { MakeAlly(attack: 10, hp: 10) };
        var defenders = new[] { MakeAlly(attack: 5, hp: 10) };

        var result = _resolver.ResolveCombat(attackers, defenders, isBossRoom: false);

        Assert.Equal(CombatOutcome.DefenderEliminated, result.Outcome);
        Assert.Equal(10, result.DamageToDefender); // full HP as damage (instant kill)
        Assert.Equal(0, result.DamageToAttacker);
    }

    [Fact]
    public void ResolveCombat_DoubleAttack_EliminatesAttacker()
    {
        var attackers = new[] { MakeAlly(attack: 3, hp: 10) };
        var defenders = new[] { MakeAlly(attack: 6, hp: 10) };

        var result = _resolver.ResolveCombat(attackers, defenders, isBossRoom: false);

        Assert.Equal(CombatOutcome.AttackerEliminated, result.Outcome);
    }

    [Fact]
    public void ResolveCombat_SimultaneousElimination_NormalRoom_DefenderWins()
    {
        // Equal ATK, equal HP → simultaneous elimination
        var attackers = new[] { MakeAlly(attack: 6, hp: 3) };
        var defenders = new[] { MakeAlly(attack: 6, hp: 3) };

        var result = _resolver.ResolveCombat(attackers, defenders, isBossRoom: false);

        Assert.Equal(CombatOutcome.SimultaneousElimination, result.Outcome);
        Assert.False(result.IsBossRoom);
        Assert.Equal("Defender", result.SimultaneousEliminationVictor);
    }

    [Fact]
    public void ResolveCombat_SimultaneousElimination_BossRoom_AttackerWins()
    {
        var attackers = new[] { MakeAlly(attack: 6, hp: 3) };
        var defenders = new[] { MakeAlly(attack: 6, hp: 3) };

        var result = _resolver.ResolveCombat(attackers, defenders, isBossRoom: true);

        Assert.Equal(CombatOutcome.SimultaneousElimination, result.Outcome);
        Assert.True(result.IsBossRoom);
        Assert.Equal("Attacker", result.SimultaneousEliminationVictor);
    }

    [Fact]
    public void ResolveCombat_MultipleAllies_SumsAttack()
    {
        // Two allies (ATK 3+4=7) vs one defender (ATK 4, HP 6)
        // 7 < 4*2=8 → no instant elimination, war of attrition
        // 7 > 4 → defender takes 1 HP per round → 6 rounds to eliminate
        var attackers = new[] { MakeAlly(attack: 3, hp: 5), MakeAlly(attack: 4, hp: 5) };
        var defenders = new[] { MakeAlly(attack: 4, hp: 6) };

        var result = _resolver.ResolveCombat(attackers, defenders, isBossRoom: false);

        Assert.Equal(7, result.AttackerAttack);
        Assert.Equal(CombatOutcome.DefenderEliminated, result.Outcome);
        Assert.Equal(6, result.DamageToDefender);
        Assert.Equal(6, result.Rounds);
    }

    [Fact]
    public void ResolveCombat_DefenderStronger_AttackerEliminatedAfterRounds()
    {
        // ATK 4 vs ATK 7, attacker HP 5 → attacker eliminated in 5 rounds
        var attackers = new[] { MakeAlly(attack: 4, hp: 5) };
        var defenders = new[] { MakeAlly(attack: 7, hp: 20) };

        var result = _resolver.ResolveCombat(attackers, defenders, isBossRoom: false);

        Assert.Equal(CombatOutcome.AttackerEliminated, result.Outcome);
        Assert.Equal(5, result.DamageToAttacker);
        Assert.Equal(0, result.DamageToDefender);
    }

    [Fact]
    public void ResolveCombat_ExactDoubleAttack_InstantElimination()
    {
        // 8 >= 4*2 → instant elimination
        var attackers = new[] { MakeAlly(attack: 8, hp: 20) };
        var defenders = new[] { MakeAlly(attack: 4, hp: 20) };

        var result = _resolver.ResolveCombat(attackers, defenders, isBossRoom: false);

        Assert.Equal(CombatOutcome.DefenderEliminated, result.Outcome);
        Assert.Equal(20, result.DamageToDefender); // instant kill = full HP
    }

    [Fact]
    public void ResolveCombat_JustUnderDoubleAttack_NoInstantElimination()
    {
        // 7 < 4*2=8 → no instant elimination, war of attrition
        // ATK 7 > ATK 4 → defender takes 1 per round, defender HP 4 → 4 rounds
        var attackers = new[] { MakeAlly(attack: 7, hp: 20) };
        var defenders = new[] { MakeAlly(attack: 4, hp: 4) };

        var result = _resolver.ResolveCombat(attackers, defenders, isBossRoom: false);

        Assert.Equal(CombatOutcome.DefenderEliminated, result.Outcome);
        Assert.Equal(4, result.DamageToDefender);
        Assert.Equal(0, result.DamageToAttacker);
        Assert.Equal(4, result.Rounds);
    }

    [Fact]
    public void ResolveCombat_OnlyWinnerDealsDamage()
    {
        // ATK 6 vs ATK 4, attacker wins each round
        // Defender HP 3 → eliminated in 3 rounds
        // Attacker takes 0 damage (only round winner deals damage)
        var attackers = new[] { MakeAlly(attack: 6, hp: 20) };
        var defenders = new[] { MakeAlly(attack: 4, hp: 3) };

        var result = _resolver.ResolveCombat(attackers, defenders, isBossRoom: false);

        Assert.Equal(0, result.DamageToAttacker);
        Assert.Equal(3, result.DamageToDefender);
    }

    #endregion

    #region Combat — Allies vs Monsters

    [Fact]
    public void ResolveCombat_VsMonsters_WarOfAttrition()
    {
        // ATK 7 vs ATK 5 → attacker wins rounds, monster HP 4 → 4 rounds
        var attackers = new[] { MakeAlly(attack: 7, hp: 20) };
        var defenders = new[] { MakeMonster(attack: 5, hp: 4) };

        var result = _resolver.ResolveCombat(attackers, defenders, isBossRoom: false);

        Assert.Equal(CombatOutcome.DefenderEliminated, result.Outcome);
        Assert.Equal(4, result.DamageToDefender);
        Assert.Equal(0, result.DamageToAttacker);
    }

    [Fact]
    public void ResolveCombat_VsMultipleMonsters_SumsAttack()
    {
        var attackers = new[] { MakeAlly(attack: 10, hp: 20) };
        var defenders = new[] { MakeMonster(attack: 3, hp: 5), MakeMonster(attack: 4, hp: 5) };

        var result = _resolver.ResolveCombat(attackers, defenders, isBossRoom: false);

        Assert.Equal(10, result.AttackerAttack);
        Assert.Equal(7, result.DefenderAttack);
    }

    #endregion

    #region Combat — Allies vs Boss

    [Fact]
    public void ResolveCombat_VsBoss_AlwaysBossRoom()
    {
        var attackers = new[] { MakeAlly(attack: 5, hp: 10), MakeAlly(attack: 5, hp: 10) };
        var boss = MakeBoss(attack: 10, hp: 20);

        var result = _resolver.ResolveCombat(attackers, boss);

        Assert.True(result.IsBossRoom);
        Assert.Equal(10, result.AttackerAttack);
        Assert.Equal(10, result.DefenderAttack);
    }

    [Fact]
    public void ResolveCombat_VsBoss_DoubleAttack_InstantElimination()
    {
        // 20 >= 10*2 → instant elimination of boss
        var attackers = new[] { MakeAlly(attack: 20, hp: 5) };
        var boss = MakeBoss(attack: 10, hp: 5);

        var result = _resolver.ResolveCombat(attackers, boss);

        Assert.Equal(CombatOutcome.DefenderEliminated, result.Outcome);
    }

    [Fact]
    public void ResolveCombat_VsBoss_AttackersOverwhelm_BossEliminated()
    {
        var attackers = new[]
        {
            MakeAlly(attack: 10, hp: 10),
            MakeAlly(attack: 10, hp: 10),
            MakeAlly(attack: 10, hp: 10)
        };
        var boss = MakeBoss(attack: 8, hp: 15);

        var result = _resolver.ResolveCombat(attackers, boss);

        // 30 >= 8*2=16 → instant elimination
        Assert.Equal(CombatOutcome.DefenderEliminated, result.Outcome);
    }

    #endregion

    #region Ambusher Targeting

    [Fact]
    public void GetValidTargets_NoAmbushers_ReturnsAll()
    {
        var allies = new[] { MakeAlly(isAmbusher: false), MakeAlly(isAmbusher: false) };

        var targets = _resolver.GetValidTargets(allies);

        Assert.Equal(2, targets.Count);
    }

    [Fact]
    public void GetValidTargets_MixedAmbushers_ExcludesAmbushers()
    {
        var normal = MakeAlly(isAmbusher: false, name: "Normal");
        var ambusher = MakeAlly(isAmbusher: true, name: "Ambusher");
        var allies = new[] { normal, ambusher };

        var targets = _resolver.GetValidTargets(allies);

        Assert.Single(targets);
        Assert.Equal("Normal", targets[0].Name);
    }

    [Fact]
    public void GetValidTargets_AllAmbushers_ReturnsAll()
    {
        var allies = new[] { MakeAlly(isAmbusher: true), MakeAlly(isAmbusher: true) };

        var targets = _resolver.GetValidTargets(allies);

        Assert.Equal(2, targets.Count);
    }

    [Fact]
    public void ValidateTarget_AmbusherWithNonAmbusherAvailable_Throws()
    {
        var normal = MakeAlly(isAmbusher: false);
        var ambusher = MakeAlly(isAmbusher: true);
        var allies = new[] { normal, ambusher };

        Assert.Throws<InvalidOperationException>(() =>
            _resolver.ValidateTarget(ambusher, allies));
    }

    [Fact]
    public void GetValidTargets_Empty_ReturnsEmpty()
    {
        var targets = _resolver.GetValidTargets([]);

        Assert.Empty(targets);
    }

    #endregion

    #region Opportunity Attack

    [Fact]
    public void ResolveOpportunityAttack_FirstUse_ReturnsDamage()
    {
        var attacker = MakeAlly(attack: 5);
        var fleeing = MakeAlly(isAmbusher: false);
        var alliesInPlay = new[] { fleeing };
        var used = new HashSet<Guid>();

        var damage = _resolver.ResolveOpportunityAttack(attacker, fleeing, alliesInPlay, used);

        Assert.Equal(5, damage);
        Assert.Contains(attacker.Id, used);
    }

    [Fact]
    public void ResolveOpportunityAttack_SecondUse_Throws()
    {
        var attacker = MakeAlly(attack: 5);
        var fleeing1 = MakeAlly(isAmbusher: false);
        var fleeing2 = MakeAlly(isAmbusher: false);
        var alliesInPlay = new[] { fleeing1, fleeing2 };
        var used = new HashSet<Guid>();

        _resolver.ResolveOpportunityAttack(attacker, fleeing1, alliesInPlay, used);

        Assert.Throws<InvalidOperationException>(() =>
            _resolver.ResolveOpportunityAttack(attacker, fleeing2, alliesInPlay, used));
    }

    [Fact]
    public void ResolveOpportunityAttack_AmbusherRule_Enforced()
    {
        var attacker = MakeAlly(attack: 5);
        var normal = MakeAlly(isAmbusher: false);
        var ambusher = MakeAlly(isAmbusher: true);
        var alliesInPlay = new[] { normal, ambusher };
        var used = new HashSet<Guid>();

        Assert.Throws<InvalidOperationException>(() =>
            _resolver.ResolveOpportunityAttack(attacker, ambusher, alliesInPlay, used));
    }

    [Fact]
    public void ResolveOpportunityAttack_DifferentAttackers_BothAllowed()
    {
        var attacker1 = MakeAlly(attack: 3);
        var attacker2 = MakeAlly(attack: 4);
        var fleeing = MakeAlly(isAmbusher: false);
        var alliesInPlay = new[] { fleeing };
        var used = new HashSet<Guid>();

        var dmg1 = _resolver.ResolveOpportunityAttack(attacker1, fleeing, alliesInPlay, used);
        var dmg2 = _resolver.ResolveOpportunityAttack(attacker2, fleeing, alliesInPlay, used);

        Assert.Equal(3, dmg1);
        Assert.Equal(4, dmg2);
    }

    #endregion

    #region Retarget

    [Fact]
    public void ResolveRetarget_DealsDamageOnlyInPrimary()
    {
        var ally = MakeAlly(attack: 6, hp: 8);
        var primaryGroup = new[] { ally, MakeAlly() };
        var secondaryGroup = new[] { MakeAlly(), MakeAlly() };

        var deck = Enumerable.Range(0, 10).Select(_ => (Card)MakeAlly()).ToList();
        var player = new PlayerState(Guid.NewGuid(), 20, deck);
        // Put ally in hand so we can use player for cost
        // Cost is paid from deck top, player just needs cards in deck

        var result = _resolver.ResolveRetarget(ally, primaryGroup, secondaryGroup, retargetCost: 2, player);

        Assert.Equal(6, result.PrimaryDamageContribution);
        Assert.Equal(0, result.SecondaryDamageContribution);
        Assert.Equal(2, result.CostPaid);
    }

    [Fact]
    public void ResolveRetarget_AllyNotInPrimaryGroup_Throws()
    {
        var ally = MakeAlly();
        var primaryGroup = new[] { MakeAlly() };
        var secondaryGroup = new[] { ally };
        var player = new PlayerState(Guid.NewGuid(), 20, []);

        Assert.Throws<InvalidOperationException>(() =>
            _resolver.ResolveRetarget(ally, primaryGroup, secondaryGroup, retargetCost: 1, player));
    }

    [Fact]
    public void ResolveRetarget_PaysCostFromDeck()
    {
        var ally = MakeAlly(attack: 4);
        var primaryGroup = new[] { ally };
        var secondaryGroup = new[] { MakeAlly() };

        var deck = Enumerable.Range(0, 5).Select(_ => (Card)MakeAlly()).ToList();
        var player = new PlayerState(Guid.NewGuid(), 20, deck);

        _resolver.ResolveRetarget(ally, primaryGroup, secondaryGroup, retargetCost: 3, player);

        Assert.Equal(2, player.Deck.Count); // 5 - 3 = 2
    }

    #endregion
}
