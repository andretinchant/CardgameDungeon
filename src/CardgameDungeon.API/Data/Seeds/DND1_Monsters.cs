using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.API.Data.Seeds;

public static partial class CardSetSeeder
{
    private static List<MonsterCard> CreateMonsters()
    {
        return new List<MonsterCard>
        {
            // =====================
            // UNIQUE (2) — Budget: Cost 5=21, 6=24; Effect budget 7-8
            // =====================

            // Budget 24: STR 7(7) + HP 9(9) + INIT 2(3) + Treasure 0(0) = 19. Effect budget = 5... too low.
            // Budget 24: STR 6(6) + HP 8(8) + INIT 1(1.5) + Treasure 1(1) = 16.5. Effect budget = 7.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000001"),
                "Ancient Red Dragon",
                Rarity.Unique,
                cost: 6,
                strength: 6,
                hitPoints: 8,
                initiative: 1,
                treasure: 1,
                effect: "Race: Dragon. At start of combat: deal 2 damage to all attacking allies. With Advantage: +3 STR this round"),
            // Budget 24: STR 6(6) + HP 8(8) + INIT 1(1.5) + Treasure 1(1) = 16.5. Effect = 7.5 ✓

            // Budget 21: STR 4(4) + HP 7(7) + INIT 1(1.5) + Treasure 1(1) = 13.5. Effect budget = 7.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000002"),
                "Elder Brain",
                Rarity.Unique,
                cost: 5,
                strength: 4,
                hitPoints: 7,
                initiative: 1,
                treasure: 1,
                effect: "Race: Aberration. Reduce all attacker INIT by 2. When this monster is eliminated: exile 2 cards from attacker's deck. Immune to consumable effects"),
            // Budget 21: STR 4(4) + HP 7(7) + INIT 1(1.5) + Treasure 1(1) = 13.5. Effect = 7.5 ✓

            // =====================
            // RARE (8) — Budget: Cost 4=18, 5=21; Effect budget 5-6
            // =====================

            // Budget 21: STR 5(5) + HP 7(7) + INIT 1(1.5) + Treasure 2(2) = 15.5. Effect budget = 5.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000003"),
                "Beholder Tyrant",
                Rarity.Rare,
                cost: 5,
                strength: 5,
                hitPoints: 7,
                initiative: 1,
                treasure: 2,
                effect: "Race: Aberration. Immune to Equipment effects. With Advantage: +2 STR and reduce one attacker's INIT by 2"),
            // Budget 21: STR 5(5) + HP 7(7) + INIT 1(1.5) + Treasure 2(2) = 15.5. Effect = 5.5 ✓

            // Budget 21: STR 4(4) + HP 7(7) + INIT 2(3) + Treasure 1(1) = 15. Effect budget = 6 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000004"),
                "Lich Lord",
                Rarity.Rare,
                cost: 5,
                strength: 4,
                hitPoints: 7,
                initiative: 2,
                treasure: 1,
                effect: "Race: Undead. When eliminated: restore this card from discard to the top of your deck. With Advantage: exile 1 card from attacker's deck"),
            // Budget 21: STR 4(4) + HP 7(7) + INIT 2(3) + Treasure 1(1) = 15. Effect = 6 ✓

            // Budget 18: STR 4(4) + HP 5(5) + INIT 1(1.5) + Treasure 2(2) = 12.5. Effect budget = 5.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000005"),
                "Mind Flayer Arcanist",
                Rarity.Rare,
                cost: 4,
                strength: 4,
                hitPoints: 5,
                initiative: 1,
                treasure: 2,
                effect: "Race: Aberration. When this monster deals damage: reduce target's STR by 2 this combat. With Disadvantage: +2 HP"),
            // Budget 18: STR 4(4) + HP 5(5) + INIT 1(1.5) + Treasure 2(2) = 12.5. Effect = 5.5 ✓

            // Budget 21: STR 5(5) + HP 6(6) + INIT 2(3) + Treasure 1(1) = 15. Effect budget = 6 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000006"),
                "Vampire Lord",
                Rarity.Rare,
                cost: 5,
                strength: 5,
                hitPoints: 6,
                initiative: 2,
                treasure: 1,
                effect: "Race: Undead. When this monster deals damage: restore 2 HP. When eliminated: restore 1 card from opponent's discard to your deck"),
            // Budget 21: STR 5(5) + HP 6(6) + INIT 2(3) + Treasure 1(1) = 15. Effect = 6 ✓

            // Budget 21: STR 6(6) + HP 7(7) + INIT 1(1.5) + Treasure 1(1) = 15.5. Effect budget = 5.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000007"),
                "Death Knight",
                Rarity.Rare,
                cost: 5,
                strength: 6,
                hitPoints: 7,
                initiative: 1,
                treasure: 1,
                effect: "Race: Undead. +2 STR against Human allies. When this monster eliminates an ally: exile that ally instead of discarding"),
            // Budget 21: STR 6(6) + HP 7(7) + INIT 1(1.5) + Treasure 1(1) = 15.5. Effect = 5.5 ✓

            // Budget 21: STR 5(5) + HP 7(7) + INIT 1(1.5) + Treasure 2(2) = 15.5. Effect budget = 5.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000008"),
                "Pit Fiend",
                Rarity.Rare,
                cost: 5,
                strength: 5,
                hitPoints: 7,
                initiative: 1,
                treasure: 2,
                effect: "Race: Demon. Immune to Bomb consumables. At start of combat: deal 1 damage to each attacking ally"),
            // Budget 21: STR 5(5) + HP 7(7) + INIT 1(1.5) + Treasure 2(2) = 15.5. Effect = 5.5 ✓

            // Budget 21: STR 6(6) + HP 8(8) + INIT 1(1.5) + Treasure 0(0) = 15.5. Effect budget = 5.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000009"),
                "Balor Demon",
                Rarity.Rare,
                cost: 5,
                strength: 6,
                hitPoints: 8,
                initiative: 1,
                treasure: 0,
                effect: "Race: Demon. When eliminated: deal 3 damage to all attacking allies. With Disadvantage: +2 HP"),
            // Budget 21: STR 6(6) + HP 8(8) + INIT 1(1.5) + Treasure 0(0) = 15.5. Effect = 5.5 ✓

            // Budget 18: STR 5(5) + HP 6(6) + INIT 1(1.5) + Treasure 1(1) = 13.5. Effect budget = 4.5... round to 5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000010"),
                "Iron Golem",
                Rarity.Rare,
                cost: 4,
                strength: 5,
                hitPoints: 6,
                initiative: 1,
                treasure: 1,
                effect: "Race: Construct. Immune to consumable effects. Equipment attacks deal no damage to this monster"),
            // Budget 18: STR 5(5) + HP 6(6) + INIT 1(1.5) + Treasure 1(1) = 13.5. Effect = 4.5 (conservative rare) ✓

            // =====================
            // UNCOMMON (20) — Budget: Cost 2=10, 3=14, 4=18; Effect budget 3-4
            // =====================

            // Budget 18: STR 5(5) + HP 6(6) + INIT 1(1.5) + Treasure 2(2) = 14.5. Effect budget = 3.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000011"),
                "Young Black Dragon",
                Rarity.Uncommon,
                cost: 4,
                strength: 5,
                hitPoints: 6,
                initiative: 1,
                treasure: 2,
                effect: "Race: Dragon. With Advantage: +2 STR. Reduces one attacker's INIT by 1 this round"),
            // Budget 18: STR 5(5) + HP 6(6) + INIT 1(1.5) + Treasure 2(2) = 14.5. Effect = 3.5 ✓

            // Budget 14: STR 3(3) + HP 5(5) + INIT 1(1.5) + Treasure 1(1) = 10.5. Effect budget = 3.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000012"),
                "Troll",
                Rarity.Uncommon,
                cost: 3,
                strength: 3,
                hitPoints: 5,
                initiative: 1,
                treasure: 1,
                effect: "Race: Giant. At start of each round: restore 2 HP. With Disadvantage: only restore 1 HP instead"),
            // Budget 14: STR 3(3) + HP 5(5) + INIT 1(1.5) + Treasure 1(1) = 10.5. Effect = 3.5 ✓

            // Budget 14: STR 3(3) + HP 4(4) + INIT 1(1.5) + Treasure 2(2) = 10.5. Effect budget = 3.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000013"),
                "Ogre Mage",
                Rarity.Uncommon,
                cost: 3,
                strength: 3,
                hitPoints: 4,
                initiative: 1,
                treasure: 2,
                effect: "Race: Giant. Ambusher. With Advantage: +3 STR this round"),
            // Budget 14: STR 3(3) + HP 4(4) + INIT 1(1.5) + Treasure 2(2) = 10.5. Effect = 3.5 ✓

            // Budget 14: STR 3(3) + HP 4(4) + INIT 2(3) + Treasure 1(1) = 11. Effect budget = 3 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000014"),
                "Wraith",
                Rarity.Uncommon,
                cost: 3,
                strength: 3,
                hitPoints: 4,
                initiative: 2,
                treasure: 1,
                effect: "Race: Undead. When this monster takes damage from Equipment: reduce that damage by 2. With Advantage: +1 STR"),
            // Budget 14: STR 3(3) + HP 4(4) + INIT 2(3) + Treasure 1(1) = 11. Effect = 3 ✓

            // Budget 18: STR 4(4) + HP 6(6) + INIT 1(1.5) + Treasure 2(2) = 13.5. Effect budget = 4.5 → 4 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000015"),
                "Mummy Lord",
                Rarity.Uncommon,
                cost: 4,
                strength: 4,
                hitPoints: 6,
                initiative: 1,
                treasure: 2,
                effect: "Race: Undead. When this monster deals damage: reduce target's STR by 1 this combat. With Disadvantage: +2 HP"),
            // Budget 18: STR 4(4) + HP 6(6) + INIT 1(1.5) + Treasure 2(2) = 13.5. Effect = 4.5 ✓

            // Budget 14: STR 3(3) + HP 4(4) + INIT 2(3) + Treasure 1(1) = 11. Effect budget = 3 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000016"),
                "Medusa",
                Rarity.Uncommon,
                cost: 3,
                strength: 3,
                hitPoints: 4,
                initiative: 2,
                treasure: 1,
                effect: "Race: Monstrosity. Before combat: reduce the highest-INIT attacker's INIT by 3 this round"),
            // Budget 14: STR 3(3) + HP 4(4) + INIT 2(3) + Treasure 1(1) = 11. Effect = 3 ✓

            // Budget 14: STR 4(4) + HP 4(4) + INIT 1(1.5) + Treasure 1(1) = 10.5. Effect budget = 3.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000017"),
                "Manticore",
                Rarity.Uncommon,
                cost: 3,
                strength: 4,
                hitPoints: 4,
                initiative: 1,
                treasure: 1,
                effect: "Race: Monstrosity. At start of combat: deal 1 damage to up to 2 attacking allies. With Disadvantage: +1 HP"),
            // Budget 14: STR 4(4) + HP 4(4) + INIT 1(1.5) + Treasure 1(1) = 10.5. Effect = 3.5 ✓

            // Budget 14: STR 3(3) + HP 5(5) + INIT 1(1.5) + Treasure 1(1) = 10.5. Effect budget = 3.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000018"),
                "Basilisk",
                Rarity.Uncommon,
                cost: 3,
                strength: 3,
                hitPoints: 5,
                initiative: 1,
                treasure: 1,
                effect: "Race: Monstrosity. When attacked: reduce attacker's INIT by 2 this round. With Advantage: +1 STR"),
            // Budget 14: STR 3(3) + HP 5(5) + INIT 1(1.5) + Treasure 1(1) = 10.5. Effect = 3.5 ✓

            // Budget 18: STR 5(5) + HP 5(5) + INIT 1(1.5) + Treasure 2(2) = 13.5. Effect budget = 4.5 → 4 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000019"),
                "Chimera",
                Rarity.Uncommon,
                cost: 4,
                strength: 5,
                hitPoints: 5,
                initiative: 1,
                treasure: 2,
                effect: "Race: Monstrosity. Deal 1 damage to a second attacker in combat. With Advantage: +2 STR"),
            // Budget 18: STR 5(5) + HP 5(5) + INIT 1(1.5) + Treasure 2(2) = 13.5. Effect = 4.5 ✓

            // Budget 14: STR 3(3) + HP 4(4) + INIT 2(3) + Treasure 1(1) = 11. Effect budget = 3 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000020"),
                "Displacer Beast",
                Rarity.Uncommon,
                cost: 3,
                strength: 3,
                hitPoints: 4,
                initiative: 2,
                treasure: 1,
                effect: "Race: Monstrosity. The first attack against this monster each round deals no damage"),
            // Budget 14: STR 3(3) + HP 4(4) + INIT 2(3) + Treasure 1(1) = 11. Effect = 3 ✓

            // Budget 14: STR 4(4) + HP 5(5) + INIT 1(1.5) + Treasure 1(1) = 11.5. Effect budget = 2.5 → 3 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000021"),
                "Minotaur Champion",
                Rarity.Uncommon,
                cost: 3,
                strength: 4,
                hitPoints: 5,
                initiative: 1,
                treasure: 1,
                effect: "Race: Monstrosity. +2 STR on the first round of combat. With Disadvantage: +1 HP"),
            // Budget 14: STR 4(4) + HP 5(5) + INIT 1(1.5) + Treasure 1(1) = 11.5. Effect = 2.5 ✓

            // Budget 10: STR 2(2) + HP 3(3) + INIT 1(1.5) + Treasure 1(1) = 7.5. Effect budget = 2.5 → 3 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000022"),
                "Gibbering Mouther",
                Rarity.Uncommon,
                cost: 2,
                strength: 2,
                hitPoints: 3,
                initiative: 1,
                treasure: 1,
                effect: "Race: Aberration. Reduce all attackers' INIT by 1 this round. With Advantage: also reduce their STR by 1"),
            // Budget 10: STR 2(2) + HP 3(3) + INIT 1(1.5) + Treasure 1(1) = 7.5. Effect = 2.5 ✓

            // Budget 10: STR 2(2) + HP 3(3) + INIT 2(3) + Treasure 1(1) = 9. Effect budget = 1 → pair with Ambusher for 3 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000023"),
                "Phase Spider",
                Rarity.Uncommon,
                cost: 2,
                strength: 2,
                hitPoints: 3,
                initiative: 2,
                treasure: 1,
                effect: "Race: Monstrosity. Ambusher. With Advantage: +1 STR"),
            // Budget 10: STR 2(2) + HP 3(3) + INIT 2(3) + Treasure 1(1) = 9. Effect = 1 (Ambusher ~2pt implicit) ✓

            // Budget 14: STR 3(3) + HP 5(5) + INIT 1(1.5) + Treasure 1(1) = 10.5. Effect budget = 3.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000024"),
                "Otyugh",
                Rarity.Uncommon,
                cost: 3,
                strength: 3,
                hitPoints: 5,
                initiative: 1,
                treasure: 1,
                effect: "Race: Aberration. When this monster deals damage: reduce target's INIT by 2 this combat. With Disadvantage: +1 STR"),
            // Budget 14: STR 3(3) + HP 5(5) + INIT 1(1.5) + Treasure 1(1) = 10.5. Effect = 3.5 ✓

            // Budget 14: STR 3(3) + HP 4(4) + INIT 1(1.5) + Treasure 2(2) = 10.5. Effect budget = 3.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000025"),
                "Grick Alpha",
                Rarity.Uncommon,
                cost: 3,
                strength: 3,
                hitPoints: 4,
                initiative: 1,
                treasure: 2,
                effect: "Race: Monstrosity. Ambusher. When this monster deals damage: exile 1 card from attacker's deck"),
            // Budget 14: STR 3(3) + HP 4(4) + INIT 1(1.5) + Treasure 2(2) = 10.5. Effect = 3.5 ✓

            // Budget 14: STR 4(4) + HP 5(5) + INIT 1(1.5) + Treasure 0(0) = 10.5. Effect budget = 3.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000026"),
                "Bulette",
                Rarity.Uncommon,
                cost: 3,
                strength: 4,
                hitPoints: 5,
                initiative: 1,
                treasure: 0,
                effect: "Race: Monstrosity. Ignores Ambusher protection on attacking allies. With Advantage: +2 STR"),
            // Budget 14: STR 4(4) + HP 5(5) + INIT 1(1.5) + Treasure 0(0) = 10.5. Effect = 3.5 ✓

            // Budget 10: STR 2(2) + HP 3(3) + INIT 1(1.5) + Treasure 1(1) = 7.5. Effect budget = 2.5 → 3 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000027"),
                "Carrion Crawler",
                Rarity.Uncommon,
                cost: 2,
                strength: 2,
                hitPoints: 3,
                initiative: 1,
                treasure: 1,
                effect: "Race: Monstrosity. When this monster deals damage: reduce target's INIT to 0 this round"),
            // Budget 10: STR 2(2) + HP 3(3) + INIT 1(1.5) + Treasure 1(1) = 7.5. Effect = 2.5 ✓

            // Budget 14: STR 4(4) + HP 4(4) + INIT 1(1.5) + Treasure 1(1) = 10.5. Effect budget = 3.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000028"),
                "Chuul",
                Rarity.Uncommon,
                cost: 3,
                strength: 4,
                hitPoints: 4,
                initiative: 1,
                treasure: 1,
                effect: "Race: Aberration. When this monster deals damage: destroy target's Equipment. With Disadvantage: +2 HP"),
            // Budget 14: STR 4(4) + HP 4(4) + INIT 1(1.5) + Treasure 1(1) = 10.5. Effect = 3.5 ✓

            // Budget 10: STR 2(2) + HP 3(3) + INIT 1(1.5) + Treasure 1(1) = 7.5. Effect budget = 2.5 → 3 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000029"),
                "Ettercap Broodmother",
                Rarity.Uncommon,
                cost: 2,
                strength: 2,
                hitPoints: 3,
                initiative: 1,
                treasure: 1,
                effect: "Race: Monstrosity. At start of each round: reduce one random attacker's INIT by 1. With Advantage: +1 STR"),
            // Budget 10: STR 2(2) + HP 3(3) + INIT 1(1.5) + Treasure 1(1) = 7.5. Effect = 2.5 ✓

            // Budget 10: STR 2(2) + HP 2(2) + INIT 1(1.5) + Treasure 2(2) = 7.5. Effect budget = 2.5 → 3 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000030"),
                "Rust Monster",
                Rarity.Uncommon,
                cost: 2,
                strength: 2,
                hitPoints: 2,
                initiative: 1,
                treasure: 2,
                effect: "Race: Monstrosity. When this monster deals damage: destroy target's Equipment. +1 STR against allies with Equipment"),
            // Budget 10: STR 2(2) + HP 2(2) + INIT 1(1.5) + Treasure 2(2) = 7.5. Effect = 2.5 ✓

            // =====================
            // COMMON (30) — Budget: Cost 1=6, 2=10; Effect budget 0-2
            // =====================

            // Budget 6: STR 2(2) + HP 2(2) + INIT 1(1.5) + Treasure 0(0) = 5.5. Effect budget = 0.5 → 1 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000031"),
                "Goblin Warrior",
                Rarity.Common,
                cost: 1,
                strength: 2,
                hitPoints: 2,
                initiative: 1,
                treasure: 0,
                effect: "Race: Goblinoid. With Advantage: +1 STR"),
            // Budget 6: STR 2(2) + HP 2(2) + INIT 1(1.5) + Treasure 0(0) = 5.5. Effect = 0.5 ✓

            // Budget 10: STR 3(3) + HP 3(3) + INIT 1(1.5) + Treasure 1(1) = 8.5. Effect budget = 1.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000032"),
                "Orc Raider",
                Rarity.Common,
                cost: 2,
                strength: 3,
                hitPoints: 3,
                initiative: 1,
                treasure: 1,
                effect: "Race: Orc. +1 STR against allies with 3 or less HP remaining"),
            // Budget 10: STR 3(3) + HP 3(3) + INIT 1(1.5) + Treasure 1(1) = 8.5. Effect = 1.5 ✓

            // Budget 6: STR 2(2) + HP 2(2) + INIT 1(1.5) + Treasure 0(0) = 5.5. Effect budget = 0.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000033"),
                "Skeleton Soldier",
                Rarity.Common,
                cost: 1,
                strength: 2,
                hitPoints: 2,
                initiative: 1,
                treasure: 0,
                effect: "Race: Undead. With Disadvantage: +1 HP"),
            // Budget 6: STR 2(2) + HP 2(2) + INIT 1(1.5) + Treasure 0(0) = 5.5. Effect = 0.5 ✓

            // Budget 6: STR 1(1) + HP 3(3) + INIT 0(0) + Treasure 0(0) = 4. Effect budget = 2 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000034"),
                "Zombie Shambler",
                Rarity.Common,
                cost: 1,
                strength: 1,
                hitPoints: 3,
                initiative: 0,
                treasure: 0,
                effect: "Race: Undead. When eliminated: restore this card from discard to the top of your deck"),
            // Budget 6: STR 1(1) + HP 3(3) + INIT 0(0) + Treasure 0(0) = 4. Effect = 2 ✓

            // Budget 6: STR 1(1) + HP 2(2) + INIT 1(1.5) + Treasure 0(0) = 4.5. Effect budget = 1.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000035"),
                "Kobold Trapper",
                Rarity.Common,
                cost: 1,
                strength: 1,
                hitPoints: 2,
                initiative: 1,
                treasure: 0,
                effect: "Race: Goblinoid. When attacked: deal 1 damage to the attacker"),
            // Budget 6: STR 1(1) + HP 2(2) + INIT 1(1.5) + Treasure 0(0) = 4.5. Effect = 1.5 ✓

            // Budget 6: STR 2(2) + HP 2(2) + INIT 1(1.5) + Treasure 0(0) = 5.5. Effect budget = 0.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000036"),
                "Wolf Pack",
                Rarity.Common,
                cost: 1,
                strength: 2,
                hitPoints: 2,
                initiative: 1,
                treasure: 0,
                effect: "Race: Beast. With Advantage: +1 STR"),
            // Budget 6: STR 2(2) + HP 2(2) + INIT 1(1.5) + Treasure 0(0) = 5.5. Effect = 0.5 ✓

            // Budget 6: STR 1(1) + HP 3(3) + INIT 1(1.5) + Treasure 0(0) = 5.5. Effect budget = 0.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000037"),
                "Giant Spider",
                Rarity.Common,
                cost: 1,
                strength: 1,
                hitPoints: 3,
                initiative: 1,
                treasure: 0,
                effect: "Race: Beast. When this monster deals damage: reduce target's INIT by 1 this round"),
            // Budget 6: STR 1(1) + HP 3(3) + INIT 1(1.5) + Treasure 0(0) = 5.5. Effect = 0.5 ✓

            // Budget 6: STR 1(1) + HP 2(2) + INIT 1(1.5) + Treasure 0(0) = 4.5. Effect budget = 1.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000038"),
                "Giant Rat",
                Rarity.Common,
                cost: 1,
                strength: 1,
                hitPoints: 2,
                initiative: 1,
                treasure: 0,
                effect: "Race: Beast. When this monster deals damage: deal 1 additional damage next round"),
            // Budget 6: STR 1(1) + HP 2(2) + INIT 1(1.5) + Treasure 0(0) = 4.5. Effect = 1.5 ✓

            // Budget 6: STR 1(1) + HP 2(2) + INIT 1(1.5) + Treasure 1(1) = 5.5. Effect budget = 0.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000039"),
                "Bandit Thug",
                Rarity.Common,
                cost: 1,
                strength: 1,
                hitPoints: 2,
                initiative: 1,
                treasure: 1,
                effect: "Race: Human. With Advantage: +1 Treasure"),
            // Budget 6: STR 1(1) + HP 2(2) + INIT 1(1.5) + Treasure 1(1) = 5.5. Effect = 0.5 ✓

            // Budget 10: STR 3(3) + HP 4(4) + INIT 1(1.5) + Treasure 0(0) = 8.5. Effect budget = 1.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000040"),
                "Cultist Fanatic",
                Rarity.Common,
                cost: 2,
                strength: 3,
                hitPoints: 4,
                initiative: 1,
                treasure: 0,
                effect: "Race: Human. When this monster deals damage: exile 1 card from the top of attacker's deck"),
            // Budget 10: STR 3(3) + HP 4(4) + INIT 1(1.5) + Treasure 0(0) = 8.5. Effect = 1.5 ✓

            // Budget 10: STR 3(3) + HP 3(3) + INIT 1(1.5) + Treasure 1(1) = 8.5. Effect budget = 1.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000041"),
                "Gnoll Hunter",
                Rarity.Common,
                cost: 2,
                strength: 3,
                hitPoints: 3,
                initiative: 1,
                treasure: 1,
                effect: "Race: Gnoll. When this monster eliminates an ally: +2 STR this combat"),
            // Budget 10: STR 3(3) + HP 3(3) + INIT 1(1.5) + Treasure 1(1) = 8.5. Effect = 1.5 ✓

            // Budget 10: STR 3(3) + HP 4(4) + INIT 1(1.5) + Treasure 0(0) = 8.5. Effect budget = 1.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000042"),
                "Hobgoblin Soldier",
                Rarity.Common,
                cost: 2,
                strength: 3,
                hitPoints: 4,
                initiative: 1,
                treasure: 0,
                effect: "Race: Goblinoid. With Advantage: +1 STR and +1 HP"),
            // Budget 10: STR 3(3) + HP 4(4) + INIT 1(1.5) + Treasure 0(0) = 8.5. Effect = 1.5 ✓

            // Budget 10: STR 3(3) + HP 3(3) + INIT 1(1.5) + Treasure 1(1) = 8.5. Effect budget = 1.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000043"),
                "Bugbear Scout",
                Rarity.Common,
                cost: 2,
                strength: 3,
                hitPoints: 3,
                initiative: 1,
                treasure: 1,
                effect: "Race: Goblinoid. Ambusher. With Disadvantage: +1 HP"),
            // Budget 10: STR 3(3) + HP 3(3) + INIT 1(1.5) + Treasure 1(1) = 8.5. Effect = 1.5 ✓

            // Budget 10: STR 2(2) + HP 3(3) + INIT 2(3) + Treasure 1(1) = 9. Effect budget = 1 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000044"),
                "Harpy",
                Rarity.Common,
                cost: 2,
                strength: 2,
                hitPoints: 3,
                initiative: 2,
                treasure: 1,
                effect: "Race: Monstrosity. Reduces one random attacker's INIT by 1 this round"),
            // Budget 10: STR 2(2) + HP 3(3) + INIT 2(3) + Treasure 1(1) = 9. Effect = 1 ✓

            // Budget 6: STR 1(1) + HP 2(2) + INIT 1(1.5) + Treasure 0(0) = 4.5. Effect budget = 1.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000045"),
                "Stirge Swarm",
                Rarity.Common,
                cost: 1,
                strength: 1,
                hitPoints: 2,
                initiative: 1,
                treasure: 0,
                effect: "Race: Beast. When this monster deals damage: restore 1 HP to this monster"),
            // Budget 6: STR 1(1) + HP 2(2) + INIT 1(1.5) + Treasure 0(0) = 4.5. Effect = 1.5 ✓

            // Budget 6: STR 1(1) + HP 2(2) + INIT 1(1.5) + Treasure 0(0) = 4.5. Effect budget = 1.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000046"),
                "Fire Beetle",
                Rarity.Common,
                cost: 1,
                strength: 1,
                hitPoints: 2,
                initiative: 1,
                treasure: 0,
                effect: "Race: Beast. Negates Ambusher protection on one attacking ally"),
            // Budget 6: STR 1(1) + HP 2(2) + INIT 1(1.5) + Treasure 0(0) = 4.5. Effect = 1.5 ✓

            // Budget 6: STR 1(1) + HP 2(2) + INIT 1(1.5) + Treasure 0(0) = 4.5. Effect budget = 1.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000047"),
                "Jackalwere",
                Rarity.Common,
                cost: 1,
                strength: 1,
                hitPoints: 2,
                initiative: 1,
                treasure: 0,
                effect: "Race: Monstrosity. With Advantage: reduce one attacker's STR by 2 this round"),
            // Budget 6: STR 1(1) + HP 2(2) + INIT 1(1.5) + Treasure 0(0) = 4.5. Effect = 1.5 ✓

            // Budget 10: STR 3(3) + HP 4(4) + INIT 1(1.5) + Treasure 0(0) = 8.5. Effect budget = 1.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000048"),
                "Ghoul",
                Rarity.Common,
                cost: 2,
                strength: 3,
                hitPoints: 4,
                initiative: 1,
                treasure: 0,
                effect: "Race: Undead. When this monster deals damage: reduce target's INIT by 1 this round"),
            // Budget 10: STR 3(3) + HP 4(4) + INIT 1(1.5) + Treasure 0(0) = 8.5. Effect = 1.5 ✓

            // Budget 6: STR 1(1) + HP 2(2) + INIT 1(1.5) + Treasure 0(0) = 4.5. Effect budget = 1.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000049"),
                "Shadow",
                Rarity.Common,
                cost: 1,
                strength: 1,
                hitPoints: 2,
                initiative: 1,
                treasure: 0,
                effect: "Race: Undead. When this monster deals damage: reduce target's STR by 1 this combat"),
            // Budget 6: STR 1(1) + HP 2(2) + INIT 1(1.5) + Treasure 0(0) = 4.5. Effect = 1.5 ✓

            // Budget 6: STR 1(1) + HP 3(3) + INIT 0(0) + Treasure 0(0) = 4. Effect budget = 2 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000050"),
                "Violet Fungus",
                Rarity.Common,
                cost: 1,
                strength: 1,
                hitPoints: 3,
                initiative: 0,
                treasure: 0,
                effect: "Race: Plant. When attacked: deal 1 damage to the attacker"),
            // Budget 6: STR 1(1) + HP 3(3) + INIT 0(0) + Treasure 0(0) = 4. Effect = 2 ✓

            // Budget 6: STR 1(1) + HP 2(2) + INIT 1(1.5) + Treasure 0(0) = 4.5. Effect budget = 1.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000051"),
                "Darkmantle",
                Rarity.Common,
                cost: 1,
                strength: 1,
                hitPoints: 2,
                initiative: 1,
                treasure: 0,
                effect: "Race: Monstrosity. When this monster deals damage: reduce target's STR by 1 this round"),
            // Budget 6: STR 1(1) + HP 2(2) + INIT 1(1.5) + Treasure 0(0) = 4.5. Effect = 1.5 ✓

            // Budget 6: STR 2(2) + HP 1(1) + INIT 1(1.5) + Treasure 0(0) = 4.5. Effect budget = 1.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000052"),
                "Piercer",
                Rarity.Common,
                cost: 1,
                strength: 2,
                hitPoints: 1,
                initiative: 1,
                treasure: 0,
                effect: "Race: Monstrosity. +2 STR on the first round of combat"),
            // Budget 6: STR 2(2) + HP 1(1) + INIT 1(1.5) + Treasure 0(0) = 4.5. Effect = 1.5 ✓

            // Budget 6: STR 1(1) + HP 2(2) + INIT 0(0) + Treasure 0(0) = 3. Effect budget = 3 → cap at 2 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000053"),
                "Gas Spore",
                Rarity.Common,
                cost: 1,
                strength: 1,
                hitPoints: 2,
                initiative: 0,
                treasure: 0,
                effect: "Race: Plant. When eliminated: deal 2 damage to the ally that eliminated this monster"),
            // Budget 6: STR 1(1) + HP 2(2) + INIT 0(0) + Treasure 0(0) = 3. Effect = 3 (strong for common, but stats are very weak) ✓

            // Budget 6: STR 1(1) + HP 2(2) + INIT 1(1.5) + Treasure 0(0) = 4.5. Effect budget = 1.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000054"),
                "Swarm of Bats",
                Rarity.Common,
                cost: 1,
                strength: 1,
                hitPoints: 2,
                initiative: 1,
                treasure: 0,
                effect: "Race: Beast. Reduce one attacker's INIT by 1 this round"),
            // Budget 6: STR 1(1) + HP 2(2) + INIT 1(1.5) + Treasure 0(0) = 4.5. Effect = 1.5 ✓

            // Budget 6: STR 1(1) + HP 1(1) + INIT 1(1.5) + Treasure 0(0) = 3.5. Effect budget = 2.5 → 2 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000055"),
                "Giant Centipede",
                Rarity.Common,
                cost: 1,
                strength: 1,
                hitPoints: 1,
                initiative: 1,
                treasure: 0,
                effect: "Race: Beast. When this monster deals damage: reduce target's STR by 1 and INIT by 1 this round"),
            // Budget 6: STR 1(1) + HP 1(1) + INIT 1(1.5) + Treasure 0(0) = 3.5. Effect = 2.5 ✓

            // Budget 10: STR 2(2) + HP 3(3) + INIT 1(1.5) + Treasure 1(1) = 7.5. Effect budget = 2.5 → 2 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000056"),
                "Magma Mephit",
                Rarity.Common,
                cost: 2,
                strength: 2,
                hitPoints: 3,
                initiative: 1,
                treasure: 1,
                effect: "Race: Elemental. When eliminated: deal 2 damage to the ally that eliminated this monster"),
            // Budget 10: STR 2(2) + HP 3(3) + INIT 1(1.5) + Treasure 1(1) = 7.5. Effect = 2.5 ✓

            // Budget 10: STR 2(2) + HP 4(4) + INIT 1(1.5) + Treasure 1(1) = 8.5. Effect budget = 1.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000057"),
                "Ice Mephit",
                Rarity.Common,
                cost: 2,
                strength: 2,
                hitPoints: 4,
                initiative: 1,
                treasure: 1,
                effect: "Race: Elemental. When this monster deals damage: reduce target's INIT by 1 this round"),
            // Budget 10: STR 2(2) + HP 4(4) + INIT 1(1.5) + Treasure 1(1) = 8.5. Effect = 1.5 ✓

            // Budget 6: STR 1(1) + HP 2(2) + INIT 1(1.5) + Treasure 0(0) = 4.5. Effect budget = 1.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000058"),
                "Quasit",
                Rarity.Common,
                cost: 1,
                strength: 1,
                hitPoints: 2,
                initiative: 1,
                treasure: 0,
                effect: "Race: Demon. Ambusher"),
            // Budget 6: STR 1(1) + HP 2(2) + INIT 1(1.5) + Treasure 0(0) = 4.5. Effect = 1.5 ✓

            // Budget 6: STR 1(1) + HP 2(2) + INIT 1(1.5) + Treasure 0(0) = 4.5. Effect budget = 1.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000059"),
                "Imp",
                Rarity.Common,
                cost: 1,
                strength: 1,
                hitPoints: 2,
                initiative: 1,
                treasure: 0,
                effect: "Race: Demon. Ambusher. With Disadvantage: +1 HP"),
            // Budget 6: STR 1(1) + HP 2(2) + INIT 1(1.5) + Treasure 0(0) = 4.5. Effect = 1.5 ✓

            // Budget 6: STR 2(2) + HP 1(1) + INIT 1(1.5) + Treasure 1(1) = 5.5. Effect budget = 0.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000060"),
                "Flying Sword",
                Rarity.Common,
                cost: 1,
                strength: 2,
                hitPoints: 1,
                initiative: 1,
                treasure: 1,
                effect: "Race: Construct. With Advantage: +1 STR"),
            // Budget 6: STR 2(2) + HP 1(1) + INIT 1(1.5) + Treasure 1(1) = 5.5. Effect = 0.5 ✓
        };
    }
}
