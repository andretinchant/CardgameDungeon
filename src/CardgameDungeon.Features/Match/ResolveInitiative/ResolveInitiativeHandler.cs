using CardgameDungeon.Domain.Repositories;
using CardgameDungeon.Domain.Services;
using CardgameDungeon.Features.Match.Shared;
using MediatR;

namespace CardgameDungeon.Features.Match.ResolveInitiative;

public class ResolveInitiativeHandler(IMatchRepository matchRepo, CombatResolver combatResolver, IMatchNotifier notifier)
    : IRequestHandler<ResolveInitiativeCommand, InitiativeResponse>
{
    public Task<InitiativeResponse> Handle(ResolveInitiativeCommand request, CancellationToken ct)
    {
        // Initiative phase has been removed from the new turn-based flow.
        // The active player is always the attacker on their turn.
        throw new NotImplementedException(
            "Initiative phase has been removed. The active player is the attacker in the new turn-based flow.");
    }
}
