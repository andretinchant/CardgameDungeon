using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.Domain.ValueObjects;

public record BattleResolutionResult
{
    public int AttackerStrength { get; }
    public int DefenderStrength { get; }
    public int DamageToAttacker { get; }
    public int DamageToDefender { get; }
    public CombatOutcome Outcome { get; }
    public bool IsBossRoom { get; }

    /// <summary>
    /// The side that wins in case of simultaneous elimination.
    /// Boss room: attacker wins. Normal room: defender wins.
    /// </summary>
    public string SimultaneousEliminationVictor =>
        IsBossRoom ? "Attacker" : "Defender";

    public BattleResolutionResult(
        int attackerStrength,
        int defenderStrength,
        int damageToAttacker,
        int damageToDefender,
        CombatOutcome outcome,
        bool isBossRoom)
    {
        AttackerStrength = attackerStrength;
        DefenderStrength = defenderStrength;
        DamageToAttacker = damageToAttacker;
        DamageToDefender = damageToDefender;
        Outcome = outcome;
        IsBossRoom = isBossRoom;
    }
}
