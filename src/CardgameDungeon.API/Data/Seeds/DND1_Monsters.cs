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
            // UNIQUE (2) — Budget: Cost 5=98, 6=113; ×1.5 rarity multiplier
            // =====================

            // Budget 113: ATK 15(42) + HP 5(12) + INIT 1(1.5) + Treasure 1(-1) = 54.5. Effect budget = 58.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000001"),
                "Ancient Red Dragon",
                Rarity.Unique,
                cost: 6,
                attack: 15,
                hitPoints: 5,
                initiative: 1,
                treasure: 1,
                effect: "Race: Dragon. At start of combat: deal 1 damage to all attacking allies. With Advantage: +3 ATK this round",
                race: Race.Dragon,
                effectTags: "ON_COMBAT_START|DAMAGE:1:ALL_ENEMIES\nWITH_ADVANTAGE|+ATK:3"),
            // Budget 113: ATK 15(42) + HP 5(12) + INIT 1(1.5) + Treasure 1(-1) = 54.5. Effect = 58.5 ✓

            // Budget 98: ATK 15(42) + HP 5(12) + INIT 1(1.5) + Treasure 1(-1) = 54.5. Effect budget = 43.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000002"),
                "Elder Brain",
                Rarity.Unique,
                cost: 5,
                attack: 15,
                hitPoints: 5,
                initiative: 1,
                treasure: 1,
                effect: "Race: Aberration. Reduce all attacker INIT by 2. When this monster is eliminated: exile 2 cards from attacker's deck. Immune to consumable effects",
                race: Race.Aberration,
                effectTags: "PASSIVE|-INIT:2:ALL_ENEMIES\nON_DEATH|EXILE_DECK:2\nPASSIVE|IMMUNE_CONSUMABLE"),
            // Budget 98: ATK 15(42) + HP 5(12) + INIT 1(1.5) + Treasure 1(-1) = 54.5. Effect = 43.5 ✓

            // =====================
            // RARE (8) — Budget: Cost 4=69, 5=81; ×1.25 rarity multiplier
            // =====================

            // Budget 81: ATK 15(42) + HP 5(12) + INIT 1(1.5) + Treasure 2(-2) = 53.5. Effect budget = 27.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000003"),
                "Beholder Tyrant",
                Rarity.Rare,
                cost: 5,
                attack: 15,
                hitPoints: 5,
                initiative: 1,
                treasure: 2,
                effect: "Race: Aberration. Immune to Equipment effects. With Advantage: +2 ATK and reduce one attacker's INIT by 2",
                race: Race.Aberration,
                effectTags: "PASSIVE|IMMUNE_EQUIPMENT\nWITH_ADVANTAGE|+ATK:2|-INIT:2:ENEMY"),
            // Budget 81: ATK 15(42) + HP 5(12) + INIT 1(1.5) + Treasure 2(-2) = 53.5. Effect = 27.5 ✓

            // Budget 81: ATK 15(42) + HP 5(12) + INIT 1(1.5) + Treasure 1(-1) = 54.5. Effect budget = 26.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000004"),
                "Lich Lord",
                Rarity.Rare,
                cost: 5,
                attack: 15,
                hitPoints: 5,
                initiative: 1,
                treasure: 1,
                effect: "Race: Undead. When eliminated: restore this card from discard to the top of your deck. With Advantage: exile 1 card from attacker's deck",
                race: Race.Undead,
                effectTags: "ON_DEATH|RESTORE_SELF_TO_DECK\nWITH_ADVANTAGE|EXILE_DECK:1"),
            // Budget 81: ATK 15(42) + HP 5(12) + INIT 1(1.5) + Treasure 1(-1) = 54.5. Effect = 26.5 ✓

            // Budget 69: ATK 14(39) + HP 5(12) + INIT 1(1.5) + Treasure 2(-2) = 50.5. Effect budget = 18.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000005"),
                "Mind Flayer Arcanist",
                Rarity.Rare,
                cost: 4,
                attack: 14,
                hitPoints: 5,
                initiative: 1,
                treasure: 2,
                effect: "Race: Aberration. When this monster deals damage: reduce target's ATK by 2 this combat. With Disadvantage: +2 HP",
                race: Race.Aberration,
                effectTags: "ON_COMBAT_START|-ATK:2:ENEMY\nWITH_DISADVANTAGE|+HP:2"),
            // Budget 69: ATK 14(39) + HP 5(12) + INIT 1(1.5) + Treasure 2(-2) = 50.5. Effect = 18.5 ✓

            // Budget 81: ATK 15(42) + HP 5(12) + INIT 1(1.5) + Treasure 1(-1) = 54.5. Effect budget = 26.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000006"),
                "Vampire Lord",
                Rarity.Rare,
                cost: 5,
                attack: 15,
                hitPoints: 5,
                initiative: 1,
                treasure: 1,
                effect: "Race: Undead. When this monster deals damage: restore 2 HP. When eliminated: restore 1 card from opponent's discard to your deck",
                race: Race.Undead,
                effectTags: "ON_COMBAT_START|HEAL:2:SELF\nON_DEATH|RESTORE_CARD_FROM_OPPONENT"),
            // Budget 81: ATK 15(42) + HP 5(12) + INIT 1(1.5) + Treasure 1(-1) = 54.5. Effect = 26.5 ✓

            // Budget 81: ATK 15(42) + HP 5(12) + INIT 1(1.5) + Treasure 1(-1) = 54.5. Effect budget = 26.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000007"),
                "Death Knight",
                Rarity.Rare,
                cost: 5,
                attack: 15,
                hitPoints: 5,
                initiative: 1,
                treasure: 1,
                effect: "Race: Undead. +2 ATK against Human allies. When this monster eliminates an ally: exile that ally instead of discarding",
                race: Race.Undead,
                effectTags: "PASSIVE|IF_RACE:Human|+ATK:2\nON_KILL|EXILE_TARGET"),
            // Budget 81: ATK 15(42) + HP 5(12) + INIT 1(1.5) + Treasure 1(-1) = 54.5. Effect = 26.5 ✓

            // Budget 81: ATK 15(42) + HP 5(12) + INIT 1(1.5) + Treasure 2(-2) = 53.5. Effect budget = 27.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000008"),
                "Pit Fiend",
                Rarity.Rare,
                cost: 5,
                attack: 15,
                hitPoints: 5,
                initiative: 1,
                treasure: 2,
                effect: "Race: Demon. Immune to Bomb consumables. At start of combat: deal 1 damage to each attacking ally",
                race: Race.Demon,
                effectTags: "PASSIVE|IMMUNE_BOMB\nON_COMBAT_START|DAMAGE:1:ALL_ENEMIES"),
            // Budget 81: ATK 15(42) + HP 5(12) + INIT 1(1.5) + Treasure 2(-2) = 53.5. Effect = 27.5 ✓

            // Budget 81: ATK 15(42) + HP 5(12) + INIT 1(1.5) + Treasure 1(-1) = 54.5. Effect budget = 26.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000009"),
                "Balor Demon",
                Rarity.Rare,
                cost: 5,
                attack: 15,
                hitPoints: 5,
                initiative: 1,
                treasure: 1,
                effect: "Race: Demon. When eliminated: deal 3 damage to all attacking allies. With Disadvantage: +2 HP",
                race: Race.Demon,
                effectTags: "ON_DEATH|DAMAGE:1:ALL_ENEMIES\nWITH_DISADVANTAGE|+HP:2"),
            // Budget 81: ATK 15(42) + HP 5(12) + INIT 1(1.5) + Treasure 1(-1) = 54.5. Effect = 26.5 ✓

            // Budget 69: ATK 14(39) + HP 5(12) + INIT 1(1.5) + Treasure 1(-1) = 51.5. Effect budget = 17.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000010"),
                "Iron Golem",
                Rarity.Rare,
                cost: 4,
                attack: 14,
                hitPoints: 5,
                initiative: 1,
                treasure: 1,
                effect: "Race: Construct. Immune to consumable effects. Equipment attacks deal no damage to this monster",
                race: Race.Construct,
                effectTags: "PASSIVE|IMMUNE_CONSUMABLE|IMMUNE_EQUIPMENT_DAMAGE"),
            // Budget 69: ATK 14(39) + HP 5(12) + INIT 1(1.5) + Treasure 1(-1) = 51.5. Effect = 17.5 ✓

            // =====================
            // UNCOMMON (20) — Budget: Cost 2=42, 3=48, 4=55×1.1=61; ×1.1 rarity multiplier
            // =====================

            // Budget 48: ATK 12(33) + HP 4(9) + INIT 1(1.5) + Treasure 1(-1) = 42.5. Effect budget = 5.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000011"),
                "Young Black Dragon",
                Rarity.Uncommon,
                cost: 3,
                attack: 12,
                hitPoints: 4,
                initiative: 1,
                treasure: 1,
                effect: "Race: Dragon. With Advantage: +2 ATK. Reduces one attacker's INIT by 1 this round",
                race: Race.Dragon,
                effectTags: "WITH_ADVANTAGE|+ATK:2\nPASSIVE|-INIT:1:ENEMY"),
            // Budget 48: ATK 12(33) + HP 4(9) + INIT 1(1.5) + Treasure 1(-1) = 42.5. Effect = 5.5 ✓

            // Budget 48: ATK 10(27) + HP 5(12) + INIT 1(1.5) + Treasure 1(-1) = 39.5. Effect budget = 8.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000012"),
                "Troll",
                Rarity.Uncommon,
                cost: 3,
                attack: 10,
                hitPoints: 5,
                initiative: 1,
                treasure: 1,
                effect: "Race: Giant. At start of each round: restore 2 HP. With Disadvantage: only restore 1 HP instead",
                race: Race.Giant,
                effectTags: "ON_ROUND_START|HEAL:2:SELF\nWITH_DISADVANTAGE|HEAL:1:SELF"),
            // Budget 48: ATK 10(27) + HP 5(12) + INIT 1(1.5) + Treasure 1(-1) = 39.5. Effect = 8.5 ✓

            // Budget 48: ATK 12(33) + HP 3(6) + INIT 1(1.5) + Treasure 1(-1) = 39.5. Effect budget = 8.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000013"),
                "Ogre Mage",
                Rarity.Uncommon,
                cost: 3,
                attack: 12,
                hitPoints: 3,
                initiative: 1,
                treasure: 1,
                effect: "Race: Giant. Ambusher. With Advantage: +3 ATK this round",
                race: Race.Giant,
                effectTags: "PASSIVE|AMBUSHER\nWITH_ADVANTAGE|+ATK:3"),
            // Budget 48: ATK 12(33) + HP 3(6) + INIT 1(1.5) + Treasure 1(-1) = 39.5. Effect = 8.5 ✓

            // Budget 48: ATK 11(30) + HP 4(9) + INIT 1(1.5) + Treasure 1(-1) = 39.5. Effect budget = 8.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000014"),
                "Wraith",
                Rarity.Uncommon,
                cost: 3,
                attack: 11,
                hitPoints: 4,
                initiative: 1,
                treasure: 1,
                effect: "Race: Undead. When this monster takes damage from Equipment: reduce that damage by 2. With Advantage: +1 ATK",
                race: Race.Undead,
                effectTags: "PASSIVE|REDUCE_EQUIPMENT_DAMAGE:2\nWITH_ADVANTAGE|+ATK:1"),
            // Budget 48: ATK 11(30) + HP 4(9) + INIT 1(1.5) + Treasure 1(-1) = 39.5. Effect = 8.5 ✓

            // Budget 48: ATK 12(33) + HP 4(9) + INIT 1(1.5) + Treasure 1(-1) = 42.5. Effect budget = 5.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000015"),
                "Mummy Lord",
                Rarity.Uncommon,
                cost: 3,
                attack: 12,
                hitPoints: 4,
                initiative: 1,
                treasure: 1,
                effect: "Race: Undead. When this monster deals damage: reduce target's ATK by 1 this combat. With Disadvantage: +2 HP",
                race: Race.Undead,
                effectTags: "ON_COMBAT_START|-ATK:1:ENEMY\nWITH_DISADVANTAGE|+HP:2"),
            // Budget 48: ATK 12(33) + HP 4(9) + INIT 1(1.5) + Treasure 1(-1) = 42.5. Effect = 5.5 ✓

            // Budget 48: ATK 12(33) + HP 3(6) + INIT 1(1.5) + Treasure 1(-1) = 39.5. Effect budget = 8.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000016"),
                "Medusa",
                Rarity.Uncommon,
                cost: 3,
                attack: 12,
                hitPoints: 3,
                initiative: 1,
                treasure: 1,
                effect: "Race: Monstrosity. Before combat: reduce the highest-INIT attacker's INIT by 3 this round",
                race: Race.Monstrosity,
                effectTags: "ON_COMBAT_START|-INIT:3:ENEMY"),
            // Budget 48: ATK 12(33) + HP 3(6) + INIT 1(1.5) + Treasure 1(-1) = 39.5. Effect = 8.5 ✓

            // Budget 48: ATK 12(33) + HP 4(9) + INIT 1(1.5) + Treasure 1(-1) = 42.5. Effect budget = 5.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000017"),
                "Manticore",
                Rarity.Uncommon,
                cost: 3,
                attack: 12,
                hitPoints: 4,
                initiative: 1,
                treasure: 1,
                effect: "Race: Monstrosity. At start of combat: deal 1 damage to up to 2 attacking allies. With Disadvantage: +1 HP",
                race: Race.Monstrosity,
                effectTags: "ON_COMBAT_START|DAMAGE:1:ENEMY|DAMAGE:1:ENEMY\nWITH_DISADVANTAGE|+HP:1"),
            // Budget 48: ATK 12(33) + HP 4(9) + INIT 1(1.5) + Treasure 1(-1) = 42.5. Effect = 5.5 ✓

            // Budget 48: ATK 13(36) + HP 3(6) + INIT 1(1.5) + Treasure 0(0) = 43.5. Effect budget = 4.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000018"),
                "Basilisk",
                Rarity.Uncommon,
                cost: 3,
                attack: 13,
                hitPoints: 3,
                initiative: 1,
                treasure: 0,
                effect: "Race: Monstrosity. When attacked: reduce attacker's INIT by 2 this round. With Advantage: +1 ATK",
                race: Race.Monstrosity,
                effectTags: "PASSIVE|-INIT:2:ENEMY\nWITH_ADVANTAGE|+ATK:1"),
            // Budget 48: ATK 13(36) + HP 3(6) + INIT 1(1.5) + Treasure 0(0) = 43.5. Effect = 4.5 ✓

            // Budget 48: ATK 12(33) + HP 4(9) + INIT 1(1.5) + Treasure 1(-1) = 42.5. Effect budget = 5.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000019"),
                "Chimera",
                Rarity.Uncommon,
                cost: 3,
                attack: 12,
                hitPoints: 4,
                initiative: 1,
                treasure: 1,
                effect: "Race: Monstrosity. Deal 1 damage to a second attacker in combat. With Advantage: +2 ATK",
                race: Race.Monstrosity,
                effectTags: "ON_COMBAT_START|DAMAGE:1:ENEMY\nWITH_ADVANTAGE|+ATK:2"),
            // Budget 48: ATK 12(33) + HP 4(9) + INIT 1(1.5) + Treasure 1(-1) = 42.5. Effect = 5.5 ✓

            // Budget 48: ATK 11(30) + HP 4(9) + INIT 1(1.5) + Treasure 1(-1) = 39.5. Effect budget = 8.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000020"),
                "Displacer Beast",
                Rarity.Uncommon,
                cost: 3,
                attack: 11,
                hitPoints: 4,
                initiative: 1,
                treasure: 1,
                effect: "Race: Monstrosity. The first attack against this monster each round deals no damage",
                race: Race.Monstrosity,
                effectTags: "PASSIVE|NEGATE_FIRST_ATTACK"),
            // Budget 48: ATK 11(30) + HP 4(9) + INIT 1(1.5) + Treasure 1(-1) = 39.5. Effect = 8.5 ✓

            // Budget 48: ATK 13(36) + HP 3(6) + INIT 1(1.5) + Treasure 0(0) = 43.5. Effect budget = 4.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000021"),
                "Minotaur Champion",
                Rarity.Uncommon,
                cost: 3,
                attack: 13,
                hitPoints: 3,
                initiative: 1,
                treasure: 0,
                effect: "Race: Monstrosity. +2 ATK on the first round of combat. With Disadvantage: +1 HP",
                race: Race.Monstrosity,
                effectTags: "ON_COMBAT_START|+ATK:2\nWITH_DISADVANTAGE|+HP:1"),
            // Budget 48: ATK 13(36) + HP 3(6) + INIT 1(1.5) + Treasure 0(0) = 43.5. Effect = 4.5 ✓

            // Budget 42: ATK 10(27) + HP 3(6) + INIT 1(1.5) + Treasure 0(0) = 34.5. Effect budget = 7.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000022"),
                "Gibbering Mouther",
                Rarity.Uncommon,
                cost: 2,
                attack: 10,
                hitPoints: 3,
                initiative: 1,
                treasure: 0,
                effect: "Race: Aberration. Reduce all attackers' INIT by 1 this round. With Advantage: also reduce their ATK by 1",
                race: Race.Aberration,
                effectTags: "PASSIVE|-INIT:1:ALL_ENEMIES\nWITH_ADVANTAGE|-ATK:1:ALL_ENEMIES"),
            // Budget 42: ATK 10(27) + HP 3(6) + INIT 1(1.5) + Treasure 0(0) = 34.5. Effect = 7.5 ✓

            // Budget 42: ATK 10(27) + HP 3(6) + INIT 1(1.5) + Treasure 1(-1) = 33.5. Effect budget = 8.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000023"),
                "Phase Spider",
                Rarity.Uncommon,
                cost: 2,
                attack: 10,
                hitPoints: 3,
                initiative: 1,
                treasure: 1,
                effect: "Race: Monstrosity. Ambusher. With Advantage: +1 ATK",
                race: Race.Monstrosity,
                effectTags: "PASSIVE|AMBUSHER\nWITH_ADVANTAGE|+ATK:1"),
            // Budget 42: ATK 10(27) + HP 3(6) + INIT 1(1.5) + Treasure 1(-1) = 33.5. Effect = 8.5 ✓

            // Budget 48: ATK 11(30) + HP 5(12) + INIT 1(1.5) + Treasure 0(0) = 43.5. Effect budget = 4.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000024"),
                "Otyugh",
                Rarity.Uncommon,
                cost: 3,
                attack: 11,
                hitPoints: 5,
                initiative: 1,
                treasure: 0,
                effect: "Race: Aberration. When this monster deals damage: reduce target's INIT by 2 this combat. With Disadvantage: +1 ATK",
                race: Race.Aberration,
                effectTags: "ON_COMBAT_START|-INIT:2:ENEMY\nWITH_DISADVANTAGE|+ATK:1"),
            // Budget 48: ATK 11(30) + HP 5(12) + INIT 1(1.5) + Treasure 0(0) = 43.5. Effect = 4.5 ✓

            // Budget 48: ATK 11(30) + HP 4(9) + INIT 1(1.5) + Treasure 1(-1) = 39.5. Effect budget = 8.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000025"),
                "Grick Alpha",
                Rarity.Uncommon,
                cost: 3,
                attack: 11,
                hitPoints: 4,
                initiative: 1,
                treasure: 1,
                effect: "Race: Monstrosity. Ambusher. When this monster deals damage: exile 1 card from attacker's deck",
                race: Race.Monstrosity,
                effectTags: "PASSIVE|AMBUSHER\nON_COMBAT_START|EXILE_DECK:1"),
            // Budget 48: ATK 11(30) + HP 4(9) + INIT 1(1.5) + Treasure 1(-1) = 39.5. Effect = 8.5 ✓

            // Budget 48: ATK 13(36) + HP 3(6) + INIT 1(1.5) + Treasure 0(0) = 43.5. Effect budget = 4.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000026"),
                "Bulette",
                Rarity.Uncommon,
                cost: 3,
                attack: 13,
                hitPoints: 3,
                initiative: 1,
                treasure: 0,
                effect: "Race: Monstrosity. Ignores Ambusher protection on attacking allies. With Advantage: +2 ATK",
                race: Race.Monstrosity,
                effectTags: "PASSIVE|IGNORE_AMBUSHER\nWITH_ADVANTAGE|+ATK:2"),
            // Budget 48: ATK 13(36) + HP 3(6) + INIT 1(1.5) + Treasure 0(0) = 43.5. Effect = 4.5 ✓

            // Budget 42: ATK 10(27) + HP 3(6) + INIT 1(1.5) + Treasure 0(0) = 34.5. Effect budget = 7.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000027"),
                "Carrion Crawler",
                Rarity.Uncommon,
                cost: 2,
                attack: 10,
                hitPoints: 3,
                initiative: 1,
                treasure: 0,
                effect: "Race: Monstrosity. When this monster deals damage: reduce target's INIT to 0 this round",
                race: Race.Monstrosity,
                effectTags: "ON_COMBAT_START|SET_INIT:0:ENEMY"),
            // Budget 42: ATK 10(27) + HP 3(6) + INIT 1(1.5) + Treasure 0(0) = 34.5. Effect = 7.5 ✓

            // Budget 48: ATK 12(33) + HP 4(9) + INIT 1(1.5) + Treasure 1(-1) = 42.5. Effect budget = 5.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000028"),
                "Chuul",
                Rarity.Uncommon,
                cost: 3,
                attack: 12,
                hitPoints: 4,
                initiative: 1,
                treasure: 1,
                effect: "Race: Aberration. When this monster deals damage: destroy target's Equipment. With Disadvantage: +2 HP",
                race: Race.Aberration,
                effectTags: "ON_COMBAT_START|DESTROY_EQUIPMENT:ENEMY\nWITH_DISADVANTAGE|+HP:2"),
            // Budget 48: ATK 12(33) + HP 4(9) + INIT 1(1.5) + Treasure 1(-1) = 42.5. Effect = 5.5 ✓

            // Budget 42: ATK 10(27) + HP 3(6) + INIT 1(1.5) + Treasure 0(0) = 34.5. Effect budget = 7.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000029"),
                "Ettercap Broodmother",
                Rarity.Uncommon,
                cost: 2,
                attack: 10,
                hitPoints: 3,
                initiative: 1,
                treasure: 0,
                effect: "Race: Monstrosity. At start of each round: reduce one random attacker's INIT by 1. With Advantage: +1 ATK",
                race: Race.Monstrosity,
                effectTags: "ON_ROUND_START|-INIT:1:ENEMY\nWITH_ADVANTAGE|+ATK:1"),
            // Budget 42: ATK 10(27) + HP 3(6) + INIT 1(1.5) + Treasure 0(0) = 34.5. Effect = 7.5 ✓

            // Budget 42: ATK 10(27) + HP 3(6) + INIT 1(1.5) + Treasure 1(-1) = 33.5. Effect budget = 8.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000030"),
                "Rust Monster",
                Rarity.Uncommon,
                cost: 2,
                attack: 10,
                hitPoints: 3,
                initiative: 1,
                treasure: 1,
                effect: "Race: Monstrosity. When this monster deals damage: destroy target's Equipment. +1 ATK against allies with Equipment",
                race: Race.Monstrosity,
                effectTags: "ON_COMBAT_START|DESTROY_EQUIPMENT:ENEMY\nPASSIVE|IF_HAS_EQUIPMENT|+ATK:1"),
            // Budget 42: ATK 10(27) + HP 3(6) + INIT 1(1.5) + Treasure 1(-1) = 33.5. Effect = 8.5 ✓

            // =====================
            // COMMON (30) — Budget: Cost 1=20, 2=38; ×1.0 rarity multiplier
            // =====================

            // Budget 20: ATK 5(12) + HP 2(3) + INIT 1(1.5) + Treasure 0(0) = 16.5. Effect budget = 3.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000031"),
                "Goblin Warrior",
                Rarity.Common,
                cost: 1,
                attack: 5,
                hitPoints: 2,
                initiative: 1,
                treasure: 0,
                effect: "Race: Goblinoid. With Advantage: +1 ATK",
                race: Race.Goblinoid,
                effectTags: "WITH_ADVANTAGE|+ATK:1"),
            // Budget 20: ATK 5(12) + HP 2(3) + INIT 1(1.5) + Treasure 0(0) = 16.5. Effect = 3.5 ✓

            // Budget 38: ATK 10(27) + HP 3(6) + INIT 1(1.5) + Treasure 1(-1) = 33.5. Effect budget = 4.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000032"),
                "Orc Raider",
                Rarity.Common,
                cost: 2,
                attack: 10,
                hitPoints: 3,
                initiative: 1,
                treasure: 1,
                effect: "Race: Orc. +1 ATK against allies with 3 or less HP remaining",
                race: Race.Orc,
                effectTags: "PASSIVE|IF_LOW_HP:3|+ATK:1"),
            // Budget 38: ATK 10(27) + HP 3(6) + INIT 1(1.5) + Treasure 1(-1) = 33.5. Effect = 4.5 ✓

            // Budget 20: ATK 5(12) + HP 2(3) + INIT 1(1.5) + Treasure 0(0) = 16.5. Effect budget = 3.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000033"),
                "Skeleton Soldier",
                Rarity.Common,
                cost: 1,
                attack: 5,
                hitPoints: 2,
                initiative: 1,
                treasure: 0,
                effect: "Race: Undead. With Disadvantage: +1 HP",
                race: Race.Undead,
                effectTags: "WITH_DISADVANTAGE|+HP:1"),
            // Budget 20: ATK 5(12) + HP 2(3) + INIT 1(1.5) + Treasure 0(0) = 16.5. Effect = 3.5 ✓

            // Budget 20: ATK 6(15) + HP 2(3) + INIT 0(0) + Treasure 0(0) = 18. Effect budget = 2 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000034"),
                "Zombie Shambler",
                Rarity.Common,
                cost: 1,
                attack: 6,
                hitPoints: 2,
                initiative: 0,
                treasure: 0,
                effect: "Race: Undead. When eliminated: restore this card from discard to the top of your deck",
                race: Race.Undead,
                effectTags: "ON_DEATH|RESTORE_SELF_TO_DECK"),
            // Budget 20: ATK 6(15) + HP 2(3) + INIT 0(0) + Treasure 0(0) = 18. Effect = 2 ✓

            // Budget 20: ATK 6(15) + HP 2(3) + INIT 0(0) + Treasure 0(0) = 18. Effect budget = 2 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000035"),
                "Kobold Trapper",
                Rarity.Common,
                cost: 1,
                attack: 6,
                hitPoints: 2,
                initiative: 0,
                treasure: 0,
                effect: "Race: Goblinoid. When attacked: deal 1 damage to the attacker",
                race: Race.Goblinoid,
                effectTags: "PASSIVE|DAMAGE:1:ENEMY"),
            // Budget 20: ATK 6(15) + HP 2(3) + INIT 0(0) + Treasure 0(0) = 18. Effect = 2 ✓

            // Budget 20: ATK 5(12) + HP 2(3) + INIT 1(1.5) + Treasure 0(0) = 16.5. Effect budget = 3.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000036"),
                "Wolf Pack",
                Rarity.Common,
                cost: 1,
                attack: 5,
                hitPoints: 2,
                initiative: 1,
                treasure: 0,
                effect: "Race: Beast. With Advantage: +1 ATK",
                race: Race.Beast,
                effectTags: "WITH_ADVANTAGE|+ATK:1"),
            // Budget 20: ATK 5(12) + HP 2(3) + INIT 1(1.5) + Treasure 0(0) = 16.5. Effect = 3.5 ✓

            // Budget 20: ATK 6(15) + HP 2(3) + INIT 0(0) + Treasure 0(0) = 18. Effect budget = 2 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000037"),
                "Giant Spider",
                Rarity.Common,
                cost: 1,
                attack: 6,
                hitPoints: 2,
                initiative: 0,
                treasure: 0,
                effect: "Race: Beast. When this monster deals damage: reduce target's INIT by 1 this round",
                race: Race.Beast,
                effectTags: "ON_COMBAT_START|-INIT:1:ENEMY"),
            // Budget 20: ATK 6(15) + HP 2(3) + INIT 0(0) + Treasure 0(0) = 18. Effect = 2 ✓

            // Budget 20: ATK 6(15) + HP 2(3) + INIT 0(0) + Treasure 0(0) = 18. Effect budget = 2 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000038"),
                "Giant Rat",
                Rarity.Common,
                cost: 1,
                attack: 6,
                hitPoints: 2,
                initiative: 0,
                treasure: 0,
                effect: "Race: Beast. When this monster deals damage: deal 1 additional damage next round",
                race: Race.Beast,
                effectTags: "ON_COMBAT_START|DAMAGE:1:ENEMY"),
            // Budget 20: ATK 6(15) + HP 2(3) + INIT 0(0) + Treasure 0(0) = 18. Effect = 2 ✓

            // Budget 20: ATK 5(12) + HP 2(3) + INIT 1(1.5) + Treasure 1(-1) = 15.5. Effect budget = 4.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000039"),
                "Bandit Thug",
                Rarity.Common,
                cost: 1,
                attack: 5,
                hitPoints: 2,
                initiative: 1,
                treasure: 1,
                effect: "Race: Human. With Advantage: +1 Treasure",
                race: Race.Human,
                effectTags: "WITH_ADVANTAGE|+TREASURE:1"),
            // Budget 20: ATK 5(12) + HP 2(3) + INIT 1(1.5) + Treasure 1(-1) = 15.5. Effect = 4.5 ✓

            // Budget 38: ATK 10(27) + HP 3(6) + INIT 1(1.5) + Treasure 0(0) = 34.5. Effect budget = 3.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000040"),
                "Cultist Fanatic",
                Rarity.Common,
                cost: 2,
                attack: 10,
                hitPoints: 3,
                initiative: 1,
                treasure: 0,
                effect: "Race: Human. When this monster deals damage: exile 1 card from the top of attacker's deck",
                race: Race.Human,
                effectTags: "ON_COMBAT_START|EXILE_DECK:1"),
            // Budget 38: ATK 10(27) + HP 3(6) + INIT 1(1.5) + Treasure 0(0) = 34.5. Effect = 3.5 ✓

            // Budget 38: ATK 10(27) + HP 3(6) + INIT 1(1.5) + Treasure 1(-1) = 33.5. Effect budget = 4.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000041"),
                "Gnoll Hunter",
                Rarity.Common,
                cost: 2,
                attack: 10,
                hitPoints: 3,
                initiative: 1,
                treasure: 1,
                effect: "Race: Gnoll. When this monster eliminates an ally: +2 ATK this combat",
                race: Race.Gnoll,
                effectTags: "ON_KILL|+ATK:2"),
            // Budget 38: ATK 10(27) + HP 3(6) + INIT 1(1.5) + Treasure 1(-1) = 33.5. Effect = 4.5 ✓

            // Budget 38: ATK 10(27) + HP 3(6) + INIT 1(1.5) + Treasure 0(0) = 34.5. Effect budget = 3.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000042"),
                "Hobgoblin Soldier",
                Rarity.Common,
                cost: 2,
                attack: 10,
                hitPoints: 3,
                initiative: 1,
                treasure: 0,
                effect: "Race: Goblinoid. With Advantage: +1 ATK and +1 HP",
                race: Race.Goblinoid,
                effectTags: "WITH_ADVANTAGE|+ATK:1|+HP:1"),
            // Budget 38: ATK 10(27) + HP 3(6) + INIT 1(1.5) + Treasure 0(0) = 34.5. Effect = 3.5 ✓

            // Budget 38: ATK 9(24) + HP 3(6) + INIT 1(1.5) + Treasure 1(-1) = 30.5. Effect budget = 7.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000043"),
                "Bugbear Scout",
                Rarity.Common,
                cost: 2,
                attack: 9,
                hitPoints: 3,
                initiative: 1,
                treasure: 1,
                effect: "Race: Goblinoid. Ambusher. With Disadvantage: +1 HP",
                race: Race.Goblinoid,
                effectTags: "PASSIVE|AMBUSHER\nWITH_DISADVANTAGE|+HP:1"),
            // Budget 38: ATK 9(24) + HP 3(6) + INIT 1(1.5) + Treasure 1(-1) = 30.5. Effect = 7.5 ✓

            // Budget 38: ATK 9(24) + HP 3(6) + INIT 1(1.5) + Treasure 1(-1) = 30.5. Effect budget = 7.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000044"),
                "Harpy",
                Rarity.Common,
                cost: 2,
                attack: 9,
                hitPoints: 3,
                initiative: 1,
                treasure: 1,
                effect: "Race: Monstrosity. Reduces one random attacker's INIT by 1 this round",
                race: Race.Monstrosity,
                effectTags: "PASSIVE|-INIT:1:ENEMY"),
            // Budget 38: ATK 9(24) + HP 3(6) + INIT 1(1.5) + Treasure 1(-1) = 30.5. Effect = 7.5 ✓

            // Budget 20: ATK 6(15) + HP 2(3) + INIT 0(0) + Treasure 0(0) = 18. Effect budget = 2 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000045"),
                "Stirge Swarm",
                Rarity.Common,
                cost: 1,
                attack: 6,
                hitPoints: 2,
                initiative: 0,
                treasure: 0,
                effect: "Race: Beast. When this monster deals damage: restore 1 HP to this monster",
                race: Race.Beast,
                effectTags: "ON_COMBAT_START|HEAL:1:SELF"),
            // Budget 20: ATK 6(15) + HP 2(3) + INIT 0(0) + Treasure 0(0) = 18. Effect = 2 ✓

            // Budget 20: ATK 5(12) + HP 2(3) + INIT 1(1.5) + Treasure 0(0) = 16.5. Effect budget = 3.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000046"),
                "Fire Beetle",
                Rarity.Common,
                cost: 1,
                attack: 5,
                hitPoints: 2,
                initiative: 1,
                treasure: 0,
                effect: "Race: Beast. Negates Ambusher protection on one attacking ally",
                race: Race.Beast,
                effectTags: "PASSIVE|IGNORE_AMBUSHER"),
            // Budget 20: ATK 5(12) + HP 2(3) + INIT 1(1.5) + Treasure 0(0) = 16.5. Effect = 3.5 ✓

            // Budget 20: ATK 6(15) + HP 2(3) + INIT 0(0) + Treasure 0(0) = 18. Effect budget = 2 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000047"),
                "Jackalwere",
                Rarity.Common,
                cost: 1,
                attack: 6,
                hitPoints: 2,
                initiative: 0,
                treasure: 0,
                effect: "Race: Monstrosity. With Advantage: reduce one attacker's ATK by 2 this round",
                race: Race.Monstrosity,
                effectTags: "WITH_ADVANTAGE|-ATK:2:ENEMY"),
            // Budget 20: ATK 6(15) + HP 2(3) + INIT 0(0) + Treasure 0(0) = 18. Effect = 2 ✓

            // Budget 38: ATK 10(27) + HP 3(6) + INIT 1(1.5) + Treasure 0(0) = 34.5. Effect budget = 3.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000048"),
                "Ghoul",
                Rarity.Common,
                cost: 2,
                attack: 10,
                hitPoints: 3,
                initiative: 1,
                treasure: 0,
                effect: "Race: Undead. When this monster deals damage: reduce target's INIT by 1 this round",
                race: Race.Undead,
                effectTags: "ON_COMBAT_START|-INIT:1:ENEMY"),
            // Budget 38: ATK 10(27) + HP 3(6) + INIT 1(1.5) + Treasure 0(0) = 34.5. Effect = 3.5 ✓

            // Budget 20: ATK 6(15) + HP 2(3) + INIT 0(0) + Treasure 0(0) = 18. Effect budget = 2 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000049"),
                "Shadow",
                Rarity.Common,
                cost: 1,
                attack: 6,
                hitPoints: 2,
                initiative: 0,
                treasure: 0,
                effect: "Race: Undead. When this monster deals damage: reduce target's ATK by 1 this combat",
                race: Race.Undead,
                effectTags: "ON_COMBAT_START|-ATK:1:ENEMY"),
            // Budget 20: ATK 6(15) + HP 2(3) + INIT 0(0) + Treasure 0(0) = 18. Effect = 2 ✓

            // Budget 20: ATK 6(15) + HP 2(3) + INIT 0(0) + Treasure 0(0) = 18. Effect budget = 2 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000050"),
                "Violet Fungus",
                Rarity.Common,
                cost: 1,
                attack: 6,
                hitPoints: 2,
                initiative: 0,
                treasure: 0,
                effect: "Race: Plant. When attacked: deal 1 damage to the attacker",
                race: Race.Plant,
                effectTags: "PASSIVE|DAMAGE:1:ENEMY"),
            // Budget 20: ATK 6(15) + HP 2(3) + INIT 0(0) + Treasure 0(0) = 18. Effect = 2 ✓

            // Budget 20: ATK 6(15) + HP 2(3) + INIT 0(0) + Treasure 0(0) = 18. Effect budget = 2 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000051"),
                "Darkmantle",
                Rarity.Common,
                cost: 1,
                attack: 6,
                hitPoints: 2,
                initiative: 0,
                treasure: 0,
                effect: "Race: Monstrosity. When this monster deals damage: reduce target's ATK by 1 this round",
                race: Race.Monstrosity,
                effectTags: "ON_COMBAT_START|-ATK:1:ENEMY"),
            // Budget 20: ATK 6(15) + HP 2(3) + INIT 0(0) + Treasure 0(0) = 18. Effect = 2 ✓

            // Budget 20: ATK 6(15) + HP 2(3) + INIT 0(0) + Treasure 0(0) = 18. Effect budget = 2 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000052"),
                "Piercer",
                Rarity.Common,
                cost: 1,
                attack: 6,
                hitPoints: 2,
                initiative: 0,
                treasure: 0,
                effect: "Race: Monstrosity. +2 ATK on the first round of combat",
                race: Race.Monstrosity,
                effectTags: "ON_COMBAT_START|+ATK:2"),
            // Budget 20: ATK 6(15) + HP 2(3) + INIT 0(0) + Treasure 0(0) = 18. Effect = 2 ✓

            // Budget 20: ATK 6(15) + HP 2(3) + INIT 0(0) + Treasure 0(0) = 18. Effect budget = 2 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000053"),
                "Gas Spore",
                Rarity.Common,
                cost: 1,
                attack: 6,
                hitPoints: 2,
                initiative: 0,
                treasure: 0,
                effect: "Race: Plant. When eliminated: deal 2 damage to the ally that eliminated this monster",
                race: Race.Plant,
                effectTags: "ON_DEATH|DAMAGE:1:ENEMY"),
            // Budget 20: ATK 6(15) + HP 2(3) + INIT 0(0) + Treasure 0(0) = 18. Effect = 2 ✓

            // Budget 20: ATK 5(12) + HP 2(3) + INIT 1(1.5) + Treasure 0(0) = 16.5. Effect budget = 3.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000054"),
                "Swarm of Bats",
                Rarity.Common,
                cost: 1,
                attack: 5,
                hitPoints: 2,
                initiative: 1,
                treasure: 0,
                effect: "Race: Beast. Reduce one attacker's INIT by 1 this round",
                race: Race.Beast,
                effectTags: "PASSIVE|-INIT:1:ENEMY"),
            // Budget 20: ATK 5(12) + HP 2(3) + INIT 1(1.5) + Treasure 0(0) = 16.5. Effect = 3.5 ✓

            // Budget 20: ATK 6(15) + HP 2(3) + INIT 0(0) + Treasure 0(0) = 18. Effect budget = 2 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000055"),
                "Giant Centipede",
                Rarity.Common,
                cost: 1,
                attack: 6,
                hitPoints: 2,
                initiative: 0,
                treasure: 0,
                effect: "Race: Beast. When this monster deals damage: reduce target's ATK by 1 and INIT by 1 this round",
                race: Race.Beast,
                effectTags: "ON_COMBAT_START|-ATK:1:ENEMY|-INIT:1:ENEMY"),
            // Budget 20: ATK 6(15) + HP 2(3) + INIT 0(0) + Treasure 0(0) = 18. Effect = 2 ✓

            // Budget 38: ATK 9(24) + HP 3(6) + INIT 1(1.5) + Treasure 1(-1) = 30.5. Effect budget = 7.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000056"),
                "Magma Mephit",
                Rarity.Common,
                cost: 2,
                attack: 9,
                hitPoints: 3,
                initiative: 1,
                treasure: 1,
                effect: "Race: Elemental. When eliminated: deal 2 damage to the ally that eliminated this monster",
                race: Race.Elemental,
                effectTags: "ON_DEATH|DAMAGE:1:ENEMY"),
            // Budget 38: ATK 9(24) + HP 3(6) + INIT 1(1.5) + Treasure 1(-1) = 30.5. Effect = 7.5 ✓

            // Budget 38: ATK 9(24) + HP 3(6) + INIT 1(1.5) + Treasure 1(-1) = 30.5. Effect budget = 7.5 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000057"),
                "Ice Mephit",
                Rarity.Common,
                cost: 2,
                attack: 9,
                hitPoints: 3,
                initiative: 1,
                treasure: 1,
                effect: "Race: Elemental. When this monster deals damage: reduce target's INIT by 1 this round",
                race: Race.Elemental,
                effectTags: "ON_COMBAT_START|-INIT:1:ENEMY"),
            // Budget 38: ATK 9(24) + HP 3(6) + INIT 1(1.5) + Treasure 1(-1) = 30.5. Effect = 7.5 ✓

            // Budget 20: ATK 6(15) + HP 2(3) + INIT 0(0) + Treasure 0(0) = 18. Effect budget = 2 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000058"),
                "Quasit",
                Rarity.Common,
                cost: 1,
                attack: 6,
                hitPoints: 2,
                initiative: 0,
                treasure: 0,
                effect: "Race: Demon. Ambusher",
                race: Race.Demon,
                effectTags: "PASSIVE|AMBUSHER"),
            // Budget 20: ATK 6(15) + HP 2(3) + INIT 0(0) + Treasure 0(0) = 18. Effect = 2 ✓

            // Budget 20: ATK 6(15) + HP 2(3) + INIT 0(0) + Treasure 0(0) = 18. Effect budget = 2 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000059"),
                "Imp",
                Rarity.Common,
                cost: 1,
                attack: 6,
                hitPoints: 2,
                initiative: 0,
                treasure: 0,
                effect: "Race: Demon. Ambusher. With Disadvantage: +1 HP",
                race: Race.Demon,
                effectTags: "PASSIVE|AMBUSHER\nWITH_DISADVANTAGE|+HP:1"),
            // Budget 20: ATK 6(15) + HP 2(3) + INIT 0(0) + Treasure 0(0) = 18. Effect = 2 ✓

            // Budget 20: ATK 5(12) + HP 2(3) + INIT 0(0) + Treasure 1(-1) = 14. Effect budget = 6 ✓
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000060"),
                "Flying Sword",
                Rarity.Common,
                cost: 1,
                attack: 5,
                hitPoints: 2,
                initiative: 0,
                treasure: 1,
                effect: "Race: Construct. With Advantage: +1 ATK",
                race: Race.Construct,
                effectTags: "WITH_ADVANTAGE|+ATK:1"),
            // Budget 20: ATK 5(12) + HP 2(3) + INIT 0(0) + Treasure 1(-1) = 14. Effect = 6 ✓
        };
    }
}
