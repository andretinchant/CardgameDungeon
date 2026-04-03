using CardgameDungeon.Domain.Services;
using CardgameDungeon.Features.Match.ResolveInitiative;
using CardgameDungeon.Tests.Match.Fakes;

namespace CardgameDungeon.Tests.Match;

public class ResolveInitiativeHandlerTests
{
    private readonly FakeMatchRepository _matchRepo = new();
    private readonly CombatResolver _combatResolver = new();
    private readonly FakeMatchNotifier _notifier = new();

    private ResolveInitiativeHandler Handler => new(_matchRepo, _combatResolver, _notifier);

    [Fact]
    public async Task DifferentInitiative_ReturnsWinner()
    {
        var match = MatchTestHelper.MakeMatchInInitiative(p1Initiative: 5, p2Initiative: 2);
        _matchRepo.Seed(match);

        var response = await Handler.Handle(
            new ResolveInitiativeCommand(match.Id),
            CancellationToken.None);

        Assert.False(response.IsTied);
        Assert.Equal(match.Player1.PlayerId, response.WinnerId);
        Assert.True(response.Player1Total > response.Player2Total);
    }

    [Fact]
    public async Task EqualInitiative_ReturnsTied()
    {
        var match = MatchTestHelper.MakeMatchInInitiative(p1Initiative: 3, p2Initiative: 3);
        _matchRepo.Seed(match);

        var response = await Handler.Handle(
            new ResolveInitiativeCommand(match.Id),
            CancellationToken.None);

        Assert.True(response.IsTied);
        Assert.Null(response.WinnerId);
    }

    [Fact]
    public async Task Player2HigherInitiative_Player2Wins()
    {
        var match = MatchTestHelper.MakeMatchInInitiative(p1Initiative: 1, p2Initiative: 8);
        _matchRepo.Seed(match);

        var response = await Handler.Handle(
            new ResolveInitiativeCommand(match.Id),
            CancellationToken.None);

        Assert.Equal(match.Player2.PlayerId, response.WinnerId);
    }

    [Fact]
    public async Task MatchNotFound_Throws()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            Handler.Handle(
                new ResolveInitiativeCommand(Guid.NewGuid()),
                CancellationToken.None));
    }
}
