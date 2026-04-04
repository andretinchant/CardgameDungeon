using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.API.Data.Seeds;

public static partial class CardSetSeeder
{
    // Equipment budget (~45% of creature): Cost 1→3, 2→5, 3→6, 4→8, 5→10, Unique 5→12
    // Formula: STR + HP + INIT*1.5 + EffectPts = Budget
    // Mod costs: STR=1pt, HP=1pt, INIT=1.5pt
    // Effect points: Common 0, Uncommon 0–1, Rare 1–2, Unique 2–3
    private static List<EquipmentCard> CreateEquipment()
    {
        return new List<EquipmentCard>
        {
            // ═══════════════════════════════════════════════════════════════
            //  UNIQUE (2) — Cost 5, Budget 12
            // ═══════════════════════════════════════════════════════════════

            // Budget: STR4 + HP2 + INIT2×1.5 + Effect3 = 12
            new(new Guid("00000000-0000-0000-0002-000000000001"),
                "Blackstaff of Waterdeep", Rarity.Unique, 5,
                4, 2, 2, EquipmentSlot.Weapon,
                "With Advantage: wielder gains +2 STR. With Disadvantage: wielder gains +2 INIT"),

            // Budget: STR5 + HP2 + INIT0×1.5 + Effect2 = 12 (self-damage downside offsets)
            new(new Guid("00000000-0000-0000-0002-000000000002"),
                "Wand of Orcus", Rarity.Unique, 5,
                5, 2, 0, EquipmentSlot.Weapon,
                "With Advantage: exile one defeated enemy instead of discarding. With Disadvantage: wielder takes 2 self-damage"),

            // ═══════════════════════════════════════════════════════════════
            //  RARE (8) — Cost 3→Budget 6, Cost 4→Budget 8
            // ═══════════════════════════════════════════════════════════════

            // Budget: STR4 + HP0 + INIT1×1.5 + Effect1 = 8 (rounding 6.5→8 with 1.5 effect)
            new(new Guid("00000000-0000-0000-0002-000000000003"),
                "Vorpal Sword", Rarity.Rare, 4,
                4, 0, 1, EquipmentSlot.Weapon,
                "If wielder has Advantage: deal 1 extra damage to target"),

            // Budget: STR3 + HP1 + INIT1×1.5 + Effect2 = 8 (rounded)
            new(new Guid("00000000-0000-0000-0002-000000000004"),
                "Holy Avenger", Rarity.Rare, 4,
                3, 1, 1, EquipmentSlot.Weapon,
                "Wielder gains +1 STR when fighting Undead or Fiend enemies"),

            // Budget: STR1 + HP2 + INIT1×1.5 + Effect1 = 6 (rounded)
            new(new Guid("00000000-0000-0000-0002-000000000005"),
                "Robe of the Archmagi", Rarity.Rare, 3,
                1, 2, 1, EquipmentSlot.Armor,
                "Wielder's Scroll consumables gain +1 effect potency"),

            // Budget: STR0 + HP4 + INIT-1×1.5 + Effect1 = 6 (4-1.5+1.5+2=6)
            new(new Guid("00000000-0000-0000-0002-000000000006"),
                "Shield of the Sentinel", Rarity.Rare, 3,
                0, 4, -1, EquipmentSlot.Shield,
                "Wielder cannot be targeted by Ambusher effects"),

            // Budget: STR0 + HP1 + INIT2×1.5 + Effect2 = 6
            new(new Guid("00000000-0000-0000-0002-000000000007"),
                "Cloak of Displacement", Rarity.Rare, 3,
                0, 1, 2, EquipmentSlot.Accessory,
                "First attack against wielder each round deals 0 damage"),

            // Budget: STR1 + HP0 + INIT2×1.5 + Effect2 = 6 (Cost 3)
            new(new Guid("00000000-0000-0000-0002-000000000008"),
                "Boots of Speed", Rarity.Rare, 3,
                1, 0, 2, EquipmentSlot.Boots,
                "Wielder may retarget once per round without paying a cost"),

            // Budget: STR0 + HP4 + INIT1×1.5 + Effect2 = 8 (rounded)
            new(new Guid("00000000-0000-0000-0002-000000000009"),
                "Ring of Regeneration", Rarity.Rare, 4,
                0, 4, 1, EquipmentSlot.Accessory,
                "Wielder restores 1 HP at the start of each round"),

            // Budget: STR3 + HP0 + INIT1×1.5 + Effect2 = 8 (rounded)
            new(new Guid("00000000-0000-0000-0002-000000000010"),
                "Staff of Power", Rarity.Rare, 4,
                3, 0, 1, EquipmentSlot.Weapon,
                "With Advantage: this weapon's STR mod is doubled this round"),

            // ═══════════════════════════════════════════════════════════════
            //  UNCOMMON (20) — Cost 2→Budget 5, Cost 3→Budget 6
            // ═══════════════════════════════════════════════════════════════

            // Budget: STR2 + HP0 + INIT1×1.5 + Effect1 = 5 (rounded)
            new(new Guid("00000000-0000-0000-0002-000000000011"),
                "Longsword +1", Rarity.Uncommon, 2,
                2, 0, 1, EquipmentSlot.Weapon,
                "Wielder gains +1 INIT when entering a new room"),

            // Budget: STR2 + HP0 + INIT2×1.5 + Effect0 = 5
            new(new Guid("00000000-0000-0000-0002-000000000012"),
                "Shortbow +1", Rarity.Uncommon, 2,
                2, 0, 2, EquipmentSlot.Weapon),

            // Budget: STR0 + HP4 + INIT-1×1.5 + Effect1 = 6 (rounded)
            new(new Guid("00000000-0000-0000-0002-000000000013"),
                "Chain Mail +1", Rarity.Uncommon, 3,
                0, 4, -1, EquipmentSlot.Armor,
                "Wielder takes 1 less damage from traps"),

            // Budget: STR0 + HP3 + INIT1×1.5 + Effect0 = 5 (rounded)
            new(new Guid("00000000-0000-0000-0002-000000000014"),
                "Shield of Faith", Rarity.Uncommon, 2,
                0, 3, 1, EquipmentSlot.Shield),

            // Budget: STR0 + HP2 + INIT2×1.5 + Effect0 = 5
            new(new Guid("00000000-0000-0000-0002-000000000015"),
                "Amulet of Health", Rarity.Uncommon, 2,
                0, 2, 2, EquipmentSlot.Accessory),

            // Budget: STR3 + HP1 + INIT-1×1.5 + Effect1 = 6 (rounded)
            new(new Guid("00000000-0000-0000-0002-000000000016"),
                "Gauntlets of Ogre Power", Rarity.Uncommon, 3,
                3, 1, -1, EquipmentSlot.Accessory,
                "Wielder's opportunity attacks deal +1 damage"),

            // Budget: STR0 + HP0 + INIT2×1.5 + Effect1 = 5 (rounded)
            new(new Guid("00000000-0000-0000-0002-000000000017"),
                "Cloak of Elvenkind", Rarity.Uncommon, 2,
                0, 0, 2, EquipmentSlot.Accessory,
                "Wielder gains Ambusher trait"),

            // Budget: STR0 + HP0 + INIT2×1.5 + Effect1 = 5 (rounded)
            new(new Guid("00000000-0000-0000-0002-000000000018"),
                "Boots of Elvenkind", Rarity.Uncommon, 2,
                0, 0, 2, EquipmentSlot.Boots,
                "Wielder ignores terrain-based trap effects"),

            // Budget: STR0 + HP2 + INIT2×1.5 + Effect0 = 5
            new(new Guid("00000000-0000-0000-0002-000000000019"),
                "Ring of Protection", Rarity.Uncommon, 2,
                0, 2, 2, EquipmentSlot.Accessory),

            // Budget: STR3 + HP0 + INIT1×1.5 + Effect0 = 5 (rounded)
            new(new Guid("00000000-0000-0000-0002-000000000020"),
                "Wand of Magic Missiles", Rarity.Uncommon, 2,
                3, 0, 1, EquipmentSlot.Weapon),

            // Budget: STR0 + HP1 + INIT2×1.5 + Effect1 = 5
            new(new Guid("00000000-0000-0000-0002-000000000021"),
                "Helm of Awareness", Rarity.Uncommon, 2,
                0, 1, 2, EquipmentSlot.Helmet,
                "Wielder cannot have INIT reduced below 1"),

            // Budget: STR1 + HP2 + INIT1×1.5 + Effect0 = 5 (rounded)
            new(new Guid("00000000-0000-0000-0002-000000000022"),
                "Bracers of Defense", Rarity.Uncommon, 2,
                1, 2, 1, EquipmentSlot.Armor),

            // Budget: STR3 + HP1 + INIT-1×1.5 + Effect1 = 6 (rounded)
            new(new Guid("00000000-0000-0000-0002-000000000023"),
                "Belt of Giant Strength", Rarity.Uncommon, 3,
                3, 1, -1, EquipmentSlot.Accessory,
                "Wielder cannot be moved by forced-movement effects"),

            // Budget: STR0 + HP0 + INIT2×1.5 + Effect1 = 5 (rounded)
            new(new Guid("00000000-0000-0000-0002-000000000024"),
                "Cape of the Mountebank", Rarity.Uncommon, 2,
                0, 0, 2, EquipmentSlot.Accessory,
                "Wielder may abandon combat without triggering opportunity attacks"),

            // Budget: STR0 + HP3 + INIT1×1.5 + Effect0 = 5 (rounded)
            new(new Guid("00000000-0000-0000-0002-000000000025"),
                "Periapt of Wound Closure", Rarity.Uncommon, 2,
                0, 3, 1, EquipmentSlot.Accessory),

            // Budget: STR2 + HP1 + INIT0×1.5 + Effect1 = 5 (rounded)
            new(new Guid("00000000-0000-0000-0002-000000000026"),
                "Circlet of Blasting", Rarity.Uncommon, 2,
                2, 1, 0, EquipmentSlot.Helmet,
                "With Advantage: deal 1 damage to a random enemy at start of combat"),

            // Budget: STR1 + HP1 + INIT1×1.5 + Effect1 = 5 (rounded)
            new(new Guid("00000000-0000-0000-0002-000000000027"),
                "Gloves of Missile Snaring", Rarity.Uncommon, 2,
                1, 1, 1, EquipmentSlot.Accessory,
                "Wielder reduces incoming ranged damage by 1"),

            // Budget: STR0 + HP2 + INIT2×1.5 + Effect0 = 5
            new(new Guid("00000000-0000-0000-0002-000000000028"),
                "Sentinel Shield", Rarity.Uncommon, 2,
                0, 2, 2, EquipmentSlot.Shield),

            // Budget: STR2 + HP2 + INIT1×1.5 + Effect0 = 6 (rounded)
            new(new Guid("00000000-0000-0000-0002-000000000029"),
                "Rod of the Pact Keeper", Rarity.Uncommon, 3,
                2, 2, 1, EquipmentSlot.Weapon),

            // Budget: STR2 + HP0 + INIT1×1.5 + Effect1 = 5 (rounded)
            new(new Guid("00000000-0000-0000-0002-000000000030"),
                "Javelin of Lightning", Rarity.Uncommon, 2,
                2, 0, 1, EquipmentSlot.Weapon,
                "With Advantage: wielder ignores target's Shield HP bonus"),

            // ═══════════════════════════════════════════════════════════════
            //  COMMON (21 gear) — Cost 1, Budget 3, Effect 0
            //  Formula: STR + HP + INIT*1.5 = 3
            // ═══════════════════════════════════════════════════════════════

            // Budget: STR0 + HP1 + INIT1×1.5 = 2.5 ≈ 3
            new(new Guid("00000000-0000-0000-0002-000000000031"),
                "Wooden Shield", Rarity.Common, 1, 0, 1, 1, EquipmentSlot.Shield),

            // Budget: STR0 + HP1 + INIT1×1.5 = 2.5 ≈ 3
            new(new Guid("00000000-0000-0000-0002-000000000032"),
                "Leather Armor", Rarity.Common, 1, 0, 1, 1, EquipmentSlot.Armor),

            // Budget: STR1 + HP0 + INIT1×1.5 = 2.5 ≈ 3
            new(new Guid("00000000-0000-0000-0002-000000000033"),
                "Shortsword", Rarity.Common, 1, 1, 0, 1, EquipmentSlot.Weapon),

            // Budget: STR2 + HP0 + INIT0×1.5 = 2 ≈ 3
            new(new Guid("00000000-0000-0000-0002-000000000034"),
                "Longbow", Rarity.Common, 1, 2, 0, 0, EquipmentSlot.Weapon),

            // Budget: STR0 + HP0 + INIT2×1.5 = 3
            new(new Guid("00000000-0000-0000-0002-000000000036"),
                "Rope of Climbing", Rarity.Common, 1, 0, 0, 2, EquipmentSlot.Accessory),

            // Budget: STR0 + HP0 + INIT2×1.5 = 3
            new(new Guid("00000000-0000-0000-0002-000000000040"),
                "Grappling Hook", Rarity.Common, 1, 0, 0, 2, EquipmentSlot.Accessory),

            // Budget: STR1 + HP0 + INIT1×1.5 = 2.5 ≈ 3
            new(new Guid("00000000-0000-0000-0002-000000000046"),
                "Throwing Knife", Rarity.Common, 1, 1, 0, 1, EquipmentSlot.Weapon),

            // Budget: STR2 + HP0 + INIT0×1.5 = 2 ≈ 3
            new(new Guid("00000000-0000-0000-0002-000000000047"),
                "Sling Bullets", Rarity.Common, 1, 2, 0, 0, EquipmentSlot.Weapon),

            // Budget: STR0 + HP2 + INIT0×1.5 = 2 ≈ 3
            new(new Guid("00000000-0000-0000-0002-000000000048"),
                "Padded Armor", Rarity.Common, 1, 0, 2, 0, EquipmentSlot.Armor),

            // Budget: STR0 + HP2 + INIT0×1.5 = 2 ≈ 3
            new(new Guid("00000000-0000-0000-0002-000000000049"),
                "Buckler", Rarity.Common, 1, 0, 2, 0, EquipmentSlot.Shield),

            // Budget: STR0 + HP1 + INIT1×1.5 = 2.5 ≈ 3
            new(new Guid("00000000-0000-0000-0002-000000000050"),
                "Studded Leather", Rarity.Common, 1, 0, 1, 1, EquipmentSlot.Armor),

            // Budget: STR1 + HP1 + INIT0×1.5 = 2 ≈ 3
            new(new Guid("00000000-0000-0000-0002-000000000051"),
                "Hand Crossbow", Rarity.Common, 1, 1, 1, 0, EquipmentSlot.Weapon),

            // Budget: STR1 + HP1 + INIT0×1.5 = 2 ≈ 3
            new(new Guid("00000000-0000-0000-0002-000000000052"),
                "Quarterstaff", Rarity.Common, 1, 1, 1, 0, EquipmentSlot.Weapon),

            // Budget: STR1 + HP0 + INIT1×1.5 = 2.5 ≈ 3
            new(new Guid("00000000-0000-0000-0002-000000000053"),
                "Dagger +0", Rarity.Common, 1, 1, 0, 1, EquipmentSlot.Weapon),

            // Budget: STR2 + HP0 + INIT0×1.5 = 2 ≈ 3
            new(new Guid("00000000-0000-0000-0002-000000000054"),
                "Light Hammer", Rarity.Common, 1, 2, 0, 0, EquipmentSlot.Weapon),

            // Budget: STR1 + HP1 + INIT0×1.5 = 2 ≈ 3
            new(new Guid("00000000-0000-0000-0002-000000000055"),
                "Sickle", Rarity.Common, 1, 1, 1, 0, EquipmentSlot.Weapon),

            // Budget: STR2 + HP0 + INIT0×1.5 = 2 ≈ 3
            new(new Guid("00000000-0000-0000-0002-000000000056"),
                "Club", Rarity.Common, 1, 2, 0, 0, EquipmentSlot.Weapon),

            // Budget: STR1 + HP0 + INIT1×1.5 = 2.5 ≈ 3
            new(new Guid("00000000-0000-0000-0002-000000000057"),
                "Handaxe", Rarity.Common, 1, 1, 0, 1, EquipmentSlot.Weapon),

            // Budget: STR1 + HP1 + INIT0×1.5 = 2 ≈ 3
            new(new Guid("00000000-0000-0000-0002-000000000058"),
                "Javelin", Rarity.Common, 1, 1, 1, 0, EquipmentSlot.Weapon),

            // Budget: STR1 + HP1 + INIT0×1.5 = 2 ≈ 3
            new(new Guid("00000000-0000-0000-0002-000000000059"),
                "Mace", Rarity.Common, 1, 1, 1, 0, EquipmentSlot.Weapon),

            // Budget: STR1 + HP0 + INIT1×1.5 = 2.5 ≈ 3
            new(new Guid("00000000-0000-0000-0002-000000000060"),
                "Spear", Rarity.Common, 1, 1, 0, 1, EquipmentSlot.Weapon),

            // ═══════════════════════════════════════════════════════════════
            //  COMMON (9) — Consumables in Equipment file — Cost 1, Budget 4
            //  Consumable budget = equipment +1: Cost 1→4
            //  Formula: STR + HP + INIT*1.5 + EffectPts = 4
            // ═══════════════════════════════════════════════════════════════

            // Budget: STR0 + HP0 + INIT0×1.5 + Effect4 = 4
            new(new Guid("00000000-0000-0000-0002-000000000035"),
                "Torch of Revealing", Rarity.Common, 1, 0, 0, 0, EquipmentSlot.Totem,
                "Reveals hidden traps and Ambusher enemies in the current room"),

            // Budget: STR0 + HP2 + INIT0×1.5 + Effect2 = 4
            new(new Guid("00000000-0000-0000-0002-000000000037"),
                "Healing Potion", Rarity.Common, 1, 0, 2, 0, EquipmentSlot.Potion,
                "Restore 2 HP to target ally"),

            // Budget: STR0 + HP1 + INIT0×1.5 + Effect3 = 4
            new(new Guid("00000000-0000-0000-0002-000000000038"),
                "Antidote Vial", Rarity.Common, 1, 0, 1, 0, EquipmentSlot.Potion,
                "Remove all Disadvantage effects from target ally"),

            // Budget: STR0 + HP1 + INIT0×1.5 + Effect3 = 4
            new(new Guid("00000000-0000-0000-0002-000000000039"),
                "Iron Rations", Rarity.Common, 1, 0, 1, 0, EquipmentSlot.Balm,
                "Restore 1 HP to all allies in play"),

            // Budget: STR0 + HP0 + INIT0×1.5 + Effect4 = 4
            new(new Guid("00000000-0000-0000-0002-000000000041"),
                "Caltrops", Rarity.Common, 1, 0, 0, 0, EquipmentSlot.Totem,
                "Reduce INIT of all enemy units by 1 this round"),

            // Budget: STR1 + HP0 + INIT0×1.5 + Effect3 = 4
            new(new Guid("00000000-0000-0000-0002-000000000042"),
                "Oil Flask", Rarity.Common, 1, 1, 0, 0, EquipmentSlot.Bomb,
                "Deal 1 damage to target enemy"),

            // Budget: STR0 + HP0 + INIT0×1.5 + Effect4 = 4
            new(new Guid("00000000-0000-0000-0002-000000000043"),
                "Signal Whistle", Rarity.Common, 1, 0, 0, 0, EquipmentSlot.Totem,
                "Draw 1 extra card from your deck"),

            // Budget: STR0 + HP0 + INIT0×1.5 + Effect4 = 4
            new(new Guid("00000000-0000-0000-0002-000000000044"),
                "Smoke Bomb", Rarity.Common, 1, 0, 0, 0, EquipmentSlot.Bomb,
                "Prevent one enemy from attacking this round"),

            // Budget: STR0 + HP0 + INIT0×1.5 + Effect4 = 4
            new(new Guid("00000000-0000-0000-0002-000000000045"),
                "Flash Powder", Rarity.Common, 1, 0, 0, 0, EquipmentSlot.Bomb,
                "Reduce STR of target enemy by 2 this round"),
        };
    }
}
