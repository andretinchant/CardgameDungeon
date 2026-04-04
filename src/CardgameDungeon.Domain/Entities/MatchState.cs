using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Domain.ValueObjects;

namespace CardgameDungeon.Domain.Entities;

public class MatchState
{
    public const int SetupMaxCost = 5;

    public Guid Id { get; private set; }
    public PlayerState Player1 { get; private set; }
    public PlayerState Player2 { get; private set; }
    public MatchPhase Phase { get; private set; }
    public int CurrentRoom { get; private set; }
    public Guid? InitiativeWinnerId { get; private set; }
    public Guid? AttackerId { get; private set; }
    public Guid? WinnerId { get; private set; }

    /// <summary>Whose turn it is (alternates after each turn ends).</summary>
    public Guid ActivePlayerId { get; private set; }

    /// <summary>How many rooms the active player has cleared THIS turn (max 2).</summary>
    public int RoomsClearedThisTurn { get; private set; }

    /// <summary>Max rooms a player can clear per turn.</summary>
    public const int MaxRoomsPerTurn = 2;

    /// <summary>Player 1's current room progress (separate tracking per player).</summary>
    public int Player1Room { get; private set; } = 1;

    /// <summary>Player 2's current room progress (separate tracking per player).</summary>
    public int Player2Room { get; private set; } = 1;

    private readonly List<DungeonRoomCard> _dungeonRooms;
    private readonly BossCard _boss;

    // Setup tracking: hidden teams until both submitted
    private readonly HashSet<Guid> _setupSubmitted = [];
    private List<AllyCard>? _player1PendingTeam;
    private List<AllyCard>? _player2PendingTeam;

    // Bet tracking for initiative ties
    public int Player1BetTotal { get; private set; }
    public int Player2BetTotal { get; private set; }

    // Combat board
    public CombatBoard CombatBoard { get; } = new();

    public IReadOnlyList<DungeonRoomCard> DungeonRooms => _dungeonRooms.AsReadOnly();
    public BossCard Boss => _boss;
    public bool IsFinished => Phase == MatchPhase.Finished;
    public bool IsBossRoom => CurrentRoom > _dungeonRooms.Count;
    public bool BothTeamsSubmitted => _setupSubmitted.Count == 2;
    public bool CanAdvance => RoomsClearedThisTurn < MaxRoomsPerTurn;

    /// <summary>The active player's current room (each player tracks separately).</summary>
    public int GetPlayerRoom(Guid playerId) =>
        playerId == Player1.PlayerId ? Player1Room : Player2Room;

    /// <summary>Gets the active player (whose turn it is).</summary>
    public PlayerState GetActivePlayer() => GetPlayer(ActivePlayerId);

    /// <summary>Gets the non-active player (defending).</summary>
    public PlayerState GetInactivePlayer() => GetOpponent(ActivePlayerId);

    public DungeonRoomCard? CurrentDungeonRoom =>
        CurrentRoom <= _dungeonRooms.Count ? _dungeonRooms[CurrentRoom - 1] : null;

    public PlayerState GetAttacker()
    {
        if (AttackerId is null)
            throw new InvalidOperationException("Role has not been chosen yet.");
        return GetPlayer(AttackerId.Value);
    }

    public PlayerState GetDefender()
    {
        if (AttackerId is null)
            throw new InvalidOperationException("Role has not been chosen yet.");
        return GetOpponent(AttackerId.Value);
    }

    public MatchState(
        Guid id,
        PlayerState player1,
        PlayerState player2,
        IEnumerable<DungeonRoomCard> dungeonRooms,
        BossCard boss)
    {
        Id = id;
        Player1 = player1 ?? throw new ArgumentNullException(nameof(player1));
        Player2 = player2 ?? throw new ArgumentNullException(nameof(player2));
        _dungeonRooms = dungeonRooms.OrderBy(r => r.Order).ToList();
        _boss = boss ?? throw new ArgumentNullException(nameof(boss));
        Phase = MatchPhase.Setup;
        CurrentRoom = 1;
        ActivePlayerId = player1.PlayerId; // P1 goes first after setup
        Player1Room = 1;
        Player2Room = 1;

        if (_dungeonRooms.Count != DungeonRoomCard.TotalRooms)
            throw new ArgumentException($"Match requires exactly {DungeonRoomCard.TotalRooms} dungeon rooms.");
    }

    public PlayerState GetPlayer(Guid playerId)
    {
        if (Player1.PlayerId == playerId) return Player1;
        if (Player2.PlayerId == playerId) return Player2;
        throw new ArgumentException($"Player {playerId} is not in this match.");
    }

    public PlayerState GetOpponent(Guid playerId)
    {
        if (Player1.PlayerId == playerId) return Player2;
        if (Player2.PlayerId == playerId) return Player1;
        throw new ArgumentException($"Player {playerId} is not in this match.");
    }

    public bool HasPlayerSubmittedTeam(Guid playerId) => _setupSubmitted.Contains(playerId);

    public void SubmitSetupTeam(Guid playerId, IReadOnlyList<AllyCard> team)
    {
        EnsurePhase(MatchPhase.Setup);

        if (_setupSubmitted.Contains(playerId))
            throw new InvalidOperationException("This player has already submitted their team.");

        var totalCost = team.Sum(a => a.Cost);
        if (totalCost > SetupMaxCost)
            throw new InvalidOperationException(
                $"Setup team cost must be at most {SetupMaxCost}, got {totalCost}.");

        if (team.Any(a => a.Cost == 0))
            throw new InvalidOperationException("Setup team cannot include cards with cost 0.");

        if (team.Count > PlayerState.MaxAlliesInPlay)
            throw new InvalidOperationException(
                $"Setup team size ({team.Count}) exceeds ally limit of {PlayerState.MaxAlliesInPlay}.");

        // Store pending — don't play to field yet (hidden until reveal)
        if (Player1.PlayerId == playerId)
            _player1PendingTeam = team.ToList();
        else
            _player2PendingTeam = team.ToList();

        _setupSubmitted.Add(playerId);
    }

    public void RevealTeams()
    {
        EnsurePhase(MatchPhase.Setup);

        if (!BothTeamsSubmitted)
            throw new InvalidOperationException("Both players must submit their teams before revealing.");

        // Setup allies come from deck (via ExtractAlliesFromDeck), not hand.
        // Add directly to allies in play without the hand check.
        foreach (var ally in _player1PendingTeam!)
            Player1.AddAllyDirectly(ally);
        foreach (var ally in _player2PendingTeam!)
            Player2.AddAllyDirectly(ally);

        _player1PendingTeam = null;
        _player2PendingTeam = null;

        // After setup, start first turn (P1 goes first)
        StartTurn();
    }

    public void ResolveCombat(int attackerDamage, int defenderDamage, bool simultaneousElimination)
    {
        EnsurePhase(MatchPhase.Combat, MatchPhase.BossRoom);

        if (simultaneousElimination)
        {
            if (IsBossRoom)
                WinnerId = AttackerId; // attackers win boss room ties
            else
                WinnerId = GetDefender().PlayerId; // defender wins normal room ties
        }
        else
        {
            var attackerPlayer = GetAttacker();
            var defenderPlayer = GetDefender();
            // attackerDamage = damage dealt TO attacker (from defender's strength)
            // defenderDamage = damage dealt TO defender (from attacker's strength)
            attackerPlayer.TakeDamage(attackerDamage);
            defenderPlayer.TakeDamage(defenderDamage);
        }

        CombatBoard.Clear();
        // After combat resolves, room is cleared
        Phase = MatchPhase.RoomCleared;
    }

    public void ConcedeRoom()
    {
        EnsurePhase(MatchPhase.Combat);

        // Defender concedes — attacker wins room without combat
        CombatBoard.Clear();
        Phase = MatchPhase.RoomCleared;
    }

    // ── Turn-Based Flow ──

    /// <summary>
    /// Start a new turn for the active player.
    /// 1. Draw up to 8 cards
    /// 2. Move to PlayCards phase
    /// </summary>
    public void StartTurn()
    {
        var active = GetActivePlayer();
        active.RefillHand();
        AttackerId = ActivePlayerId;
        RoomsClearedThisTurn = 0;
        CurrentRoom = GetPlayerRoom(ActivePlayerId);
        Phase = MatchPhase.PlayCards;
    }

    /// <summary>
    /// Active player finishes playing cards. Move to defender setup.
    /// </summary>
    public void FinishPlayingCards()
    {
        EnsurePhase(MatchPhase.PlayCards);
        Phase = MatchPhase.DefenderSetup;
    }

    /// <summary>
    /// Defender finishes placing monsters and traps. Move to combat.
    /// </summary>
    public void FinishDefenderSetup()
    {
        EnsurePhase(MatchPhase.DefenderSetup);
        Phase = IsBossRoom ? MatchPhase.BossRoom : MatchPhase.Combat;
    }

    /// <summary>
    /// Room has been cleared (all monsters defeated). Move to RoomCleared choice.
    /// </summary>
    public void RoomCleared()
    {
        EnsurePhase(MatchPhase.Combat, MatchPhase.BossRoom);
        CombatBoard.Clear();
        RoomsClearedThisTurn++;

        // Advance the active player's room counter
        if (ActivePlayerId == Player1.PlayerId)
            Player1Room++;
        else
            Player2Room++;

        // Check win: if player cleared all 5 rooms + boss
        var playerRoom = GetPlayerRoom(ActivePlayerId);
        if (playerRoom > _dungeonRooms.Count + 1)
        {
            WinnerId = ActivePlayerId;
            Phase = MatchPhase.Finished;
            return;
        }

        Phase = MatchPhase.RoomCleared;
    }

    /// <summary>
    /// Active player chooses to STOP after clearing a room.
    /// Heals 1 HP to all allies. Passes turn.
    /// </summary>
    public void StopAndHeal()
    {
        EnsurePhase(MatchPhase.RoomCleared);

        var active = GetActivePlayer();
        // Heal 1 HP to all allies (add to current HP — handled by caller or effect)
        // For now, signal the intent; actual HP restoration needs a mechanism

        PassTurn();
    }

    /// <summary>
    /// Active player chooses to ADVANCE to the next room.
    /// Defender can reshuffle hand and draw 8, or draw up to 8.
    /// Back to PlayCards phase for attacker.
    /// </summary>
    public void AdvanceToNextRoom(bool defenderReshuffles)
    {
        EnsurePhase(MatchPhase.RoomCleared);

        if (!CanAdvance)
            throw new InvalidOperationException($"Cannot advance — already cleared {MaxRoomsPerTurn} rooms this turn.");

        // Increment room counter for active player
        RoomsClearedThisTurn++;
        if (ActivePlayerId == Player1.PlayerId)
            Player1Room++;
        else
            Player2Room++;

        CurrentRoom = GetPlayerRoom(ActivePlayerId);

        // Check win: if player cleared all rooms + boss
        if (CurrentRoom > _dungeonRooms.Count + 1)
        {
            WinnerId = ActivePlayerId;
            Phase = MatchPhase.Finished;
            return;
        }

        // Defender gets to refresh hand
        var defender = GetInactivePlayer();
        if (defenderReshuffles)
            defender.ReshuffleHandAndRedraw();
        else
            defender.RefillHand();

        // Back to attacker playing cards
        Phase = MatchPhase.PlayCards;
    }

    /// <summary>Pass the turn to the other player.</summary>
    public void PassTurn()
    {
        ActivePlayerId = ActivePlayerId == Player1.PlayerId ? Player2.PlayerId : Player1.PlayerId;
        RoomsClearedThisTurn = 0;
        StartTurn();
    }

    public void Finish(Guid winnerId)
    {
        WinnerId = winnerId;
        Phase = MatchPhase.Finished;
    }

    private void EnsurePhase(params MatchPhase[] expected)
    {
        if (!expected.Contains(Phase))
            throw new InvalidOperationException(
                $"Cannot perform this action during {Phase} phase. Expected: {string.Join(" or ", expected)}.");
    }
}
