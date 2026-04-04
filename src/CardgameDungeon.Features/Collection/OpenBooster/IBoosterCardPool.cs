using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.Features.Collection.OpenBooster;

public interface IBoosterCardPool
{
    Task<Card> GetRandomCardByRarityAsync(Rarity rarity, string setCode, CancellationToken ct = default);
}
