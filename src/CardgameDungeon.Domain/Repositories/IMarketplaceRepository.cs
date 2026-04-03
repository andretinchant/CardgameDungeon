using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.Domain.Repositories;

public interface IMarketplaceRepository
{
    Task<MarketplaceListing?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<MarketplaceListing>> GetActiveListingsAsync(
        CardType? cardType = null, Rarity? rarity = null, CancellationToken ct = default);
    Task SaveAsync(MarketplaceListing listing, CancellationToken ct = default);
    Task UpdateAsync(MarketplaceListing listing, CancellationToken ct = default);
}
