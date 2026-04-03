namespace CardgameDungeon.API.Data.Seeds;

public record SetTheme(
    string SetName,
    string Code,
    string Description,
    int GuidPrefix,
    AllyTheme[] Allies,
    EquipmentTheme[] Equipment,
    MonsterTheme[] Monsters,
    TrapTheme[] Traps,
    RoomTheme[] Rooms,
    BossTheme[] Bosses,
    EquipmentTheme[]? Consumables = null);

public record AllyTheme(string Name, int Strength, int HitPoints, int Initiative, int Cost, int Treasure, bool IsAmbusher, string Effect);
public record EquipmentTheme(string Name, int Cost, int StrMod, int HpMod, int InitMod,
    CardgameDungeon.Domain.Enums.EquipmentSlot Slot = CardgameDungeon.Domain.Enums.EquipmentSlot.Accessory,
    string? Effect = null);
public record MonsterTheme(string Name, int Strength, int HitPoints, int Initiative, int Cost, int Treasure, string Effect);
public record TrapTheme(string Name, int Cost, int Damage, string Effect);
public record RoomTheme(string Name, int MonsterCostBudget, string Effect);
public record BossTheme(string Name, int Strength, int HitPoints, int Initiative, int Cost, string Effect);
