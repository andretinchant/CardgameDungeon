using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Features.Match.SetupInitialTeam;
using CardgameDungeon.Tests.Match.Fakes;

namespace CardgameDungeon.Tests.Match;

public class SetupInitialTeamHandlerTests
{
    private readonly FakeMatchRepository _matchRepo = new();
    private readonly FakeMatchNotifier _notifier = new();

    private SetupInitialTeamHandler Handler => new(_matchRepo, _notifier);

    [Fact]
    public async Task ValidTeam_SubmitsSuccessfully()
    {
        var match = MatchTestHelper.MakeMatchInSetup();
        _matchRepo.Seed(match);

        var p1Id = match.Player1.PlayerId;
        var allyIds = match.Player1.Hand.OfType<AllyCard>().Take(5).Select(a => a.Id).ToList();

        var response = await Handler.Handle(
            new SetupInitialTeamCommand(match.Id, p1Id, allyIds),
            CancellationToken.None);

        Assert.True(response.Submitted);
        Assert.False(response.BothReady); // Only one player submitted
    }

    [Fact]
    public async Task BothPlayersSubmit_BothReady()
    {
        var match = MatchTestHelper.MakeMatchInSetup();
        _matchRepo.Seed(match);

        var p1Allies = match.Player1.Hand.OfType<AllyCard>().Take(5).Select(a => a.Id).ToList();
        var p2Allies = match.Player2.Hand.OfType<AllyCard>().Take(5).Select(a => a.Id).ToList();

        await Handler.Handle(
            new SetupInitialTeamCommand(match.Id, match.Player1.PlayerId, p1Allies),
            CancellationToken.None);

        var response = await Handler.Handle(
            new SetupInitialTeamCommand(match.Id, match.Player2.PlayerId, p2Allies),
            CancellationToken.None);

        Assert.True(response.BothReady);
    }

    [Fact]
    public async Task CostExceedsMax_Throws()
    {
        var match = MatchTestHelper.MakeMatchInSetup();
        _matchRepo.Seed(match);

        // Select 6 allies (cost=6, exceeds max of 5)
        var allyIds = match.Player1.Hand.OfType<AllyCard>().Take(6).Select(a => a.Id).ToList();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Handler.Handle(
                new SetupInitialTeamCommand(match.Id, match.Player1.PlayerId, allyIds),
                CancellationToken.None));
    }

    [Fact]
    public async Task DuplicateSubmission_Throws()
    {
        var match = MatchTestHelper.MakeMatchInSetup();
        _matchRepo.Seed(match);

        var allyIds = match.Player1.Hand.OfType<AllyCard>().Take(5).Select(a => a.Id).ToList();

        await Handler.Handle(
            new SetupInitialTeamCommand(match.Id, match.Player1.PlayerId, allyIds),
            CancellationToken.None);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Handler.Handle(
                new SetupInitialTeamCommand(match.Id, match.Player1.PlayerId, allyIds),
                CancellationToken.None));
    }

    [Fact]
    public async Task AllyNotInHand_Throws()
    {
        var match = MatchTestHelper.MakeMatchInSetup();
        _matchRepo.Seed(match);

        var fakeIds = Enumerable.Range(0, 5).Select(_ => Guid.NewGuid()).ToList();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Handler.Handle(
                new SetupInitialTeamCommand(match.Id, match.Player1.PlayerId, fakeIds),
                CancellationToken.None));
    }

    [Fact]
    public async Task MatchNotFound_Throws()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            Handler.Handle(
                new SetupInitialTeamCommand(Guid.NewGuid(), Guid.NewGuid(), [Guid.NewGuid()]),
                CancellationToken.None));
    }
}
