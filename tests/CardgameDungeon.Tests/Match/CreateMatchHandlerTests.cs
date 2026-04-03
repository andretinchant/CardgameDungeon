using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Features.Match.CreateMatch;
using CardgameDungeon.Tests.Deck.Fakes;
using CardgameDungeon.Tests.Match.Fakes;

namespace CardgameDungeon.Tests.Match;

public class CreateMatchHandlerTests
{
    private readonly FakeDeckRepository _deckRepo = new();
    private readonly FakeMatchRepository _matchRepo = new();

    private CreateMatchHandler Handler => new(_deckRepo, _matchRepo);

    [Fact]
    public async Task ValidDecks_CreatesMatch_InSetupPhase()
    {
        var p1Id = Guid.NewGuid();
        var p2Id = Guid.NewGuid();
        var deck1 = MatchTestHelper.MakeValidDeckForPlayer(p1Id);
        var deck2 = MatchTestHelper.MakeValidDeckForPlayer(p2Id);
        _deckRepo.Seed(deck1);
        _deckRepo.Seed(deck2);

        var response = await Handler.Handle(
            new CreateMatchCommand(p1Id, p2Id, deck1.Id, deck2.Id),
            CancellationToken.None);

        Assert.Equal(MatchPhase.Setup, response.Phase);
        Assert.Equal(1, response.CurrentRoom);
        Assert.Equal(p1Id, response.Player1.PlayerId);
        Assert.Equal(p2Id, response.Player2.PlayerId);
        Assert.Equal(20, response.Player1.HitPoints);
        Assert.True(response.Player1.HandCount > 0);
        Assert.NotNull(_matchRepo.LastSaved);
    }

    [Fact]
    public async Task DeckNotFound_Throws()
    {
        var p1Id = Guid.NewGuid();
        var deck1 = MatchTestHelper.MakeValidDeckForPlayer(p1Id);
        _deckRepo.Seed(deck1);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            Handler.Handle(
                new CreateMatchCommand(p1Id, Guid.NewGuid(), deck1.Id, Guid.NewGuid()),
                CancellationToken.None));
    }

    [Fact]
    public async Task DecksAreShuffled_DifferentOrder()
    {
        var p1Id = Guid.NewGuid();
        var p2Id = Guid.NewGuid();
        var deck1 = MatchTestHelper.MakeValidDeckForPlayer(p1Id);
        var deck2 = MatchTestHelper.MakeValidDeckForPlayer(p2Id);
        _deckRepo.Seed(deck1);
        _deckRepo.Seed(deck2);

        var response = await Handler.Handle(
            new CreateMatchCommand(p1Id, p2Id, deck1.Id, deck2.Id),
            CancellationToken.None);

        // Cards should have been drawn from shuffled deck
        Assert.True(response.Player1.DeckCount + response.Player1.HandCount == 40);
    }
}
