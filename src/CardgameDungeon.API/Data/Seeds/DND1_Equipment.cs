using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.API.Data.Seeds;

public static partial class CardSetSeeder
{
    private static List<EquipmentCard> CreateEquipment()
    {
        return new List<EquipmentCard>
        {
            // ===== UNIQUE (2) =====
            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000001"),
                "Blackstaff of Waterdeep",
                Rarity.Unique,
                5,
                4, 2, 1),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000002"),
                "Wand of Orcus",
                Rarity.Unique,
                5,
                5, 1, -1),

            // ===== RARE (8) =====
            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000003"),
                "Vorpal Sword",
                Rarity.Rare,
                4,
                4, 0, 1),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000004"),
                "Holy Avenger",
                Rarity.Rare,
                4,
                3, 1, 1),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000005"),
                "Robe of the Archmagi",
                Rarity.Rare,
                3,
                1, 3, 1),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000006"),
                "Shield of the Sentinel",
                Rarity.Rare,
                3,
                0, 4, -1),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000007"),
                "Cloak of Displacement",
                Rarity.Rare,
                3,
                0, 2, 3),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000008"),
                "Boots of Speed",
                Rarity.Rare,
                3,
                1, 0, 4),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000009"),
                "Ring of Regeneration",
                Rarity.Rare,
                4,
                0, 4, 1),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000010"),
                "Staff of Power",
                Rarity.Rare,
                4,
                3, 1, 1),

            // ===== UNCOMMON (20) =====
            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000011"),
                "Longsword +1",
                Rarity.Uncommon,
                2,
                2, 0, 0),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000012"),
                "Shortbow +1",
                Rarity.Uncommon,
                2,
                2, 0, 1),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000013"),
                "Chain Mail +1",
                Rarity.Uncommon,
                3,
                0, 2, -1),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000014"),
                "Shield of Faith",
                Rarity.Uncommon,
                2,
                0, 2, 0),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000015"),
                "Amulet of Health",
                Rarity.Uncommon,
                2,
                0, 2, 0),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000016"),
                "Gauntlets of Ogre Power",
                Rarity.Uncommon,
                3,
                2, 0, -1),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000017"),
                "Cloak of Elvenkind",
                Rarity.Uncommon,
                2,
                0, 1, 2),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000018"),
                "Boots of Elvenkind",
                Rarity.Uncommon,
                2,
                0, 0, 2),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000019"),
                "Ring of Protection",
                Rarity.Uncommon,
                2,
                0, 1, 1),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000020"),
                "Wand of Magic Missiles",
                Rarity.Uncommon,
                2,
                2, 0, 1),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000021"),
                "Helm of Awareness",
                Rarity.Uncommon,
                2,
                0, 1, 2),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000022"),
                "Bracers of Defense",
                Rarity.Uncommon,
                2,
                1, 2, 0),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000023"),
                "Belt of Giant Strength",
                Rarity.Uncommon,
                3,
                2, 1, -1),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000024"),
                "Cape of the Mountebank",
                Rarity.Uncommon,
                2,
                0, 0, 2),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000025"),
                "Periapt of Wound Closure",
                Rarity.Uncommon,
                2,
                0, 2, 0),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000026"),
                "Circlet of Blasting",
                Rarity.Uncommon,
                2,
                2, 0, 0),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000027"),
                "Gloves of Missile Snaring",
                Rarity.Uncommon,
                2,
                1, 1, 1),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000028"),
                "Sentinel Shield",
                Rarity.Uncommon,
                2,
                0, 2, 1),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000029"),
                "Rod of the Pact Keeper",
                Rarity.Uncommon,
                3,
                2, 1, 0),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000030"),
                "Javelin of Lightning",
                Rarity.Uncommon,
                2,
                2, 0, 1),

            // ===== COMMON (30) =====
            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000031"),
                "Wooden Shield",
                Rarity.Common,
                1,
                0, 1, 0),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000032"),
                "Leather Armor",
                Rarity.Common,
                1,
                0, 1, 0),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000033"),
                "Shortsword",
                Rarity.Common,
                1,
                1, 0, 0),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000034"),
                "Longbow",
                Rarity.Common,
                1,
                1, 0, 0),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000035"),
                "Torch of Revealing",
                Rarity.Common,
                1,
                0, 0, 1),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000036"),
                "Rope of Climbing",
                Rarity.Common,
                1,
                0, 0, 1),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000037"),
                "Healing Potion",
                Rarity.Common,
                1,
                0, 1, 0),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000038"),
                "Antidote Vial",
                Rarity.Common,
                1,
                0, 1, 0),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000039"),
                "Iron Rations",
                Rarity.Common,
                1,
                0, 1, 0),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000040"),
                "Grappling Hook",
                Rarity.Common,
                1,
                0, 0, 1),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000041"),
                "Caltrops",
                Rarity.Common,
                1,
                0, 0, 1),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000042"),
                "Oil Flask",
                Rarity.Common,
                1,
                1, 0, 0),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000043"),
                "Signal Whistle",
                Rarity.Common,
                1,
                0, 0, 1),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000044"),
                "Smoke Bomb",
                Rarity.Common,
                1,
                0, 0, 1),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000045"),
                "Flash Powder",
                Rarity.Common,
                1,
                0, 0, 1),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000046"),
                "Throwing Knife",
                Rarity.Common,
                1,
                1, 0, 0),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000047"),
                "Sling Bullets",
                Rarity.Common,
                1,
                1, 0, 0),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000048"),
                "Padded Armor",
                Rarity.Common,
                1,
                0, 1, 0),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000049"),
                "Buckler",
                Rarity.Common,
                1,
                0, 1, 0),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000050"),
                "Studded Leather",
                Rarity.Common,
                1,
                0, 1, 0),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000051"),
                "Hand Crossbow",
                Rarity.Common,
                1,
                1, 0, 0),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000052"),
                "Quarterstaff",
                Rarity.Common,
                1,
                1, 0, 0),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000053"),
                "Dagger +0",
                Rarity.Common,
                1,
                1, 0, 0),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000054"),
                "Light Hammer",
                Rarity.Common,
                1,
                1, 0, 0),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000055"),
                "Sickle",
                Rarity.Common,
                1,
                1, 0, 0),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000056"),
                "Club",
                Rarity.Common,
                1,
                1, 0, 0),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000057"),
                "Handaxe",
                Rarity.Common,
                1,
                1, 0, 0),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000058"),
                "Javelin",
                Rarity.Common,
                1,
                1, 0, 0),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000059"),
                "Mace",
                Rarity.Common,
                1,
                1, 0, 0),

            new EquipmentCard(
                new Guid("00000000-0000-0000-0002-000000000060"),
                "Spear",
                Rarity.Common,
                1,
                1, 0, 0),
        };
    }
}
