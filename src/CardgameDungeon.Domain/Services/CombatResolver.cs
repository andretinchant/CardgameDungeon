using CardgameDungeon.Domain.Effects;
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
        bool isBossRoom,
        PlayerState? attackerState = null,
        PlayerState? defenderState = null)
    {
        var advantage = CombatAdvantage.Calculate(attackers.Count, defenders.Count);

        var atkMods = CalculateGroupModifiers(attackers, advantage.AttackerHasAdvantage, advantage.AttackerHasDisadvantage);
        var defMods = CalculateGroupModifiers(defenders, advantage.DefenderHasAdvantage, advantage.DefenderHasDisadvantage);

        var (atkBaseAtk, atkBaseHp, _) = CalculateGroupStatsWithEquipment(attackers, attackerState);
        var (defBaseAtk, defBaseHp, _) = CalculateGroupStatsWithEquipment(defenders, defenderState);

        var atkAttack = atkBaseAtk + atkMods.Attack;
        var defAttack = defBaseAtk + defMods.Attack;
        var atkHp = atkBaseHp + atkMods.HitPoints;
        var defHp = defBaseHp + defMods.HitPoints;

        return ResolveCore(atkAttack, defAttack, atkHp, defHp, isBossRoom, advantage,
            atkMods.EliminationDoubled, defMods.EliminationDoubled,
            atkMods.DamageReduction, defMods.DamageReduction);
    }

    public BattleResolutionResult ResolveCombat(
        IReadOnlyList<AllyCard> attackers,
        IReadOnlyList<MonsterCard> defenders,
        bool isBossRoom,
        PlayerState? attackerState = null)
    {
        var advantage = CombatAdvantage.Calculate(attackers.Count, defenders.Count);

        var atkMods = CalculateGroupModifiers(attackers, advantage.AttackerHasAdvantage, advantage.AttackerHasDisadvantage);
        var defMods = CalculateGroupModifiers(defenders, advantage.DefenderHasAdvantage, advantage.DefenderHasDisadvantage);

        var (atkBaseAtk, atkBaseHp, _) = CalculateGroupStatsWithEquipment(attackers, attackerState);

        var atkAttack = atkBaseAtk + atkMods.Attack;
        var defAttack = defenders.Sum(m => m.Attack) + defMods.Attack;
        var atkHp = atkBaseHp + atkMods.HitPoints;
        var defHp = defenders.Sum(m => m.HitPoints) + defMods.HitPoints;

        return ResolveCore(atkAttack, defAttack, atkHp, defHp, isBossRoom, advantage,
            atkMods.EliminationDoubled, defMods.EliminationDoubled,
            atkMods.DamageReduction, defMods.DamageReduction);
    }

    public BattleResolutionResult ResolveCombat(
        IReadOnlyList<AllyCard> attackers,
        BossCard boss,
        PlayerState? attackerState = null)
    {
        var advantage = CombatAdvantage.Calculate(attackers.Count, 1);

        var atkMods = CalculateGroupModifiers(attackers, advantage.AttackerHasAdvantage, advantage.AttackerHasDisadvantage);
        var bossMods = CalculateCardModifiers(boss, advantage.DefenderHasAdvantage, advantage.DefenderHasDisadvantage);

        var (atkBaseAtk, atkBaseHp, _) = CalculateGroupStatsWithEquipment(attackers, attackerState);

        var atkAttack = atkBaseAtk + atkMods.Attack;
        var defAttack = boss.Attack + bossMods.Attack;
        var atkHp = atkBaseHp + atkMods.HitPoints;
        var defHp = boss.HitPoints + bossMods.HitPoints;

        return ResolveCore(atkAttack, defAttack, atkHp, defHp, isBossRoom: true, advantage,
            atkMods.EliminationDoubled, bossMods.EliminationDoubled,
            atkMods.DamageReduction, bossMods.DamageReduction);
    }

    /// <summary>
    /// Resolves a single round of combat, applying damage to the ActiveCombatState.
    /// Returns the round result including whether either side was eliminated.
    /// Used for round-by-round combat where the attacker can pause between rounds.
    /// </summary>
    public CombatRoundResult ResolveSingleRound(
        IReadOnlyList<AllyCard> attackers,
        IReadOnlyList<MonsterCard> defenders,
        ActiveCombatState combatState,
        PlayerState? attackerState = null)
    {
        var advantage = CombatAdvantage.Calculate(attackers.Count, defenders.Count);
        var atkMods = CalculateGroupModifiers(attackers, advantage.AttackerHasAdvantage, advantage.AttackerHasDisadvantage);
        var defMods = CalculateGroupModifiers(defenders, advantage.DefenderHasAdvantage, advantage.DefenderHasDisadvantage);

        var (atkBaseAtk, atkBaseHp, _) = CalculateGroupStatsWithEquipment(attackers, attackerState);
        var atkAttack = atkBaseAtk + atkMods.Attack;
        var defAttack = defenders.Sum(m => m.Attack) + defMods.Attack;

        // Calculate current HP (base - accumulated damage from previous rounds)
        var atkCurrentHp = atkBaseHp + atkMods.HitPoints;
        foreach (var ally in attackers)
            atkCurrentHp -= combatState.GetDamageTaken(ally.Id);

        var defCurrentHp = defenders.Sum(m => m.HitPoints) + defMods.HitPoints;
        foreach (var monster in defenders)
            defCurrentHp -= combatState.GetDamageTaken(monster.Id);

        atkCurrentHp = Math.Max(0, atkCurrentHp);
        defCurrentHp = Math.Max(0, defCurrentHp);

        // Check instant elimination (2x rule)
        var atkEffAtk = atkMods.EliminationDoubled ? atkAttack * 2 : atkAttack;
        var defEffAtk = defMods.EliminationDoubled ? defAttack * 2 : defAttack;
        var atkInstantElim = defEffAtk >= atkAttack * EliminationMultiplier && atkAttack > 0;
        var defInstantElim = atkEffAtk >= defAttack * EliminationMultiplier && defAttack > 0;

        if (atkInstantElim || defInstantElim)
        {
            // Apply all remaining HP as damage
            if (atkInstantElim)
                foreach (var ally in attackers)
                    combatState.ApplyDamage(ally.Id, ally.HitPoints);
            if (defInstantElim)
                foreach (var monster in defenders)
                    combatState.ApplyDamage(monster.Id, monster.HitPoints);

            combatState.CompleteRound();
            return new CombatRoundResult(
                atkAttack, defAttack, atkInstantElim, defInstantElim,
                true, combatState.RoundsCompleted, advantage);
        }

        // Resolve 1 round of attrition
        var dmgReductionAtk = atkMods.DamageReduction;
        var dmgReductionDef = defMods.DamageReduction;
        int damageToAttacker = 0, damageToDefender = 0;

        if (atkAttack > defAttack)
        {
            damageToDefender = Math.Max(0, 1 - dmgReductionDef);
        }
        else if (defAttack > atkAttack)
        {
            damageToAttacker = Math.Max(0, 1 - dmgReductionAtk);
        }
        else
        {
            damageToAttacker = Math.Max(0, 1 - dmgReductionAtk);
            damageToDefender = Math.Max(0, 1 - dmgReductionDef);
        }

        // Distribute damage across cards (weakest first)
        if (damageToAttacker > 0)
        {
            var weakest = attackers
                .Where(a => combatState.GetRemainingHp(a.Id, a.HitPoints) > 0)
                .OrderBy(a => combatState.GetRemainingHp(a.Id, a.HitPoints))
                .FirstOrDefault();
            if (weakest != null)
                combatState.ApplyDamage(weakest.Id, damageToAttacker);
        }

        if (damageToDefender > 0)
        {
            var weakest = defenders
                .Where(m => combatState.GetRemainingHp(m.Id, m.HitPoints) > 0)
                .OrderBy(m => combatState.GetRemainingHp(m.Id, m.HitPoints))
                .FirstOrDefault();
            if (weakest != null)
                combatState.ApplyDamage(weakest.Id, damageToDefender);
        }

        combatState.CompleteRound();

        // Check if either side is fully eliminated
        var atkEliminated = attackers.All(a => combatState.GetRemainingHp(a.Id, a.HitPoints) <= 0);
        var defEliminated = defenders.All(m => combatState.GetRemainingHp(m.Id, m.HitPoints) <= 0);

        return new CombatRoundResult(
            atkAttack, defAttack, atkEliminated, defEliminated,
            atkEliminated || defEliminated,
            combatState.RoundsCompleted, advantage);
    }

    private static BattleResolutionResult ResolveCore(
        int attackerAttack,
        int defenderAttack,
        int attackerHp,
        int defenderHp,
        bool isBossRoom,
        CombatAdvantage? advantage = null,
        bool attackerElimDoubled = false,
        bool defenderElimDoubled = false,
        int attackerDamageReduction = 0,
        int defenderDamageReduction = 0)
    {
        // Instant elimination: if one side's ATK is double the other's (with elimination doubled modifier)
        var atkEffectiveAtk = attackerElimDoubled ? attackerAttack * 2 : attackerAttack;
        var defEffectiveAtk = defenderElimDoubled ? defenderAttack * 2 : defenderAttack;

        var attackerInstantElim = defEffectiveAtk >= attackerAttack * EliminationMultiplier && attackerAttack > 0;
        var defenderInstantElim = atkEffectiveAtk >= defenderAttack * EliminationMultiplier && defenderAttack > 0;

        if (attackerInstantElim || defenderInstantElim)
        {
            var outcome = (attackerInstantElim, defenderInstantElim) switch
            {
                (true, true) => CombatOutcome.SimultaneousElimination,
                (true, false) => CombatOutcome.AttackerEliminated,
                _ => CombatOutcome.DefenderEliminated
            };

            return new BattleResolutionResult(
                attackerAttack, defenderAttack,
                attackerInstantElim ? attackerHp : 0,
                defenderInstantElim ? defenderHp : 0,
                outcome, isBossRoom, advantage);
        }

        // War of attrition: each round, compare ATK. Winner deals 1 HP damage (reduced by damage reduction).
        // Tie = both take 1 HP. Repeat until one side's HP reaches 0.
        var atkHpRemaining = attackerHp;
        var defHpRemaining = defenderHp;
        var rounds = 0;
        const int maxRounds = 500; // safety limit

        while (atkHpRemaining > 0 && defHpRemaining > 0 && rounds < maxRounds)
        {
            rounds++;

            if (attackerAttack > defenderAttack)
            {
                // Attacker wins this round — defender takes 1 HP (reduced by damage reduction)
                var dmg = Math.Max(0, 1 - defenderDamageReduction);
                defHpRemaining -= Math.Max(dmg, 0);
                // If damage reduction negates all damage, combat is a stalemate
                if (dmg <= 0) break;
            }
            else if (defenderAttack > attackerAttack)
            {
                // Defender wins this round — attacker takes 1 HP
                var dmg = Math.Max(0, 1 - attackerDamageReduction);
                atkHpRemaining -= Math.Max(dmg, 0);
                if (dmg <= 0) break;
            }
            else
            {
                // Tie — both take 1 HP
                var dmgToAtk = Math.Max(0, 1 - attackerDamageReduction);
                var dmgToDef = Math.Max(0, 1 - defenderDamageReduction);
                atkHpRemaining -= dmgToAtk;
                defHpRemaining -= dmgToDef;
                // If both are fully reduced, stalemate
                if (dmgToAtk <= 0 && dmgToDef <= 0) break;
            }
        }

        var damageToAttacker = attackerHp - Math.Max(0, atkHpRemaining);
        var damageToDefender = defenderHp - Math.Max(0, defHpRemaining);

        var attackerEliminated = atkHpRemaining <= 0;
        var defenderEliminated = defHpRemaining <= 0;

        var combatOutcome = (attackerEliminated, defenderEliminated) switch
        {
            (true, true) => CombatOutcome.SimultaneousElimination,
            (true, false) => CombatOutcome.AttackerEliminated,
            (false, true) => CombatOutcome.DefenderEliminated,
            _ when damageToAttacker > 0 || damageToDefender > 0 => CombatOutcome.BothTakeDamage,
            _ when attackerAttack > defenderAttack => CombatOutcome.AttackerWins,
            _ when defenderAttack > attackerAttack => CombatOutcome.DefenderWins,
            _ => CombatOutcome.BothTakeDamage
        };

        return new BattleResolutionResult(
            attackerAttack,
            defenderAttack,
            damageToAttacker,
            damageToDefender,
            combatOutcome,
            isBossRoom,
            advantage,
            rounds);
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
    /// when an ally abandons combat. The attacker deals attack damage.
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

        return attacker.Attack;
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

        // Ally contributes attack only to primary combat
        var primaryAttack = ally.Attack;
        const int secondaryAttack = 0;

        return new RetargetResult(
            ally.Id,
            primaryAttack,
            secondaryAttack,
            retargetCost);
    }

    #endregion

    #region Equipment Stats

    private static (int str, int hp, int init) CalculateGroupStatsWithEquipment(
        IReadOnlyList<AllyCard> allies, PlayerState? ownerState)
    {
        var str = allies.Sum(a => a.Attack);
        var hp = allies.Sum(a => a.HitPoints);
        var init = allies.Sum(a => a.Initiative);

        if (ownerState != null)
        {
            foreach (var ally in allies)
            {
                foreach (var equip in ownerState.GetEquippedForAlly(ally.Id))
                {
                    if (!equip.Slot.IsCompanionOrSummon() && !equip.Slot.IsShapeshift())
                    {
                        str += equip.AttackModifier;
                        hp += equip.HitPointsModifier;
                        init += equip.InitiativeModifier;
                    }
                }
            }
        }

        return (str, hp, init);
    }

    #endregion

    #region Effect Processing

    /// <summary>
    /// Calculates cumulative stat modifiers from all cards' ParsedEffects in a combat group.
    /// Evaluates PASSIVE, ON_COMBAT_START, WITH_ADVANTAGE, WITH_DISADVANTAGE triggers.
    /// </summary>
    private static StatModifiers CalculateGroupModifiers<T>(
        IReadOnlyList<T> cards, bool hasAdvantage, bool hasDisadvantage) where T : Card
    {
        var combined = new StatModifiers();

        foreach (var card in cards)
        {
            var mods = CalculateCardModifiers(card, hasAdvantage, hasDisadvantage);
            combined.Attack += mods.Attack;
            combined.HitPoints += mods.HitPoints;
            combined.Initiative += mods.Initiative;
            combined.DamageReduction += mods.DamageReduction;
            combined.EliminationDoubled = combined.EliminationDoubled || mods.EliminationDoubled;
            combined.OpportunityAttackDoubled = combined.OpportunityAttackDoubled || mods.OpportunityAttackDoubled;
            combined.IgnoreOpportunityAttackLimit = combined.IgnoreOpportunityAttackLimit || mods.IgnoreOpportunityAttackLimit;
            combined.ForfeitsTreasure = combined.ForfeitsTreasure || mods.ForfeitsTreasure;
        }

        return combined;
    }

    private static StatModifiers CalculateCardModifiers(Card card, bool hasAdvantage, bool hasDisadvantage,
        PlayerState? ownerState = null)
    {
        var tags = card.ParsedEffects;
        if (tags.Count == 0) return new StatModifiers();

        var context = new EffectContext
        {
            SourceCardId = card.Id,
            HasAdvantage = hasAdvantage,
            HasDisadvantage = hasDisadvantage,
            DeckCount = ownerState?.Deck.Count ?? 0,
            HandCount = ownerState?.Hand.Count ?? 0,
            AlliesInPlay = ownerState?.AlliesInPlay.Count ?? 0
        };

        // Feed classes in play
        if (ownerState != null)
        {
            foreach (var ally in ownerState.AlliesInPlay)
                context.AddClassInPlay(ally.Class.ToString());

            // Feed equipped slots for this card if it's an AllyCard
            if (card is AllyCard allyCard)
            {
                foreach (var equip in ownerState.GetEquippedForAlly(allyCard.Id))
                    context.AddEquippedSlot(equip.Slot.ToString());
            }
        }

        var mods = new StatModifiers();

        // Evaluate combat-relevant triggers
        var triggers = new[] { EffectTrigger.Passive, EffectTrigger.OnCombatStart };
        if (hasAdvantage) triggers = [..triggers, EffectTrigger.WithAdvantage];
        if (hasDisadvantage) triggers = [..triggers, EffectTrigger.WithDisadvantage];

        foreach (var trigger in triggers)
        {
            var triggerMods = EffectEngine.CalculateModifiers(tags, trigger, context);
            mods.Attack += triggerMods.Attack;
            mods.HitPoints += triggerMods.HitPoints;
            mods.Initiative += triggerMods.Initiative;
            mods.DamageReduction += triggerMods.DamageReduction;
            mods.EliminationDoubled = mods.EliminationDoubled || triggerMods.EliminationDoubled;
            mods.OpportunityAttackDoubled = mods.OpportunityAttackDoubled || triggerMods.OpportunityAttackDoubled;
            mods.IgnoreOpportunityAttackLimit = mods.IgnoreOpportunityAttackLimit || triggerMods.IgnoreOpportunityAttackLimit;
            mods.ForfeitsTreasure = mods.ForfeitsTreasure || triggerMods.ForfeitsTreasure;
        }

        return mods;
    }

    #endregion
}
