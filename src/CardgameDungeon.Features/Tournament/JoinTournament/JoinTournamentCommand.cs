using MediatR;

namespace CardgameDungeon.Features.Tournament.JoinTournament;

public record JoinTournamentCommand(
    Guid TournamentId,
    Guid PlayerId,
    Guid DeckId) : IRequest<JoinTournamentResponse>;

public record JoinTournamentResponse(
    Guid TournamentId, Guid PlayerId, int ParticipantCount, int PrizePool);
