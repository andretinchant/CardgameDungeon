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

        foreach (var ally in _player1PendingTeam!)
            Player1.PlayAlly(ally);
        foreach (var ally in _player2PendingTeam!)
            Player2.PlayAlly(ally);

        _player1PendingTeam = null;
        _player2PendingTeam = null;

        Phase = MatchPhase.RoomReveal;
    }

    public void RevealRoom()
    {
        EnsurePhase(MatchPhase.RoomReveal);

        var room = CurrentDungeonRoom;

        if (room is not null && !room.HasMonsters)
        {
            AdvanceRoom();
            return;
        }

        Phase = MatchPhase.Initiative;
    }

    public void ResolveInitiative(int player1Total, int player2Total)
    {
        EnsurePhase(MatchPhase.Initiative);

        if (player1Total != player2Total)
        {
            InitiativeWinnerId = player1Total > player2Total
                ? Player1.PlayerId
                : Player2.PlayerId;
            Player1BetTotal = 0;
            Player2BetTotal = 0;
            Phase = MatchPhase.RoleSelection;
        }
        // Tie: callers must handle the bid war externally, then call again
    }

    /// <summary>
    /// Initiative winner chooses whether to be the attacker or defender.
    /// </summary>
    public void ChooseRole(Guid playerId, bool choosesToAttack)
    {
        EnsurePhase(MatchPhase.RoleSelection);

        if (InitiativeWinnerId != playerId)
            throw new InvalidOperationException("Only the initiative winner can choose the role.");

        if (choosesToAttack)
        {
            AttackerId = playerId;
        }
        else
        {
            // Winner chose to defend — opponent becomes the attacker
            AttackerId = GetOpponent(playerId).PlayerId;
        }

        Phase = IsBossRoom ? MatchPhase.BossRoom : MatchPhase.Combat;
    }

    public void PlaceBet(Guid playerId, int amount, bool exile)
    {
        EnsurePhase(MatchPhase.Initiative);

        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Bet amount must be positive.");

        var player = GetPlayer(playerId);
        player.PayCostFromDeck(amount, exile);

        if (Player1.PlayerId == playerId)
            Player1BetTotal += amount;
        else
            Player2BetTotal += amount;
    }

    public bool TryResolveBets()
    {
        EnsurePhase(MatchPhase.Initiative);

        if (Player1BetTotal == Player2BetTotal)
            return false; // Still tied, need more bets

        InitiativeWinnerId = Player1BetTotal > Player2BetTotal
            ? Player1.PlayerId
            : Player2.PlayerId;

        Player1BetTotal = 0;
        Player2BetTotal = 0;
        Phase = MatchPhase.RoleSelection;
        return true;
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
        Phase = MatchPhase.RoomResolution;
    }

    public void ConcedeRoom()
    {
        EnsurePhase(MatchPhase.Combat);

        // Defender concedes — attacker wins room without combat
        CombatBoard.Clear();
        Phase = MatchPhase.RoomResolution;
    }

    public void AdvanceRoom()
    {
        CurrentRoom++;

        // Only defender draws up to 8 when entering a new room
        if (AttackerId.HasValue)
            GetDefender().RefillHand();

        // Reset roles for next room
        AttackerId = null;
        InitiativeWinnerId = null;

        if (CurrentRoom > _dungeonRooms.Count + 1) // past boss = finished
        {
            Phase = MatchPhase.Finished;
            return;
        }

        if (IsBossRoom)
        {
            Phase = MatchPhase.Initiative;
            return;
        }

        Phase = MatchPhase.RoomReveal;
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
