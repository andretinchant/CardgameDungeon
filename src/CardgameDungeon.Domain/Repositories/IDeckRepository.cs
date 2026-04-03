using CardgameDungeon.Domain.Entities;

namespace CardgameDungeon.Domain.Repositories;

public interface IDeckRepository
{
    Task<DeckList?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task SaveAsync(DeckList deck, CancellationToken ct = default);
    Task UpdateAsync(DeckList deck, CancellationToken ct = default);
}
