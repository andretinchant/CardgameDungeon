using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.Domain.Entities;

public abstract class Card
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public abstract CardType Type { get; }
    public Rarity Rarity { get; private set; }
    public int Cost { get; private set; }

    protected Card(Guid id, string name, Rarity rarity, int cost)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Card name cannot be empty.", nameof(name));

        if (cost < 0)
            throw new ArgumentOutOfRangeException(nameof(cost), "Cost cannot be negative.");

        Id = id;
        Name = name;
        Rarity = rarity;
        Cost = cost;
    }
}
