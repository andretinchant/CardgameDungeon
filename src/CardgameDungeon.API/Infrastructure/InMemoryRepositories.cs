using System.Collections.Concurrent;
using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Domain.Repositories;
using CardgameDungeon.Features.Collection.OpenBooster;

namespace CardgameDungeon.API.Infrastructure;

public class InMemoryDeckRepository : IDeckRepository
{
    private readonly ConcurrentDictionary<Guid, DeckList> _decks = new();

    public Task<DeckList?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(_decks.GetValueOrDefault(id));

    public Task SaveAsync(DeckList deck, CancellationToken ct = default)
    {
        _decks[deck.Id] = deck;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(DeckList deck, CancellationToken ct = default)
    {
        _decks[deck.Id] = deck;
        return Task.CompletedTask;
    }
}

public class InMemoryCardRepository : ICardRepository
{
    private readonly ConcurrentDictionary<Guid, Card> _cards = new();
    private readonly ConcurrentDictionary<Guid, DungeonRoomCard> _rooms = new();
    private readonly ConcurrentDictionary<Guid, BossCard> _bosses = new();

    public Task<IReadOnlyList<Card>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var result = ids.Select(id => _cards.GetValueOrDefault(id)).Where(c => c is not null).Cast<Card>().ToList();
        return Task.FromResult<IReadOnlyList<Card>>(result);
    }

    public Task<IReadOnlyList<DungeonRoomCard>> GetDungeonRoomsByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var result = ids.Select(id => _rooms.GetValueOrDefault(id)).Where(r => r is not null).Cast<DungeonRoomCard>().ToList();
        return Task.FromResult<IReadOnlyList<DungeonRoomCard>>(result);
    }

    public Task<BossCard?> GetBossByIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(_bosses.GetValueOrDefault(id));
}

public class InMemoryMatchRepository : IMatchRepository
{
    private readonly ConcurrentDictionary<Guid, MatchState> _matches = new();

    public Task<MatchState?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(_matches.GetValueOrDefault(id));

    public Task SaveAsync(MatchState match, CancellationToken ct = default)
    {
        _matches[match.Id] = match;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(MatchState match, CancellationToken ct = default)
    {
        _matches[match.Id] = match;
        return Task.CompletedTask;
    }
}

public class InMemoryCollectionRepository : ICollectionRepository
{
    private readonly ConcurrentDictionary<Guid, PlayerCollection> _collections = new();

    public Task<PlayerCollection?> GetByPlayerIdAsync(Guid playerId, CancellationToken ct = default)
        => Task.FromResult(_collections.GetValueOrDefault(playerId));

    public Task SaveAsync(PlayerCollection collection, CancellationToken ct = default)
    {
        _collections[collection.PlayerId] = collection;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(PlayerCollection collection, CancellationToken ct = default)
    {
        _collections[collection.PlayerId] = collection;
        return Task.CompletedTask;
    }
}

public class InMemoryWalletRepository : IWalletRepository
{
    private readonly ConcurrentDictionary<Guid, PlayerWallet> _wallets = new();

    public Task<PlayerWallet?> GetByPlayerIdAsync(Guid playerId, CancellationToken ct = default)
        => Task.FromResult(_wallets.GetValueOrDefault(playerId));

    public Task SaveAsync(PlayerWallet wallet, CancellationToken ct = default)
    {
        _wallets[wallet.PlayerId] = wallet;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(PlayerWallet wallet, CancellationToken ct = default)
    {
        _wallets[wallet.PlayerId] = wallet;
        return Task.CompletedTask;
    }
}

public class InMemoryMarketplaceRepository : IMarketplaceRepository
{
    private readonly ConcurrentDictionary<Guid, MarketplaceListing> _listings = new();

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
}

public class InMemoryQueueRepository : IQueueRepository
{
    private readonly ConcurrentDictionary<Guid, QueueEntry> _entries = new();

    public Task<QueueEntry?> GetByPlayerIdAsync(Guid playerId, CancellationToken ct = default)
        => Task.FromResult(_entries.GetValueOrDefault(playerId));

    public Task<IReadOnlyList<QueueEntry>> GetByQueueTypeAsync(QueueType queueType, CancellationToken ct = default)
    {
        var result = _entries.Values.Where(e => e.QueueType == queueType).ToList();
        return Task.FromResult<IReadOnlyList<QueueEntry>>(result);
    }

    public Task AddAsync(QueueEntry entry, CancellationToken ct = default)
    {
        _entries[entry.PlayerId] = entry;
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Guid playerId, CancellationToken ct = default)
    {
        _entries.TryRemove(playerId, out _);
        return Task.CompletedTask;
    }
}

public class InMemoryRatingRepository : IRatingRepository
{
    private readonly ConcurrentDictionary<Guid, PlayerRating> _ratings = new();

    public Task<PlayerRating?> GetByPlayerIdAsync(Guid playerId, CancellationToken ct = default)
        => Task.FromResult(_ratings.GetValueOrDefault(playerId));

    public Task<IReadOnlyList<PlayerRating>> GetAllActiveAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<PlayerRating>>(_ratings.Values.ToList());

    public Task SaveAsync(PlayerRating rating, CancellationToken ct = default)
    {
        _ratings[rating.PlayerId] = rating;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(PlayerRating rating, CancellationToken ct = default)
    {
        _ratings[rating.PlayerId] = rating;
        return Task.CompletedTask;
    }

    public Task UpdateManyAsync(IEnumerable<PlayerRating> ratings, CancellationToken ct = default)
    {
        foreach (var r in ratings) _ratings[r.PlayerId] = r;
        return Task.CompletedTask;
    }
}

public class InMemoryTournamentRepository : ITournamentRepository
{
    private readonly ConcurrentDictionary<Guid, Tournament> _tournaments = new();

    public Task<Tournament?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(_tournaments.GetValueOrDefault(id));

    public Task SaveAsync(Tournament tournament, CancellationToken ct = default)
    {
        _tournaments[tournament.Id] = tournament;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Tournament tournament, CancellationToken ct = default)
    {
        _tournaments[tournament.Id] = tournament;
        return Task.CompletedTask;
    }
}

public class RandomBoosterCardPool : IBoosterCardPool
{
    public Task<Card> GetRandomCardByRarityAsync(Rarity rarity, CancellationToken ct = default)
    {
        Card card = new AllyCard(
            Guid.NewGuid(),
            $"{rarity}-{Guid.NewGuid():N}"[..16],
            rarity,
            cost: rarity switch { Rarity.Common => 1, Rarity.Uncommon => 2, Rarity.Rare => 3, _ => 5 },
            strength: Random.Shared.Next(1, 10),
            hitPoints: Random.Shared.Next(1, 10),
            initiative: Random.Shared.Next(1, 5));
        return Task.FromResult(card);
    }
}
