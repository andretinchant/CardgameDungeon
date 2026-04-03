using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Features.Matchmaking.JoinQueue;
using CardgameDungeon.Tests.Deck;
using CardgameDungeon.Tests.Deck.Fakes;
using CardgameDungeon.Tests.Matchmaking.Fakes;

namespace CardgameDungeon.Tests.Matchmaking;

public class JoinQueueHandlerTests
{
    private readonly FakeQueueRepository _queueRepo = new();
    private readonly FakeDeckRepository _deckRepo = new();
    private readonly FakeRatingRepository _ratingRepo = new();
    private JoinQueueHandler Handler => new(_queueRepo, _deckRepo, _ratingRepo);

    [Fact]
    public async Task ValidJoin_EntersQueue()
    {
        var playerId = Guid.NewGuid();
        var deck = DeckTestHelper.MakeValidDeck();
        _deckRepo.Seed(deck);

        var response = await Handler.Handle(
            new JoinQueueCommand(playerId, deck.Id, QueueType.Ranked), CancellationToken.None);

        Assert.True(response.Joined);
        Assert.Equal(QueueType.Ranked, response.QueueType);
    }

    [Fact]
    public async Task AlreadyInQueue_Throws()
    {
        var playerId = Guid.NewGuid();
        var deck = DeckTestHelper.MakeValidDeck();
        _deckRepo.Seed(deck);
        _queueRepo.Seed(new QueueEntry(playerId, deck.Id, QueueType.Ranked, 1000));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Handler.Handle(new JoinQueueCommand(playerId, deck.Id, QueueType.Ranked), CancellationToken.None));
    }

    [Fact]
    public async Task DeckNotFound_Throws()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            Handler.Handle(new JoinQueueCommand(Guid.NewGuid(), Guid.NewGuid(), QueueType.Casual), CancellationToken.None));
    }

    [Fact]
    public async Task UsesPlayerElo_WhenRatingExists()
    {
        var playerId = Guid.NewGuid();
        var deck = DeckTestHelper.MakeValidDeck();
        _deckRepo.Seed(deck);
        _ratingRepo.Seed(new PlayerRating(playerId, 1500));

        await Handler.Handle(new JoinQueueCommand(playerId, deck.Id, QueueType.Ranked), CancellationToken.None);

        var entry = await _queueRepo.GetByPlayerIdAsync(playerId);
        Assert.Equal(1500, entry!.Elo);
    }
}
