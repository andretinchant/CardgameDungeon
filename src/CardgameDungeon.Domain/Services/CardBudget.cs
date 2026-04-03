using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.Domain.Services;

/// <summary>
/// Controls power creep via a point-budget system for card stats and effects.
///
/// Budget per cost (diminishing returns at higher costs):
///   Cost 1=6, 2=10, 3=14, 4=18, 5=21, 6=24, 7=27, 8=29, 9=31, 10=33
///
/// Unique/Boss bonus: +5 at cost 5, scaling to +10 at cost 10.
///
/// Stat costs: STR=1, HP=1, INIT=1.5, Treasure=1, Ambusher=2
/// Effect budget = total budget - stat spend. Rarity determines effect allocation:
///   Common: 0-2 effect pts (stat stick)
///   Uncommon: 3-4 effect pts (moderate ability)
///   Rare: 5-6 effect pts (strong ability)
///   Unique: 7-8 effect pts (irreplaceable ability)
/// </summary>
public static class CardBudget
{
    private static readonly int[] NormalBudget =
    //  cost:  0   1   2   3   4   5   6   7   8   9  10
             [ 0,  6, 10, 14, 18, 21, 24, 27, 29, 31, 33 ];

    private static readonly int[] UniqueBonus =
    //  cost:  0   1   2   3   4   5   6   7   8   9  10
             [ 0,  0,  0,  0,  0,  5,  6,  7,  8,  9, 10 ];

    public const float StrengthCost = 1.0f;
    public const float HitPointsCost = 1.0f;
    public const float InitiativeCost = 1.5f;
    public const float TreasureCost = 1.0f;
    public const float AmbusherCost = 2.0f;

    /// <summary>Returns the total budget for a card at the given cost and rarity.</summary>
    public static int GetBudget(int cost, bool isUniqueOrBoss = false)
    {
        var c = Math.Clamp(cost, 0, 10);
        var budget = NormalBudget[c];
        if (isUniqueOrBoss) budget += UniqueBonus[c];
        return budget;
    }

    /// <summary>Calculates how many budget points a card's stats consume.</summary>
    public static float CalculateStatSpend(int strength, int hitPoints, int initiative,
        int treasure = 0, bool isAmbusher = false)
    {
        return strength * StrengthCost
             + hitPoints * HitPointsCost
             + initiative * InitiativeCost
             + treasure * TreasureCost
             + (isAmbusher ? AmbusherCost : 0);
    }

    /// <summary>Returns the remaining budget available for effects.</summary>
    public static float GetEffectBudget(int cost, int strength, int hitPoints, int initiative,
        int treasure = 0, bool isAmbusher = false, bool isUniqueOrBoss = false)
    {
        var total = GetBudget(cost, isUniqueOrBoss);
        var statSpend = CalculateStatSpend(strength, hitPoints, initiative, treasure, isAmbusher);
        return total - statSpend;
    }

    /// <summary>Returns the recommended effect point range for a given rarity.</summary>
    public static (int min, int max) GetEffectRange(Rarity rarity) => rarity switch
    {
        Rarity.Common => (0, 2),
        Rarity.Uncommon => (3, 4),
        Rarity.Rare => (5, 6),
        Rarity.Unique => (7, 8),
        _ => (0, 2)
    };

    /// <summary>
    /// Validates that a card's total stat spend + effect budget fits within its budget.
    /// Returns (isValid, budget, statSpend, effectBudget, recommendedEffectRange).
    /// </summary>
    public static CardBudgetValidation Validate(
        int cost, Rarity rarity, int strength, int hitPoints, int initiative,
        int treasure = 0, bool isAmbusher = false)
    {
        var isUnique = rarity == Rarity.Unique;
        var budget = GetBudget(cost, isUnique);
        var statSpend = CalculateStatSpend(strength, hitPoints, initiative, treasure, isAmbusher);
        var effectBudget = budget - statSpend;
        var (minEffect, maxEffect) = GetEffectRange(rarity);
        var isValid = effectBudget >= minEffect && effectBudget <= maxEffect + 1;

        return new CardBudgetValidation(budget, statSpend, effectBudget, minEffect, maxEffect, isValid);
    }

    /// <summary>Calculates budget for equipment modifier totals.</summary>
    public static int GetEquipmentBudget(int cost, bool isUnique = false)
    {
        // Equipment budget is roughly half the creature budget
        // since modifiers stack on top of ally stats
        var c = Math.Clamp(cost, 0, 10);
        var budget = (int)Math.Ceiling(NormalBudget[c] * 0.45f);
        if (isUnique) budget += UniqueBonus[c] / 2;
        return budget;
    }

    /// <summary>Calculates budget for trap damage values.</summary>
    public static int GetTrapBudget(int cost, bool isUnique = false)
    {
        // Traps: budget is roughly damage * 1.0 + effect points
        return GetBudget(cost, isUnique);
    }
}

public record CardBudgetValidation(
    int TotalBudget,
    float StatSpend,
    float EffectBudget,
    int RecommendedEffectMin,
    int RecommendedEffectMax,
    bool IsValid);
