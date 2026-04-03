using CardgameDungeon.Domain.Repositories;
using CardgameDungeon.Domain.Services;
using CardgameDungeon.Features.Match.Shared;
using MediatR;

namespace CardgameDungeon.Features.Match.ResolveInitiative;

public class ResolveInitiativeHandler(IMatchRepository matchRepo, CombatResolver combatResolver, IMatchNotifier notifier)
    : IRequestHandler<ResolveInitiativeCommand, InitiativeResponse>
{
    public async Task<InitiativeResponse> Handle(ResolveInitiativeCommand request, CancellationToken ct)
    {
        var match = await matchRepo.GetByIdAsync(request.MatchId, ct)
            ?? throw new KeyNotFoundException($"Match {request.MatchId} not found.");

        var result = combatResolver.ResolveInitiative(
            match.Player1.AlliesInPlay,
            match.Player1.PlayerId,
            match.Player2.AlliesInPlay,
            match.Player2.PlayerId);

        match.ResolveInitiative(result.Player1Total, result.Player2Total);

        await matchRepo.UpdateAsync(match, ct);

        var response = new InitiativeResponse(
            match.Id,
            result.Player1Total,
            result.Player2Total,
            result.WinnerId,
            result.IsTied);

        await notifier.InitiativeResolved(match.Id, response);

        return response;
    }
}
