using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Features.Wallet.AddFunds;
using CardgameDungeon.Tests.MetaSystems.Fakes;

namespace CardgameDungeon.Tests.MetaSystems;

public class AddFundsHandlerTests
{
    private readonly FakeWalletRepository _walletRepo = new();
    private AddFundsHandler Handler => new(_walletRepo);

    [Fact]
    public async Task RealMoneyPurchase_AddsBalance()
    {
        var playerId = Guid.NewGuid();
        _walletRepo.Seed(new PlayerWallet(playerId, 100));

        var response = await Handler.Handle(
            new AddFundsCommand(playerId, 500, FundSource.RealMoneyPurchase),
            CancellationToken.None);

        Assert.Equal(600, response.NewBalance);
    }

    [Fact]
    public async Task DailyReward_AddsBalance()
    {
        var playerId = Guid.NewGuid();
        _walletRepo.Seed(new PlayerWallet(playerId, 0));

        var response = await Handler.Handle(
            new AddFundsCommand(playerId, 50, FundSource.DailyReward),
            CancellationToken.None);

        Assert.Equal(50, response.NewBalance);
    }

    [Fact]
    public async Task DailyReward_SecondClaimSameDay_Throws()
    {
        var playerId = Guid.NewGuid();
        var wallet = new PlayerWallet(playerId, 0);
        wallet.AddFunds(50, FundSource.DailyReward, DateOnly.FromDateTime(DateTime.UtcNow));
        _walletRepo.Seed(wallet);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Handler.Handle(
                new AddFundsCommand(playerId, 50, FundSource.DailyReward),
                CancellationToken.None));
    }

    [Fact]
    public async Task EventPrize_AddsBalance()
    {
        var playerId = Guid.NewGuid();
        _walletRepo.Seed(new PlayerWallet(playerId, 200));

        var response = await Handler.Handle(
            new AddFundsCommand(playerId, 300, FundSource.EventPrize),
            CancellationToken.None);

        Assert.Equal(500, response.NewBalance);
    }

    [Fact]
    public async Task WalletNotFound_Throws()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            Handler.Handle(
                new AddFundsCommand(Guid.NewGuid(), 100, FundSource.RealMoneyPurchase),
                CancellationToken.None));
    }
}
