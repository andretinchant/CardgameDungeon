using CardgameDungeon.Domain.Repositories;
using CardgameDungeon.Features.Match.Shared;
using MediatR;

namespace CardgameDungeon.Features.Match.PlaceBet;

public class PlaceBetHandler(IMatchRepository matchRepo, IMatchNotifier notifier)
    : IRequestHandler<PlaceBetCommand, PlaceBetResponse>
{
    public async Task<PlaceBetResponse> Handle(PlaceBetCommand request, CancellationToken ct)
    {
        var match = await matchRepo.GetByIdAsync(request.MatchId, ct)
            ?? throw new KeyNotFoundException($"Match {request.MatchId} not found.");

        match.PlaceBet(request.PlayerId, request.Amount, request.Exile);

        var resolved = match.TryResolveBets();

        await matchRepo.UpdateAsync(match, ct);

        var response = new PlaceBetResponse(
            match.Id,
            request.PlayerId,
            match.Player1BetTotal,
            match.Player2BetTotal,
            resolved,
            match.InitiativeWinnerId);

        await notifier.BetPlaced(match.Id, response);

        return response;
    }
}
