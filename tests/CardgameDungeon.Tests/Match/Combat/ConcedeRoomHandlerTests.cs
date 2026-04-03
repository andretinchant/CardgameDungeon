using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Features.Match.Combat.ConcedeRoom;
using CardgameDungeon.Tests.Match.Fakes;

namespace CardgameDungeon.Tests.Match.Combat;

public class ConcedeRoomHandlerTests
{
    private readonly FakeMatchRepository _matchRepo = new();
    private readonly FakeMatchNotifier _notifier = new();
    private ConcedeRoomHandler Handler => new(_matchRepo, _notifier);

    [Fact]
    public async Task DefenderConcedes_AdvancesToRoomResolution()
    {
        var match = MatchTestHelper.MakeMatchInCombat();
        _matchRepo.Seed(match);

        var defender = match.GetDefender();

        var response = await Handler.Handle(
            new ConcedeRoomCommand(match.Id, defender.PlayerId),
            CancellationToken.None);

        Assert.Equal(MatchPhase.RoomResolution, response.Phase);
    }

    [Fact]
    public async Task AttackerTriesToConcede_Throws()
    {
        var match = MatchTestHelper.MakeMatchInCombat();
        _matchRepo.Seed(match);

        var attacker = match.GetAttacker();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Handler.Handle(
                new ConcedeRoomCommand(match.Id, attacker.PlayerId),
                CancellationToken.None));
    }

    [Fact]
    public async Task ConcedeRoom_NoCombatHappens()
    {
        var match = MatchTestHelper.MakeMatchInCombat();
        _matchRepo.Seed(match);

        var defender = match.GetDefender();
        var hpBefore = defender.HitPoints;

        await Handler.Handle(
            new ConcedeRoomCommand(match.Id, defender.PlayerId),
            CancellationToken.None);

        // No damage taken
        Assert.Equal(hpBefore, defender.HitPoints);
        Assert.Empty(match.CombatBoard.Assignments);
    }

    [Fact]
    public async Task MatchNotFound_Throws()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            Handler.Handle(
                new ConcedeRoomCommand(Guid.NewGuid(), Guid.NewGuid()),
                CancellationToken.None));
    }
}
