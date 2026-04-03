using CardgameDungeon.Domain.Entities;

namespace CardgameDungeon.Domain.Repositories;

public interface ITournamentRepository
{
    Task<Tournament?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task SaveAsync(Tournament tournament, CancellationToken ct = default);
    Task UpdateAsync(Tournament tournament, CancellationToken ct = default);
}
