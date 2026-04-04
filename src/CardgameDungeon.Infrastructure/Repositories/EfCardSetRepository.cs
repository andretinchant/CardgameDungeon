using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Repositories;
using CardgameDungeon.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CardgameDungeon.Infrastructure.Repositories;

public class EfCardSetRepository(CardgameDungeonDbContext db) : ICardSetRepository
{
    public async Task<IReadOnlyList<CardSet>> GetAllAsync(CancellationToken ct = default)
    {
        return await db.CardSets
            .Include(cs => cs.Cards)
            .OrderBy(cs => cs.ReleaseDate)
            .ToListAsync(ct);
    }
}
