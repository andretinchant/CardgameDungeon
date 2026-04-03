using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.Domain.Entities;

public class TrapCard : Card
{
    public override CardType Type => CardType.Trap;
    public int Damage { get; private set; }
    public string Effect { get; private set; }

    private TrapCard() { Effect = null!; } // EF Core

    public TrapCard(
        Guid id,
        string name,
        Rarity rarity,
        int cost,
        int damage,
        string effect)
        : base(id, name, rarity, cost)
    {
        if (damage < 0)
            throw new ArgumentOutOfRangeException(nameof(damage));
        if (string.IsNullOrWhiteSpace(effect))
            throw new ArgumentException("Trap effect cannot be empty.", nameof(effect));

        Damage = damage;
        Effect = effect;
    }
}
