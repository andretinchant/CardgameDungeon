using CardgameDungeon.Domain.Effects;
using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.Domain.Entities;

public abstract class Card
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public abstract CardType Type { get; }
    public Rarity Rarity { get; private set; }
    public int Cost { get; private set; }

    /// <summary>
    /// Structured effect tags string. Parsed at runtime by EffectParser.
    /// Null means no scripted effects (card may still have flavor-only Effect text).
    /// </summary>
    public string? EffectTags { get; private set; }

    /// <summary>Lazily parsed effect tags.</summary>
    private IReadOnlyList<EffectTag>? _parsedTags;
    public IReadOnlyList<EffectTag> ParsedEffects => _parsedTags ??= EffectParser.Parse(EffectTags);

    protected Card() { Name = null!; } // EF Core

    protected Card(Guid id, string name, Rarity rarity, int cost, string? effectTags = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Card name cannot be empty.", nameof(name));

        if (cost < 0)
            throw new ArgumentOutOfRangeException(nameof(cost), "Cost cannot be negative.");

        Id = id;
        Name = name;
        Rarity = rarity;
        Cost = cost;
        EffectTags = effectTags;
    }
}
