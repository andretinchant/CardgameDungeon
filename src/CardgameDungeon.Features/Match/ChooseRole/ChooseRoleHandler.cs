using CardgameDungeon.Domain.Repositories;
using CardgameDungeon.Features.Match.Shared;
using MediatR;

namespace CardgameDungeon.Features.Match.ChooseRole;

public class ChooseRoleHandler(IMatchRepository matchRepo, IMatchNotifier notifier)
    : IRequestHandler<ChooseRoleCommand, MatchResponse>
{
    public async Task<MatchResponse> Handle(ChooseRoleCommand request, CancellationToken ct)
    {
        var match = await matchRepo.GetByIdAsync(request.MatchId, ct)
            ?? throw new KeyNotFoundException($"Match {request.MatchId} not found.");

        match.ChooseRole(request.PlayerId, request.ChoosesToAttack);

        await matchRepo.UpdateAsync(match, ct);

        var response = MatchMapper.ToResponse(match);
        await notifier.TeamRevealed(match.Id, response);
        return response;
    }
}
