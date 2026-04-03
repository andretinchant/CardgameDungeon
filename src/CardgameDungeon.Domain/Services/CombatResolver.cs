using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Domain.ValueObjects;

namespace CardgameDungeon.Domain.Services;

public class CombatResolver
{
    private const int EliminationMultiplier = 2;

    #region Initiative

    public InitiativeResult ResolveInitiative(
        IReadOnlyList<AllyCard> player1Allies,
        Guid player1Id,
        IReadOnlyList<AllyCard> player2Allies,
        Guid player2Id)
    {
        var p1Total = player1Allies.Sum(a => a.Initiative);
        var p2Total = player2Allies.Sum(a => a.Initiative);

        if (p1Total == p2Total)
            return InitiativeResult.Tied(p1Total, p2Total);

        var winnerId = p1Total > p2Total ? player1Id : player2Id;
        return InitiativeResult.Won(p1Total, p2Total, winnerId);
    }

    #endregion

    #region Combat

    public BattleResolutionResult ResolveCombat(
        IReadOnlyList<AllyCard> attackers,
        IReadOnlyList<AllyCard> defenders,
        bool isBossRoom)
    {
        var advantage = CombatAdvantage.Calculate(attackers.Count, defenders.Count);
        var atkStrength = attackers.Sum(a => a.Strength);
        var defStrength = defenders.Sum(a => a.Strength);
        var atkHp = attackers.Sum(a => a.HitPoints);
        var defHp = defenders.Sum(a => a.HitPoints);

        return ResolveCore(atkStrength, defStrength, atkHp, defHp, isBossRoom, advantage);
    }

    public BattleResolutionResult ResolveCombat(
        IReadOnlyList<AllyCard> attackers,
        IReadOnlyList<MonsterCard> defenders,
        bool isBossRoom)
    {
        var advantage = CombatAdvantage.Calculate(attackers.Count, defenders.Count);
        var atkStrength = attackers.Sum(a => a.Strength);
        var defStrength = defenders.Sum(m => m.Strength);
        var atkHp = attackers.Sum(a => a.HitPoints);
        var defHp = defenders.Sum(m => m.HitPoints);

        return ResolveCore(atkStrength, defStrength, atkHp, defHp, isBossRoom, advantage);
    }

    public BattleResolutionResult ResolveCombat(
        IReadOnlyList<AllyCard> attackers,
        BossCard boss)
    {
        var advantage = CombatAdvantage.Calculate(attackers.Count, 1);
        var atkStrength = attackers.Sum(a => a.Strength);
        var atkHp = attackers.Sum(a => a.HitPoints);

        return ResolveCore(atkStrength, boss.Strength, atkHp, boss.HitPoints, isBossRoom: true, advantage);
    }

    private static BattleResolutionResult ResolveCore(
        int attackerStrength,
        int defenderStrength,
        int attackerHp,
        int defenderHp,
        bool isBossRoom,
        CombatAdvantage? advantage = null)
    {
        // Each side deals damage equal to their strength
        var damageToAttacker = defenderStrength;
        var damageToDefender = attackerStrength;

        var attackerEliminated = IsEliminated(attackerStrength, defenderStrength, attackerHp);
        var defenderEliminated = IsEliminated(defenderStrength, attackerStrength, defenderHp);

        var outcome = (attackerEliminated, defenderEliminated) switch
        {
            (true, true) => CombatOutcome.SimultaneousElimination,
            (true, false) => CombatOutcome.AttackerEliminated,
            (false, true) => CombatOutcome.DefenderEliminated,
            _ when attackerStrength == defenderStrength => CombatOutcome.BothTakeDamage,
            _ when attackerStrength > defenderStrength => CombatOutcome.AttackerWins,
            _ => CombatOutcome.DefenderWins
        };

        return new BattleResolutionResult(
            attackerStrength,
            defenderStrength,
            damageToAttacker,
            damageToDefender,
            outcome,
            isBossRoom,
            advantage);
    }

    /// <summary>
    /// A side is eliminated if the opponent's strength is at least double theirs,
    /// or if the incoming damage exceeds their total HP.
    /// </summary>
    private static bool IsEliminated(int ownStrength, int opponentStrength, int ownHp)
    {
        if (opponentStrength >= ownStrength * EliminationMultiplier)
            return true;

        return opponentStrength >= ownHp;
    }

    #endregion

    #region Ambusher Targeting

    /// <summary>
    /// Filters allies that can be targeted, enforcing the Ambusher rule:
    /// ambushers cannot be targeted if any non-ambusher ally is available.
    /// </summary>
    public IReadOnlyList<AllyCard> GetValidTargets(IReadOnlyList<AllyCard> alliesInPlay)
    {
        if (alliesInPlay.Count == 0)
            return [];

        var nonAmbushers = alliesInPlay.Where(a => !a.IsAmbusher).ToList();
        return nonAmbushers.Count > 0 ? nonAmbushers : alliesInPlay.ToList();
    }

    public void ValidateTarget(AllyCard target, IReadOnlyList<AllyCard> alliesInPlay)
    {
        var validTargets = GetValidTargets(alliesInPlay);

        if (!validTargets.Contains(target))
            throw new InvalidOperationException(
                $"Cannot target '{target.Name}': ambusher cannot be targeted while non-ambusher allies are available.");
    }

    #endregion

    #region Opportunity Attack

    /// <summary>
    /// Resolves an opportunity attack (1 per player per round) triggered
    /// when an ally abandons combat. The attacker deals strength damage.
    /// </summary>
    public int ResolveOpportunityAttack(
        AllyCard attacker,
        AllyCard fleeingTarget,
        IReadOnlyList<AllyCard> alliesInPlay,
        HashSet<Guid> playersWhoUsedOpportunityAttack)
    {
        if (!playersWhoUsedOpportunityAttack.Add(attacker.Id))
            throw new InvalidOperationException(
                "This player has already used their opportunity attack this round.");

        ValidateTarget(fleeingTarget, alliesInPlay);

        return attacker.Strength;
    }

    #endregion

    #region Retarget

    /// <summary>
    /// Resolves a retarget action at the end of the initiative sequence.
    /// The ally participates in two combats but only deals damage in the primary one.
    /// Returns the damage contribution for primary and secondary combats.
    /// </summary>
    public RetargetResult ResolveRetarget(
        AllyCard ally,
        IReadOnlyList<AllyCard> primaryCombatGroup,
        IReadOnlyList<AllyCard> secondaryCombatGroup,
        int retargetCost,
        PlayerState player)
    {
        if (!primaryCombatGroup.Contains(ally))
            throw new InvalidOperationException(
                $"Ally '{ally.Name}' must be part of the primary combat group.");

        player.PayCostFromDeck(retargetCost);

        // Ally contributes strength only to primary combat
        var primaryStrength = ally.Strength;
        const int secondaryStrength = 0;

        return new RetargetResult(
            ally.Id,
            primaryStrength,
            secondaryStrength,
            retargetCost);
    }

    #endregion
}
