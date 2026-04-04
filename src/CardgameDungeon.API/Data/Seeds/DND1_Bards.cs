using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.API.Data.Seeds;

public static partial class CardSetSeeder
{
    private static List<AllyCard> CreateBards()
    {
        return new List<AllyCard>
        {
            // ═════════════════════════════════════════════════════
            //  BARDS — Card recycling: return cards from hand to deck
            //  Budget: Cost 1=6, 2=10, 3=14, 4=18, 5=26(unique)
            // ═════════════════════════════════════════════════════

            // ── UNIQUE (1) ──
            // Budget 30 (Unique cost 6): STR5(5) + HP6(6) + INIT3(4.5) + Treasure1(1) = 16.5
            // Effect budget: 30 - 16.5 = 13.5 → capped at 8 ✓ (extra into stats)
            // Readjust: STR 5(5) + HP 7(7) + INIT 3(4.5) + Treasure 2(2) = 18.5
            // Effect budget: 30 - 18.5 = 11.5 → 8 ✓
            new AllyCard(new Guid("00000000-0000-0000-0008-000000000001"),
                "Volo the Chronicler", Rarity.Unique, 6,
                5, 7, 3, treasure: 2,
                effect: "The pen that rewrites fate. Return up to 3 cards from hand to top of deck in any order, then draw 2 cards. Exile 8 cards from deck: look at opponent's hand and return 1 of their cards to top of their deck. All Bard allies gain +1 INIT",
                race: Race.Human, allyClass: AllyClass.Bard),

            // ── RARE (2) ──
            // Budget 18: STR3(3) + HP4(4) + INIT2(3) + Treasure1(1) = 11. Effect = 7 → capped at 6 ✓
            new AllyCard(new Guid("00000000-0000-0000-0008-000000000002"),
                "Danilo Thann", Rarity.Rare, 4,
                3, 4, 2, treasure: 1,
                effect: "Return up to 2 cards from hand to bottom of deck, then draw 2 cards. If a Bard is already in play: draw 3 instead",
                race: Race.Human, allyClass: AllyClass.Bard),

            // Budget 14: STR2(2) + HP3(3) + INIT2(3) + Treasure0(0) = 8. Effect = 6 ✓
            new AllyCard(new Guid("00000000-0000-0000-0008-000000000003"),
                "Lliira's Songweaver", Rarity.Rare, 3,
                2, 3, 2, treasure: 0,
                effect: "Return 1 equipment card from hand to top of deck. All allies gain +1 INIT this round. If Cleric in play: also restore 1 HP to each ally",
                race: Race.Elf, allyClass: AllyClass.Bard),

            // ── UNCOMMON (3) ──
            // Budget 14: STR3(3) + HP4(4) + INIT1(1.5) + Treasure1(1) = 9.5. Effect = 4.5 → 4 ✓
            new AllyCard(new Guid("00000000-0000-0000-0008-000000000004"),
                "Half-Elf Minstrel", Rarity.Uncommon, 3,
                3, 4, 1, treasure: 1,
                effect: "Return 1 card from hand to top of deck, then draw 1 card. With Advantage: return 2 and draw 2 instead",
                race: Race.Elf, allyClass: AllyClass.Bard),

            // Budget 10: STR2(2) + HP3(3) + INIT1(1.5) + Treasure0(0) = 6.5. Effect = 3.5 ✓
            new AllyCard(new Guid("00000000-0000-0000-0008-000000000005"),
                "Tiefling Vocalist", Rarity.Uncommon, 2,
                2, 3, 1, treasure: 0,
                effect: "Return 1 card from hand to bottom of deck, then draw 1 card. All allies gain +1 STR this round",
                race: Race.Tiefling, allyClass: AllyClass.Bard),

            // Budget 14: STR2(2) + HP4(4) + INIT2(3) + Treasure1(1) = 10. Effect = 4 ✓
            new AllyCard(new Guid("00000000-0000-0000-0008-000000000006"),
                "Dwarven War Chanter", Rarity.Uncommon, 3,
                2, 4, 2, treasure: 1,
                effect: "Return 1 consumable from hand to top of deck. All Dwarf allies gain +1 HP this round. With Disadvantage: also +1 STR",
                race: Race.Dwarf, allyClass: AllyClass.Bard),

            // ── COMMON (4) ──
            // Budget 6: STR1(1) + HP3(3) + INIT1(1.5) + Treasure0(0) = 5.5. Effect = 0.5 ✓
            new AllyCard(new Guid("00000000-0000-0000-0008-000000000007"),
                "Traveling Minstrel", Rarity.Common, 1,
                1, 3, 1, treasure: 0,
                effect: "Return 1 card from hand to bottom of deck",
                race: Race.Human, allyClass: AllyClass.Bard),

            // Budget 10: STR2(2) + HP3(3) + INIT1(1.5) + Treasure1(1) = 7.5. Effect = 2.5 → 2 ✓
            new AllyCard(new Guid("00000000-0000-0000-0008-000000000008"),
                "Halfling Storyteller", Rarity.Common, 2,
                2, 3, 1, treasure: 1,
                effect: "Return 1 card from hand to top of deck. Draw 1 card",
                race: Race.Halfling, allyClass: AllyClass.Bard),

            // Budget 6: STR2(2) + HP2(2) + INIT1(1.5) + Treasure0(0) = 5.5. Effect = 0.5 ✓
            new AllyCard(new Guid("00000000-0000-0000-0008-000000000009"),
                "Drum Beater", Rarity.Common, 1,
                2, 2, 1, treasure: 0,
                effect: "Return 1 card from hand to bottom of deck",
                race: Race.Orc, allyClass: AllyClass.Bard),

            // Budget 10: STR2(2) + HP4(4) + INIT1(1.5) + Treasure0(0) = 7.5. Effect = 2.5 → 2 ✓
            new AllyCard(new Guid("00000000-0000-0000-0008-000000000010"),
                "Lute Player", Rarity.Common, 2,
                2, 4, 1, treasure: 0,
                effect: "Return 1 card from hand to deck and shuffle. All allies gain +1 INIT this round",
                race: Race.Human, allyClass: AllyClass.Bard),
        };
    }
}
