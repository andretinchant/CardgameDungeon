using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Features.Elo.RecalculateTiers;
using CardgameDungeon.Tests.Matchmaking.Fakes;

namespace CardgameDungeon.Tests.Matchmaking;

public class RecalculateTiersHandlerTests
{
    private readonly FakeRatingRepository _ratingRepo = new();
    private RecalculateTiersHandler Handler => new(_ratingRepo);

    [Fact]
    public async Task TenPlayers_CorrectDistribution()
    {
        // 10 players: bottom 5 Bronze, middle 3 Silver, top 2 Gold
        for (var i = 0; i < 10; i++)
            _ratingRepo.Seed(new PlayerRating(Guid.NewGuid(), 100 * (i + 1)));

        var response = await Handler.Handle(new RecalculateTiersCommand(), CancellationToken.None);

        Assert.Equal(10, response.TotalPlayers);
        Assert.Equal(5, response.BronzeCount);
        Assert.Equal(3, response.SilverCount);
        Assert.Equal(2, response.GoldCount);
    }

    [Fact]
    public async Task TiersAssignedToPlayers()
    {
        var bronzePlayer = new PlayerRating(Guid.NewGuid(), 100);
        var goldPlayer = new PlayerRating(Guid.NewGuid(), 2000);
        _ratingRepo.Seed(bronzePlayer);
        _ratingRepo.Seed(goldPlayer);

        // With 2 players: bottom 50%=1 Bronze, top 20%=1 Gold
        // But with only 2, indices may overlap. Let's add more.
        for (var i = 0; i < 8; i++)
            _ratingRepo.Seed(new PlayerRating(Guid.NewGuid(), 500 + i * 100));

        await Handler.Handle(new RecalculateTiersCommand(), CancellationToken.None);

        Assert.Equal(Tier.Bronze, bronzePlayer.Tier);
        Assert.Equal(Tier.Gold, goldPlayer.Tier);
    }

    [Fact]
    public async Task EmptyPopulation_ReturnsZeros()
    {
        var response = await Handler.Handle(new RecalculateTiersCommand(), CancellationToken.None);

        Assert.Equal(0, response.TotalPlayers);
    }
}
