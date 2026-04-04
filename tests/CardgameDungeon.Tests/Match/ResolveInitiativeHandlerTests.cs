using CardgameDungeon.Domain.Services;
using CardgameDungeon.Features.Match.ResolveInitiative;
using CardgameDungeon.Tests.Match.Fakes;

namespace CardgameDungeon.Tests.Match;

/// <summary>
/// ResolveInitiative handler is obsolete in the new turn-based flow.
/// All tests verify that the handler throws NotImplementedException.
/// </summary>
public class ResolveInitiativeHandlerTests
{
    private readonly FakeMatchRepository _matchRepo = new();
    private readonly CombatResolver _combatResolver = new();
    private readonly FakeMatchNotifier _notifier = new();

    private ResolveInitiativeHandler Handler => new(_matchRepo, _combatResolver, _notifier);

    [Fact]
    public async Task DifferentInitiative_ReturnsWinner()
    {
        await Assert.ThrowsAsync<NotImplementedException>(() =>
            Handler.Handle(
                new ResolveInitiativeCommand(Guid.NewGuid()),
                CancellationToken.None));
    }

    [Fact]
    public async Task EqualInitiative_ReturnsTied()
    {
        await Assert.ThrowsAsync<NotImplementedException>(() =>
            Handler.Handle(
                new ResolveInitiativeCommand(Guid.NewGuid()),
                CancellationToken.None));
    }

    [Fact]
    public async Task Player2HigherInitiative_Player2Wins()
    {
        await Assert.ThrowsAsync<NotImplementedException>(() =>
            Handler.Handle(
                new ResolveInitiativeCommand(Guid.NewGuid()),
                CancellationToken.None));
    }

    [Fact]
    public async Task MatchNotFound_Throws()
    {
        await Assert.ThrowsAsync<NotImplementedException>(() =>
            Handler.Handle(
                new ResolveInitiativeCommand(Guid.NewGuid()),
                CancellationToken.None));
    }
}
