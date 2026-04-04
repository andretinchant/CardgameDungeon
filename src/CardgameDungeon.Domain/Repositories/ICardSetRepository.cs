using CardgameDungeon.Domain.Entities;

namespace CardgameDungeon.Domain.Repositories;

public interface ICardSetRepository
{
    Task<IReadOnlyList<CardSet>> GetAllAsync(CancellationToken ct = default);
}
