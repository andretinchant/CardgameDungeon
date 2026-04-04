using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.Domain.Services;

/// <summary>
/// Controls power creep via a point-budget system for all card types.
///
/// Stat costs (universal):
///   ATK  = first point FREE, then 3 pts per additional (threshold-based)
///   HP   = first point FREE, then 3 pts per additional (survival is premium)
///   INIT = 1.5 pt per point (determines role selection)
///   Caps: Allies ATK≤10 HP≤5, Monsters ATK≤15 HP≤5, Bosses no cap
///   Treasure = -1.0 pt (CREDIT — downside for owner, opponent recovers cards)
///   Ambusher = 2.0 pt
///
/// Rarity multiplier (applied to base budget):
///   Common ×1.0, Uncommon ×1.1, Rare ×1.25, Unique ×1.5
///
/// Separate budget curves per card type:
///   Allies:      modest — they fight in groups of 3-5
///   Monsters:    30% more generous — they fight outnumbered
///   Bosses:      very generous — they fight alone vs full party
///   Equipment:   ~45% of ally budget (mods stack on ally stats)
///   Consumables: equipment + 1 per cost (single-use bonus)
///   Traps:       damage is always 1 base + effect budget
/// </summary>
public static class CardBudget
{
    #region Stat Costs

    public const float AttackAdditionalCost = 3.0f;    // First ATK free, each beyond 1 costs 3
    public const float HitPointsAdditionalCost = 3.0f; // First HP free, each beyond 1 costs 3
    public const float InitiativeCost = 1.5f;
    public const float TreasureCredit = -1.0f; // Credit: treasure is a downside
    public const float AmbusherCost = 2.0f;

    /// <summary>ATK cost: first point free, each additional costs 3.</summary>
    public static float CalculateAtkCost(int attack)
        => attack <= 1 ? 0 : (attack - 1) * AttackAdditionalCost;

    /// <summary>HP cost: first point free, each additional costs 3.</summary>
    public static float CalculateHpCost(int hitPoints)
        => hitPoints <= 1 ? 0 : (hitPoints - 1) * HitPointsAdditionalCost;

    #endregion

    #region Budget Curves

    // Ally budget — baseline designed around Common stat lines:
    //   C1: ATK4 HP2 INIT1 (13.5 stat, ~1.5 effect)
    //   C2: ATK7 HP3 INIT2 (27 stat, ~2 effect)
    //   C3: ATK8 HP3 INIT3 (31.5 stat, ~2.5 effect)
    //   C4: ATK9 HP4 INIT4 (39 stat, ~3 effect)
    private static readonly int[] AllyBudget =
    //  cost:  0   1   2   3   4   5   6   7   8   9  10
             [ 0, 15, 29, 34, 42, 50, 58, 66, 74, 82, 90 ];

    // Monster budget — 30% more generous. Monsters fight outnumbered (1-3 vs 3-5 allies).
    private static readonly int[] MonsterBudget =
    //  cost:  0   1   2   3   4   5   6   7   8   9  10
             [ 0, 20, 38, 44, 55, 65, 75, 86, 96,107,117 ];

    // Boss budget — very generous. Bosses fight alone against a full party.
    private static readonly int[] BossBudget =
    //  cost:  0   1   2   3   4   5   6   7   8   9  10
             [ 0,  0,  0,  0,  0, 80, 95,110,125,140,155 ];

    // Equipment budget — ~45% of ally budget. Mods stack on ally stats.
    private static readonly int[] EquipmentBudget =
    //  cost:  0   1   2   3   4   5   6   7   8   9  10
             [ 0,  7, 13, 15, 19, 23, 26, 30, 33, 37, 41 ];

    // Consumable budget — equipment + bonus per cost level (single-use).
    private static readonly int[] ConsumableBudget =
    //  cost:  0   1   2   3   4   5   6   7   8   9  10
             [ 0,  9, 15, 18, 22, 26, 30, 34, 38, 42, 46 ];

    // Trap budget — damage is always 1 base HP + effect budget.
    private static readonly int[] TrapBudget =
    //  cost:  0   1   2   3   4   5   6   7   8   9  10
             [ 0,  9, 15, 18, 22, 26, 30, 34, 38, 42, 46 ];

    #endregion

    #region Rarity Multipliers

    public static float GetRarityMultiplier(Rarity rarity) => rarity switch
    {
        Rarity.Common => 1.0f,
        Rarity.Uncommon => 1.1f,
        Rarity.Rare => 1.25f,
        Rarity.Unique => 1.5f,
        _ => 1.0f
    };

    #endregion

    #region Budget Lookup

    public static int GetAllyBudget(int cost, Rarity rarity = Rarity.Common)
    {
        var c = Math.Clamp(cost, 0, 10);
        return (int)Math.Round(AllyBudget[c] * GetRarityMultiplier(rarity));
    }

    public static int GetMonsterBudget(int cost, Rarity rarity = Rarity.Common)
    {
        var c = Math.Clamp(cost, 0, 10);
        return (int)Math.Round(MonsterBudget[c] * GetRarityMultiplier(rarity));
    }

    public static int GetBossBudget(int cost, Rarity rarity = Rarity.Unique)
    {
        var c = Math.Clamp(cost, 0, 10);
        return (int)Math.Round(BossBudget[c] * GetRarityMultiplier(rarity));
    }

    public static int GetEquipmentBudget(int cost, Rarity rarity = Rarity.Common)
    {
        var c = Math.Clamp(cost, 0, 10);
        return (int)Math.Round(EquipmentBudget[c] * GetRarityMultiplier(rarity));
    }

    public static int GetConsumableBudget(int cost, Rarity rarity = Rarity.Common)
    {
        var c = Math.Clamp(cost, 0, 10);
        return (int)Math.Round(ConsumableBudget[c] * GetRarityMultiplier(rarity));
    }

    public static int GetTrapBudget(int cost, Rarity rarity = Rarity.Common)
    {
        var c = Math.Clamp(cost, 0, 10);
        return (int)Math.Round(TrapBudget[c] * GetRarityMultiplier(rarity));
    }

    /// <summary>Legacy overload for backwards compatibility.</summary>
    public static int GetBudget(int cost, bool isUniqueOrBoss = false)
    {
        var rarity = isUniqueOrBoss ? Rarity.Unique : Rarity.Common;
        return GetAllyBudget(cost, rarity);
    }

    #endregion

    #region Stat Spend Calculation

    /// <summary>Calculates how many budget points a card's stats consume.</summary>
    public static float CalculateStatSpend(int attack, int hitPoints, int initiative,
        int treasure = 0, bool isAmbusher = false)
    {
        return CalculateAtkCost(attack)
             + CalculateHpCost(hitPoints)
             + initiative * InitiativeCost
             + treasure * TreasureCredit // Negative — treasure gives budget back
             + (isAmbusher ? AmbusherCost : 0);
    }

    /// <summary>Returns the remaining budget available for effects.</summary>
    public static float GetEffectBudget(int totalBudget, int attack, int hitPoints, int initiative,
        int treasure = 0, bool isAmbusher = false)
    {
        var statSpend = CalculateStatSpend(attack, hitPoints, initiative, treasure, isAmbusher);
        return totalBudget - statSpend;
    }

    #endregion

    #region Validation

    /// <summary>
    /// Validates that a card's total stat spend + effect budget fits within its budget.
    /// </summary>
    public static CardBudgetValidation ValidateAlly(
        int cost, Rarity rarity, int attack, int hitPoints, int initiative,
        int treasure = 0, bool isAmbusher = false)
    {
        var budget = GetAllyBudget(cost, rarity);
        return BuildValidation(budget, rarity, attack, hitPoints, initiative, treasure, isAmbusher);
    }

    public static CardBudgetValidation ValidateMonster(
        int cost, Rarity rarity, int attack, int hitPoints, int initiative,
        int treasure = 0)
    {
        var budget = GetMonsterBudget(cost, rarity);
        return BuildValidation(budget, rarity, attack, hitPoints, initiative, treasure);
    }

    public static CardBudgetValidation ValidateBoss(
        int cost, int attack, int hitPoints, int initiative)
    {
        var budget = GetBossBudget(cost);
        return BuildValidation(budget, Rarity.Unique, attack, hitPoints, initiative);
    }

    public static CardBudgetValidation ValidateEquipment(
        int cost, Rarity rarity, int attackMod, int hpMod, int initMod)
    {
        var budget = GetEquipmentBudget(cost, rarity);
        return BuildValidation(budget, rarity, attackMod, hpMod, initMod);
    }

    /// <summary>Legacy overload.</summary>
    public static CardBudgetValidation Validate(
        int cost, Rarity rarity, int attack, int hitPoints, int initiative,
        int treasure = 0, bool isAmbusher = false)
    {
        return ValidateAlly(cost, rarity, attack, hitPoints, initiative, treasure, isAmbusher);
    }

    private static CardBudgetValidation BuildValidation(
        int budget, Rarity rarity, int attack, int hitPoints, int initiative,
        int treasure = 0, bool isAmbusher = false)
    {
        var statSpend = CalculateStatSpend(attack, hitPoints, initiative, treasure, isAmbusher);
        var effectBudget = budget - statSpend;
        var isValid = effectBudget >= 0;

        return new CardBudgetValidation(budget, statSpend, effectBudget, isValid);
    }

    #endregion

    #region Effect Cost Reference

    // Stat modifier effects (effect bonuses use flat 3 cost, matching base stat cost)
    public const float EffectModAtkSelf = 3.0f;     // +1 ATK to self/ally
    public const float EffectModHpSelf = 3.0f;      // +1 HP to self/ally
    public const float EffectModInitSelf = 1.5f;     // +1 INIT to self/ally
    public const float EffectDebuffAtkEnemy = 4.0f;  // -1 ATK to enemy (debuff premium)
    public const float EffectDebuffHpEnemy = 4.0f;   // -1 HP to enemy (permanent damage)
    public const float EffectDebuffInitEnemy = 2.0f;  // -1 INIT to enemy
    public const float EffectAoeBuff = 2.0f;          // AoE multiplier (single cost × 2)
    public const float EffectAoeDebuff = 2.0f;         // AoE multiplier for debuffs

    // Damage & healing
    public const float EffectDamageEnemy = 2.0f;     // DAMAGE:1 to single enemy
    public const float EffectDamageAllEnemies = 4.0f; // DAMAGE:1 to all enemies
    public const float EffectHealAlly = 1.0f;         // HEAL:1 to single ally
    public const float EffectHealAllAllies = 2.0f;    // HEAL:1 to all allies

    // Control
    public const float EffectStun = 3.0f;
    public const float EffectDisableRetarget = 1.5f;
    public const float EffectDisableOppAttack = 1.5f;
    public const float EffectDisableConsumable = 2.0f;
    public const float EffectForceAbandon = 3.0f;
    public const float EffectReturnToHand = 3.0f;
    public const float EffectRemoveFromCombat = 3.0f;

    // Destruction & exile
    public const float EffectDestroyEquipment = 2.0f;
    public const float EffectDestroySlotSpecific = 2.5f;
    public const float EffectExileTarget = 5.0f;
    public const float EffectExileDeck1 = 2.0f;
    public const float EffectExileSelf = -1.0f; // Downside — gives budget back

    // Defensive
    public const float EffectReduceDamage1 = 2.0f;
    public const float EffectRevealTrap = 1.5f;       // Reveal next trap — lets player plan around it
    public const float EffectDisarmTrapSacrifice = 2.5f; // Discard this equipment/consumable to disarm target trap
    public const float EffectDisarmTrapExile = 3.0f;    // Pay exile cost from deck to disarm trap (for allies)
    public const float EffectImmuneOppAttack = 2.0f;
    public const float EffectImmuneElim = 5.0f;
    public const float EffectElimResist = 3.0f;  // Raises elimination threshold from 2x to 3x
    public const float EffectAmbusher = 2.0f;
    public const float EffectNegateEffect = 3.0f;
    public const float EffectRedirectDamage = 4.0f;
    public const float EffectFreeRetarget = 2.0f;

    // Card flow
    public const float EffectDraw1 = 1.5f;
    public const float EffectDraw2 = 3.0f;
    public const float EffectReveal = 1.0f;
    public const float EffectSearchAny = 5.0f;
    public const float EffectRestoreFromDiscard = 3.0f;

    // Trigger multipliers
    public const float TriggerPassive = 1.5f;
    public const float TriggerOnCombatStart = 1.0f;
    public const float TriggerOnActivate = 0.8f;
    public const float TriggerConditional = 0.5f;   // WITH_ADVANTAGE, WITH_DISADVANTAGE
    public const float TriggerOnDeath = 0.5f;
    public const float TriggerOnKill = 0.5f;
    public const float TriggerOnRoundStart = 1.2f;
    public const float TriggerRaceCondition = 0.6f;  // IF_RACE:X
    public const float TriggerClassCondition = 0.6f;  // IF_CLASS:X

    #endregion
}

public record CardBudgetValidation(
    int TotalBudget,
    float StatSpend,
    float EffectBudget,
    bool IsValid);
