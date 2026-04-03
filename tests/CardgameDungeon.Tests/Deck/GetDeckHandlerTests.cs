using CardgameDungeon.Features.Deck.GetDeck;
using CardgameDungeon.Tests.Deck.Fakes;

namespace CardgameDungeon.Tests.Deck;

public class GetDeckHandlerTests
{
    private readonly FakeDeckRepository _deckRepo = new();

    private GetDeckHandler Handler => new(_deckRepo);

    [Fact]
    public async Task ExistingDeck_ReturnsResponse()
    {
        var deck = DeckTestHelper.MakeValidDeck();
        _deckRepo.Seed(deck);

        var response = await Handler.Handle(new GetDeckQuery(deck.Id), CancellationToken.None);

        Assert.Equal(deck.Id, response.Id);
        Assert.Equal(deck.PlayerId, response.PlayerId);
        Assert.Equal(40, response.AdventurerCards.Count);
        Assert.Equal(40, response.EnemyCards.Count);
        Assert.Equal(5, response.DungeonRooms.Count);
    }

    [Fact]
    public async Task NonExistentDeck_Throws()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => Handler.Handle(new GetDeckQuery(Guid.NewGuid()), CancellationToken.None));
    }
}
