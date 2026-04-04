using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.API.Data.Seeds;

public static partial class CardSetSeeder
{
    // Summons: require Mage or Sorcerer in play. Cost 1. Don't count toward 5-ally limit.
    // No Unique or Rare. Budget: Cost 1 = 6.
    private static List<AllyCard> CreateSummons()
    {
        return new List<AllyCard>
        {
            // ═══ UNCOMMON (5) ═══

            // Budget 6: STR2(2) + HP2(2) + INIT0(0) = 4. Effect = 2 ✓
            new AllyCard(new Guid("00000000-0000-0000-000E-000000000001"),
                "Fire Elemental Spark", Rarity.Uncommon, 1, 2, 2, 0,
                effect: "Requires Mage or Sorcerer in play. Does not count toward 5-ally limit. On death: deal 2 damage to the enemy that killed it",
                race: Race.Elemental, allyClass: AllyClass.Summon,
                effectTags: "PASSIVE|REQUIRE_CLASS:Mage,Sorcerer\nPASSIVE|IGNORE_ALLY_LIMIT\nON_DEATH|DAMAGE:2:ENEMY"),

            // Budget 6: STR1(1) + HP3(3) + INIT0(0) = 4. Effect = 2 ✓
            new AllyCard(new Guid("00000000-0000-0000-000E-000000000002"),
                "Earth Elemental Shard", Rarity.Uncommon, 1, 1, 3, 0,
                effect: "Requires Mage or Sorcerer in play. Does not count toward 5-ally limit. Reduces damage to one adjacent ally by 1",
                race: Race.Elemental, allyClass: AllyClass.Summon,
                effectTags: "PASSIVE|REQUIRE_CLASS:Mage,Sorcerer\nPASSIVE|IGNORE_ALLY_LIMIT\nPASSIVE|REDUCE_DAMAGE:1:ALLY"),

            // Budget 6: STR1(1) + HP1(1) + INIT2(3) = 5. Effect = 1 ✓
            new AllyCard(new Guid("00000000-0000-0000-000E-000000000003"),
                "Air Elemental Wisp", Rarity.Uncommon, 1, 1, 1, 2,
                effect: "Requires Mage or Sorcerer in play. Does not count toward 5-ally limit. All Scrolls used this round gain +1 to effects",
                race: Race.Elemental, allyClass: AllyClass.Summon,
                effectTags: "PASSIVE|REQUIRE_CLASS:Mage,Sorcerer\nPASSIVE|IGNORE_ALLY_LIMIT\nON_SCROLL_USED|+STR:1"),

            // Budget 6: STR2(2) + HP1(1) + INIT1(1.5) = 4.5. Effect = 1.5 ✓
            new AllyCard(new Guid("00000000-0000-0000-000E-000000000004"),
                "Arcane Construct", Rarity.Uncommon, 1, 2, 1, 1,
                effect: "Requires Mage or Sorcerer in play. Does not count toward 5-ally limit. Immune to trap effects",
                race: Race.Construct, allyClass: AllyClass.Summon,
                effectTags: "PASSIVE|REQUIRE_CLASS:Mage,Sorcerer\nPASSIVE|IGNORE_ALLY_LIMIT\nPASSIVE|IMMUNE_TRAP"),

            // Budget 6: STR1(1) + HP2(2) + INIT1(1.5) = 4.5. Effect = 1.5 ✓
            new AllyCard(new Guid("00000000-0000-0000-000E-000000000005"),
                "Shadow Servant", Rarity.Uncommon, 1, 1, 2, 1,
                effect: "Requires Mage or Sorcerer in play. Does not count toward 5-ally limit. With Advantage: +1 STR. Ambusher",
                race: Race.Undead, allyClass: AllyClass.Summon,
                isAmbusher: true,
                effectTags: "PASSIVE|REQUIRE_CLASS:Mage,Sorcerer\nPASSIVE|IGNORE_ALLY_LIMIT\nWITH_ADVANTAGE|+STR:1"),

            // ═══ COMMON (5) ═══

            // Budget 6: STR2(2) + HP2(2) + INIT0(0) = 4. Effect = 0 ✓
            new AllyCard(new Guid("00000000-0000-0000-000E-000000000006"),
                "Mage Hand Construct", Rarity.Common, 1, 2, 2, 0,
                effect: "Requires Mage or Sorcerer in play. Does not count toward 5-ally limit",
                race: Race.Construct, allyClass: AllyClass.Summon,
                effectTags: "PASSIVE|REQUIRE_CLASS:Mage,Sorcerer\nPASSIVE|IGNORE_ALLY_LIMIT"),

            // Budget 6: STR1(1) + HP3(3) + INIT0(0) = 4. Effect = 0 ✓
            new AllyCard(new Guid("00000000-0000-0000-000E-000000000007"),
                "Water Elemental Drop", Rarity.Common, 1, 1, 3, 0,
                effect: "Requires Mage or Sorcerer in play. Does not count toward 5-ally limit",
                race: Race.Elemental, allyClass: AllyClass.Summon,
                effectTags: "PASSIVE|REQUIRE_CLASS:Mage,Sorcerer\nPASSIVE|IGNORE_ALLY_LIMIT"),

            // Budget 6: STR1(1) + HP2(2) + INIT1(1.5) = 4.5. Effect = 0 ✓
            new AllyCard(new Guid("00000000-0000-0000-000E-000000000008"),
                "Spectral Hand", Rarity.Common, 1, 1, 2, 1,
                effect: "Requires Mage or Sorcerer in play. Does not count toward 5-ally limit",
                race: Race.Undead, allyClass: AllyClass.Summon,
                effectTags: "PASSIVE|REQUIRE_CLASS:Mage,Sorcerer\nPASSIVE|IGNORE_ALLY_LIMIT"),

            // Budget 6: STR2(2) + HP1(1) + INIT1(1.5) = 4.5. Effect = 0 ✓
            new AllyCard(new Guid("00000000-0000-0000-000E-000000000009"),
                "Dancing Sword", Rarity.Common, 1, 2, 1, 1,
                effect: "Requires Mage or Sorcerer in play. Does not count toward 5-ally limit",
                race: Race.Construct, allyClass: AllyClass.Summon,
                effectTags: "PASSIVE|REQUIRE_CLASS:Mage,Sorcerer\nPASSIVE|IGNORE_ALLY_LIMIT"),

            // Budget 6: STR1(1) + HP2(2) + INIT0(0) = 3. Effect = 1 ✓
            new AllyCard(new Guid("00000000-0000-0000-000E-000000000010"),
                "Familiar Imp", Rarity.Common, 1, 1, 2, 0,
                effect: "Requires Mage or Sorcerer in play. Does not count toward 5-ally limit. Draw 1 card when this summon enters play",
                race: Race.Demon, allyClass: AllyClass.Summon,
                effectTags: "PASSIVE|REQUIRE_CLASS:Mage,Sorcerer\nPASSIVE|IGNORE_ALLY_LIMIT\nON_PLAY|DRAW:1"),
        };
    }
}
