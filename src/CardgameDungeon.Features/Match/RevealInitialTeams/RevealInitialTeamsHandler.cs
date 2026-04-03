using CardgameDungeon.Domain.Repositories;
using CardgameDungeon.Features.Match.Shared;
using MediatR;

namespace CardgameDungeon.Features.Match.RevealInitialTeams;

public class RevealInitialTeamsHandler(IMatchRepository matchRepo, IMatchNotifier notifier)
    : IRequestHandler<RevealInitialTeamsCommand, MatchResponse>
{
    public async Task<MatchResponse> Handle(RevealInitialTeamsCommand request, CancellationToken ct)
    {
        var match = await matchRepo.GetByIdAsync(request.MatchId, ct)
            ?? throw new KeyNotFoundException($"Match {request.MatchId} not found.");

        match.RevealTeams();
        match.RevealRoom();

        await matchRepo.UpdateAsync(match, ct);

        var response = MatchMapper.ToResponse(match);
        await notifier.TeamRevealed(match.Id, response);
        return response;
    }
}
