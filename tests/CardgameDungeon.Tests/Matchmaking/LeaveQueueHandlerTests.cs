using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Features.Matchmaking.LeaveQueue;
using CardgameDungeon.Tests.Matchmaking.Fakes;

namespace CardgameDungeon.Tests.Matchmaking;

public class LeaveQueueHandlerTests
{
    private readonly FakeQueueRepository _queueRepo = new();
    private LeaveQueueHandler Handler => new(_queueRepo);

    [Fact]
    public async Task InQueue_LeavesSuccessfully()
    {
        var playerId = Guid.NewGuid();
        _queueRepo.Seed(new QueueEntry(playerId, Guid.NewGuid(), QueueType.Ranked, 1000));

        var response = await Handler.Handle(new LeaveQueueCommand(playerId), CancellationToken.None);

        Assert.True(response.Left);
        Assert.Null(await _queueRepo.GetByPlayerIdAsync(playerId));
    }

    [Fact]
    public async Task NotInQueue_Throws()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Handler.Handle(new LeaveQueueCommand(Guid.NewGuid()), CancellationToken.None));
    }
}
