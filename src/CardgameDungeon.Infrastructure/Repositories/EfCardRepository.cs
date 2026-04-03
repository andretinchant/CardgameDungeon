using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Repositories;
using CardgameDungeon.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CardgameDungeon.Infrastructure.Repositories;

public class EfCardRepository(CardgameDungeonDbContext db) : ICardRepository
{
    public async Task<IReadOnlyList<Card>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var idList = ids.ToList();
        var cards = await db.Cards.Where(c => idList.Contains(c.Id)).ToListAsync(ct);
        return cards;
    }

    public async Task<IReadOnlyList<DungeonRoomCard>> GetDungeonRoomsByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var idList = ids.ToList();
        var rooms = await db.DungeonRoomCards.Where(c => idList.Contains(c.Id)).ToListAsync(ct);
        return rooms;
    }

    public async Task<BossCard?> GetBossByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await db.BossCards.FirstOrDefaultAsync(b => b.Id == id, ct);
    }
}
