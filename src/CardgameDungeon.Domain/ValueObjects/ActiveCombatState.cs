namespace CardgameDungeon.Domain.ValueObjects;

/// <summary>
/// Tracks an ongoing (possibly suspended) combat in a room.
/// Persists HP damage taken by each unit across turns, allowing
/// players to pause combat, reinforce, and resume.
/// </summary>
public class ActiveCombatState
{
    /// <summary>Damage taken by each card (Guid → HP lost). Accumulates across rounds.</summary>
    private readonly Dictionary<Guid, int> _damageTaken = new();

    /// <summary>Total combat rounds fought so far.</summary>
    public int RoundsCompleted { get; private set; }

    /// <summary>Whether combat is currently suspended (paused between turns).</summary>
    public bool IsSuspended { get; private set; }

    /// <summary>The room number where this combat is happening.</summary>
    public int RoomNumber { get; }

    /// <summary>Whether this is a boss room combat.</summary>
    public bool IsBossRoom { get; }

    public IReadOnlyDictionary<Guid, int> DamageTaken => _damageTaken;

    public ActiveCombatState(int roomNumber, bool isBossRoom)
    {
        RoomNumber = roomNumber;
        IsBossRoom = isBossRoom;
    }

    /// <summary>Record damage dealt to a specific card.</summary>
    public void ApplyDamage(Guid cardId, int damage)
    {
        if (damage <= 0) return;
        _damageTaken.TryGetValue(cardId, out var existing);
        _damageTaken[cardId] = existing + damage;
    }

    /// <summary>Get the remaining HP for a card given its base HP.</summary>
    public int GetRemainingHp(Guid cardId, int baseHp)
    {
        _damageTaken.TryGetValue(cardId, out var damage);
        return Math.Max(0, baseHp - damage);
    }

    /// <summary>Get total damage taken by a card.</summary>
    public int GetDamageTaken(Guid cardId)
    {
        _damageTaken.TryGetValue(cardId, out var damage);
        return damage;
    }

    /// <summary>Record that a round was completed.</summary>
    public void CompleteRound() => RoundsCompleted++;

    /// <summary>Suspend combat (pause for turn pass).</summary>
    public void Suspend() => IsSuspended = true;

    /// <summary>Resume combat after suspension.</summary>
    public void Resume() => IsSuspended = false;
}
