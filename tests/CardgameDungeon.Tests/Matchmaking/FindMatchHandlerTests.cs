using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Features.Matchmaking.FindMatch;
using CardgameDungeon.Tests.Deck;
using CardgameDungeon.Tests.Deck.Fakes;
using CardgameDungeon.Tests.Match.Fakes;
using CardgameDungeon.Tests.Matchmaking.Fakes;

namespace CardgameDungeon.Tests.Matchmaking;

public class FindMatchHandlerTests
{
    private readonly FakeQueueRepository _queueRepo = new();
    private readonly FakeDeckRepository _deckRepo = new();
    private readonly FakeMatchRepository _matchRepo = new();
    private FindMatchHandler Handler => new(_queueRepo, _deckRepo, _matchRepo);

    [Fact]
    public async Task TwoPlayersInCasual_MatchFound()
    {
        var deck1 = DeckTestHelper.MakeValidDeck();
        var deck2 = DeckTestHelper.MakeValidDeck();
        _deckRepo.Seed(deck1);
        _deckRepo.Seed(deck2);

        var p1 = Guid.NewGuid();
        var p2 = Guid.NewGuid();
        _queueRepo.Seed(new QueueEntry(p1, deck1.Id, QueueType.Casual, 1000));
        _queueRepo.Seed(new QueueEntry(p2, deck2.Id, QueueType.Casual, 500));

        var response = await Handler.Handle(new FindMatchCommand(QueueType.Casual), CancellationToken.None);

        Assert.True(response.MatchFound);
        Assert.NotNull(response.MatchId);
        Assert.Equal(0, _queueRepo.Count); // Both removed from queue
    }

    [Fact]
    public async Task RankedInRange_MatchFound()
    {
        var deck1 = DeckTestHelper.MakeValidDeck();
        var deck2 = DeckTestHelper.MakeValidDeck();
        _deckRepo.Seed(deck1);
        _deckRepo.Seed(deck2);

        _queueRepo.Seed(new QueueEntry(Guid.NewGuid(), deck1.Id, QueueType.Ranked, 1000));
        _queueRepo.Seed(new QueueEntry(Guid.NewGuid(), deck2.Id, QueueType.Ranked, 1050));

        var response = await Handler.Handle(new FindMatchCommand(QueueType.Ranked), CancellationToken.None);

        Assert.True(response.MatchFound);
    }

    [Fact]
    public async Task OnlyOnePlayer_NoMatch()
    {
        var deck = DeckTestHelper.MakeValidDeck();
        _deckRepo.Seed(deck);
        _queueRepo.Seed(new QueueEntry(Guid.NewGuid(), deck.Id, QueueType.Ranked, 1000));

        var response = await Handler.Handle(new FindMatchCommand(QueueType.Ranked), CancellationToken.None);

        Assert.False(response.MatchFound);
    }

    [Fact]
    public async Task EmptyQueue_NoMatch()
    {
        var response = await Handler.Handle(new FindMatchCommand(QueueType.Casual), CancellationToken.None);

        Assert.False(response.MatchFound);
    }
}
