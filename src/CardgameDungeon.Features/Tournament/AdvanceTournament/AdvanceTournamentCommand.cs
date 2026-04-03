using CardgameDungeon.Domain.Enums;
using MediatR;

namespace CardgameDungeon.Features.Tournament.AdvanceTournament;

public record AdvanceTournamentCommand(
    Guid TournamentId,
    Guid LoserId) : IRequest<AdvanceTournamentResponse>;

public record AdvanceTournamentResponse(
    Guid TournamentId, int CurrentRound, int RemainingPlayers, TournamentStatus Status);
