using CardgameDungeon.Features.Match.Shared;
using MediatR;

namespace CardgameDungeon.Features.Match.GetMatchState;

public record GetMatchStateQuery(Guid MatchId) : IRequest<MatchResponse>;
