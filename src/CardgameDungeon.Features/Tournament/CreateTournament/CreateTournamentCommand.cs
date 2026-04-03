using CardgameDungeon.Domain.Enums;
using MediatR;

namespace CardgameDungeon.Features.Tournament.CreateTournament;

public record CreateTournamentCommand(Tier RequiredTier, int EntryFee) : IRequest<TournamentResponse>;

public record TournamentResponse(
    Guid Id, Tier RequiredTier, int EntryFee, int PrizePool,
    TournamentStatus Status, int ParticipantCount, int CurrentRound);
