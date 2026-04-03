using CardgameDungeon.Domain.Entities;

namespace CardgameDungeon.Domain.Repositories;

public interface IRatingRepository
{
    Task<PlayerRating?> GetByPlayerIdAsync(Guid playerId, CancellationToken ct = default);
    Task<IReadOnlyList<PlayerRating>> GetAllActiveAsync(CancellationToken ct = default);
    Task SaveAsync(PlayerRating rating, CancellationToken ct = default);
    Task UpdateAsync(PlayerRating rating, CancellationToken ct = default);
    Task UpdateManyAsync(IEnumerable<PlayerRating> ratings, CancellationToken ct = default);
}
