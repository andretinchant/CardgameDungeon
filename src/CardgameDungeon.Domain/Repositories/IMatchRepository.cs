using CardgameDungeon.Domain.Entities;

namespace CardgameDungeon.Domain.Repositories;

public interface IMatchRepository
{
    Task<MatchState?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task SaveAsync(MatchState match, CancellationToken ct = default);
    Task UpdateAsync(MatchState match, CancellationToken ct = default);
}
