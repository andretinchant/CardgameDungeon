using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Features.Marketplace.ListCard;
using CardgameDungeon.Tests.MetaSystems.Fakes;

namespace CardgameDungeon.Tests.MetaSystems;

public class ListCardHandlerTests
{
    private readonly FakeCollectionRepository _collectionRepo = new();
    private readonly FakeMarketplaceRepository _marketRepo = new();
    private ListCardHandler Handler => new(_collectionRepo, _marketRepo);

    [Fact]
    public async Task ValidListing_ReservesCardAndCreatesListing()
    {
        var sellerId = Guid.NewGuid();
        var collection = new PlayerCollection(sellerId);
        var owned = collection.AddCard(Guid.NewGuid());
        _collectionRepo.Seed(collection);

        var response = await Handler.Handle(
            new ListCardCommand(sellerId, owned.Id, 100), CancellationToken.None);

        Assert.True(response.IsActive);
        Assert.Equal(100, response.Price);
        Assert.Equal(10, response.Fee); // 10% of 100
        Assert.True(owned.IsReserved);
    }

    [Fact]
    public async Task AlreadyReservedCard_Throws()
    {
        var sellerId = Guid.NewGuid();
        var collection = new PlayerCollection(sellerId);
        var owned = collection.AddCard(Guid.NewGuid());
        owned.Reserve();
        _collectionRepo.Seed(collection);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Handler.Handle(new ListCardCommand(sellerId, owned.Id, 100), CancellationToken.None));
    }

    [Fact]
    public async Task CardNotInCollection_Throws()
    {
        var sellerId = Guid.NewGuid();
        _collectionRepo.Seed(new PlayerCollection(sellerId));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Handler.Handle(new ListCardCommand(sellerId, Guid.NewGuid(), 100), CancellationToken.None));
    }

    [Fact]
    public async Task FeeCalculation_10Percent()
    {
        var sellerId = Guid.NewGuid();
        var collection = new PlayerCollection(sellerId);
        var owned = collection.AddCard(Guid.NewGuid());
        _collectionRepo.Seed(collection);

        var response = await Handler.Handle(
            new ListCardCommand(sellerId, owned.Id, 250), CancellationToken.None);

        Assert.Equal(25, response.Fee); // ceil(250 * 0.10) = 25
    }
}
