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
            // UNIQUE (2)
            // =====================
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000001"),
                "Ancient Red Dragon",
                Rarity.Unique,
                cost: 6,
                strength: 10,
                hitPoints: 12,
                initiative: 6,
                treasure: 5,
                effect: "When played, deals 3 fire damage to all enemy monsters on the board"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000002"),
                "Elder Brain",
                Rarity.Unique,
                cost: 5,
                strength: 7,
                hitPoints: 10,
                initiative: 5,
                treasure: 4,
                effect: "At the start of each turn, takes control of one random enemy monster with 4 or less Strength"),

            // =====================
            // RARE (8)
            // =====================
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000003"),
                "Beholder Tyrant",
                Rarity.Rare,
                cost: 5,
                strength: 7,
                hitPoints: 9,
                initiative: 4,
                treasure: 4,
                effect: "Each turn, one of its eye rays activates at random: disintegrate (deal 4 damage), charm (steal a monster), or petrify (stun for 2 turns)"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000004"),
                "Lich Lord",
                Rarity.Rare,
                cost: 5,
                strength: 6,
                hitPoints: 8,
                initiative: 5,
                treasure: 4,
                effect: "When destroyed, returns to play after 1 turn with half HP if its phylactery has not been found"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000005"),
                "Mind Flayer Arcanist",
                Rarity.Rare,
                cost: 4,
                strength: 5,
                hitPoints: 7,
                initiative: 4,
                treasure: 3,
                effect: "On attack, reduces the target's Strength by 2 permanently as it devours their intellect"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000006"),
                "Vampire Lord",
                Rarity.Rare,
                cost: 5,
                strength: 6,
                hitPoints: 8,
                initiative: 5,
                treasure: 4,
                effect: "Heals HP equal to half the damage dealt and converts destroyed humanoid monsters into Zombie Shamblers"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000007"),
                "Death Knight",
                Rarity.Rare,
                cost: 5,
                strength: 8,
                hitPoints: 10,
                initiative: 3,
                treasure: 3,
                effect: "Deals 2 extra damage to targets with the Holy or Radiant keyword and ignores healing effects"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000008"),
                "Pit Fiend",
                Rarity.Rare,
                cost: 5,
                strength: 7,
                hitPoints: 9,
                initiative: 4,
                treasure: 4,
                effect: "Immune to fire damage and at the end of each turn deals 1 fire damage to all adjacent enemies"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000009"),
                "Balor Demon",
                Rarity.Rare,
                cost: 5,
                strength: 8,
                hitPoints: 10,
                initiative: 3,
                treasure: 3,
                effect: "When destroyed, explodes dealing 4 fire damage to every creature on the board"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000010"),
                "Iron Golem",
                Rarity.Rare,
                cost: 4,
                strength: 7,
                hitPoints: 10,
                initiative: 3,
                treasure: 3,
                effect: "Immune to all magic effects and spells; fire damage heals it instead of dealing damage"),

            // =====================
            // UNCOMMON (20)
            // =====================
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000011"),
                "Young Black Dragon",
                Rarity.Uncommon,
                cost: 4,
                strength: 6,
                hitPoints: 8,
                initiative: 4,
                treasure: 3,
                effect: "Sprays acid in a line, dealing 2 damage to the target and 1 damage to the creature behind it"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000012"),
                "Troll",
                Rarity.Uncommon,
                cost: 3,
                strength: 5,
                hitPoints: 7,
                initiative: 3,
                treasure: 2,
                effect: "Regenerates 2 HP at the start of each turn unless it took fire or acid damage last turn"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000013"),
                "Ogre Mage",
                Rarity.Uncommon,
                cost: 3,
                strength: 5,
                hitPoints: 6,
                initiative: 3,
                treasure: 3,
                effect: "Can turn invisible for one turn, guaranteeing the next attack hits and deals double damage"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000014"),
                "Wraith",
                Rarity.Uncommon,
                cost: 3,
                strength: 4,
                hitPoints: 5,
                initiative: 4,
                treasure: 2,
                effect: "Incorporeal: has a 50% chance to avoid all physical damage from each attack"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000015"),
                "Mummy Lord",
                Rarity.Uncommon,
                cost: 4,
                strength: 5,
                hitPoints: 7,
                initiative: 2,
                treasure: 3,
                effect: "Inflicts a rotting curse on attackers, reducing their max HP by 1 each turn until cured"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000016"),
                "Medusa",
                Rarity.Uncommon,
                cost: 3,
                strength: 4,
                hitPoints: 6,
                initiative: 3,
                treasure: 3,
                effect: "Before combat, forces a saving throw; on failure the attacker is petrified and skips their next turn"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000017"),
                "Manticore",
                Rarity.Uncommon,
                cost: 3,
                strength: 5,
                hitPoints: 6,
                initiative: 3,
                treasure: 2,
                effect: "Launches tail spikes at the start of combat dealing 1 damage to up to 3 random enemies"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000018"),
                "Basilisk",
                Rarity.Uncommon,
                cost: 3,
                strength: 4,
                hitPoints: 7,
                initiative: 2,
                treasure: 2,
                effect: "Any creature that attacks it in melee must pass a saving throw or be petrified for 1 turn"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000019"),
                "Chimera",
                Rarity.Uncommon,
                cost: 4,
                strength: 6,
                hitPoints: 7,
                initiative: 3,
                treasure: 2,
                effect: "Attacks three times per turn with reduced damage: bite for 2, horns for 2, and fire breath for 2"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000020"),
                "Displacer Beast",
                Rarity.Uncommon,
                cost: 3,
                strength: 5,
                hitPoints: 6,
                initiative: 4,
                treasure: 2,
                effect: "Displacement illusion causes the first attack against it each turn to automatically miss"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000021"),
                "Minotaur Champion",
                Rarity.Uncommon,
                cost: 3,
                strength: 6,
                hitPoints: 6,
                initiative: 3,
                treasure: 2,
                effect: "Charges into battle dealing 2 bonus damage on its first attack if it has not taken damage this turn"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000022"),
                "Gibbering Mouther",
                Rarity.Uncommon,
                cost: 2,
                strength: 3,
                hitPoints: 5,
                initiative: 2,
                treasure: 2,
                effect: "Emits maddening gibberish that reduces all adjacent enemies' Initiative by 2"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000023"),
                "Phase Spider",
                Rarity.Uncommon,
                cost: 2,
                strength: 4,
                hitPoints: 4,
                initiative: 4,
                treasure: 2,
                effect: "Phases to the Ethereal Plane after attacking, becoming untargetable until its next turn"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000024"),
                "Otyugh",
                Rarity.Uncommon,
                cost: 3,
                strength: 5,
                hitPoints: 7,
                initiative: 2,
                treasure: 2,
                effect: "Grapples a target with its tentacles, preventing it from acting for 1 turn while dealing 1 damage per turn"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000025"),
                "Grick Alpha",
                Rarity.Uncommon,
                cost: 3,
                strength: 4,
                hitPoints: 6,
                initiative: 3,
                treasure: 2,
                effect: "Stone camouflage grants it a surprise attack dealing double damage if played from hand this turn"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000026"),
                "Bulette",
                Rarity.Uncommon,
                cost: 3,
                strength: 5,
                hitPoints: 8,
                initiative: 2,
                treasure: 2,
                effect: "Burrows underground and leaps out, ignoring taunt effects and targeting the weakest enemy directly"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000027"),
                "Carrion Crawler",
                Rarity.Uncommon,
                cost: 2,
                strength: 3,
                hitPoints: 5,
                initiative: 3,
                treasure: 2,
                effect: "Paralyzing tentacles stun the target for 1 turn on a successful hit"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000028"),
                "Chuul",
                Rarity.Uncommon,
                cost: 3,
                strength: 5,
                hitPoints: 6,
                initiative: 2,
                treasure: 2,
                effect: "Detects and dispels one magical effect on the target when it attacks, removing their active buffs"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000029"),
                "Ettercap Broodmother",
                Rarity.Uncommon,
                cost: 2,
                strength: 3,
                hitPoints: 5,
                initiative: 3,
                treasure: 2,
                effect: "Spawns a 1/1 Spiderling token at the end of each turn it remains alive"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000030"),
                "Rust Monster",
                Rarity.Uncommon,
                cost: 2,
                strength: 3,
                hitPoints: 4,
                initiative: 3,
                treasure: 3,
                effect: "Corrodes enemy equipment on hit, permanently reducing the target's Strength by 1"),

            // =====================
            // COMMON (30)
            // =====================
            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000031"),
                "Goblin Warrior",
                Rarity.Common,
                cost: 1,
                strength: 2,
                hitPoints: 3,
                initiative: 3,
                treasure: 1,
                effect: "Gains +1 Strength for each other Goblin on the board"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000032"),
                "Orc Raider",
                Rarity.Common,
                cost: 2,
                strength: 4,
                hitPoints: 4,
                initiative: 2,
                treasure: 2,
                effect: "Deals 1 extra damage when attacking a target that has already been damaged this turn"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000033"),
                "Skeleton Soldier",
                Rarity.Common,
                cost: 1,
                strength: 2,
                hitPoints: 3,
                initiative: 2,
                treasure: 1,
                effect: "Takes half damage from piercing and slashing attacks but double damage from bludgeoning"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000034"),
                "Zombie Shambler",
                Rarity.Common,
                cost: 1,
                strength: 2,
                hitPoints: 5,
                initiative: 1,
                treasure: 1,
                effect: "When reduced to 0 HP, has a 50% chance to stand back up with 1 HP"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000035"),
                "Kobold Trapper",
                Rarity.Common,
                cost: 1,
                strength: 1,
                hitPoints: 2,
                initiative: 3,
                treasure: 1,
                effect: "Places a trap that deals 2 damage to the next enemy that attacks it"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000036"),
                "Wolf Pack",
                Rarity.Common,
                cost: 1,
                strength: 3,
                hitPoints: 3,
                initiative: 3,
                treasure: 1,
                effect: "Pack tactics: gains +1 Strength when an ally is adjacent to the same target"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000037"),
                "Giant Spider",
                Rarity.Common,
                cost: 1,
                strength: 2,
                hitPoints: 4,
                initiative: 2,
                treasure: 1,
                effect: "Webs the target on hit, reducing their Initiative by 2 for the next turn"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000038"),
                "Giant Rat",
                Rarity.Common,
                cost: 1,
                strength: 1,
                hitPoints: 2,
                initiative: 3,
                treasure: 1,
                effect: "Diseased bite: the target loses 1 HP at the start of each turn for 2 turns"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000039"),
                "Bandit Thug",
                Rarity.Common,
                cost: 1,
                strength: 2,
                hitPoints: 3,
                initiative: 2,
                treasure: 2,
                effect: "Steals 1 treasure from the opponent when it deals damage"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000040"),
                "Cultist Fanatic",
                Rarity.Common,
                cost: 2,
                strength: 3,
                hitPoints: 4,
                initiative: 2,
                treasure: 1,
                effect: "Sacrifices 1 HP to deal 2 extra dark damage on its next attack"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000041"),
                "Gnoll Hunter",
                Rarity.Common,
                cost: 2,
                strength: 3,
                hitPoints: 4,
                initiative: 2,
                treasure: 1,
                effect: "Enters a rampage after destroying an enemy, gaining +2 Strength until end of turn"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000042"),
                "Hobgoblin Soldier",
                Rarity.Common,
                cost: 2,
                strength: 3,
                hitPoints: 4,
                initiative: 2,
                treasure: 1,
                effect: "Martial discipline: gains +1 Strength when adjacent to another Hobgoblin or ally in formation"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000043"),
                "Bugbear Scout",
                Rarity.Common,
                cost: 2,
                strength: 4,
                hitPoints: 3,
                initiative: 2,
                treasure: 1,
                effect: "Ambush: deals 2 extra damage on its first attack if it was played this turn"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000044"),
                "Harpy",
                Rarity.Common,
                cost: 2,
                strength: 3,
                hitPoints: 3,
                initiative: 3,
                treasure: 1,
                effect: "Luring song forces the weakest enemy to attack the Harpy instead of its intended target"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000045"),
                "Stirge Swarm",
                Rarity.Common,
                cost: 1,
                strength: 1,
                hitPoints: 2,
                initiative: 3,
                treasure: 1,
                effect: "Attaches to target and drains 1 HP per turn, healing itself for the same amount"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000046"),
                "Fire Beetle",
                Rarity.Common,
                cost: 1,
                strength: 1,
                hitPoints: 2,
                initiative: 2,
                treasure: 1,
                effect: "Glowing glands illuminate hidden enemies, removing stealth from one random enemy"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000047"),
                "Jackalwere",
                Rarity.Common,
                cost: 1,
                strength: 2,
                hitPoints: 3,
                initiative: 2,
                treasure: 1,
                effect: "Sleep gaze puts one enemy with 3 or less HP to sleep, stunning them for 1 turn"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000048"),
                "Ghoul",
                Rarity.Common,
                cost: 2,
                strength: 3,
                hitPoints: 4,
                initiative: 2,
                treasure: 1,
                effect: "Paralyzing claws: the target cannot act on its next turn if it has 5 or less HP"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000049"),
                "Shadow",
                Rarity.Common,
                cost: 1,
                strength: 2,
                hitPoints: 3,
                initiative: 2,
                treasure: 1,
                effect: "Strength drain: permanently reduces the target's Strength by 1 on a successful hit"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000050"),
                "Violet Fungus",
                Rarity.Common,
                cost: 1,
                strength: 2,
                hitPoints: 4,
                initiative: 1,
                treasure: 1,
                effect: "Rotting touch deals 1 necrotic damage to all enemies that attack it in melee"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000051"),
                "Darkmantle",
                Rarity.Common,
                cost: 1,
                strength: 2,
                hitPoints: 3,
                initiative: 2,
                treasure: 1,
                effect: "Drops from above and engulfs the target's head, blinding them and reducing their Strength by 1 for 2 turns"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000052"),
                "Piercer",
                Rarity.Common,
                cost: 1,
                strength: 3,
                hitPoints: 2,
                initiative: 1,
                treasure: 1,
                effect: "Falls from the ceiling on its first attack dealing triple damage, but only attacks once then is revealed"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000053"),
                "Gas Spore",
                Rarity.Common,
                cost: 1,
                strength: 1,
                hitPoints: 2,
                initiative: 1,
                treasure: 1,
                effect: "When destroyed, releases spores that deal 2 poison damage to all adjacent creatures"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000054"),
                "Swarm of Bats",
                Rarity.Common,
                cost: 1,
                strength: 2,
                hitPoints: 3,
                initiative: 3,
                treasure: 1,
                effect: "Echolocation makes it immune to blinding effects and it reduces enemy Initiative by 1"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000055"),
                "Giant Centipede",
                Rarity.Common,
                cost: 1,
                strength: 1,
                hitPoints: 2,
                initiative: 2,
                treasure: 1,
                effect: "Venomous bite poisons the target, dealing 1 damage at the start of each turn for 3 turns"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000056"),
                "Magma Mephit",
                Rarity.Common,
                cost: 2,
                strength: 3,
                hitPoints: 3,
                initiative: 2,
                treasure: 2,
                effect: "On death, erupts in molten rock dealing 2 fire damage to all adjacent enemies"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000057"),
                "Ice Mephit",
                Rarity.Common,
                cost: 2,
                strength: 2,
                hitPoints: 4,
                initiative: 2,
                treasure: 2,
                effect: "Frost breath slows the target, reducing their Initiative by 2 until end of next turn"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000058"),
                "Quasit",
                Rarity.Common,
                cost: 1,
                strength: 2,
                hitPoints: 3,
                initiative: 3,
                treasure: 1,
                effect: "Shapeshifts to avoid the first attack each combat, then reverts and counterattacks for 1 damage"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000059"),
                "Imp",
                Rarity.Common,
                cost: 1,
                strength: 2,
                hitPoints: 3,
                initiative: 3,
                treasure: 1,
                effect: "Invisibility lets it bypass taunt effects; its sting deals 1 extra poison damage"),

            new MonsterCard(
                new Guid("00000000-0000-0000-0003-000000000060"),
                "Flying Sword",
                Rarity.Common,
                cost: 1,
                strength: 3,
                hitPoints: 2,
                initiative: 2,
                treasure: 2,
                effect: "Animated by magic, it attacks automatically each turn but is instantly destroyed by Dispel Magic effects"),
        };
    }
}
