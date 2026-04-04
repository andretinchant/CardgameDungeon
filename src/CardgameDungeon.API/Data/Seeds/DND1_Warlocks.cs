using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.API.Data.Seeds;

public static partial class CardSetSeeder
{
    private static List<AllyCard> CreateWarlocks()
    {
        return new List<AllyCard>
        {
            // ═══════════════════════════════════════════════════════
            //  WARLOCKS — Mark enemy during initiative; if marked dies, trigger effects
            //  Budget: Cost 1=6, 2=10, 3=14, 4=18, 6=30(unique)
            // ═══════════════════════════════════════════════════════

            // ── UNIQUE (1) ── Budget 30
            // STR5(5) + HP7(7) + INIT3(4.5) + Treasure1(1) = 17.5. Effect = 8 ✓
            new AllyCard(new Guid("00000000-0000-0000-000A-000000000001"),
                "Fiend Pact Archwarlock", Rarity.Unique, 6,
                5, 7, 3, treasure: 1,
                effect: "During initiative: mark 1 enemy. If marked enemy is eliminated this round: restore 3 HP to this Warlock, deal 3 damage to all other enemies in same combat group, and exile 1 card from opponent's deck. If mark not triggered: discard 1 from hand",
                race: Race.Tiefling, allyClass: AllyClass.Warlock),

            // ── RARE (2) ── Budget 14-18
            // STR3(3) + HP5(5) + INIT2(3) + Treasure1(1) = 12. Effect = 6 ✓ Budget 18
            new AllyCard(new Guid("00000000-0000-0000-000A-000000000002"),
                "Hexblade Champion", Rarity.Rare, 4,
                3, 5, 2, treasure: 1,
                effect: "During initiative: mark 1 enemy. If marked enemy is eliminated: +3 STR until end of room and draw 2 cards. This Warlock's weapon deals +1 damage to marked target",
                race: Race.Human, allyClass: AllyClass.Warlock),

            // STR2(2) + HP4(4) + INIT2(3) + Treasure0(0) = 9. Effect = 5 ✓ Budget 14
            new AllyCard(new Guid("00000000-0000-0000-000A-000000000003"),
                "Fey Pact Trickster", Rarity.Rare, 3,
                2, 4, 2, treasure: 0,
                effect: "During initiative: mark 1 enemy. If marked enemy is eliminated: return 2 cards from discard to hand. Marked enemy has -1 STR",
                race: Race.Elf, allyClass: AllyClass.Warlock),

            // ── UNCOMMON (3) ── Budget 10-14
            // STR3(3) + HP4(4) + INIT1(1.5) + Treasure1(1) = 9.5. Effect = 4.5 ✓ Budget 14
            new AllyCard(new Guid("00000000-0000-0000-000A-000000000004"),
                "Fiend Pact Adept", Rarity.Uncommon, 3,
                3, 4, 1, treasure: 1,
                effect: "During initiative: mark 1 enemy. If marked enemy is eliminated: deal 2 damage to all enemies in same combat group",
                race: Race.Tiefling, allyClass: AllyClass.Warlock),

            // STR2(2) + HP3(3) + INIT1(1.5) + Treasure1(1) = 7.5. Effect = 2.5 ✓ Budget 10
            new AllyCard(new Guid("00000000-0000-0000-000A-000000000005"),
                "Great Old One Cultist", Rarity.Uncommon, 2,
                2, 3, 1, treasure: 1,
                effect: "During initiative: mark 1 enemy. If marked enemy is eliminated: exile 1 card from opponent's deck",
                race: Race.Human, allyClass: AllyClass.Warlock),

            // STR3(3) + HP3(3) + INIT2(3) + Treasure1(1) = 10. Effect = 4 ✓ Budget 14
            new AllyCard(new Guid("00000000-0000-0000-000A-000000000006"),
                "Undead Pact Binder", Rarity.Uncommon, 3,
                3, 3, 2, treasure: 1,
                effect: "During initiative: mark 1 enemy. If marked enemy is eliminated: restore this Warlock to full HP. +2 STR against Undead",
                race: Race.Human, allyClass: AllyClass.Warlock),

            // ── COMMON (4) ── Budget 6-10
            // STR2(2) + HP3(3) + INIT0(0) + Treasure0(0) = 5. Effect = 1 ✓ Budget 6
            new AllyCard(new Guid("00000000-0000-0000-000A-000000000007"),
                "Pact Initiate", Rarity.Common, 1,
                2, 3, 0, treasure: 0,
                effect: "During initiative: mark 1 enemy. If marked enemy is eliminated: draw 1 card",
                race: Race.Human, allyClass: AllyClass.Warlock),

            // STR3(3) + HP3(3) + INIT1(1.5) + Treasure1(1) = 8.5. Effect = 1.5 ✓ Budget 10
            new AllyCard(new Guid("00000000-0000-0000-000A-000000000008"),
                "Hex Caster", Rarity.Common, 2,
                3, 3, 1, treasure: 1,
                effect: "During initiative: mark 1 enemy. Marked enemy has -1 STR this round",
                race: Race.Tiefling, allyClass: AllyClass.Warlock),

            // STR1(1) + HP2(2) + INIT1(1.5) + Treasure1(1) = 5.5. Effect = 0.5 ✓ Budget 6
            new AllyCard(new Guid("00000000-0000-0000-000A-000000000009"),
                "Dark Apprentice", Rarity.Common, 1,
                1, 2, 1, treasure: 1,
                effect: "During initiative: mark 1 enemy. If marked enemy is eliminated: +1 STR next round",
                race: Race.Human, allyClass: AllyClass.Warlock),

            // STR3(3) + HP4(4) + INIT1(1.5) + Treasure0(0) = 8.5. Effect = 1.5 ✓ Budget 10
            new AllyCard(new Guid("00000000-0000-0000-000A-000000000010"),
                "Eldritch Knight Pact", Rarity.Common, 2,
                3, 4, 1, treasure: 0,
                effect: "During initiative: mark 1 enemy. If marked enemy is eliminated: restore 2 HP to this Warlock",
                race: Race.Human, allyClass: AllyClass.Warlock),
        };
    }
}
