using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.API.Data.Seeds;

public static partial class CardSetSeeder
{
    private static List<AllyCard> CreateBarbarians()
    {
        return new List<AllyCard>
        {
            // ═══════════════════════════════════════════════════════
            //  BARBARIANS — Highest STR/HP, Rage via exile for:
            //    +STR, damage reduction, double STR for elimination
            //  Budget: Cost 1=6, 2=10, 3=14, 4=18, 6=30(unique)
            //  Barbarians invest almost everything in STR+HP, minimal INIT
            // ═══════════════════════════════════════════════════════

            // ── UNIQUE (1) ── Budget 30
            // STR9(9) + HP9(9) + INIT0(0) + Treasure5(5) = 23. Effect = 7 ✓
            new AllyCard(new Guid("00000000-0000-0000-000C-000000000001"),
                "Uthgar, Blood of the Ancestors", Rarity.Unique, 6,
                9, 9, 0, treasure: 5,
                effect: "Born from a bloodline that predates the gods. Exile 4 cards from top of deck once per combat: +2 STR until end of this combat and Uthgar's STR counts as double for elimination checks. If Uthgar eliminates a monster while raging, he can join another fight immediately. While raging, Uthgar forfeits all Treasure from eliminated enemies",
                effectTags: "ON_ACTIVATE|COST:EXILE_DECK:4|+STR:2|ELIM_DOUBLE|ONCE_PER_COMBAT\nON_KILL|IF_RAGING|JOIN_COMBAT|FORFEIT_TREASURE",
                race: Race.Human, allyClass: AllyClass.Barbarian),

            // ── RARE (2) ── Budget 14-18
            // STR6(6) + HP6(6) + INIT1(1.5) + Treasure0(0) = 13.5. Effect = 4.5 ✓ Budget 18
            new AllyCard(new Guid("00000000-0000-0000-000C-000000000002"),
                "Frostborn Berserker", Rarity.Rare, 4,
                6, 6, 1, treasure: 0,
                effect: "Exile 1 from hand: enter Rage (+2 STR, take 1 less damage). While raging, STR counts as double for elimination checks against enemies with lower HP than this ally",
                race: Race.Human, allyClass: AllyClass.Barbarian,
                effectTags: "ON_ACTIVATE|COST:EXILE_HAND:1|+STR:2|REDUCE_DAMAGE:1\nIF_RAGING|ELIM_DOUBLE"),

            // STR4(4) + HP5(5) + INIT1(1.5) + Treasure0(0) = 10.5. Effect = 3.5 ✓ Budget 14
            new AllyCard(new Guid("00000000-0000-0000-000C-000000000003"),
                "Totem Warrior", Rarity.Rare, 3,
                4, 5, 1, treasure: 0,
                effect: "Exile 1 from hand: enter Rage (+2 STR, take 1 less damage this round). If Beast race ally in play: also gain +2 HP while raging",
                race: Race.Human, allyClass: AllyClass.Barbarian,
                effectTags: "ON_ACTIVATE|COST:EXILE_HAND:1|+STR:2|REDUCE_DAMAGE:1\nIF_RAGING|IF_RACE:Beast|+HP:2"),

            // ── UNCOMMON (3) ── Budget 10-14
            // STR5(5) + HP5(5) + INIT1(1.5) + Treasure0(0) = 11.5. Effect = 2.5 ✓ Budget 14
            new AllyCard(new Guid("00000000-0000-0000-000C-000000000004"),
                "Half-Orc Rager", Rarity.Uncommon, 3,
                5, 5, 1, treasure: 0,
                effect: "Exile 1 from hand: +2 STR and take 1 less damage this round. When reduced to 0 HP while raging: survive with 1 HP instead",
                race: Race.Orc, allyClass: AllyClass.Barbarian,
                effectTags: "ON_ACTIVATE|COST:EXILE_HAND:1|+STR:2|REDUCE_DAMAGE:1\nON_DEATH|IF_RAGING|+HP:1"),

            // STR3(3) + HP4(4) + INIT1(1.5) + Treasure0(0) = 8.5. Effect = 1.5 ✓ Budget 10
            new AllyCard(new Guid("00000000-0000-0000-000C-000000000005"),
                "Tribal Fury", Rarity.Uncommon, 2,
                3, 4, 1, treasure: 0,
                effect: "Exile 1 from hand: +2 STR this round. With Disadvantage: +1 HP",
                race: Race.Human, allyClass: AllyClass.Barbarian,
                effectTags: "ON_ACTIVATE|COST:EXILE_HAND:1|+STR:2\nWITH_DISADVANTAGE|+HP:1"),

            // STR5(5) + HP4(4) + INIT1(1.5) + Treasure1(1) = 11.5. Effect = 2.5 ✓ Budget 14
            new AllyCard(new Guid("00000000-0000-0000-000C-000000000006"),
                "Goliath Ravager", Rarity.Uncommon, 3,
                5, 4, 1, treasure: 1,
                effect: "Exile 1 from hand: +2 STR and STR counts as double for elimination checks this round",
                race: Race.Giant, allyClass: AllyClass.Barbarian,
                effectTags: "ON_ACTIVATE|COST:EXILE_HAND:1|+STR:2|ELIM_DOUBLE"),

            // ── COMMON (4) ── Budget 6-10
            // STR3(3) + HP3(3) + INIT0(0) + Treasure0(0) = 6. Effect = 0 ✓ Budget 6
            new AllyCard(new Guid("00000000-0000-0000-000C-000000000007"),
                "Reckless Brute", Rarity.Common, 1,
                3, 3, 0, treasure: 0,
                effect: "Exile 1 from hand: +1 STR this round",
                race: Race.Orc, allyClass: AllyClass.Barbarian,
                effectTags: "ON_ACTIVATE|COST:EXILE_HAND:1|+STR:1"),

            // STR4(4) + HP4(4) + INIT0(0) + Treasure1(1) = 9. Effect = 1 ✓ Budget 10
            new AllyCard(new Guid("00000000-0000-0000-000C-000000000008"),
                "Pit Brawler", Rarity.Common, 2,
                4, 4, 0, treasure: 1,
                effect: "Exile 1 from hand: +2 STR this round",
                race: Race.Human, allyClass: AllyClass.Barbarian,
                effectTags: "ON_ACTIVATE|COST:EXILE_HAND:1|+STR:2"),

            // STR2(2) + HP3(3) + INIT0(0) + Treasure0(0) = 5. Effect = 1 ✓ Budget 6
            new AllyCard(new Guid("00000000-0000-0000-000C-000000000009"),
                "Feral Savage", Rarity.Common, 1,
                2, 3, 0, treasure: 0,
                effect: "Exile 1 from hand: take 1 less damage this round",
                race: Race.Orc, allyClass: AllyClass.Barbarian,
                effectTags: "ON_ACTIVATE|COST:EXILE_HAND:1|REDUCE_DAMAGE:1"),

            // STR4(4) + HP4(4) + INIT1(1.5) + Treasure0(0) = 9.5. Effect = 0.5 ✓ Budget 10
            new AllyCard(new Guid("00000000-0000-0000-000C-000000000010"),
                "Berserker Recruit", Rarity.Common, 2,
                4, 4, 1, treasure: 0,
                effect: "Exile 1 from hand: +1 STR and +1 HP this round",
                race: Race.Human, allyClass: AllyClass.Barbarian,
                effectTags: "ON_ACTIVATE|COST:EXILE_HAND:1|+STR:1|+HP:1"),
        };
    }
}
