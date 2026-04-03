using CardgameDungeon.Domain.Repositories;
using MediatR;

namespace CardgameDungeon.Features.Elo.GetPlayerRank;

public class GetPlayerRankHandler(IRatingRepository ratingRepo)
    : IRequestHandler<GetPlayerRankQuery, PlayerRankResponse>
{
    public async Task<PlayerRankResponse> Handle(GetPlayerRankQuery request, CancellationToken ct)
    {
        var rating = await ratingRepo.GetByPlayerIdAsync(request.PlayerId, ct)
            ?? throw new KeyNotFoundException($"Rating for player {request.PlayerId} not found.");

        var allRatings = await ratingRepo.GetAllActiveAsync(ct);
        var rank = allRatings.Count(r => r.Elo > rating.Elo) + 1;

        return new PlayerRankResponse(
            rating.PlayerId, rating.Elo, rating.Tier,
            rating.Wins, rating.Losses, rank);
    }
}
