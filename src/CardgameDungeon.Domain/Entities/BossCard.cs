using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.Domain.Entities;

public class BossCard : Card
{
    public override CardType Type => CardType.Boss;
    public Race Race { get; private set; }
    public int Strength { get; private set; }
    public int HitPoints { get; private set; }
    public int Initiative { get; private set; }
    public string? Effect { get; private set; }

    private BossCard() { } // EF Core

    public BossCard(
        Guid id,
        string name,
        Rarity rarity,
        int cost,
        int strength,
        int hitPoints,
        int initiative,
        string? effect = null,
        Race race = Race.Dragon)
        : base(id, name, rarity, cost)
    {
        if (strength <= 0)
            throw new ArgumentOutOfRangeException(nameof(strength), "Boss strength must be positive.");
        if (hitPoints <= 0)
            throw new ArgumentOutOfRangeException(nameof(hitPoints), "Boss hit points must be positive.");
        if (initiative < 0)
            throw new ArgumentOutOfRangeException(nameof(initiative));

        Race = race;
        Strength = strength;
        HitPoints = hitPoints;
        Initiative = initiative;
        Effect = effect;
    }
}
