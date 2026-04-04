using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.Domain.Entities;

public class EquipmentCard : Card
{
    public override CardType Type => CardType.Equipment;
    public EquipmentSlot Slot { get; private set; }
    public int StrengthModifier { get; private set; }
    public int HitPointsModifier { get; private set; }
    public int InitiativeModifier { get; private set; }
    public string? Effect { get; private set; }

    /// <summary>
    /// Consumables (Scroll, Potion, Balm, Bomb, Totem) have no equip limit
    /// and are destroyed after a single use. Gear slots are limited to 1 per ally.
    /// </summary>
    public bool IsConsumable => Slot.IsConsumable();

    private EquipmentCard() { } // EF Core

    public EquipmentCard(
        Guid id,
        string name,
        Rarity rarity,
        int cost,
        int strengthModifier,
        int hitPointsModifier,
        int initiativeModifier,
        EquipmentSlot slot = EquipmentSlot.Accessory,
        string? effect = null,
        string? effectTags = null)
        : base(id, name, rarity, cost, effectTags)
    {
        Slot = slot;
        StrengthModifier = strengthModifier;
        HitPointsModifier = hitPointsModifier;
        InitiativeModifier = initiativeModifier;
        Effect = effect;
    }
}
