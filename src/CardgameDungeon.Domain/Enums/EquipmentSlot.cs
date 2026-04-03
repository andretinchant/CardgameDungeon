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

    public static bool IsGear(this EquipmentSlot slot) => slot < EquipmentSlot.Scroll;
}
