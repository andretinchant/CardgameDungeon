using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Features.Wallet.GetBalance;
using CardgameDungeon.Tests.MetaSystems.Fakes;

namespace CardgameDungeon.Tests.MetaSystems;

public class GetBalanceHandlerTests
{
    private readonly FakeWalletRepository _walletRepo = new();
    private GetBalanceHandler Handler => new(_walletRepo);

    [Fact]
    public async Task ExistingWallet_ReturnsBalance()
    {
        var playerId = Guid.NewGuid();
        _walletRepo.Seed(new PlayerWallet(playerId, 500));

        var response = await Handler.Handle(new GetBalanceQuery(playerId), CancellationToken.None);

        Assert.Equal(500, response.Balance);
        Assert.Equal(playerId, response.PlayerId);
    }

    [Fact]
    public async Task WalletNotFound_Throws()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            Handler.Handle(new GetBalanceQuery(Guid.NewGuid()), CancellationToken.None));
    }
}
