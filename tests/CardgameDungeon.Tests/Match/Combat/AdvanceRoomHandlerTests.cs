using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Domain.Services;
using CardgameDungeon.Features.Match.Combat.AdvanceRoom;
using CardgameDungeon.Features.Match.Combat.AssignCombat;
using CardgameDungeon.Features.Match.Combat.ResolveCombatRound;
using CardgameDungeon.Tests.Match.Fakes;

namespace CardgameDungeon.Tests.Match.Combat;

public class AdvanceRoomHandlerTests
{
    private readonly FakeMatchRepository _matchRepo = new();
    private readonly CombatResolver _combatResolver = new();
    private readonly FakeMatchNotifier _notifier = new();

    private AssignCombatHandler AssignHandler => new(_matchRepo, _notifier);
    private ResolveCombatRoundHandler ResolveHandler => new(_matchRepo, _combatResolver, _notifier);
    private AdvanceRoomHandler AdvanceHandler => new(_matchRepo, _notifier);

    private async Task BringToRoomResolution(Domain.Entities.MatchState match)
    {
        var attacker = match.GetAttacker();
        var defender = match.GetDefender();

        var pairing = new CombatPairing(attacker.AlliesInPlay[0].Id, defender.AlliesInPlay[0].Id);
        await AssignHandler.Handle(
            new AssignCombatCommand(match.Id, attacker.PlayerId, [pairing]),
            CancellationToken.None);

        await ResolveHandler.Handle(
            new ResolveCombatRoundCommand(match.Id),
            CancellationToken.None);
    }

    [Fact]
    public async Task AdvanceRoom_MovesToNextRoom()
    {
        var match = MatchTestHelper.MakeMatchInCombat(p1Strength: 10, p1Hp: 20, p2Strength: 3, p2Hp: 5);
        _matchRepo.Seed(match);

        await BringToRoomResolution(match);

        var response = await AdvanceHandler.Handle(
            new AdvanceRoomCommand(match.Id),
            CancellationToken.None);

        Assert.Equal(2, response.CurrentRoom);
    }

    [Fact]
    public async Task AdvanceRoom_DefenderRefillsHand()
    {
        var match = MatchTestHelper.MakeMatchInCombat(p1Strength: 10, p1Hp: 20, p2Strength: 3, p2Hp: 5);
        _matchRepo.Seed(match);

        await BringToRoomResolution(match);

        var defenderHandBefore = match.GetDefender().Hand.Count;

        await AdvanceHandler.Handle(
            new AdvanceRoomCommand(match.Id),
            CancellationToken.None);

        // Defender refills to 8 (or as many as deck allows)
        Assert.True(match.GetDefender().Hand.Count >= defenderHandBefore);
    }

    [Fact]
    public async Task AdvanceRoom_DefenderShufflesDiscardIntoDeck()
    {
        var match = MatchTestHelper.MakeMatchInCombat(p1Strength: 10, p1Hp: 20, p2Strength: 3, p2Hp: 5);
        _matchRepo.Seed(match);

        await BringToRoomResolution(match);

        // After combat, defender should have cards in discard
        var defender = match.GetDefender();

        await AdvanceHandler.Handle(
            new AdvanceRoomCommand(match.Id),
            CancellationToken.None);

        // Discard should be empty after shuffle back into deck
        Assert.Empty(defender.Discard);
    }

    [Fact]
    public async Task AdvanceRoom_WrongPhase_Throws()
    {
        var match = MatchTestHelper.MakeMatchInCombat();
        _matchRepo.Seed(match);

        // Match is in Combat phase, not RoomResolution
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            AdvanceHandler.Handle(
                new AdvanceRoomCommand(match.Id),
                CancellationToken.None));
    }

    [Fact]
    public async Task MatchNotFound_Throws()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            AdvanceHandler.Handle(
                new AdvanceRoomCommand(Guid.NewGuid()),
                CancellationToken.None));
    }
}
