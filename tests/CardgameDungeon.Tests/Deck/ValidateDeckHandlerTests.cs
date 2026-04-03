using CardgameDungeon.Features.Deck.ValidateDeck;
using CardgameDungeon.Tests.Deck.Fakes;

namespace CardgameDungeon.Tests.Deck;

public class ValidateDeckHandlerTests
{
    private readonly FakeDeckRepository _deckRepo = new();

    private ValidateDeckHandler Handler => new(_deckRepo);

    [Fact]
    public async Task ValidDeck_ReturnsIsValid()
    {
        var deck = DeckTestHelper.MakeValidDeck();
        _deckRepo.Seed(deck);

        var response = await Handler.Handle(new ValidateDeckCommand(deck.Id), CancellationToken.None);

        Assert.True(response.IsValid);
        Assert.Empty(response.Errors);
        Assert.Equal(deck.Id, response.DeckId);
    }

    [Fact]
    public async Task NonExistentDeck_Throws()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => Handler.Handle(new ValidateDeckCommand(Guid.NewGuid()), CancellationToken.None));
    }
}
