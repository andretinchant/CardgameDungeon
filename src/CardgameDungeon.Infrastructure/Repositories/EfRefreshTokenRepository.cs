using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Repositories;
using CardgameDungeon.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CardgameDungeon.Infrastructure.Repositories;

public class EfRefreshTokenRepository(CardgameDungeonDbContext db) : IRefreshTokenRepository
{
    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default)
        => await db.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token, ct);

    public async Task SaveAsync(RefreshToken refreshToken, CancellationToken ct = default)
    {
        db.RefreshTokens.Add(refreshToken);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(RefreshToken refreshToken, CancellationToken ct = default)
    {
        var entry = db.RefreshTokens.Entry(refreshToken);
        if (entry.State == EntityState.Detached)
        {
            var existing = await db.RefreshTokens.FindAsync([refreshToken.Id], ct);
            if (existing is not null)
                db.RefreshTokens.Entry(existing).CurrentValues.SetValues(refreshToken);
            else
                db.RefreshTokens.Add(refreshToken);
        }
        await db.SaveChangesAsync(ct);
    }

    public async Task RevokeAllForPlayerAsync(Guid playerId, CancellationToken ct = default)
    {
        var tokens = await db.RefreshTokens
            .Where(rt => rt.PlayerId == playerId && !rt.IsRevoked)
            .ToListAsync(ct);

        foreach (var token in tokens)
            token.Revoke();

        await db.SaveChangesAsync(ct);
    }
}
