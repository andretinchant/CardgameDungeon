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
                "With Advantage: wielder gains +2 STR. With Disadvantage: wielder gains +2 INIT",
                effectTags: "PASSIVE|+STR:4|+HP:2|+INIT:2\nWITH_ADVANTAGE|+STR:2\nWITH_DISADVANTAGE|+INIT:2"),

            // Budget: STR5 + HP2 + INIT0×1.5 + Effect2 = 12 (self-damage downside offsets)
            new(new Guid("00000000-0000-0000-0002-000000000002"),
                "Wand of Orcus", Rarity.Unique, 5,
                5, 2, 0, EquipmentSlot.Weapon,
                "With Advantage: exile one defeated enemy instead of discarding. With Disadvantage: wielder takes 2 self-damage",
                effectTags: "PASSIVE|+STR:5|+HP:2\nWITH_ADVANTAGE|EXILE_TARGET\nWITH_DISADVANTAGE|DAMAGE:2:SELF"),

            // ═══════════════════════════════════════════════════════════════
            //  RARE (8) — Cost 3→Budget 6, Cost 4→Budget 8
            // ═══════════════════════════════════════════════════════════════

            // Budget: STR4 + HP0 + INIT1×1.5 + Effect1 = 8 (rounding 6.5→8 with 1.5 effect)
            new(new Guid("00000000-0000-0000-0002-000000000003"),
                "Vorpal Sword", Rarity.Rare, 4,
                4, 0, 1, EquipmentSlot.Weapon,
                "If wielder has Advantage: deal 1 extra damage to target",
                effectTags: "PASSIVE|+STR:4|+INIT:1\nWITH_ADVANTAGE|DAMAGE:1:ENEMY"),

            // Budget: STR3 + HP1 + INIT1×1.5 + Effect2 = 8 (rounded)
            new(new Guid("00000000-0000-0000-0002-000000000004"),
                "Holy Avenger", Rarity.Rare, 4,
                3, 1, 1, EquipmentSlot.Weapon,
                "Wielder gains +1 STR when fighting Undead or Fiend enemies",
                effectTags: "PASSIVE|+STR:3|+HP:1|+INIT:1\nPASSIVE|IF_RACE:Undead|+STR:1\nPASSIVE|IF_RACE:Demon|+STR:1"),

            // Budget: STR1 + HP2 + INIT1×1.5 + Effect1 = 6 (rounded)
            new(new Guid("00000000-0000-0000-0002-000000000005"),
                "Robe of the Archmagi", Rarity.Rare, 3,
                1, 2, 1, EquipmentSlot.Armor,
                "Wielder's Scroll consumables gain +1 effect potency",
                effectTags: "PASSIVE|+STR:1|+HP:2|+INIT:1\nPASSIVE|SCROLL_POTENCY:1"),

            // Budget: STR0 + HP4 + INIT-1×1.5 + Effect1 = 6 (4-1.5+1.5+2=6)
            new(new Guid("00000000-0000-0000-0002-000000000006"),
                "Shield of the Sentinel", Rarity.Rare, 3,
                0, 4, -1, EquipmentSlot.Shield,
                "Wielder cannot be targeted by Ambusher effects",
                effectTags: "PASSIVE|+HP:4|-INIT:1\nPASSIVE|IMMUNE_AMBUSHER"),

            // Budget: STR0 + HP1 + INIT2×1.5 + Effect2 = 6
            new(new Guid("00000000-0000-0000-0002-000000000007"),
                "Cloak of Displacement", Rarity.Rare, 3,
                0, 1, 2, EquipmentSlot.Accessory,
                "First attack against wielder each round deals 0 damage",
                effectTags: "PASSIVE|+HP:1|+INIT:2\nPASSIVE|NEGATE_FIRST_ATTACK"),

            // Budget: STR1 + HP0 + INIT2×1.5 + Effect2 = 6 (Cost 3)
            new(new Guid("00000000-0000-0000-0002-000000000008"),
                "Boots of Speed", Rarity.Rare, 3,
                1, 0, 2, EquipmentSlot.Boots,
                "Wielder may retarget once per round without paying a cost",
                effectTags: "PASSIVE|+STR:1|+INIT:2\nPASSIVE|FREE_RETARGET"),

            // Budget: STR0 + HP4 + INIT1×1.5 + Effect2 = 8 (rounded)
            new(new Guid("00000000-0000-0000-0002-000000000009"),
                "Ring of Regeneration", Rarity.Rare, 4,
                0, 4, 1, EquipmentSlot.Accessory,
                "Wielder restores 1 HP at the start of each round",
                effectTags: "PASSIVE|+HP:4|+INIT:1\nON_ROUND_START|HEAL:1:SELF"),

            // Budget: STR3 + HP0 + INIT1×1.5 + Effect2 = 8 (rounded)
            new(new Guid("00000000-0000-0000-0002-000000000010"),
                "Staff of Power", Rarity.Rare, 4,
                3, 0, 1, EquipmentSlot.Weapon,
                "With Advantage: this weapon's STR mod is doubled this round",
                effectTags: "PASSIVE|+STR:3|+INIT:1\nWITH_ADVANTAGE|DOUBLE_STR"),

            // ═══════════════════════════════════════════════════════════════
            //  UNCOMMON (20) — Cost 2→Budget 5, Cost 3→Budget 6
            // ═══════════════════════════════════════════════════════════════

            // Budget: STR2 + HP0 + INIT1×1.5 + Effect1 = 5 (rounded)
            new(new Guid("00000000-0000-0000-0002-000000000011"),
                "Longsword +1", Rarity.Uncommon, 2,
                2, 0, 1, EquipmentSlot.Weapon,
                "Wielder gains +1 INIT when entering a new room",
                effectTags: "PASSIVE|+STR:2|+INIT:1\nON_PLAY|+INIT:1"),

            // Budget: STR2 + HP0 + INIT2×1.5 + Effect0 = 5
            new(new Guid("00000000-0000-0000-0002-000000000012"),
                "Shortbow +1", Rarity.Uncommon, 2,
                2, 0, 2, EquipmentSlot.Weapon,
                effectTags: "PASSIVE|+STR:2|+INIT:2"),

            // Budget: STR0 + HP4 + INIT-1×1.5 + Effect1 = 6 (rounded)
            new(new Guid("00000000-0000-0000-0002-000000000013"),
                "Chain Mail +1", Rarity.Uncommon, 3,
                0, 4, -1, EquipmentSlot.Armor,
                "Wielder takes 1 less damage from traps",
                effectTags: "PASSIVE|+HP:4|-INIT:1\nPASSIVE|REDUCE_TRAP_DAMAGE:1"),

            // Budget: STR0 + HP3 + INIT1×1.5 + Effect0 = 5 (rounded)
            new(new Guid("00000000-0000-0000-0002-000000000014"),
                "Shield of Faith", Rarity.Uncommon, 2,
                0, 3, 1, EquipmentSlot.Shield,
                effectTags: "PASSIVE|+HP:3|+INIT:1"),

            // Budget: STR0 + HP2 + INIT2×1.5 + Effect0 = 5
            new(new Guid("00000000-0000-0000-0002-000000000015"),
                "Amulet of Health", Rarity.Uncommon, 2,
                0, 2, 2, EquipmentSlot.Accessory,
                effectTags: "PASSIVE|+HP:2|+INIT:2"),

            // Budget: STR3 + HP1 + INIT-1×1.5 + Effect1 = 6 (rounded)
            new(new Guid("00000000-0000-0000-0002-000000000016"),
                "Gauntlets of Ogre Power", Rarity.Uncommon, 3,
                3, 1, -1, EquipmentSlot.Accessory,
                "Wielder's opportunity attacks deal +1 damage",
                effectTags: "PASSIVE|+STR:3|+HP:1|-INIT:1\nPASSIVE|OPP_ATTACK_DAMAGE:1"),

            // Budget: STR0 + HP0 + INIT2×1.5 + Effect1 = 5 (rounded)
            new(new Guid("00000000-0000-0000-0002-000000000017"),
                "Cloak of Elvenkind", Rarity.Uncommon, 2,
                0, 0, 2, EquipmentSlot.Accessory,
                "Wielder gains Ambusher trait",
                effectTags: "PASSIVE|+INIT:2\nPASSIVE|AMBUSHER"),

            // Budget: STR0 + HP0 + INIT2×1.5 + Effect1 = 5 (rounded)
            new(new Guid("00000000-0000-0000-0002-000000000018"),
                "Boots of Elvenkind", Rarity.Uncommon, 2,
                0, 0, 2, EquipmentSlot.Boots,
                "Wielder ignores terrain-based trap effects",
                effectTags: "PASSIVE|+INIT:2\nPASSIVE|IMMUNE_TERRAIN_TRAP"),

            // Budget: STR0 + HP2 + INIT2×1.5 + Effect0 = 5
            new(new Guid("00000000-0000-0000-0002-000000000019"),
                "Ring of Protection", Rarity.Uncommon, 2,
                0, 2, 2, EquipmentSlot.Accessory,
                effectTags: "PASSIVE|+HP:2|+INIT:2"),

            // Budget: STR3 + HP0 + INIT1×1.5 + Effect0 = 5 (rounded)
            new(new Guid("00000000-0000-0000-0002-000000000020"),
                "Wand of Magic Missiles", Rarity.Uncommon, 2,
                3, 0, 1, EquipmentSlot.Weapon,
                effectTags: "PASSIVE|+STR:3|+INIT:1"),

            // Budget: STR0 + HP1 + INIT2×1.5 + Effect1 = 5
            new(new Guid("00000000-0000-0000-0002-000000000021"),
                "Helm of Awareness", Rarity.Uncommon, 2,
                0, 1, 2, EquipmentSlot.Helmet,
                "Wielder cannot have INIT reduced below 1",
                effectTags: "PASSIVE|+HP:1|+INIT:2\nPASSIVE|MIN_INIT:1"),

            // Budget: STR1 + HP2 + INIT1×1.5 + Effect0 = 5 (rounded)
            new(new Guid("00000000-0000-0000-0002-000000000022"),
                "Bracers of Defense", Rarity.Uncommon, 2,
                1, 2, 1, EquipmentSlot.Armor,
                effectTags: "PASSIVE|+STR:1|+HP:2|+INIT:1"),

            // Budget: STR3 + HP1 + INIT-1×1.5 + Effect1 = 6 (rounded)
            new(new Guid("00000000-0000-0000-0002-000000000023"),
                "Belt of Giant Strength", Rarity.Uncommon, 3,
                3, 1, -1, EquipmentSlot.Accessory,
                "Wielder cannot be moved by forced-movement effects",
                effectTags: "PASSIVE|+STR:3|+HP:1|-INIT:1\nPASSIVE|IMMUNE_FORCED_MOVEMENT"),

            // Budget: STR0 + HP0 + INIT2×1.5 + Effect1 = 5 (rounded)
            new(new Guid("00000000-0000-0000-0002-000000000024"),
                "Cape of the Mountebank", Rarity.Uncommon, 2,
                0, 0, 2, EquipmentSlot.Accessory,
                "Wielder may abandon combat without triggering opportunity attacks",
                effectTags: "PASSIVE|+INIT:2\nPASSIVE|IMMUNE_OPP_ATTACK"),

            // Budget: STR0 + HP3 + INIT1×1.5 + Effect0 = 5 (rounded)
            new(new Guid("00000000-0000-0000-0002-000000000025"),
                "Periapt of Wound Closure", Rarity.Uncommon, 2,
                0, 3, 1, EquipmentSlot.Accessory,
                effectTags: "PASSIVE|+HP:3|+INIT:1"),

            // Budget: STR2 + HP1 + INIT0×1.5 + Effect1 = 5 (rounded)
            new(new Guid("00000000-0000-0000-0002-000000000026"),
                "Circlet of Blasting", Rarity.Uncommon, 2,
                2, 1, 0, EquipmentSlot.Helmet,
                "With Advantage: deal 1 damage to a random enemy at start of combat",
                effectTags: "PASSIVE|+STR:2|+HP:1\nWITH_ADVANTAGE|ON_COMBAT_START|DAMAGE:1:ENEMY"),

            // Budget: STR1 + HP1 + INIT1×1.5 + Effect1 = 5 (rounded)
            new(new Guid("00000000-0000-0000-0002-000000000027"),
                "Gloves of Missile Snaring", Rarity.Uncommon, 2,
                1, 1, 1, EquipmentSlot.Accessory,
                "Wielder reduces incoming ranged damage by 1",
                effectTags: "PASSIVE|+STR:1|+HP:1|+INIT:1\nPASSIVE|REDUCE_RANGED_DAMAGE:1"),

            // Budget: STR0 + HP2 + INIT2×1.5 + Effect0 = 5
            new(new Guid("00000000-0000-0000-0002-000000000028"),
                "Sentinel Shield", Rarity.Uncommon, 2,
                0, 2, 2, EquipmentSlot.Shield,
                effectTags: "PASSIVE|+HP:2|+INIT:2"),

            // Budget: STR2 + HP2 + INIT1×1.5 + Effect0 = 6 (rounded)
            new(new Guid("00000000-0000-0000-0002-000000000029"),
                "Rod of the Pact Keeper", Rarity.Uncommon, 3,
                2, 2, 1, EquipmentSlot.Weapon,
                effectTags: "PASSIVE|+STR:2|+HP:2|+INIT:1"),

            // Budget: STR2 + HP0 + INIT1×1.5 + Effect1 = 5 (rounded)
            new(new Guid("00000000-0000-0000-0002-000000000030"),
                "Javelin of Lightning", Rarity.Uncommon, 2,
                2, 0, 1, EquipmentSlot.Weapon,
                "With Advantage: wielder ignores target's Shield HP bonus",
                effectTags: "PASSIVE|+STR:2|+INIT:1\nWITH_ADVANTAGE|IGNORE_SHIELD"),

            // ═══════════════════════════════════════════════════════════════
            //  COMMON (21 gear) — Cost 1, Budget 3, Effect 0
            //  Formula: STR + HP + INIT*1.5 = 3
            // ═══════════════════════════════════════════════════════════════

            // Budget: STR0 + HP1 + INIT1×1.5 = 2.5 ≈ 3
            new(new Guid("00000000-0000-0000-0002-000000000031"),
                "Wooden Shield", Rarity.Common, 1, 0, 1, 1, EquipmentSlot.Shield,
                effectTags: "PASSIVE|+HP:1|+INIT:1"),

            // Budget: STR0 + HP1 + INIT1×1.5 = 2.5 ≈ 3
            new(new Guid("00000000-0000-0000-0002-000000000032"),
                "Leather Armor", Rarity.Common, 1, 0, 1, 1, EquipmentSlot.Armor,
                effectTags: "PASSIVE|+HP:1|+INIT:1"),

            // Budget: STR1 + HP0 + INIT1×1.5 = 2.5 ≈ 3
            new(new Guid("00000000-0000-0000-0002-000000000033"),
                "Shortsword", Rarity.Common, 1, 1, 0, 1, EquipmentSlot.Weapon,
                effectTags: "PASSIVE|+STR:1|+INIT:1"),

            // Budget: STR2 + HP0 + INIT0×1.5 = 2 ≈ 3
            new(new Guid("00000000-0000-0000-0002-000000000034"),
                "Longbow", Rarity.Common, 1, 2, 0, 0, EquipmentSlot.Weapon,
                effectTags: "PASSIVE|+STR:2"),

            // Budget: STR0 + HP2 + INIT0×1.5 = 2 ≈ 3
            new(new Guid("00000000-0000-0000-0002-000000000048"),
                "Padded Armor", Rarity.Common, 1, 0, 2, 0, EquipmentSlot.Armor,
                effectTags: "PASSIVE|+HP:2"),

            // Budget: STR0 + HP2 + INIT0×1.5 = 2 ≈ 3
            new(new Guid("00000000-0000-0000-0002-000000000049"),
                "Buckler", Rarity.Common, 1, 0, 2, 0, EquipmentSlot.Shield,
                effectTags: "PASSIVE|+HP:2"),

            // ═══════════════════════════════════════════════════════════════
            //  SHAPESHIFTS (5) — Druid only. Always cost 2. Equip budget = 5.
            //  Replaces Druid on field with beast form. Druid returns when form dies.
            //  STR/HP/INIT modifiers become the form's actual stats.
            // ═══════════════════════════════════════════════════════════════

            // Bear Form — tanky, high HP
            new(new Guid("00000000-0000-0000-0002-000000000036"),
                "Bear Form", Rarity.Uncommon, 2, 4, 5, 0, EquipmentSlot.Shapeshift,
                effect: "Druid only. Replaces Druid with Bear Form (STR 4, HP 5, INIT 0). When eliminated: Druid returns with all equipment. With Disadvantage: +2 HP",
                effectTags: "PASSIVE|REQUIRE_CLASS:Druid\nON_ACTIVATE|MATERIALIZE_ALLY:STR4:HP5:INIT0\nWITH_DISADVANTAGE|+HP:2"),

            // Wolf Form — balanced, initiative
            new(new Guid("00000000-0000-0000-0002-000000000040"),
                "Dire Wolf Form", Rarity.Common, 2, 3, 3, 1, EquipmentSlot.Shapeshift,
                effect: "Druid only. Replaces Druid with Dire Wolf Form (STR 3, HP 3, INIT 1). When eliminated: Druid returns. With Advantage: +2 STR",
                effectTags: "PASSIVE|REQUIRE_CLASS:Druid\nON_ACTIVATE|MATERIALIZE_ALLY:STR3:HP3:INIT1\nWITH_ADVANTAGE|+STR:2"),

            // Eagle Form — fast, scouting
            new(new Guid("00000000-0000-0000-0002-000000000046"),
                "Giant Eagle Form", Rarity.Common, 2, 2, 2, 2, EquipmentSlot.Shapeshift,
                effect: "Druid only. Replaces Druid with Giant Eagle Form (STR 2, HP 2, INIT 2). When eliminated: Druid returns. Reveals top 2 cards of opponent's deck",
                effectTags: "PASSIVE|REQUIRE_CLASS:Druid\nON_ACTIVATE|MATERIALIZE_ALLY:STR2:HP2:INIT2\nON_PLAY|REVEAL_DECK:2"),

            // Elemental Form — rare, powerful
            new(new Guid("00000000-0000-0000-0002-000000000047"),
                "Fire Elemental Form", Rarity.Uncommon, 2, 5, 3, 0, EquipmentSlot.Shapeshift,
                effect: "Druid only. Replaces Druid with Fire Elemental Form (STR 5, HP 3, INIT 0). When eliminated: Druid returns and deal 2 damage to all enemies in combat group",
                effectTags: "PASSIVE|REQUIRE_CLASS:Druid\nON_ACTIVATE|MATERIALIZE_ALLY:STR5:HP3:INIT0\nON_DEATH|DAMAGE:2:ENEMY_GROUP"),

            // Spider Form — ambusher
            new(new Guid("00000000-0000-0000-0002-000000000050"),
                "Giant Spider Form", Rarity.Common, 2, 3, 3, 1, EquipmentSlot.Shapeshift,
                effect: "Druid only. Replaces Druid with Giant Spider Form (STR 3, HP 3, INIT 1). Gains Ambusher. When eliminated: Druid returns. Target enemy -1 INIT (web)",
                effectTags: "PASSIVE|REQUIRE_CLASS:Druid\nON_ACTIVATE|MATERIALIZE_ALLY:STR3:HP3:INIT1:AMBUSHER\nON_COMBAT_START|-INIT:1:ENEMY"),

            // ═══════════════════════════════════════════════════════════════
            //  COMPANIONS (5) — Equip on Ranger/Druid. Materializes as ally in combat.
            //  Cost 1, Budget 3. No slot limit. Extra ally (ignores 5-limit).
            // ═══════════════════════════════════════════════════════════════

            new(new Guid("00000000-0000-0000-0002-000000000051"),
                "Wolf Companion", Rarity.Common, 1, 2, 1, 0, EquipmentSlot.Companion,
                effect: "Equip on Ranger or Druid. In combat: materializes as ally (STR 2, HP 1). Does not count toward 5-ally limit. With Advantage: +1 STR",
                effectTags: "PASSIVE|REQUIRE_CLASS:Ranger,Druid\nON_COMBAT_START|MATERIALIZE_ALLY:STR2:HP1\nPASSIVE|IGNORE_ALLY_LIMIT\nWITH_ADVANTAGE|+STR:1"),

            new(new Guid("00000000-0000-0000-0002-000000000052"),
                "Eagle Companion", Rarity.Common, 1, 1, 1, 1, EquipmentSlot.Companion,
                effect: "Equip on Ranger or Druid. In combat: materializes as ally (STR 1, HP 1, INIT 1). Does not count toward 5-ally limit. Reveals top card of opponent deck",
                effectTags: "PASSIVE|REQUIRE_CLASS:Ranger,Druid\nON_COMBAT_START|MATERIALIZE_ALLY:STR1:HP1:INIT1\nPASSIVE|IGNORE_ALLY_LIMIT\nON_PLAY|REVEAL_DECK:1"),

            new(new Guid("00000000-0000-0000-0002-000000000053"),
                "Bear Companion", Rarity.Uncommon, 1, 2, 2, 0, EquipmentSlot.Companion,
                effect: "Equip on Ranger or Druid. In combat: materializes as ally (STR 2, HP 2). Does not count toward 5-ally limit. With Disadvantage: +1 STR and +1 HP",
                effectTags: "PASSIVE|REQUIRE_CLASS:Ranger,Druid\nON_COMBAT_START|MATERIALIZE_ALLY:STR2:HP2\nPASSIVE|IGNORE_ALLY_LIMIT\nWITH_DISADVANTAGE|+STR:1|+HP:1"),

            new(new Guid("00000000-0000-0000-0002-000000000054"),
                "Hawk Familiar", Rarity.Common, 1, 1, 1, 1, EquipmentSlot.Companion,
                effect: "Equip on Ranger or Druid. In combat: materializes as ally (STR 1, HP 1, INIT 1). Does not count toward 5-ally limit",
                effectTags: "PASSIVE|REQUIRE_CLASS:Ranger,Druid\nON_COMBAT_START|MATERIALIZE_ALLY:STR1:HP1:INIT1\nPASSIVE|IGNORE_ALLY_LIMIT"),

            new(new Guid("00000000-0000-0000-0002-000000000055"),
                "Spider Companion", Rarity.Uncommon, 1, 1, 2, 0, EquipmentSlot.Companion,
                effect: "Equip on Ranger or Druid. In combat: materializes as ally (STR 1, HP 2). Does not count toward 5-ally limit. Target enemy -1 INIT (web)",
                effectTags: "PASSIVE|REQUIRE_CLASS:Ranger,Druid\nON_COMBAT_START|MATERIALIZE_ALLY:STR1:HP2\nPASSIVE|IGNORE_ALLY_LIMIT\nON_COMBAT_START|-INIT:1:ENEMY"),

            // ═══════════════════════════════════════════════════════════════
            //  SUMMONS (5) — Equip on Mage/Sorcerer. Materializes as ally in combat.
            //  Cost 1, Budget 3. No slot limit. Extra ally (ignores 5-limit).
            // ═══════════════════════════════════════════════════════════════

            new(new Guid("00000000-0000-0000-0002-000000000056"),
                "Fire Elemental Spark", Rarity.Common, 1, 2, 1, 0, EquipmentSlot.Summon,
                effect: "Equip on Mage or Sorcerer. In combat: materializes as ally (STR 2, HP 1). Does not count toward 5-ally limit. On death: deal 2 damage to killer",
                effectTags: "PASSIVE|REQUIRE_CLASS:Mage,Sorcerer\nON_COMBAT_START|MATERIALIZE_ALLY:STR2:HP1\nPASSIVE|IGNORE_ALLY_LIMIT\nON_DEATH|DAMAGE:2:ENEMY"),

            new(new Guid("00000000-0000-0000-0002-000000000057"),
                "Earth Elemental Shard", Rarity.Common, 1, 1, 2, 0, EquipmentSlot.Summon,
                effect: "Equip on Mage or Sorcerer. In combat: materializes as ally (STR 1, HP 2). Does not count toward 5-ally limit. Reduces damage to carrier by 1",
                effectTags: "PASSIVE|REQUIRE_CLASS:Mage,Sorcerer\nON_COMBAT_START|MATERIALIZE_ALLY:STR1:HP2\nPASSIVE|IGNORE_ALLY_LIMIT\nPASSIVE|REDUCE_DAMAGE:1"),

            new(new Guid("00000000-0000-0000-0002-000000000058"),
                "Arcane Construct", Rarity.Uncommon, 1, 2, 1, 0, EquipmentSlot.Summon,
                effect: "Equip on Mage or Sorcerer. In combat: materializes as ally (STR 2, HP 1). Does not count toward 5-ally limit. Immune to trap effects",
                effectTags: "PASSIVE|REQUIRE_CLASS:Mage,Sorcerer\nON_COMBAT_START|MATERIALIZE_ALLY:STR2:HP1\nPASSIVE|IGNORE_ALLY_LIMIT\nPASSIVE|IMMUNE_TRAP"),

            new(new Guid("00000000-0000-0000-0002-000000000059"),
                "Shadow Servant", Rarity.Uncommon, 1, 1, 1, 1, EquipmentSlot.Summon,
                effect: "Equip on Mage or Sorcerer. In combat: materializes as ally (STR 1, HP 1, INIT 1). Does not count toward 5-ally limit. Ambusher. With Advantage: +1 STR",
                effectTags: "PASSIVE|REQUIRE_CLASS:Mage,Sorcerer\nON_COMBAT_START|MATERIALIZE_ALLY:STR1:HP1:INIT1:AMBUSHER\nPASSIVE|IGNORE_ALLY_LIMIT\nWITH_ADVANTAGE|+STR:1"),

            new(new Guid("00000000-0000-0000-0002-000000000060"),
                "Familiar Imp", Rarity.Common, 1, 1, 1, 0, EquipmentSlot.Summon,
                effect: "Equip on Mage or Sorcerer. In combat: materializes as ally (STR 1, HP 1). Does not count toward 5-ally limit. Draw 1 card when equipped ally enters combat",
                effectTags: "PASSIVE|REQUIRE_CLASS:Mage,Sorcerer\nON_COMBAT_START|MATERIALIZE_ALLY:STR1:HP1\nPASSIVE|IGNORE_ALLY_LIMIT\nON_PLAY|DRAW:1"),

            // ═══════════════════════════════════════════════════════════════
            //  COMMON (9) — Consumables in Equipment file — Cost 1, Budget 4
            //  Consumable budget = equipment +1: Cost 1→4
            //  Formula: STR + HP + INIT*1.5 + EffectPts = 4
            // ═══════════════════════════════════════════════════════════════

            // Budget: STR0 + HP0 + INIT0×1.5 + Effect4 = 4
            new(new Guid("00000000-0000-0000-0002-000000000035"),
                "Torch of Revealing", Rarity.Common, 1, 0, 0, 0, EquipmentSlot.Totem,
                "Reveals hidden traps and Ambusher enemies in the current room",
                effectTags: "ON_ACTIVATE|REVEAL_TRAPS|REVEAL_AMBUSHER"),

            // Budget: STR0 + HP2 + INIT0×1.5 + Effect2 = 4
            new(new Guid("00000000-0000-0000-0002-000000000037"),
                "Healing Potion", Rarity.Common, 1, 0, 2, 0, EquipmentSlot.Potion,
                "Restore 2 HP to target ally",
                effectTags: "ON_ACTIVATE|HEAL:2:ALLY"),

            // Budget: STR0 + HP1 + INIT0×1.5 + Effect3 = 4
            new(new Guid("00000000-0000-0000-0002-000000000038"),
                "Antidote Vial", Rarity.Common, 1, 0, 1, 0, EquipmentSlot.Potion,
                "Remove all Disadvantage effects from target ally",
                effectTags: "ON_ACTIVATE|REMOVE_DISADVANTAGE:ALLY"),

            // Budget: STR0 + HP1 + INIT0×1.5 + Effect3 = 4
            new(new Guid("00000000-0000-0000-0002-000000000039"),
                "Iron Rations", Rarity.Common, 1, 0, 1, 0, EquipmentSlot.Balm,
                "Restore 1 HP to all allies in play",
                effectTags: "ON_ACTIVATE|HEAL:1:ALL_ALLIES"),

            // Budget: STR0 + HP0 + INIT0×1.5 + Effect4 = 4
            new(new Guid("00000000-0000-0000-0002-000000000041"),
                "Caltrops", Rarity.Common, 1, 0, 0, 0, EquipmentSlot.Totem,
                "Reduce INIT of all enemy units by 1 this round",
                effectTags: "ON_ACTIVATE|-INIT:1:ALL_ENEMIES"),

            // Budget: STR1 + HP0 + INIT0×1.5 + Effect3 = 4
            new(new Guid("00000000-0000-0000-0002-000000000042"),
                "Oil Flask", Rarity.Common, 1, 1, 0, 0, EquipmentSlot.Bomb,
                "Deal 1 damage to target enemy",
                effectTags: "ON_ACTIVATE|DAMAGE:1:ENEMY"),

            // Budget: STR0 + HP0 + INIT0×1.5 + Effect4 = 4
            new(new Guid("00000000-0000-0000-0002-000000000043"),
                "Signal Whistle", Rarity.Common, 1, 0, 0, 0, EquipmentSlot.Totem,
                "Draw 1 extra card from your deck",
                effectTags: "ON_ACTIVATE|DRAW:1"),

            // Budget: STR0 + HP0 + INIT0×1.5 + Effect4 = 4
            new(new Guid("00000000-0000-0000-0002-000000000044"),
                "Smoke Bomb", Rarity.Common, 1, 0, 0, 0, EquipmentSlot.Bomb,
                "Prevent one enemy from attacking this round",
                effectTags: "ON_ACTIVATE|STUN:ENEMY"),

            // Budget: STR0 + HP0 + INIT0×1.5 + Effect4 = 4
            new(new Guid("00000000-0000-0000-0002-000000000045"),
                "Flash Powder", Rarity.Common, 1, 0, 0, 0, EquipmentSlot.Bomb,
                "Reduce STR of target enemy by 2 this round",
                effectTags: "ON_ACTIVATE|-STR:2:ENEMY"),
        };
    }
}
