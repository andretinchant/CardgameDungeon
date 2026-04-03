using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Domain.Repositories;
using CardgameDungeon.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CardgameDungeon.Infrastructure.Repositories;

public class EfMarketplaceRepository(CardgameDungeonDbContext db) : IMarketplaceRepository
{
    public async Task<MarketplaceListing?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await db.MarketplaceListings.FirstOrDefaultAsync(ml => ml.Id == id, ct);
    }

    public async Task<IReadOnlyList<MarketplaceListing>> GetActiveListingsAsync(
        CardType? cardType = null, Rarity? rarity = null, CancellationToken ct = default)
    {
        var query = db.MarketplaceListings.Where(ml => ml.IsActive);

        if (cardType.HasValue || rarity.HasValue)
        {
            // Join with Cards table for filtering
            var cardQuery = db.Cards.AsQueryable();
            if (rarity.HasValue)
                cardQuery = cardQuery.Where(c => c.Rarity == rarity.Value);

            var cardIds = await cardQuery.Select(c => c.Id).ToListAsync(ct);

            query = query.Where(ml => cardIds.Contains(ml.CardId));
        }

        return await query.ToListAsync(ct);
    }

    public async Task SaveAsync(MarketplaceListing listing, CancellationToken ct = default)
    {
        db.MarketplaceListings.Add(listing);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(MarketplaceListing listing, CancellationToken ct = default)
    {
        var entry = db.MarketplaceListings.Entry(listing);
        if (entry.State == EntityState.Detached)
        {
            var existing = await db.MarketplaceListings.FindAsync([listing.Id], ct);
            if (existing is not null)
                db.MarketplaceListings.Entry(existing).CurrentValues.SetValues(listing);
            else
                db.MarketplaceListings.Add(listing);
        }
        await db.SaveChangesAsync(ct);
    }
}
