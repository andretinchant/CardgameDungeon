using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Domain.Repositories;

namespace CardgameDungeon.Tests.Matchmaking.Fakes;

public class FakeQueueRepository : IQueueRepository
{
    private readonly Dictionary<Guid, QueueEntry> _entries = new();

    public Task<QueueEntry?> GetByPlayerIdAsync(Guid playerId, CancellationToken ct = default)
        => Task.FromResult(_entries.GetValueOrDefault(playerId));

    public Task<IReadOnlyList<QueueEntry>> GetByQueueTypeAsync(QueueType queueType, CancellationToken ct = default)
    {
        var result = _entries.Values.Where(e => e.QueueType == queueType).ToList();
        return Task.FromResult<IReadOnlyList<QueueEntry>>(result);
    }

    public Task AddAsync(QueueEntry entry, CancellationToken ct = default)
    {
        _entries[entry.PlayerId] = entry;
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Guid playerId, CancellationToken ct = default)
    {
        _entries.Remove(playerId);
        return Task.CompletedTask;
    }

    public void Seed(QueueEntry entry) => _entries[entry.PlayerId] = entry;
    public int Count => _entries.Count;
}
