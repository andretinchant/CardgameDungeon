using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Features.Collection.GetCollection;
using CardgameDungeon.Tests.Deck.Fakes;
using CardgameDungeon.Tests.MetaSystems.Fakes;

namespace CardgameDungeon.Tests.MetaSystems;

public class GetCollectionHandlerTests
{
    private readonly FakeCollectionRepository _collectionRepo = new();
    private readonly FakeCardRepository _cardRepo = new();
    private GetCollectionHandler Handler => new(_collectionRepo, _cardRepo);

    [Fact]
    public async Task ExistingCollection_ReturnsCards()
    {
        var playerId = Guid.NewGuid();
        var allyId = Guid.NewGuid();
        var monsterId = Guid.NewGuid();

        _cardRepo.Add(new AllyCard(allyId, "Sir Gareth", Domain.Enums.Rarity.Common, 2, 3, 4, 2));
        _cardRepo.Add(new MonsterCard(monsterId, "Bone Ghoul", Domain.Enums.Rarity.Uncommon, 3, 4, 5, 1));

        var collection = new PlayerCollection(playerId);
        collection.AddCard(allyId);
        collection.AddCard(monsterId);
        _collectionRepo.Seed(collection);

        var response = await Handler.Handle(new GetCollectionQuery(playerId), CancellationToken.None);

        Assert.Equal(playerId, response.PlayerId);
        Assert.Equal(2, response.TotalCards);
        Assert.Equal(2, response.AvailableCards);
        Assert.Contains(response.Cards, c => c.CardName == "Sir Gareth" && c.CardType == "Ally");
        Assert.Contains(response.Cards, c => c.CardName == "Bone Ghoul" && c.CardType == "Monster");
    }

    [Fact]
    public async Task ReservedCards_NotCountedAsAvailable()
    {
        var playerId = Guid.NewGuid();
        var firstCardId = Guid.NewGuid();
        var secondCardId = Guid.NewGuid();

        _cardRepo.Add(new AllyCard(firstCardId, "Shield Maiden", Domain.Enums.Rarity.Common, 1, 2, 3, 1));
        _cardRepo.Add(new AllyCard(secondCardId, "Arcane Scout", Domain.Enums.Rarity.Common, 1, 2, 2, 2));

        var collection = new PlayerCollection(playerId);
        var card1 = collection.AddCard(firstCardId);
        collection.AddCard(secondCardId);
        card1.Reserve();
        _collectionRepo.Seed(collection);

        var response = await Handler.Handle(new GetCollectionQuery(playerId), CancellationToken.None);

        Assert.Equal(2, response.TotalCards);
        Assert.Equal(1, response.AvailableCards);
        Assert.Single(response.Cards, c => c.IsReserved);
    }

    [Fact]
    public async Task PlayerNotFound_Throws()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            Handler.Handle(new GetCollectionQuery(Guid.NewGuid()), CancellationToken.None));
    }
}
