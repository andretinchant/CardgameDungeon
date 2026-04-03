using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.Domain.Entities;

public class Tournament
{
    public const int MaxPlayers = 8;
    public const int TotalRounds = 3; // 8→4→2→1

    public Guid Id { get; private set; }
    public Tier RequiredTier { get; private set; }
    public int EntryFee { get; private set; }
    public int PrizePool { get; private set; }
    public TournamentStatus Status { get; private set; }
    public int CurrentRound { get; private set; }

    public IReadOnlyList<TournamentParticipant> Participants => _participants.AsReadOnly();
    public IReadOnlyList<TournamentParticipant> ActiveParticipants
        => _participants.Where(p => !p.IsEliminated).ToList();

    private readonly List<TournamentParticipant> _participants = [];

    private Tournament() { } // EF Core

    public Tournament(Guid id, Tier requiredTier, int entryFee)
    {
        if (entryFee <= 0)
            throw new ArgumentOutOfRangeException(nameof(entryFee), "Entry fee must be positive.");

        Id = id;
        RequiredTier = requiredTier;
        EntryFee = entryFee;
        Status = TournamentStatus.Registration;
        CurrentRound = 0;
    }

    public void AddParticipant(Guid playerId, Guid deckId, Tier playerTier)
    {
        if (Status != TournamentStatus.Registration)
            throw new InvalidOperationException("Tournament is not accepting registrations.");

        if (_participants.Count >= MaxPlayers)
            throw new InvalidOperationException($"Tournament is full ({MaxPlayers} players).");

        if (playerTier != RequiredTier)
            throw new InvalidOperationException(
                $"Player tier {playerTier} does not match required tier {RequiredTier}.");

        if (_participants.Any(p => p.PlayerId == playerId))
            throw new InvalidOperationException("Player is already registered.");

        _participants.Add(new TournamentParticipant(playerId, deckId));
        PrizePool += EntryFee;
    }

    public void Start()
    {
        if (Status != TournamentStatus.Registration)
            throw new InvalidOperationException("Tournament has already started.");

        if (_participants.Count != MaxPlayers)
            throw new InvalidOperationException(
                $"Tournament requires exactly {MaxPlayers} players, got {_participants.Count}.");

        Status = TournamentStatus.InProgress;
        CurrentRound = 1;
    }

    public void EliminatePlayer(Guid playerId)
    {
        if (Status != TournamentStatus.InProgress)
            throw new InvalidOperationException("Tournament is not in progress.");

        var participant = _participants.FirstOrDefault(p => p.PlayerId == playerId)
            ?? throw new InvalidOperationException($"Player {playerId} is not in this tournament.");

        participant.Eliminate(CurrentRound);
    }

    public void AdvanceRound()
    {
        if (Status != TournamentStatus.InProgress)
            throw new InvalidOperationException("Tournament is not in progress.");

        CurrentRound++;

        if (ActiveParticipants.Count <= 1)
        {
            Status = TournamentStatus.Finished;
        }
    }

    public (Guid first, Guid second, Guid third) GetTopThree()
    {
        if (Status != TournamentStatus.Finished)
            throw new InvalidOperationException("Tournament is not finished.");

        var sorted = _participants
            .OrderByDescending(p => p.EliminatedInRound ?? int.MaxValue)
            .ToList();

        return (sorted[0].PlayerId, sorted[1].PlayerId, sorted[2].PlayerId);
    }

    public (int first, int second, int third) CalculatePrizes()
    {
        var first = (int)(PrizePool * 0.50);
        var second = (int)(PrizePool * 0.30);
        var third = PrizePool - first - second; // remainder ≈ 20%
        return (first, second, third);
    }
}

public class TournamentParticipant
{
    public Guid PlayerId { get; private set; }
    public Guid DeckId { get; private set; }
    public bool IsEliminated { get; private set; }
    public int? EliminatedInRound { get; private set; }

    private TournamentParticipant() { } // EF Core

    public TournamentParticipant(Guid playerId, Guid deckId)
    {
        PlayerId = playerId;
        DeckId = deckId;
    }

    public void Eliminate(int round)
    {
        if (IsEliminated)
            throw new InvalidOperationException("Player is already eliminated.");
        IsEliminated = true;
        EliminatedInRound = round;
    }
}
