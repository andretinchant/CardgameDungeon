using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Services;
using CardgameDungeon.Features.Match.Combat.AssignCombat;
using CardgameDungeon.Features.Match.Combat.Retarget;
using CardgameDungeon.Tests.Match.Fakes;

namespace CardgameDungeon.Tests.Match.Combat;

public class RetargetHandlerTests
{
    private readonly FakeMatchRepository _matchRepo = new();
    private readonly CombatResolver _combatResolver = new();

    private AssignCombatHandler AssignHandler => new(_matchRepo);
    private RetargetHandler RetargetHandler => new(_matchRepo, _combatResolver);

    [Fact]
    public async Task ValidRetarget_DealsDamageOnlyInPrimary()
    {
        var match = MatchTestHelper.MakeMatchInCombat();
        _matchRepo.Seed(match);

        var attacker = match.GetAttacker();
        var defender = match.GetDefender();

        // Assign ally[0] to defender[0]
        var pairing = new CombatPairing(attacker.AlliesInPlay[0].Id, defender.AlliesInPlay[0].Id);
        await AssignHandler.Handle(
            new AssignCombatCommand(match.Id, attacker.PlayerId, [pairing]),
            CancellationToken.None);

        // Retarget ally[0] to also engage defender[1]
        var response = await RetargetHandler.Handle(
            new RetargetCommand(
                match.Id,
                attacker.PlayerId,
                attacker.AlliesInPlay[0].Id,
                defender.AlliesInPlay[1].Id,
                Cost: 2,
                ExileCost: false),
            CancellationToken.None);

        Assert.Equal(attacker.AlliesInPlay[0].Strength, response.PrimaryDamageContribution);
        Assert.Equal(0, response.SecondaryDamageContribution);
        Assert.Equal(2, response.CostPaid);
    }

    [Fact]
    public async Task Retarget_PaysCostFromDeck()
    {
        var match = MatchTestHelper.MakeMatchInCombat();
        _matchRepo.Seed(match);

        var attacker = match.GetAttacker();
        var defender = match.GetDefender();
        var deckBefore = attacker.Deck.Count;

        var pairing = new CombatPairing(attacker.AlliesInPlay[0].Id, defender.AlliesInPlay[0].Id);
        await AssignHandler.Handle(
            new AssignCombatCommand(match.Id, attacker.PlayerId, [pairing]),
            CancellationToken.None);

        await RetargetHandler.Handle(
            new RetargetCommand(
                match.Id, attacker.PlayerId,
                attacker.AlliesInPlay[0].Id,
                defender.AlliesInPlay[1].Id,
                Cost: 3, ExileCost: false),
            CancellationToken.None);

        Assert.Equal(deckBefore - 3, attacker.Deck.Count);
    }

    [Fact]
    public async Task Retarget_AmbusherTarget_Throws()
    {
        var match = MatchTestHelper.MakeMatchInCombatWithAmbusher();
        _matchRepo.Seed(match);

        var attacker = match.GetAttacker();
        var defender = match.GetDefender();
        var ambusher = defender.AlliesInPlay.First(a => a.IsAmbusher);

        var pairing = new CombatPairing(
            attacker.AlliesInPlay[0].Id,
            defender.AlliesInPlay.First(a => !a.IsAmbusher).Id);
        await AssignHandler.Handle(
            new AssignCombatCommand(match.Id, attacker.PlayerId, [pairing]),
            CancellationToken.None);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            RetargetHandler.Handle(
                new RetargetCommand(
                    match.Id, attacker.PlayerId,
                    attacker.AlliesInPlay[0].Id,
                    ambusher.Id,
                    Cost: 1, ExileCost: false),
                CancellationToken.None));
    }

    [Fact]
    public async Task AllyNotInPlay_Throws()
    {
        var match = MatchTestHelper.MakeMatchInCombat();
        _matchRepo.Seed(match);

        var attacker = match.GetAttacker();
        var defender = match.GetDefender();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            RetargetHandler.Handle(
                new RetargetCommand(
                    match.Id, attacker.PlayerId,
                    Guid.NewGuid(),
                    defender.AlliesInPlay[0].Id,
                    Cost: 1, ExileCost: false),
                CancellationToken.None));
    }
}
