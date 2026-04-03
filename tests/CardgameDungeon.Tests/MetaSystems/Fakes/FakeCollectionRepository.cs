using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Repositories;

namespace CardgameDungeon.Tests.MetaSystems.Fakes;

public class FakeCollectionRepository : ICollectionRepository
{
    private readonly Dictionary<Guid, PlayerCollection> _collections = new();

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

    public void Seed(PlayerCollection collection) => _collections[collection.PlayerId] = collection;
}
