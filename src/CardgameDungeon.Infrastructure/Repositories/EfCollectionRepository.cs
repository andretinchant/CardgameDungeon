using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Repositories;
using CardgameDungeon.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CardgameDungeon.Infrastructure.Repositories;

public class EfCollectionRepository(CardgameDungeonDbContext db) : ICollectionRepository
{
    public async Task<PlayerCollection?> GetByPlayerIdAsync(Guid playerId, CancellationToken ct = default)
    {
        var ownedCards = await db.OwnedCards
            .Where(oc => oc.PlayerId == playerId)
            .ToListAsync(ct);

        // If no cards found, check if this player ever had a collection saved
        // Return null only if the player is truly unknown
        if (ownedCards.Count == 0)
        {
            // Check if any record exists (could be empty collection)
            var exists = await db.OwnedCards.AnyAsync(oc => oc.PlayerId == playerId, ct);
            if (!exists)
            {
                // Check if we've seen this player in wallets (as a proxy for "known player")
                var walletExists = await db.PlayerWallets.AnyAsync(pw => pw.PlayerId == playerId, ct);
                if (!walletExists) return null;
            }
        }

        return new PlayerCollection(playerId, ownedCards);
    }

    public async Task SaveAsync(PlayerCollection collection, CancellationToken ct = default)
    {
        foreach (var card in collection.Cards)
        {
            var exists = await db.OwnedCards.AnyAsync(oc => oc.Id == card.Id, ct);
            if (!exists)
                db.OwnedCards.Add(card);
        }
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(PlayerCollection collection, CancellationToken ct = default)
    {
        // Get existing owned cards for this player
        var existing = await db.OwnedCards
            .Where(oc => oc.PlayerId == collection.PlayerId)
            .ToDictionaryAsync(oc => oc.Id, ct);

        // Add new cards
        foreach (var card in collection.Cards)
        {
            if (!existing.ContainsKey(card.Id))
                db.OwnedCards.Add(card);
            else
                db.OwnedCards.Entry(existing[card.Id]).CurrentValues.SetValues(card);
        }

        // Remove cards no longer in collection
        var currentIds = collection.Cards.Select(c => c.Id).ToHashSet();
        foreach (var (id, entity) in existing)
        {
            if (!currentIds.Contains(id))
                db.OwnedCards.Remove(entity);
        }

        await db.SaveChangesAsync(ct);
    }
}
