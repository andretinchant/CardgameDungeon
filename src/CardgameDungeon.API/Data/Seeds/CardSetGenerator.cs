using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.API.Data.Seeds;

public static class CardSetGenerator
{
    // Rarity distribution: 2 Unique, 8 Rare, 20 Uncommon, 30 Common = 60
    private static readonly Rarity[] RarityDistribution60 = Enumerable.Empty<Rarity>()
        .Concat(Enumerable.Repeat(Rarity.Unique, 2))
        .Concat(Enumerable.Repeat(Rarity.Rare, 8))
        .Concat(Enumerable.Repeat(Rarity.Uncommon, 20))
        .Concat(Enumerable.Repeat(Rarity.Common, 30))
        .ToArray();

    public static CardSet Generate(SetTheme theme, DateOnly releaseDate)
    {
        var setId = MakeGuid(theme.GuidPrefix, 0, 0);
        var set = new CardSet(setId, theme.SetName, theme.Code, releaseDate, theme.Description);

        for (var i = 0; i < theme.Allies.Length; i++)
        {
            var a = theme.Allies[i];
            var rarity = RarityDistribution60[i];
            set.AddCard(new AllyCard(
                MakeGuid(theme.GuidPrefix, 1, i + 1),
                a.Name, rarity, a.Cost, a.Strength, a.HitPoints, a.Initiative,
                a.IsAmbusher, a.Treasure, a.Effect));
        }

        for (var i = 0; i < theme.Equipment.Length; i++)
        {
            var e = theme.Equipment[i];
            var rarity = RarityDistribution60[i];
            set.AddCard(new EquipmentCard(
                MakeGuid(theme.GuidPrefix, 2, i + 1),
                e.Name, rarity, e.Cost, e.StrMod, e.HpMod, e.InitMod, e.Slot, e.Effect));
        }

        if (theme.Consumables is not null)
        {
            for (var i = 0; i < theme.Consumables.Length; i++)
            {
                var c = theme.Consumables[i];
                var rarity = RarityDistribution60[i];
                set.AddCard(new EquipmentCard(
                    MakeGuid(theme.GuidPrefix, 7, i + 1),
                    c.Name, rarity, c.Cost, c.StrMod, c.HpMod, c.InitMod, c.Slot, c.Effect));
            }
        }

        for (var i = 0; i < theme.Monsters.Length; i++)
        {
            var m = theme.Monsters[i];
            var rarity = RarityDistribution60[i];
            set.AddCard(new MonsterCard(
                MakeGuid(theme.GuidPrefix, 3, i + 1),
                m.Name, rarity, m.Cost, m.Strength, m.HitPoints, m.Initiative,
                m.Treasure, m.Effect));
        }

        for (var i = 0; i < theme.Traps.Length; i++)
        {
            var t = theme.Traps[i];
            var rarity = RarityDistribution60[i];
            set.AddCard(new TrapCard(
                MakeGuid(theme.GuidPrefix, 4, i + 1),
                t.Name, rarity, t.Cost, t.Damage, t.Effect));
        }

        var roomOrder = 1;
        for (var i = 0; i < theme.Rooms.Length; i++)
        {
            var r = theme.Rooms[i];
            set.AddCard(new DungeonRoomCard(
                MakeGuid(theme.GuidPrefix, 5, i + 1),
                r.Name, Rarity.Common, roomOrder,
                monsterCostBudget: r.MonsterCostBudget, effect: r.Effect));
            if ((i + 1) % 10 == 0 && roomOrder < 5) roomOrder++;
        }

        for (var i = 0; i < theme.Bosses.Length; i++)
        {
            var b = theme.Bosses[i];
            set.AddCard(new BossCard(
                MakeGuid(theme.GuidPrefix, 6, i + 1),
                b.Name, Rarity.Unique, b.Cost, b.Strength, b.HitPoints, b.Initiative,
                b.Effect));
        }

        return set;
    }

    private static Guid MakeGuid(int prefix, int category, int index)
        => new($"{prefix:D8}-0000-0000-{category:D4}-{index:D12}");
}
