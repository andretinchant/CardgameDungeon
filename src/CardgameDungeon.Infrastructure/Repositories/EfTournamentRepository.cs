using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Repositories;
using CardgameDungeon.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CardgameDungeon.Infrastructure.Repositories;

public class EfTournamentRepository(CardgameDungeonDbContext db) : ITournamentRepository
{
    public async Task<Tournament?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await db.Tournaments
            .Include(t => t.Participants)
            .FirstOrDefaultAsync(t => t.Id == id, ct);
    }

    public async Task SaveAsync(Tournament tournament, CancellationToken ct = default)
    {
        db.Tournaments.Add(tournament);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Tournament tournament, CancellationToken ct = default)
    {
        var entry = db.Tournaments.Entry(tournament);
        if (entry.State == EntityState.Detached)
            db.Tournaments.Update(tournament);

        await db.SaveChangesAsync(ct);
    }
}
