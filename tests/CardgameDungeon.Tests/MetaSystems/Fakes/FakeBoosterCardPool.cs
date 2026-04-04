using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Features.Collection.OpenBooster;

namespace CardgameDungeon.Tests.MetaSystems.Fakes;

public class FakeBoosterCardPool : IBoosterCardPool
{
    public Task<Card> GetRandomCardByRarityAsync(Rarity rarity, string setCode, CancellationToken ct = default)
    {
        Card card = rarity switch
        {
            Rarity.Common => new AllyCard(Guid.NewGuid(), $"Common-{Guid.NewGuid():N}"[..12],
                Rarity.Common, 1, 2, 3, 1),
            Rarity.Uncommon => new AllyCard(Guid.NewGuid(), $"Uncommon-{Guid.NewGuid():N}"[..12],
                Rarity.Uncommon, 2, 3, 4, 2),
            Rarity.Rare => new AllyCard(Guid.NewGuid(), $"Rare-{Guid.NewGuid():N}"[..12],
                Rarity.Rare, 3, 5, 6, 3),
            Rarity.Unique => new AllyCard(Guid.NewGuid(), $"Unique-{Guid.NewGuid():N}"[..12],
                Rarity.Unique, 5, 8, 10, 5),
            _ => throw new ArgumentOutOfRangeException(nameof(rarity))
        };
        return Task.FromResult(card);
    }
}
