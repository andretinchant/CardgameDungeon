using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Domain.Repositories;
using MediatR;

namespace CardgameDungeon.Features.Elo.RecalculateTiers;

public class RecalculateTiersHandler(IRatingRepository ratingRepo)
    : IRequestHandler<RecalculateTiersCommand, RecalculateTiersResponse>
{
    // Bronze 50%, Silver 30%, Gold 20%
    private const double BronzePercentile = 0.50;
    private const double SilverPercentile = 0.80; // bottom 80% = bronze + silver

    public async Task<RecalculateTiersResponse> Handle(RecalculateTiersCommand request, CancellationToken ct)
    {
        var allRatings = await ratingRepo.GetAllActiveAsync(ct);
        if (allRatings.Count == 0)
            return new RecalculateTiersResponse(0, 0, 0, 0, 0, 0);

        var sorted = allRatings.OrderBy(r => r.Elo).ToList();
        var bronzeIndex = (int)(sorted.Count * BronzePercentile);
        var silverIndex = (int)(sorted.Count * SilverPercentile);

        var bronzeCutoff = sorted[Math.Min(bronzeIndex, sorted.Count - 1)].Elo;
        var goldCutoff = sorted[Math.Min(silverIndex, sorted.Count - 1)].Elo;

        int bronzeCount = 0, silverCount = 0, goldCount = 0;

        foreach (var rating in sorted)
        {
            if (rating.Elo < bronzeCutoff)
            {
                rating.SetTier(Tier.Bronze);
                bronzeCount++;
            }
            else if (rating.Elo < goldCutoff)
            {
                rating.SetTier(Tier.Silver);
                silverCount++;
            }
            else
            {
                rating.SetTier(Tier.Gold);
                goldCount++;
            }
        }

        await ratingRepo.UpdateManyAsync(sorted, ct);

        return new RecalculateTiersResponse(
            sorted.Count, bronzeCount, silverCount, goldCount, bronzeCutoff, goldCutoff);
    }
}
