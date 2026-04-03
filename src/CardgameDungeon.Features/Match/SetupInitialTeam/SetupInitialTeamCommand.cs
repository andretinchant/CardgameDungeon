using CardgameDungeon.Features.Match.Shared;
using MediatR;

namespace CardgameDungeon.Features.Match.SetupInitialTeam;

public record SetupInitialTeamCommand(
    Guid MatchId,
    Guid PlayerId,
    IReadOnlyList<Guid> AllyCardIds) : IRequest<SetupInitialTeamResponse>;

public record SetupInitialTeamResponse(
    Guid MatchId,
    Guid PlayerId,
    bool Submitted,
    bool BothReady);
