namespace CardgameDungeon.Domain.Enums;

public enum MatchPhase
{
    /// <summary>Both players search deck for starting team (cost ≤ 5).</summary>
    Setup,

    /// <summary>Active player draws up to 8, plays allies/equipment/consumables from hand.</summary>
    PlayCards,

    /// <summary>Defender places monsters and traps from hand.</summary>
    DefenderSetup,

    /// <summary>Resolve combat: attacker's allies vs defender's monsters.</summary>
    Combat,

    /// <summary>Room cleared. Attacker chooses: STOP (heal 1 HP all allies) or ADVANCE.</summary>
    RoomCleared,

    /// <summary>Boss room combat (same as Combat but boss rules apply).</summary>
    BossRoom,

    /// <summary>Match over.</summary>
    Finished
}
