using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Domain.Repositories;

namespace CardgameDungeon.Tests.MetaSystems.Fakes;

public class FakeMarketplaceRepository : IMarketplaceRepository
{
    private readonly Dictionary<Guid, MarketplaceListing> _listings = new();

    public Task<MarketplaceListing?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(_listings.GetValueOrDefault(id));

    public Task<IReadOnlyList<MarketplaceListing>> GetActiveListingsAsync(
        CardType? cardType = null, Rarity? rarity = null, CancellationToken ct = default)
    {
        var result = _listings.Values.Where(l => l.IsActive).ToList();
        return Task.FromResult<IReadOnlyList<MarketplaceListing>>(result);
    }

    public Task SaveAsync(MarketplaceListing listing, CancellationToken ct = default)
    {
        _listings[listing.Id] = listing;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(MarketplaceListing listing, CancellationToken ct = default)
    {
        _listings[listing.Id] = listing;
        return Task.CompletedTask;
    }

    public void Seed(MarketplaceListing listing) => _listings[listing.Id] = listing;
}
