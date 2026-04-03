using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Features.Match.Combat.AssignCombat;
using CardgameDungeon.Tests.Match.Fakes;

namespace CardgameDungeon.Tests.Match.Combat;

public class AssignCombatHandlerTests
{
    private readonly FakeMatchRepository _matchRepo = new();
    private readonly FakeMatchNotifier _notifier = new();
    private AssignCombatHandler Handler => new(_matchRepo, _notifier);

    [Fact]
    public async Task ValidAssignment_CreatesParings()
    {
        var match = MatchTestHelper.MakeMatchInCombat();
        _matchRepo.Seed(match);

        var attacker = match.GetAttacker();
        var defender = match.GetDefender();

        var pairing = new CombatPairing(
            attacker.AlliesInPlay[0].Id,
            defender.AlliesInPlay[0].Id);

        var response = await Handler.Handle(
            new AssignCombatCommand(match.Id, attacker.PlayerId, [pairing]),
            CancellationToken.None);

        Assert.Equal(1, response.TotalAssignments);
    }

    [Fact]
    public async Task MultipleAttackersOnOneDefender_Allowed()
    {
        var match = MatchTestHelper.MakeMatchInCombat();
        _matchRepo.Seed(match);

        var attacker = match.GetAttacker();
        var defender = match.GetDefender();
        var targetId = defender.AlliesInPlay[0].Id;

        var pairings = new[]
        {
            new CombatPairing(attacker.AlliesInPlay[0].Id, targetId),
            new CombatPairing(attacker.AlliesInPlay[1].Id, targetId)
        };

        var response = await Handler.Handle(
            new AssignCombatCommand(match.Id, attacker.PlayerId, pairings),
            CancellationToken.None);

        Assert.Equal(2, response.TotalAssignments);
    }

    [Fact]
    public async Task NonInitiativeWinner_CannotAssign()
    {
        var match = MatchTestHelper.MakeMatchInCombat();
        _matchRepo.Seed(match);

        var defender = match.GetDefender();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Handler.Handle(
                new AssignCombatCommand(match.Id, defender.PlayerId, []),
                CancellationToken.None));
    }

    [Fact]
    public async Task AmbusherTargeted_WithNonAmbusherAvailable_Throws()
    {
        var match = MatchTestHelper.MakeMatchInCombatWithAmbusher();
        _matchRepo.Seed(match);

        var attacker = match.GetAttacker();
        var defender = match.GetDefender();
        var ambusher = defender.AlliesInPlay.First(a => a.IsAmbusher);

        var pairing = new CombatPairing(attacker.AlliesInPlay[0].Id, ambusher.Id);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Handler.Handle(
                new AssignCombatCommand(match.Id, attacker.PlayerId, [pairing]),
                CancellationToken.None));
    }

    [Fact]
    public async Task NonAmbusherTargeted_WithAmbusherPresent_Allowed()
    {
        var match = MatchTestHelper.MakeMatchInCombatWithAmbusher();
        _matchRepo.Seed(match);

        var attacker = match.GetAttacker();
        var defender = match.GetDefender();
        var nonAmbusher = defender.AlliesInPlay.First(a => !a.IsAmbusher);

        var pairing = new CombatPairing(attacker.AlliesInPlay[0].Id, nonAmbusher.Id);

        var response = await Handler.Handle(
            new AssignCombatCommand(match.Id, attacker.PlayerId, [pairing]),
            CancellationToken.None);

        Assert.Equal(1, response.TotalAssignments);
    }

    [Fact]
    public async Task AttackerAllyNotInPlay_Throws()
    {
        var match = MatchTestHelper.MakeMatchInCombat();
        _matchRepo.Seed(match);

        var attacker = match.GetAttacker();
        var defender = match.GetDefender();

        var pairing = new CombatPairing(Guid.NewGuid(), defender.AlliesInPlay[0].Id);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Handler.Handle(
                new AssignCombatCommand(match.Id, attacker.PlayerId, [pairing]),
                CancellationToken.None));
    }
}
