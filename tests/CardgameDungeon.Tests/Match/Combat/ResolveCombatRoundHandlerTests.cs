using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Domain.Services;
using CardgameDungeon.Features.Match.Combat.AssignCombat;
using CardgameDungeon.Features.Match.Combat.ResolveCombatRound;
using CardgameDungeon.Tests.Match.Fakes;

namespace CardgameDungeon.Tests.Match.Combat;

public class ResolveCombatRoundHandlerTests
{
    private readonly FakeMatchRepository _matchRepo = new();
    private readonly CombatResolver _combatResolver = new();

    private AssignCombatHandler AssignHandler => new(_matchRepo);
    private ResolveCombatRoundHandler ResolveHandler => new(_matchRepo, _combatResolver);

    [Fact]
    public async Task AttackerStronger_DefenderEliminated()
    {
        // P1 strength=10, P2 strength=3, P2 hp=5 → 10 >= 5 → P2 eliminated
        var match = MatchTestHelper.MakeMatchInCombat(p1Strength: 10, p1Hp: 20, p2Strength: 3, p2Hp: 5);
        _matchRepo.Seed(match);

        var attacker = match.GetAttacker();
        var defender = match.GetDefender();
        var pairing = new CombatPairing(attacker.AlliesInPlay[0].Id, defender.AlliesInPlay[0].Id);

        await AssignHandler.Handle(
            new AssignCombatCommand(match.Id, attacker.PlayerId, [pairing]),
            CancellationToken.None);

        var response = await ResolveHandler.Handle(
            new ResolveCombatRoundCommand(match.Id),
            CancellationToken.None);

        Assert.Single(response.Results);
        Assert.True(response.Results[0].DefenderEliminated);
        Assert.Equal("RoomResolution", response.Phase);
    }

    [Fact]
    public async Task EqualStrength_BothTakeDamage()
    {
        var match = MatchTestHelper.MakeMatchInCombat(p1Strength: 5, p1Hp: 20, p2Strength: 5, p2Hp: 20);
        _matchRepo.Seed(match);

        var attacker = match.GetAttacker();
        var defender = match.GetDefender();
        var pairing = new CombatPairing(attacker.AlliesInPlay[0].Id, defender.AlliesInPlay[0].Id);

        await AssignHandler.Handle(
            new AssignCombatCommand(match.Id, attacker.PlayerId, [pairing]),
            CancellationToken.None);

        var response = await ResolveHandler.Handle(
            new ResolveCombatRoundCommand(match.Id),
            CancellationToken.None);

        Assert.Equal(CombatOutcome.BothTakeDamage, response.Results[0].Outcome);
    }

    [Fact]
    public async Task DoubleStrength_Eliminates()
    {
        // P1 str=10, P2 str=5 → 10 >= 5*2 → defender eliminated
        var match = MatchTestHelper.MakeMatchInCombat(p1Strength: 10, p1Hp: 20, p2Strength: 5, p2Hp: 20);
        _matchRepo.Seed(match);

        var attacker = match.GetAttacker();
        var defender = match.GetDefender();
        var pairing = new CombatPairing(attacker.AlliesInPlay[0].Id, defender.AlliesInPlay[0].Id);

        await AssignHandler.Handle(
            new AssignCombatCommand(match.Id, attacker.PlayerId, [pairing]),
            CancellationToken.None);

        var response = await ResolveHandler.Handle(
            new ResolveCombatRoundCommand(match.Id),
            CancellationToken.None);

        Assert.Equal(CombatOutcome.DefenderEliminated, response.Results[0].Outcome);
    }

    [Fact]
    public async Task NoAssignments_Throws()
    {
        var match = MatchTestHelper.MakeMatchInCombat();
        _matchRepo.Seed(match);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            ResolveHandler.Handle(
                new ResolveCombatRoundCommand(match.Id),
                CancellationToken.None));
    }

    [Fact]
    public async Task EliminatedAllies_MovedToDiscard()
    {
        var match = MatchTestHelper.MakeMatchInCombat(p1Strength: 10, p1Hp: 20, p2Strength: 3, p2Hp: 5);
        _matchRepo.Seed(match);

        var attacker = match.GetAttacker();
        var defender = match.GetDefender();
        var defenderAllyCount = defender.AlliesInPlay.Count;
        var pairing = new CombatPairing(attacker.AlliesInPlay[0].Id, defender.AlliesInPlay[0].Id);

        await AssignHandler.Handle(
            new AssignCombatCommand(match.Id, attacker.PlayerId, [pairing]),
            CancellationToken.None);

        await ResolveHandler.Handle(
            new ResolveCombatRoundCommand(match.Id),
            CancellationToken.None);

        Assert.Equal(defenderAllyCount - 1, defender.AlliesInPlay.Count);
        Assert.True(defender.Discard.Count > 0);
    }
}
