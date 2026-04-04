using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.API.Data.Seeds;

public static partial class CardSetSeeder
{
    private static List<AllyCard> CreateSorcerers()
    {
        return new List<AllyCard>
        {
            // ═══════════════════════════════════════════════════════
            //  SORCERERS — Amplify, copy, and recover Scrolls
            //  Budget: Cost 1=6, 2=10, 3=14, 4=18, 6=30(unique)
            // ═══════════════════════════════════════════════════════

            // ── UNIQUE (1) ── Budget 30
            // STR5(5) + HP6(6) + INIT3(4.5) + Treasure2(2) = 17.5. Effect = 8 ✓
            new AllyCard(new Guid("00000000-0000-0000-000B-000000000001"),
                "Archmage of the Weave", Rarity.Unique, 6,
                5, 6, 3, treasure: 2,
                effect: "When a Scroll is used by any ally: copy its effect (apply it a second time for free). Discard 2 from hand: retrieve 1 Scroll from exile to hand. All Scrolls used by allies cost 1 less",
                race: Race.Human, allyClass: AllyClass.Sorcerer),

            // ── RARE (2) ── Budget 14-18
            // STR3(3) + HP4(4) + INIT2(3) + Treasure1(1) = 11. Effect = 7 → 6 ✓ Budget 18
            // Readjust: STR4(4) + HP4(4) + INIT2(3) + Treasure1(1) = 12. Effect = 6 ✓
            new AllyCard(new Guid("00000000-0000-0000-000B-000000000002"),
                "Wild Magic Sorcerer", Rarity.Rare, 4,
                4, 4, 2, treasure: 1,
                effect: "When a Scroll is used: reveal top card of deck. If it's an ally: +2 STR to all allies. If monster/trap: opponent draws 1 fewer card next room. Discard 1: recover 1 Scroll from discard to hand",
                race: Race.Human, allyClass: AllyClass.Sorcerer),

            // STR2(2) + HP3(3) + INIT2(3) + Treasure1(1) = 9. Effect = 5 ✓ Budget 14
            new AllyCard(new Guid("00000000-0000-0000-000B-000000000003"),
                "Draconic Bloodline Sorcerer", Rarity.Rare, 3,
                2, 3, 2, treasure: 1,
                effect: "Exile 1 from hand: a Scroll used this round has its numeric effects increased by +2. +1 STR against Dragon enemies. If Dragonborn: +1 HP",
                race: Race.Dragonborn, allyClass: AllyClass.Sorcerer),

            // ── UNCOMMON (3) ── Budget 10-14
            // STR3(3) + HP4(4) + INIT1(1.5) + Treasure1(1) = 9.5. Effect = 4.5 ✓ Budget 14
            new AllyCard(new Guid("00000000-0000-0000-000B-000000000004"),
                "Storm Sorcerer", Rarity.Uncommon, 3,
                3, 4, 1, treasure: 1,
                effect: "When a Scroll is used by this ally: deal 1 damage to all enemies in this combat group. Discard 1: recover 1 Scroll from discard to hand",
                race: Race.Human, allyClass: AllyClass.Sorcerer),

            // STR2(2) + HP3(3) + INIT1(1.5) + Treasure1(1) = 7.5. Effect = 2.5 ✓ Budget 10
            new AllyCard(new Guid("00000000-0000-0000-000B-000000000005"),
                "Shadow Sorcerer", Rarity.Uncommon, 2,
                2, 3, 1, treasure: 1,
                effect: "Exile 1 from hand: copy the last Scroll used this round (apply its effect again)",
                race: Race.Human, allyClass: AllyClass.Sorcerer),

            // STR3(3) + HP3(3) + INIT2(3) + Treasure1(1) = 10. Effect = 4 ✓ Budget 14
            new AllyCard(new Guid("00000000-0000-0000-000B-000000000006"),
                "Clockwork Soul Sorcerer", Rarity.Uncommon, 3,
                3, 3, 2, treasure: 1,
                effect: "Scrolls used by allies in this combat group have their numeric effects increased by +1. Discard 2: recover 1 Scroll from discard to hand",
                race: Race.Human, allyClass: AllyClass.Sorcerer),

            // ── COMMON (4) ── Budget 6-10
            // STR2(2) + HP2(2) + INIT1(1.5) + Treasure0(0) = 5.5. Effect = 0.5 ✓ Budget 6
            new AllyCard(new Guid("00000000-0000-0000-000B-000000000007"),
                "Innate Caster", Rarity.Common, 1,
                2, 2, 1, treasure: 0,
                effect: "Discard 1: recover 1 Scroll with cost 1 from discard to hand",
                race: Race.Human, allyClass: AllyClass.Sorcerer),

            // STR3(3) + HP3(3) + INIT1(1.5) + Treasure1(1) = 8.5. Effect = 1.5 ✓ Budget 10
            new AllyCard(new Guid("00000000-0000-0000-000B-000000000008"),
                "Arcane Prodigy", Rarity.Common, 2,
                3, 3, 1, treasure: 1,
                effect: "When a Scroll is used by this ally: +1 STR this round",
                race: Race.Elf, allyClass: AllyClass.Sorcerer),

            // STR1(1) + HP3(3) + INIT1(1.5) + Treasure0(0) = 5.5. Effect = 0.5 ✓ Budget 6
            new AllyCard(new Guid("00000000-0000-0000-000B-000000000009"),
                "Spark Mage", Rarity.Common, 1,
                1, 3, 1, treasure: 0,
                effect: "With Advantage: +1 STR when a Scroll was used this round",
                race: Race.Halfling, allyClass: AllyClass.Sorcerer),

            // STR3(3) + HP4(4) + INIT1(1.5) + Treasure0(0) = 8.5. Effect = 1.5 ✓ Budget 10
            new AllyCard(new Guid("00000000-0000-0000-000B-000000000010"),
                "Bloodline Adept", Rarity.Common, 2,
                3, 4, 1, treasure: 0,
                effect: "Discard 1: a Scroll used this round has +1 to all numeric effects",
                race: Race.Dragonborn, allyClass: AllyClass.Sorcerer),
        };
    }
}
