using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Features.Collection.GetCollection;
using CardgameDungeon.Tests.MetaSystems.Fakes;

namespace CardgameDungeon.Tests.MetaSystems;

public class GetCollectionHandlerTests
{
    private readonly FakeCollectionRepository _collectionRepo = new();
    private GetCollectionHandler Handler => new(_collectionRepo);

    [Fact]
    public async Task ExistingCollection_ReturnsCards()
    {
        var playerId = Guid.NewGuid();
        var collection = new PlayerCollection(playerId);
        collection.AddCard(Guid.NewGuid());
        collection.AddCard(Guid.NewGuid());
        _collectionRepo.Seed(collection);

        var response = await Handler.Handle(new GetCollectionQuery(playerId), CancellationToken.None);

        Assert.Equal(playerId, response.PlayerId);
        Assert.Equal(2, response.TotalCards);
        Assert.Equal(2, response.AvailableCards);
    }

    [Fact]
    public async Task ReservedCards_NotCountedAsAvailable()
    {
        var playerId = Guid.NewGuid();
        var collection = new PlayerCollection(playerId);
        var card1 = collection.AddCard(Guid.NewGuid());
        collection.AddCard(Guid.NewGuid());
        card1.Reserve();
        _collectionRepo.Seed(collection);

        var response = await Handler.Handle(new GetCollectionQuery(playerId), CancellationToken.None);

        Assert.Equal(2, response.TotalCards);
        Assert.Equal(1, response.AvailableCards);
    }

    [Fact]
    public async Task PlayerNotFound_Throws()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            Handler.Handle(new GetCollectionQuery(Guid.NewGuid()), CancellationToken.None));
    }
}
