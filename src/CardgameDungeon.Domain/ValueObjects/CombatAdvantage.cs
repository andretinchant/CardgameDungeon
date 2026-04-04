namespace CardgameDungeon.Domain.ValueObjects;

public enum CombatAdvantageState
{
    /// <summary>Neither side has numerical superiority (equal unit counts or 1v1).</summary>
    Neutral,

    /// <summary>This side has more units than the opponent in this combat group.</summary>
    Advantage,

    /// <summary>This side has fewer units than the opponent in this combat group.</summary>
    Disadvantage
}

public record CombatAdvantage(
    CombatAdvantageState AttackerState,
    CombatAdvantageState DefenderState,
    int AttackerCount,
    int DefenderCount)
{
    /// <summary>
    /// Calculates advantage states from unit counts.
    /// No automatic ATK bonus — advantage only triggers card effects.
    /// </summary>
    public static CombatAdvantage Calculate(int attackerCount, int defenderCount)
    {
        if (attackerCount == defenderCount)
            return new CombatAdvantage(CombatAdvantageState.Neutral, CombatAdvantageState.Neutral, attackerCount, defenderCount);

        if (attackerCount > defenderCount)
            return new CombatAdvantage(CombatAdvantageState.Advantage, CombatAdvantageState.Disadvantage, attackerCount, defenderCount);

        return new CombatAdvantage(CombatAdvantageState.Disadvantage, CombatAdvantageState.Advantage, attackerCount, defenderCount);
    }

    public bool AttackerHasAdvantage => AttackerState == CombatAdvantageState.Advantage;
    public bool DefenderHasAdvantage => DefenderState == CombatAdvantageState.Advantage;
    public bool AttackerHasDisadvantage => AttackerState == CombatAdvantageState.Disadvantage;
    public bool DefenderHasDisadvantage => DefenderState == CombatAdvantageState.Disadvantage;
    public bool IsNeutral => AttackerState == CombatAdvantageState.Neutral;
}
