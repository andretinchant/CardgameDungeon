using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Repositories;
using CardgameDungeon.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CardgameDungeon.Infrastructure.Repositories;

public class EfRatingRepository(CardgameDungeonDbContext db) : IRatingRepository
{
    public async Task<PlayerRating?> GetByPlayerIdAsync(Guid playerId, CancellationToken ct = default)
    {
        return await db.PlayerRatings.FirstOrDefaultAsync(pr => pr.PlayerId == playerId, ct);
    }

    public async Task<IReadOnlyList<PlayerRating>> GetAllActiveAsync(CancellationToken ct = default)
    {
        return await db.PlayerRatings.ToListAsync(ct);
    }

    public async Task SaveAsync(PlayerRating rating, CancellationToken ct = default)
    {
        db.PlayerRatings.Add(rating);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(PlayerRating rating, CancellationToken ct = default)
    {
        var entry = db.PlayerRatings.Entry(rating);
        if (entry.State == EntityState.Detached)
        {
            var existing = await db.PlayerRatings.FindAsync([rating.PlayerId], ct);
            if (existing is not null)
                db.PlayerRatings.Entry(existing).CurrentValues.SetValues(rating);
            else
                db.PlayerRatings.Add(rating);
        }
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateManyAsync(IEnumerable<PlayerRating> ratings, CancellationToken ct = default)
    {
        foreach (var rating in ratings)
        {
            var existing = await db.PlayerRatings.FindAsync([rating.PlayerId], ct);
            if (existing is not null)
                db.PlayerRatings.Entry(existing).CurrentValues.SetValues(rating);
            else
                db.PlayerRatings.Add(rating);
        }
        await db.SaveChangesAsync(ct);
    }
}
