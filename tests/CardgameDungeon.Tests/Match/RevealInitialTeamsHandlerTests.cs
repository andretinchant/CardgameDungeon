using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Features.Match.RevealInitialTeams;
using CardgameDungeon.Tests.Match.Fakes;

namespace CardgameDungeon.Tests.Match;

public class RevealInitialTeamsHandlerTests
{
    private readonly FakeMatchRepository _matchRepo = new();

    private RevealInitialTeamsHandler Handler => new(_matchRepo);

    [Fact]
    public async Task BothTeamsSubmitted_RevealsAndAdvancesToInitiative()
    {
        var match = MatchTestHelper.MakeMatchInSetup();
        _matchRepo.Seed(match);

        // Submit both teams
        var p1Allies = match.Player1.Hand.OfType<AllyCard>().Take(5).ToList();
        var p2Allies = match.Player2.Hand.OfType<AllyCard>().Take(5).ToList();
        match.SubmitSetupTeam(match.Player1.PlayerId, p1Allies);
        match.SubmitSetupTeam(match.Player2.PlayerId, p2Allies);

        var response = await Handler.Handle(
            new RevealInitialTeamsCommand(match.Id),
            CancellationToken.None);

        // After reveal + room reveal, should be in Initiative (rooms have monsters)
        Assert.Equal(MatchPhase.Initiative, response.Phase);
        Assert.Equal(5, response.Player1.AlliesInPlay.Count);
        Assert.Equal(5, response.Player2.AlliesInPlay.Count);
    }

    [Fact]
    public async Task OnlyOneTeamSubmitted_Throws()
    {
        var match = MatchTestHelper.MakeMatchInSetup();
        _matchRepo.Seed(match);

        var p1Allies = match.Player1.Hand.OfType<AllyCard>().Take(5).ToList();
        match.SubmitSetupTeam(match.Player1.PlayerId, p1Allies);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Handler.Handle(
                new RevealInitialTeamsCommand(match.Id),
                CancellationToken.None));
    }

    [Fact]
    public async Task MatchNotFound_Throws()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            Handler.Handle(
                new RevealInitialTeamsCommand(Guid.NewGuid()),
                CancellationToken.None));
    }
}
