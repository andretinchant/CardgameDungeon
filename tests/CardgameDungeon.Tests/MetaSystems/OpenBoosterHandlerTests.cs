using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Features.Collection.OpenBooster;
using CardgameDungeon.Tests.MetaSystems.Fakes;

namespace CardgameDungeon.Tests.MetaSystems;

public class OpenBoosterHandlerTests
{
    private readonly FakeCollectionRepository _collectionRepo = new();
    private readonly FakeWalletRepository _walletRepo = new();
    private readonly FakeBoosterCardPool _cardPool = new();

    private OpenBoosterHandler Handler => new(_collectionRepo, _walletRepo, _cardPool);

    [Fact]
    public async Task ValidBooster_Returns10Cards()
    {
        var playerId = Guid.NewGuid();
        _walletRepo.Seed(new PlayerWallet(playerId, 100));
        _collectionRepo.Seed(new PlayerCollection(playerId));

        var response = await Handler.Handle(
            new OpenBoosterCommand(playerId, 50), CancellationToken.None);

        Assert.Equal(10, response.Cards.Count);
    }

    [Fact]
    public async Task BoosterComposition_1Rare3Uncommon6Common()
    {
        var playerId = Guid.NewGuid();
        _walletRepo.Seed(new PlayerWallet(playerId, 100));
        _collectionRepo.Seed(new PlayerCollection(playerId));

        var response = await Handler.Handle(
            new OpenBoosterCommand(playerId, 50), CancellationToken.None);

        Assert.Equal(1, response.Cards.Count(c => c.Rarity == Rarity.Rare));
        Assert.Equal(3, response.Cards.Count(c => c.Rarity == Rarity.Uncommon));
        Assert.Equal(6, response.Cards.Count(c => c.Rarity == Rarity.Common));
    }

    [Fact]
    public async Task BoosterDeductsPrice()
    {
        var playerId = Guid.NewGuid();
        var wallet = new PlayerWallet(playerId, 100);
        _walletRepo.Seed(wallet);
        _collectionRepo.Seed(new PlayerCollection(playerId));

        await Handler.Handle(new OpenBoosterCommand(playerId, 50), CancellationToken.None);

        Assert.Equal(50, wallet.Balance);
    }

    [Fact]
    public async Task CardsAddedToCollection()
    {
        var playerId = Guid.NewGuid();
        _walletRepo.Seed(new PlayerWallet(playerId, 100));
        var collection = new PlayerCollection(playerId);
        _collectionRepo.Seed(collection);

        await Handler.Handle(new OpenBoosterCommand(playerId, 50), CancellationToken.None);

        Assert.Equal(10, collection.Cards.Count);
    }

    [Fact]
    public async Task InsufficientBalance_Throws()
    {
        var playerId = Guid.NewGuid();
        _walletRepo.Seed(new PlayerWallet(playerId, 10));
        _collectionRepo.Seed(new PlayerCollection(playerId));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Handler.Handle(new OpenBoosterCommand(playerId, 50), CancellationToken.None));
    }
}
