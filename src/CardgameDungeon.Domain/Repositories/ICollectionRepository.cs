using CardgameDungeon.Domain.Entities;

namespace CardgameDungeon.Domain.Repositories;

public interface ICollectionRepository
{
    Task<PlayerCollection?> GetByPlayerIdAsync(Guid playerId, CancellationToken ct = default);
    Task SaveAsync(PlayerCollection collection, CancellationToken ct = default);
    Task UpdateAsync(PlayerCollection collection, CancellationToken ct = default);
}
