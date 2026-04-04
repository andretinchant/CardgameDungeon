using CardgameDungeon.Domain.Repositories;
using CardgameDungeon.Features.Match.Shared;
using MediatR;

namespace CardgameDungeon.Features.Match.ChooseRole;

public class ChooseRoleHandler(IMatchRepository matchRepo, IMatchNotifier notifier)
    : IRequestHandler<ChooseRoleCommand, MatchResponse>
{
    public Task<MatchResponse> Handle(ChooseRoleCommand request, CancellationToken ct)
    {
        // Role selection has been removed. The active player is always the attacker.
        throw new NotImplementedException(
            "ChooseRole has been removed. The active player is always the attacker in the new turn-based flow.");
    }
}
