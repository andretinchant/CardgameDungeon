using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Features.Elo.GetPlayerRank;
using CardgameDungeon.Tests.Matchmaking.Fakes;

namespace CardgameDungeon.Tests.Matchmaking;

public class GetPlayerRankHandlerTests
{
    private readonly FakeRatingRepository _ratingRepo = new();
    private GetPlayerRankHandler Handler => new(_ratingRepo);

    [Fact]
    public async Task ExistingPlayer_ReturnsRank()
    {
        var playerId = Guid.NewGuid();
        var rating = new PlayerRating(playerId, 1200);
        rating.SetTier(Tier.Silver);
        _ratingRepo.Seed(rating);

        // Add other players for ranking
        _ratingRepo.Seed(new PlayerRating(Guid.NewGuid(), 1500));
        _ratingRepo.Seed(new PlayerRating(Guid.NewGuid(), 900));

        var response = await Handler.Handle(new GetPlayerRankQuery(playerId), CancellationToken.None);

        Assert.Equal(1200, response.Elo);
        Assert.Equal(Tier.Silver, response.Tier);
        Assert.Equal(2, response.Rank); // 1 player above at 1500
    }

    [Fact]
    public async Task TopPlayer_Rank1()
    {
        var playerId = Guid.NewGuid();
        _ratingRepo.Seed(new PlayerRating(playerId, 2000));
        _ratingRepo.Seed(new PlayerRating(Guid.NewGuid(), 1000));

        var response = await Handler.Handle(new GetPlayerRankQuery(playerId), CancellationToken.None);

        Assert.Equal(1, response.Rank);
    }

    [Fact]
    public async Task PlayerNotFound_Throws()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            Handler.Handle(new GetPlayerRankQuery(Guid.NewGuid()), CancellationToken.None));
    }
}
