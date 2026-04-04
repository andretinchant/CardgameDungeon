namespace CardgameDungeon.Domain.Enums;

public enum EquipmentSlot
{
    // Gear — limited to 1 per slot per ally
    Weapon,
    Armor,
    Shield,
    Helmet,
    Boots,
    Accessory,

    // Companions — equip on Ranger/Druid. Materializes as extra ally in combat.
    // No slot limit. Does not count toward 5-ally limit.
    Companion,

    // Summons — equip on Mage/Sorcerer. Materializes as extra ally in combat.
    // No slot limit. Does not count toward 5-ally limit.
    Summon,

    // Shapeshift — equip on Druid only. Always cost 2.
    // Replaces the Druid on the field with the Shapeshift form (has own ATK/HP/INIT).
    // Druid and all equipment are "stored inside" the form.
    // When the Shapeshift form is eliminated, the Druid returns with all equipment.
    Shapeshift,

    // Consumables — no equip limit, single-use, destroyed after activation
    Scroll,
    Potion,
    Balm,
    Bomb,
    Totem
}

public static class EquipmentSlotExtensions
{
    public static bool IsConsumable(this EquipmentSlot slot) => slot >= EquipmentSlot.Scroll;

    public static bool IsGear(this EquipmentSlot slot) => slot <= EquipmentSlot.Accessory;

    public static bool IsCompanionOrSummon(this EquipmentSlot slot) =>
        slot is EquipmentSlot.Companion or EquipmentSlot.Summon;

    public static bool IsShapeshift(this EquipmentSlot slot) => slot == EquipmentSlot.Shapeshift;
}
