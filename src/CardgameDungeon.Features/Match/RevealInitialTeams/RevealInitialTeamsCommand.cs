using CardgameDungeon.Features.Match.Shared;
using MediatR;

namespace CardgameDungeon.Features.Match.RevealInitialTeams;

public record RevealInitialTeamsCommand(Guid MatchId) : IRequest<MatchResponse>;
