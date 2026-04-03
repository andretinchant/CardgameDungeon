using CardgameDungeon.Domain.Services;
using CardgameDungeon.Features.Match.Combat.ResolveOpportunityAttack;
using CardgameDungeon.Tests.Match.Fakes;

namespace CardgameDungeon.Tests.Match.Combat;

public class ResolveOpportunityAttackHandlerTests
{
    private readonly FakeMatchRepository _matchRepo = new();
    private readonly CombatResolver _combatResolver = new();
    private readonly FakeMatchNotifier _notifier = new();

    private ResolveOpportunityAttackHandler Handler => new(_matchRepo, _combatResolver, _notifier);

    [Fact]
    public async Task ValidOpportunityAttack_ReturnsDamage()
    {
        var match = MatchTestHelper.MakeMatchInCombat(p1Strength: 7);
        _matchRepo.Seed(match);

        var attacker = match.GetAttacker();
        var defender = match.GetDefender();

        var response = await Handler.Handle(
            new ResolveOpportunityAttackCommand(
                match.Id,
                attacker.PlayerId,
                attacker.AlliesInPlay[0].Id,
                defender.AlliesInPlay[0].Id),
            CancellationToken.None);

        Assert.Equal(7, response.Damage);
    }

    [Fact]
    public async Task SecondOpportunityAttack_SamePlayer_Throws()
    {
        var match = MatchTestHelper.MakeMatchInCombat();
        _matchRepo.Seed(match);

        var attacker = match.GetAttacker();
        var defender = match.GetDefender();

        await Handler.Handle(
            new ResolveOpportunityAttackCommand(
                match.Id,
                attacker.PlayerId,
                attacker.AlliesInPlay[0].Id,
                defender.AlliesInPlay[0].Id),
            CancellationToken.None);

        // Same player trying again
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Handler.Handle(
                new ResolveOpportunityAttackCommand(
                    match.Id,
                    attacker.PlayerId,
                    attacker.AlliesInPlay[1].Id,
                    defender.AlliesInPlay[1].Id),
                CancellationToken.None));
    }

    [Fact]
    public async Task DifferentPlayers_BothCanUseOpportunityAttack()
    {
        var match = MatchTestHelper.MakeMatchInCombat();
        _matchRepo.Seed(match);

        var p1 = match.Player1;
        var p2 = match.Player2;

        // P1 attacks fleeing P2 ally
        await Handler.Handle(
            new ResolveOpportunityAttackCommand(
                match.Id, p1.PlayerId,
                p1.AlliesInPlay[0].Id,
                p2.AlliesInPlay[0].Id),
            CancellationToken.None);

        // P2 attacks fleeing P1 ally
        var response = await Handler.Handle(
            new ResolveOpportunityAttackCommand(
                match.Id, p2.PlayerId,
                p2.AlliesInPlay[1].Id,
                p1.AlliesInPlay[1].Id),
            CancellationToken.None);

        Assert.True(response.Damage > 0);
    }

    [Fact]
    public async Task AmbusherTargeted_WithNonAmbusherAvailable_Throws()
    {
        var match = MatchTestHelper.MakeMatchInCombatWithAmbusher();
        _matchRepo.Seed(match);

        var attacker = match.GetAttacker();
        var defender = match.GetDefender();
        var ambusher = defender.AlliesInPlay.First(a => a.IsAmbusher);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Handler.Handle(
                new ResolveOpportunityAttackCommand(
                    match.Id,
                    attacker.PlayerId,
                    attacker.AlliesInPlay[0].Id,
                    ambusher.Id),
                CancellationToken.None));
    }

    [Fact]
    public async Task MatchNotFound_Throws()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            Handler.Handle(
                new ResolveOpportunityAttackCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()),
                CancellationToken.None));
    }
}
