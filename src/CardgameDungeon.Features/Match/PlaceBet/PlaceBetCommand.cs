using MediatR;

namespace CardgameDungeon.Features.Match.PlaceBet;

public record PlaceBetCommand(
    Guid MatchId,
    Guid PlayerId,
    int Amount,
    bool Exile) : IRequest<PlaceBetResponse>;

public record PlaceBetResponse(
    Guid MatchId,
    Guid PlayerId,
    int Player1BetTotal,
    int Player2BetTotal,
    bool Resolved,
    Guid? WinnerId);
