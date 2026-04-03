using MediatR;

namespace CardgameDungeon.Features.Tournament.FinalizeTournament;

public record FinalizeTournamentCommand(Guid TournamentId) : IRequest<FinalizeTournamentResponse>;

public record FinalizeTournamentResponse(
    Guid TournamentId,
    Guid FirstPlaceId, int FirstPrize,
    Guid SecondPlaceId, int SecondPrize,
    Guid ThirdPlaceId, int ThirdPrize);
