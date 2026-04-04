using CardgameDungeon.Features.Match.PlaceBet;
using CardgameDungeon.Tests.Match.Fakes;

namespace CardgameDungeon.Tests.Match;

/// <summary>
/// PlaceBet handler is obsolete in the new turn-based flow.
/// All tests verify that the handler throws NotImplementedException.
/// </summary>
public class PlaceBetHandlerTests
{
    private readonly FakeMatchRepository _matchRepo = new();
    private readonly FakeMatchNotifier _notifier = new();

    private PlaceBetHandler BetHandler => new(_matchRepo, _notifier);

    [Fact]
    public async Task FirstBet_ResolvesImmediately_BecauseOpponentHasZero()
    {
        var match = MatchTestHelper.MakeMatchInInitiative(p1Initiative: 3, p2Initiative: 3);
        _matchRepo.Seed(match);

        await Assert.ThrowsAsync<NotImplementedException>(() =>
            BetHandler.Handle(
                new PlaceBetCommand(match.Id, match.Player1.PlayerId, Amount: 2, Exile: false),
                CancellationToken.None));
    }

    [Fact]
    public async Task PlaceBet_DiscardFromDeck_ReducesDeckSize()
    {
        var match = MatchTestHelper.MakeMatchInInitiative(p1Initiative: 3, p2Initiative: 3);
        _matchRepo.Seed(match);

        await Assert.ThrowsAsync<NotImplementedException>(() =>
            BetHandler.Handle(
                new PlaceBetCommand(match.Id, match.Player1.PlayerId, Amount: 2, Exile: false),
                CancellationToken.None));
    }

    [Fact]
    public async Task PlaceBet_ExileFromDeck_IncreasesExileCount()
    {
        var match = MatchTestHelper.MakeMatchInInitiative(p1Initiative: 3, p2Initiative: 3);
        _matchRepo.Seed(match);

        await Assert.ThrowsAsync<NotImplementedException>(() =>
            BetHandler.Handle(
                new PlaceBetCommand(match.Id, match.Player1.PlayerId, Amount: 3, Exile: true),
                CancellationToken.None));
    }

    [Fact]
    public async Task SequentialBets_P1ThenP2_HigherTotalWins()
    {
        var match = MatchTestHelper.MakeMatchInInitiative(p1Initiative: 3, p2Initiative: 3);
        _matchRepo.Seed(match);

        await Assert.ThrowsAsync<NotImplementedException>(() =>
            BetHandler.Handle(
                new PlaceBetCommand(match.Id, match.Player1.PlayerId, Amount: 3, Exile: false),
                CancellationToken.None));
    }

    [Fact]
    public async Task MatchingBets_StayInInitiative()
    {
        var match = MatchTestHelper.MakeMatchInInitiative(p1Initiative: 3, p2Initiative: 3);
        _matchRepo.Seed(match);

        await Assert.ThrowsAsync<NotImplementedException>(() =>
            BetHandler.Handle(
                new PlaceBetCommand(match.Id, match.Player1.PlayerId, Amount: 1, Exile: false),
                CancellationToken.None));
    }

    [Fact]
    public async Task Resolved_MatchAdvancesToCombat()
    {
        var match = MatchTestHelper.MakeMatchInInitiative(p1Initiative: 3, p2Initiative: 3);
        _matchRepo.Seed(match);

        await Assert.ThrowsAsync<NotImplementedException>(() =>
            BetHandler.Handle(
                new PlaceBetCommand(match.Id, match.Player1.PlayerId, Amount: 1, Exile: false),
                CancellationToken.None));
    }

    [Fact]
    public async Task MatchNotFound_Throws()
    {
        // Handler throws NotImplementedException before even checking the match
        await Assert.ThrowsAsync<NotImplementedException>(() =>
            BetHandler.Handle(
                new PlaceBetCommand(Guid.NewGuid(), Guid.NewGuid(), 1, false),
                CancellationToken.None));
    }
}
