using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.API.Data.Seeds;

public static partial class CardSetSeeder
{
    // Companions: require Ranger or Druid in play. Cost 1. Don't count toward 5-ally limit.
    // No Unique or Rare. Budget: Cost 1 = 6.
    private static List<AllyCard> CreateCompanions()
    {
        return new List<AllyCard>
        {
            // ═══ UNCOMMON (5) ═══

            // Budget 6: STR2(2) + HP2(2) + INIT1(1.5) = 5.5. Effect = 0.5 ✓
            new AllyCard(new Guid("00000000-0000-0000-000D-000000000001"),
                "Dire Wolf", Rarity.Uncommon, 1, 2, 2, 1,
                effect: "Requires Ranger or Druid in play. Does not count toward 5-ally limit. With Advantage: +1 STR",
                race: Race.Beast, allyClass: AllyClass.Companion,
                effectTags: "PASSIVE|REQUIRE_CLASS:Ranger,Druid\nPASSIVE|IGNORE_ALLY_LIMIT\nWITH_ADVANTAGE|+STR:1"),

            // Budget 6: STR1(1) + HP3(3) + INIT1(1.5) = 5.5. Effect = 0.5 ✓
            new AllyCard(new Guid("00000000-0000-0000-000D-000000000002"),
                "Giant Eagle", Rarity.Uncommon, 1, 1, 3, 1,
                effect: "Requires Ranger or Druid in play. Does not count toward 5-ally limit. Reveals top card of opponent's deck",
                race: Race.Beast, allyClass: AllyClass.Companion,
                effectTags: "PASSIVE|REQUIRE_CLASS:Ranger,Druid\nPASSIVE|IGNORE_ALLY_LIMIT\nON_PLAY|REVEAL_DECK:1"),

            // Budget 6: STR2(2) + HP2(2) + INIT0(0) = 4. Effect = 2 ✓
            new AllyCard(new Guid("00000000-0000-0000-000D-000000000003"),
                "Brown Bear", Rarity.Uncommon, 1, 2, 2, 0,
                effect: "Requires Ranger or Druid in play. Does not count toward 5-ally limit. With Disadvantage: +1 STR and +1 HP",
                race: Race.Beast, allyClass: AllyClass.Companion,
                effectTags: "PASSIVE|REQUIRE_CLASS:Ranger,Druid\nPASSIVE|IGNORE_ALLY_LIMIT\nWITH_DISADVANTAGE|+STR:1|+HP:1"),

            // Budget 6: STR1(1) + HP2(2) + INIT1(1.5) = 4.5. Effect = 1.5 ✓
            new AllyCard(new Guid("00000000-0000-0000-000D-000000000004"),
                "Giant Spider Companion", Rarity.Uncommon, 1, 1, 2, 1,
                effect: "Requires Ranger or Druid in play. Does not count toward 5-ally limit. Target enemy loses 1 INIT this round (web)",
                race: Race.Beast, allyClass: AllyClass.Companion,
                effectTags: "PASSIVE|REQUIRE_CLASS:Ranger,Druid\nPASSIVE|IGNORE_ALLY_LIMIT\nON_COMBAT_START|-INIT:1:ENEMY"),

            // Budget 6: STR1(1) + HP1(1) + INIT2(3) = 5. Effect = 1 ✓
            new AllyCard(new Guid("00000000-0000-0000-000D-000000000005"),
                "Hawk Familiar", Rarity.Uncommon, 1, 1, 1, 2,
                effect: "Requires Ranger or Druid in play. Does not count toward 5-ally limit. All allies gain +1 INIT this round",
                race: Race.Beast, allyClass: AllyClass.Companion,
                effectTags: "PASSIVE|REQUIRE_CLASS:Ranger,Druid\nPASSIVE|IGNORE_ALLY_LIMIT\nPASSIVE|+INIT:1:ALL_ALLIES"),

            // ═══ COMMON (5) ═══

            // Budget 6: STR2(2) + HP2(2) + INIT0(0) = 4. Effect = 0 ✓ (stat stick)
            new AllyCard(new Guid("00000000-0000-0000-000D-000000000006"),
                "Wolf Pack Runner", Rarity.Common, 1, 2, 2, 0,
                effect: "Requires Ranger or Druid in play. Does not count toward 5-ally limit",
                race: Race.Beast, allyClass: AllyClass.Companion,
                effectTags: "PASSIVE|REQUIRE_CLASS:Ranger,Druid\nPASSIVE|IGNORE_ALLY_LIMIT"),

            // Budget 6: STR1(1) + HP3(3) + INIT0(0) = 4. Effect = 0 ✓
            new AllyCard(new Guid("00000000-0000-0000-000D-000000000007"),
                "Guard Dog", Rarity.Common, 1, 1, 3, 0,
                effect: "Requires Ranger or Druid in play. Does not count toward 5-ally limit",
                race: Race.Beast, allyClass: AllyClass.Companion,
                effectTags: "PASSIVE|REQUIRE_CLASS:Ranger,Druid\nPASSIVE|IGNORE_ALLY_LIMIT"),

            // Budget 6: STR1(1) + HP2(2) + INIT1(1.5) = 4.5. Effect = 0 ✓
            new AllyCard(new Guid("00000000-0000-0000-000D-000000000008"),
                "Riding Horse", Rarity.Common, 1, 1, 2, 1,
                effect: "Requires Ranger or Druid in play. Does not count toward 5-ally limit",
                race: Race.Beast, allyClass: AllyClass.Companion,
                effectTags: "PASSIVE|REQUIRE_CLASS:Ranger,Druid\nPASSIVE|IGNORE_ALLY_LIMIT"),

            // Budget 6: STR2(2) + HP1(1) + INIT1(1.5) = 4.5. Effect = 0 ✓
            new AllyCard(new Guid("00000000-0000-0000-000D-000000000009"),
                "Hunting Falcon", Rarity.Common, 1, 2, 1, 1,
                effect: "Requires Ranger or Druid in play. Does not count toward 5-ally limit",
                race: Race.Beast, allyClass: AllyClass.Companion,
                effectTags: "PASSIVE|REQUIRE_CLASS:Ranger,Druid\nPASSIVE|IGNORE_ALLY_LIMIT"),

            // Budget 6: STR1(1) + HP2(2) + INIT0(0) = 3. Effect = 1 ✓
            new AllyCard(new Guid("00000000-0000-0000-000D-000000000010"),
                "Toad Familiar", Rarity.Common, 1, 1, 2, 0,
                effect: "Requires Ranger or Druid in play. Does not count toward 5-ally limit. Detects 1 trap in current room",
                race: Race.Beast, allyClass: AllyClass.Companion,
                effectTags: "PASSIVE|REQUIRE_CLASS:Ranger,Druid\nPASSIVE|IGNORE_ALLY_LIMIT\nON_PLAY|DETECT_TRAP:1"),
        };
    }
}
