using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Repositories;
using CardgameDungeon.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CardgameDungeon.Infrastructure.Repositories;

public class EfPlayerRepository(CardgameDungeonDbContext db) : IPlayerRepository
{
    public async Task<Player?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.Players.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<Player?> GetByUsernameAsync(string username, CancellationToken ct = default)
        => await db.Players.FirstOrDefaultAsync(p => p.Username == username, ct);

    public async Task<Player?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await db.Players.FirstOrDefaultAsync(p => p.Email == email, ct);

    public async Task<bool> ExistsAsync(string username, string email, CancellationToken ct = default)
        => await db.Players.AnyAsync(p => p.Username == username || p.Email == email, ct);

    public async Task SaveAsync(Player player, CancellationToken ct = default)
    {
        db.Players.Add(player);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Player player, CancellationToken ct = default)
    {
        var entry = db.Players.Entry(player);
        if (entry.State == EntityState.Detached)
        {
            var existing = await db.Players.FindAsync([player.Id], ct);
            if (existing is not null)
                db.Players.Entry(existing).CurrentValues.SetValues(player);
            else
                db.Players.Add(player);
        }
        await db.SaveChangesAsync(ct);
    }
}
