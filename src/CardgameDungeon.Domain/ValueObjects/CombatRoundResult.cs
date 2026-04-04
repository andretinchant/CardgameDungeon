namespace CardgameDungeon.Domain.ValueObjects;

/// <summary>
/// Result of a single combat round.
/// Used in round-by-round combat resolution where players can pause between rounds.
/// </summary>
public record CombatRoundResult(
    int AttackerAttack,
    int DefenderAttack,
    bool AttackerEliminated,
    bool DefenderEliminated,
    bool CombatEnded,
    int TotalRounds,
    CombatAdvantage Advantage)
{
    public bool IsStalemate => !CombatEnded && AttackerAttack == DefenderAttack;
}
