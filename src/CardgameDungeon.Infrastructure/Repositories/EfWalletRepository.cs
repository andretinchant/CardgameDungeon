using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Repositories;
using CardgameDungeon.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CardgameDungeon.Infrastructure.Repositories;

public class EfWalletRepository(CardgameDungeonDbContext db) : IWalletRepository
{
    public async Task<PlayerWallet?> GetByPlayerIdAsync(Guid playerId, CancellationToken ct = default)
    {
        return await db.PlayerWallets.FirstOrDefaultAsync(pw => pw.PlayerId == playerId, ct);
    }

    public async Task SaveAsync(PlayerWallet wallet, CancellationToken ct = default)
    {
        db.PlayerWallets.Add(wallet);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(PlayerWallet wallet, CancellationToken ct = default)
    {
        var entry = db.PlayerWallets.Entry(wallet);
        if (entry.State == EntityState.Detached)
        {
            var existing = await db.PlayerWallets.FindAsync([wallet.PlayerId], ct);
            if (existing is not null)
                db.PlayerWallets.Entry(existing).CurrentValues.SetValues(wallet);
            else
                db.PlayerWallets.Add(wallet);
        }
        await db.SaveChangesAsync(ct);
    }
}
