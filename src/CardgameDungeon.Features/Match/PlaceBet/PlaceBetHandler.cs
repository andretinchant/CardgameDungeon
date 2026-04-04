using CardgameDungeon.Domain.Repositories;
using CardgameDungeon.Features.Match.Shared;
using MediatR;

namespace CardgameDungeon.Features.Match.PlaceBet;

public class PlaceBetHandler(IMatchRepository matchRepo, IMatchNotifier notifier)
    : IRequestHandler<PlaceBetCommand, PlaceBetResponse>
{
    public Task<PlaceBetResponse> Handle(PlaceBetCommand request, CancellationToken ct)
    {
        // Betting (initiative tie-breaker) has been removed from the new turn-based flow.
        throw new NotImplementedException(
            "PlaceBet has been removed. Initiative and betting are obsolete in the new turn-based flow.");
    }
}
