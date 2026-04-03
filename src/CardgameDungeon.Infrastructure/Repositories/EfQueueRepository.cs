using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Domain.Repositories;
using CardgameDungeon.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CardgameDungeon.Infrastructure.Repositories;

public class EfQueueRepository(CardgameDungeonDbContext db) : IQueueRepository
{
    public async Task<QueueEntry?> GetByPlayerIdAsync(Guid playerId, CancellationToken ct = default)
    {
        return await db.QueueEntries.FirstOrDefaultAsync(qe => qe.PlayerId == playerId, ct);
    }

    public async Task<IReadOnlyList<QueueEntry>> GetByQueueTypeAsync(QueueType queueType, CancellationToken ct = default)
    {
        return await db.QueueEntries.Where(qe => qe.QueueType == queueType).ToListAsync(ct);
    }

    public async Task AddAsync(QueueEntry entry, CancellationToken ct = default)
    {
        var existing = await db.QueueEntries.FindAsync([entry.PlayerId], ct);
        if (existing is not null)
            db.QueueEntries.Remove(existing);

        db.QueueEntries.Add(entry);
        await db.SaveChangesAsync(ct);
    }

    public async Task RemoveAsync(Guid playerId, CancellationToken ct = default)
    {
        var entry = await db.QueueEntries.FindAsync([playerId], ct);
        if (entry is not null)
        {
            db.QueueEntries.Remove(entry);
            await db.SaveChangesAsync(ct);
        }
    }
}
