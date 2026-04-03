using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.Domain.Repositories;

public interface IQueueRepository
{
    Task<QueueEntry?> GetByPlayerIdAsync(Guid playerId, CancellationToken ct = default);
    Task<IReadOnlyList<QueueEntry>> GetByQueueTypeAsync(QueueType queueType, CancellationToken ct = default);
    Task AddAsync(QueueEntry entry, CancellationToken ct = default);
    Task RemoveAsync(Guid playerId, CancellationToken ct = default);
}
