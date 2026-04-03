using CardgameDungeon.Domain.Repositories;
using CardgameDungeon.Features.Match.Shared;
using MediatR;

namespace CardgameDungeon.Features.Match.GetMatchState;

public class GetMatchStateHandler(IMatchRepository matchRepo)
    : IRequestHandler<GetMatchStateQuery, MatchResponse>
{
    public async Task<MatchResponse> Handle(GetMatchStateQuery request, CancellationToken ct)
    {
        var match = await matchRepo.GetByIdAsync(request.MatchId, ct)
            ?? throw new KeyNotFoundException($"Match {request.MatchId} not found.");

        return MatchMapper.ToResponse(match);
    }
}
