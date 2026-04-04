using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.Domain.ValueObjects;

public record BattleResolutionResult
{
    public int AttackerAttack { get; }
    public int DefenderAttack { get; }
    public int DamageToAttacker { get; }
    public int DamageToDefender { get; }
    public CombatOutcome Outcome { get; }
    public bool IsBossRoom { get; }
    public CombatAdvantage Advantage { get; }

    /// <summary>Number of ATK comparison rounds fought before combat ended.</summary>
    public int Rounds { get; }

    /// <summary>
    /// The side that wins in case of simultaneous elimination.
    /// Boss room: attacker wins. Normal room: defender wins.
    /// </summary>
    public string SimultaneousEliminationVictor =>
        IsBossRoom ? "Attacker" : "Defender";

    public BattleResolutionResult(
        int attackerAttack,
        int defenderAttack,
        int damageToAttacker,
        int damageToDefender,
        CombatOutcome outcome,
        bool isBossRoom,
        CombatAdvantage? advantage = null,
        int rounds = 0)
    {
        AttackerAttack = attackerAttack;
        DefenderAttack = defenderAttack;
        DamageToAttacker = damageToAttacker;
        DamageToDefender = damageToDefender;
        Outcome = outcome;
        IsBossRoom = isBossRoom;
        Advantage = advantage ?? CombatAdvantage.Calculate(0, 0);
        Rounds = rounds;
    }
}
