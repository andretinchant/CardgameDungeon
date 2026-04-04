using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.API.Data.Seeds;

public static partial class CardSetSeeder
{
    private static List<AllyCard> CreateMonks()
    {
        return new List<AllyCard>
        {
            // ═══════════════════════════════════════════════════════
            //  MONKS — Discard/exile from hand to cancel combat, attack twice, or disengage
            //  Budget: Cost 1=6, 2=10, 3=14, 4=18, 6=30(unique)
            // ═══════════════════════════════════════════════════════

            // ── UNIQUE (1) ── Budget 30
            // STR4(4) + HP8(8) + INIT3(4.5) + Treasure1(1) = 17.5. Effect = 8 ✓
            new AllyCard(new Guid("00000000-0000-0000-0009-000000000001"),
                "Kane, the Unbroken Circle", Rarity.Unique, 6,
                4, 8, 3, treasure: 1,
                effect: "The immovable sentinel who holds the line so others may strike. Exile 3 cards from hand: cancel ALL damage in this combat group this round (both sides deal 0). While Kane holds, allies in other combat groups gain +1 STR. This hold can be maintained each round by paying the cost again",
                race: Race.Human, allyClass: AllyClass.Monk),

            // ── RARE (2) ── Budget 18
            // STR4(4) + HP4(4) + INIT2(3) + Treasure1(1) = 12. Effect = 6 ✓
            new AllyCard(new Guid("00000000-0000-0000-0009-000000000002"),
                "Shadow Monk", Rarity.Rare, 4,
                4, 4, 2, treasure: 1,
                effect: "Discard 1 from hand: cancel one combat this ally is in. Exile 1 from hand: this ally cannot be targeted until next round. With Advantage: +2 STR",
                race: Race.Human, allyClass: AllyClass.Monk),

            // STR3(3) + HP5(5) + INIT2(3) + Treasure0(0) = 11. Effect = 3. Budget 14 ✓
            new AllyCard(new Guid("00000000-0000-0000-0009-000000000003"),
                "Kensei Blademaster", Rarity.Rare, 3,
                3, 5, 2, treasure: 0,
                effect: "Exile 1 from hand: attack twice this round. If equipped with Weapon: +1 STR on each attack",
                race: Race.Human, allyClass: AllyClass.Monk),

            // ── UNCOMMON (3) ── Budget 10-14
            // STR3(3) + HP4(4) + INIT2(3) + Treasure0(0) = 10. Effect = 4 ✓ Budget 14
            new AllyCard(new Guid("00000000-0000-0000-0009-000000000004"),
                "Open Hand Adept", Rarity.Uncommon, 3,
                3, 4, 2, treasure: 0,
                effect: "Discard 1 from hand: cancel one combat this ally is in. With Disadvantage: +2 HP",
                race: Race.Human, allyClass: AllyClass.Monk),

            // STR2(2) + HP3(3) + INIT2(3) + Treasure0(0) = 8. Effect = 2. Budget 10 ✓
            new AllyCard(new Guid("00000000-0000-0000-0009-000000000005"),
                "Drunken Fist Brawler", Rarity.Uncommon, 2,
                2, 3, 2, treasure: 0,
                effect: "Discard 1 from hand: disengage without triggering opportunity attack. +1 STR against enemies with higher INIT",
                race: Race.Halfling, allyClass: AllyClass.Monk),

            // STR3(3) + HP3(3) + INIT2(3) + Treasure1(1) = 10. Effect = 4 ✓ Budget 14
            new AllyCard(new Guid("00000000-0000-0000-0009-000000000006"),
                "Sun Soul Disciple", Rarity.Uncommon, 3,
                3, 3, 2, treasure: 1,
                effect: "Exile 1 from hand: deal STR damage to an enemy in a different combat group. +1 STR against Undead",
                race: Race.Human, allyClass: AllyClass.Monk),

            // ── COMMON (4) ── Budget 6-10
            // STR2(2) + HP3(3) + INIT0(0) + Treasure0(0) = 5. Effect = 1 ✓ Budget 6
            new AllyCard(new Guid("00000000-0000-0000-0009-000000000007"),
                "Temple Initiate", Rarity.Common, 1,
                2, 3, 0, treasure: 0,
                effect: "Discard 1 from hand: +1 STR this round",
                race: Race.Human, allyClass: AllyClass.Monk),

            // STR3(3) + HP3(3) + INIT1(1.5) + Treasure1(1) = 8.5. Effect = 1.5 ✓ Budget 10
            new AllyCard(new Guid("00000000-0000-0000-0009-000000000008"),
                "Traveling Ascetic", Rarity.Common, 2,
                3, 3, 1, treasure: 1,
                effect: "Discard 1 from hand: disengage without triggering opportunity attack",
                race: Race.Human, allyClass: AllyClass.Monk),

            // STR1(1) + HP3(3) + INIT1(1.5) + Treasure0(0) = 5.5. Effect = 0.5 ✓ Budget 6
            new AllyCard(new Guid("00000000-0000-0000-0009-000000000009"),
                "Meditation Student", Rarity.Common, 1,
                1, 3, 1, treasure: 0,
                effect: "With Disadvantage: +1 HP",
                race: Race.Elf, allyClass: AllyClass.Monk),

            // STR3(3) + HP4(4) + INIT1(1.5) + Treasure0(0) = 8.5. Effect = 1.5 ✓ Budget 10
            new AllyCard(new Guid("00000000-0000-0000-0009-000000000010"),
                "Street Fighter", Rarity.Common, 2,
                3, 4, 1, treasure: 0,
                effect: "Discard 1 from hand: cancel one combat this ally is in",
                race: Race.Human, allyClass: AllyClass.Monk),
        };
    }
}
