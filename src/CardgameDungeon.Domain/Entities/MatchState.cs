using CardgameDungeon.Domain.Enums;

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
    public Guid? WinnerId { get; private set; }

    private readonly List<DungeonRoomCard> _dungeonRooms;
    private readonly BossCard _boss;

    public IReadOnlyList<DungeonRoomCard> DungeonRooms => _dungeonRooms.AsReadOnly();
    public BossCard Boss => _boss;
    public bool IsFinished => Phase == MatchPhase.Finished;
    public bool IsBossRoom => CurrentRoom > _dungeonRooms.Count;
    public DungeonRoomCard? CurrentDungeonRoom =>
        CurrentRoom <= _dungeonRooms.Count ? _dungeonRooms[CurrentRoom - 1] : null;

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

    public void SubmitSetupTeam(Guid playerId, IReadOnlyList<AllyCard> team)
    {
        EnsurePhase(MatchPhase.Setup);

        var totalCost = team.Sum(a => a.Cost);
        if (totalCost > SetupMaxCost)
            throw new InvalidOperationException(
                $"Setup team cost ({totalCost}) exceeds maximum of {SetupMaxCost}.");

        if (team.Count > PlayerState.MaxAlliesInPlay)
            throw new InvalidOperationException(
                $"Setup team size ({team.Count}) exceeds ally limit of {PlayerState.MaxAlliesInPlay}.");

        var player = GetPlayer(playerId);
        foreach (var ally in team)
            player.PlayAlly(ally);
    }

    public void AdvanceToRoomReveal()
    {
        EnsurePhase(MatchPhase.Setup);
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
            Phase = IsBossRoom ? MatchPhase.BossRoom : MatchPhase.Combat;
        }
        // Tie: callers must handle the bid war externally, then call again
    }

    public void ResolveCombat(int attackerDamage, int defenderDamage, bool simultaneousElimination)
    {
        EnsurePhase(MatchPhase.Combat, MatchPhase.BossRoom);

        if (simultaneousElimination)
        {
            if (IsBossRoom)
                WinnerId = InitiativeWinnerId; // attackers win boss room ties
            else
                WinnerId = InitiativeWinnerId == Player1.PlayerId
                    ? Player2.PlayerId
                    : Player1.PlayerId; // defender wins normal room ties
        }
        else
        {
            Player1.TakeDamage(defenderDamage);
            Player2.TakeDamage(attackerDamage);
        }

        Phase = MatchPhase.RoomResolution;
    }

    public void AdvanceRoom()
    {
        CurrentRoom++;
        Player1.RefillHand();
        Player2.RefillHand();

        if (IsBossRoom)
        {
            Phase = MatchPhase.Initiative;
            return;
        }

        if (CurrentRoom > _dungeonRooms.Count + 1) // past boss
        {
            Phase = MatchPhase.Finished;
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
