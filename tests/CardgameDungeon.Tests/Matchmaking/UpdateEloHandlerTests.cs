using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Features.Elo.UpdateElo;
using CardgameDungeon.Tests.Matchmaking.Fakes;

namespace CardgameDungeon.Tests.Matchmaking;

public class UpdateEloHandlerTests
{
    private readonly FakeRatingRepository _ratingRepo = new();
    private UpdateEloHandler Handler => new(_ratingRepo);

    [Fact]
    public async Task WinnerGainsElo_LoserLosesElo()
    {
        var winnerId = Guid.NewGuid();
        var loserId = Guid.NewGuid();
        _ratingRepo.Seed(new PlayerRating(winnerId, 1000));
        _ratingRepo.Seed(new PlayerRating(loserId, 1000));

        var response = await Handler.Handle(
            new UpdateEloCommand(winnerId, loserId), CancellationToken.None);

        Assert.True(response.WinnerDelta > 0);
        Assert.True(response.LoserDelta < 0);
        Assert.True(response.WinnerNewElo > 1000);
        Assert.True(response.LoserNewElo < 1000);
    }

    [Fact]
    public async Task UpsetWin_LargerDelta()
    {
        var underdogId = Guid.NewGuid();
        var favoriteId = Guid.NewGuid();
        _ratingRepo.Seed(new PlayerRating(underdogId, 800));
        _ratingRepo.Seed(new PlayerRating(favoriteId, 1200));

        var response = await Handler.Handle(
            new UpdateEloCommand(underdogId, favoriteId), CancellationToken.None);

        // Underdog wins upset — larger delta
        Assert.True(response.WinnerDelta > 16); // K/2=16 is expected win delta
    }

    [Fact]
    public async Task WinsAndLossesTracked()
    {
        var winnerId = Guid.NewGuid();
        var loserId = Guid.NewGuid();
        _ratingRepo.Seed(new PlayerRating(winnerId, 1000));
        _ratingRepo.Seed(new PlayerRating(loserId, 1000));

        await Handler.Handle(new UpdateEloCommand(winnerId, loserId), CancellationToken.None);

        var winner = await _ratingRepo.GetByPlayerIdAsync(winnerId);
        var loser = await _ratingRepo.GetByPlayerIdAsync(loserId);
        Assert.Equal(1, winner!.Wins);
        Assert.Equal(1, loser!.Losses);
    }

    [Fact]
    public async Task PlayerNotFound_Throws()
    {
        _ratingRepo.Seed(new PlayerRating(Guid.NewGuid(), 1000));

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            Handler.Handle(new UpdateEloCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None));
    }
}
