using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.Domain.Entities;

public class MonsterCard : Card
{
    public override CardType Type => CardType.Monster;
    public int Strength { get; private set; }
    public int HitPoints { get; private set; }
    public int Initiative { get; private set; }
    public int Treasure { get; private set; }
    public string? Effect { get; private set; }

    private MonsterCard() { } // EF Core

    public MonsterCard(
        Guid id,
        string name,
        Rarity rarity,
        int cost,
        int strength,
        int hitPoints,
        int initiative,
        int treasure = 0,
        string? effect = null)
        : base(id, name, rarity, cost)
    {
        if (strength < 0)
            throw new ArgumentOutOfRangeException(nameof(strength));
        if (hitPoints <= 0)
            throw new ArgumentOutOfRangeException(nameof(hitPoints), "Hit points must be positive.");
        if (initiative < 0)
            throw new ArgumentOutOfRangeException(nameof(initiative));

        Strength = strength;
        HitPoints = hitPoints;
        Initiative = initiative;
        Treasure = treasure;
        Effect = effect;
    }
}
