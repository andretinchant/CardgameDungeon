using CardgameDungeon.Domain.Entities;

namespace CardgameDungeon.Domain.Repositories;

public interface ICardRepository
{
    Task<IReadOnlyList<Card>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
    Task<IReadOnlyList<DungeonRoomCard>> GetDungeonRoomsByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
    Task<BossCard?> GetBossByIdAsync(Guid id, CancellationToken ct = default);
}
