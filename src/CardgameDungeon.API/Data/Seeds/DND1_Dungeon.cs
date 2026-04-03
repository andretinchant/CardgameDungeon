using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.API.Data.Seeds;

public static partial class CardSetSeeder
{
    private static List<DungeonRoomCard> CreateDungeonRooms()
    {
        return new List<DungeonRoomCard>
        {
            // Order 1 - Entry Rooms (budget 3-5)
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000001"), "Goblin Warren", Rarity.Common, 1, null, null, 3, "Goblins ambush from hidden tunnels; all allies take 1 damage at the start of combat"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000002"), "Rat-Infested Cellar", Rarity.Common, 1, null, null, 3, "Swarms of rats reduce the Strength of the weakest ally by 1 for this combat"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000003"), "Abandoned Guardroom", Rarity.Common, 1, null, null, 4, "Rusted weapons line the walls; monsters in this room gain +1 Strength"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000004"), "Dusty Antechamber", Rarity.Common, 1, null, null, 4, "Clouds of dust obscure vision; all attacks have a 25% chance to miss this round"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000005"), "Collapsed Tunnel", Rarity.Common, 1, null, null, 3, "Rubble blocks the path; the first ally to act must spend their turn clearing debris"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000006"), "Mushroom Grotto", Rarity.Common, 1, null, null, 5, "Toxic spores fill the air; allies lose 1 HP at the end of each round"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000007"), "Cobweb Gallery", Rarity.Common, 1, null, null, 4, "Sticky webs reduce all ally Initiative by 1 for this combat"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000008"), "Flooded Passage", Rarity.Common, 1, null, null, 5, "Waist-deep water slows movement; allies with Initiative 3 or less act last"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000009"), "Bone-Littered Cave", Rarity.Common, 1, null, null, 3, "Skeletal remains animate mid-combat; a 2/2 Skeleton joins the monster side"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000010"), "Rusty Gate Hall", Rarity.Common, 1, null, null, 5, "The gate slams shut behind the party; retreat is impossible until all monsters are defeated"),

            // Order 2 - Mid-Early Rooms (budget 5-7)
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000011"), "Orc Barracks", Rarity.Common, 2, null, null, 5, "War drums echo; monsters in this room gain +1 Initiative"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000012"), "Spider Nest", Rarity.Common, 2, null, null, 6, "Venomous bites poison one random ally, dealing 2 damage at the start of each round"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000013"), "Alchemist Workshop", Rarity.Common, 2, null, null, 5, "Volatile potions explode when disturbed; both sides take 2 damage at the start of combat"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000014"), "Cursed Chapel", Rarity.Common, 2, null, null, 7, "Dark prayers empower the undead; undead monsters gain +2 Strength"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000015"), "Sunken Crypt", Rarity.Common, 2, null, null, 6, "Waterlogged graves slow healing; allies cannot restore HP in this room"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000016"), "Gladiator Arena", Rarity.Common, 2, null, null, 7, "The crowd demands blood; the ally with the highest Strength must attack first each round"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000017"), "Forge of Chains", Rarity.Common, 2, null, null, 5, "Burning chains lash out; one random ally is restrained and skips their first turn"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000018"), "Bandit Hideout", Rarity.Common, 2, null, null, 6, "Thieves steal from the party; discard 1 random card from your hand at combat start"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000019"), "Poison Garden", Rarity.Common, 2, null, null, 7, "Thorny vines entangle allies; each ally takes 1 damage when they attack"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000020"), "Smuggler Cove", Rarity.Common, 2, null, null, 6, "Hidden trapdoors spring open; the lowest-HP ally falls and takes 3 damage"),

            // Order 3 - Mid Rooms (budget 7-9)
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000021"), "Hall of Mirrors", Rarity.Common, 3, null, null, 7, "Reflections confuse targeting; ally attacks are randomly assigned to any monster in the room"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000022"), "Elemental Nexus", Rarity.Common, 3, null, null, 8, "Elemental energy surges; monsters deal an additional 2 damage of a random element each round"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000023"), "Petrified Forest", Rarity.Common, 3, null, null, 7, "Stone trees animate as guardians; a 4/5 Treant joins the monster side"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000024"), "Shadow Labyrinth", Rarity.Common, 3, null, null, 9, "Darkness engulfs the room; allies have a 30% miss chance on all attacks"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000025"), "Blood Altar", Rarity.Common, 3, null, null, 8, "Sacrificial magic empowers monsters; they heal 2 HP each time they eliminate an ally"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000026"), "Golem Foundry", Rarity.Common, 3, null, null, 9, "Automated defenses activate; a 5/6 Iron Golem joins the monster side at round 2"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000027"), "Tomb of the Fallen", Rarity.Common, 3, null, null, 7, "Spirits of fallen warriors haunt the tomb; all allies lose 1 Strength permanently"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000028"), "Arcane Library", Rarity.Common, 3, null, null, 8, "Spell-warded shelves retaliate; any ally that deals magical damage takes 2 damage in return"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000029"), "Throne of Bones", Rarity.Common, 3, null, null, 9, "The throne commands undeath; eliminated monsters return as 2/2 Skeletons once"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000030"), "Crystal Cavern", Rarity.Common, 3, null, null, 8, "Resonating crystals amplify damage; all damage dealt and received is increased by 1"),

            // Order 4 - Mid-Late Rooms (budget 9-11)
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000031"), "Dragon Hatchery", Rarity.Common, 4, null, null, 9, "Eggs hatch mid-combat; a 5/5 Young Dragon joins the monster side at round 3"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000032"), "Lich Sanctum", Rarity.Common, 4, null, null, 10, "Necrotic aura drains life; allies lose 2 HP at the start of each round"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000033"), "Beholder Lair", Rarity.Common, 4, null, null, 11, "Antimagic cone disables effects; all ally card effects are suppressed for the first 2 rounds"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000034"), "Abyssal Rift", Rarity.Common, 4, null, null, 10, "Demonic energy seeps through; monsters gain +2 Strength and +2 HP"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000035"), "Vampire Catacombs", Rarity.Common, 4, null, null, 9, "Bloodthirsty mist fills the air; monsters heal HP equal to half the damage they deal"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000036"), "Mind Flayer Colony", Rarity.Common, 4, null, null, 11, "Psychic assault stuns the party; one random ally is stunned and cannot act each round"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000037"), "Drow War Camp", Rarity.Common, 4, null, null, 10, "Poison-coated blades cut deep; all monster attacks apply a 1-damage poison for 2 rounds"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000038"), "Frozen Throne Room", Rarity.Common, 4, null, null, 9, "Bitter cold slows the party; all ally Initiative is reduced by 2"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000039"), "Lava Forge", Rarity.Common, 4, null, null, 11, "Molten metal erupts periodically; all combatants take 3 damage at the end of every other round"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000040"), "Astral Observatory", Rarity.Common, 4, null, null, 10, "Planar distortion warps reality; ally and monster positions are shuffled each round"),

            // Order 5 - Final Rooms (budget 11-14)
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000041"), "Chamber of the Dracolich", Rarity.Common, 5, null, null, 13, "Ancient draconic undeath pervades; monsters are immune to damage on the first round of combat"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000042"), "Vault of Nine Hells", Rarity.Common, 5, null, null, 14, "Hellfire burns relentlessly; all allies take 3 damage at the start of each round"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000043"), "Cathedral of Shadows", Rarity.Common, 5, null, null, 12, "Living shadows duplicate monsters; each monster creates a 3/3 Shadow copy at combat start"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000044"), "Citadel of the Dead God", Rarity.Common, 5, null, null, 14, "Divine necrotic energy pulses; allies cannot heal and lose 1 max HP each round"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000045"), "Planar Convergence", Rarity.Common, 5, null, null, 11, "Reality fractures unpredictably; random allies are banished for 1 round then return"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000046"), "World Tree Root", Rarity.Common, 5, null, null, 13, "Primeval nature magic empowers monsters; all monsters regenerate 3 HP at the start of each round"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000047"), "Chrono Chamber", Rarity.Common, 5, null, null, 12, "Time distortion gives monsters an extra action; monsters attack twice per round"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000048"), "Demonweb Pits", Rarity.Common, 5, null, null, 14, "Abyssal webbing traps the party; two random allies are restrained and skip their turns each round"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000049"), "Far Realm Breach", Rarity.Common, 5, null, null, 11, "Eldritch madness warps the mind; allies have a 20% chance to attack their own team each round"),
            new DungeonRoomCard(new Guid("00000000-0000-0000-0005-000000000050"), "Primordial Cradle", Rarity.Common, 5, null, null, 13, "Raw elemental chaos rages; all combatants take 2 random elemental damage at the end of each round"),
        };
    }

    private static List<BossCard> CreateBosses()
    {
        return new List<BossCard>
        {
            new BossCard(new Guid("00000000-0000-0000-0006-000000000001"), "Tiamat", Rarity.Unique, 6, 15, 25, 5, "Each round, deals damage equal to the number of different ally types in play"),
            new BossCard(new Guid("00000000-0000-0000-0006-000000000002"), "Vecna the Undying", Rarity.Unique, 6, 12, 22, 6, "At the start of combat, exiles the top 2 cards from each attacker's deck"),
            new BossCard(new Guid("00000000-0000-0000-0006-000000000003"), "Strahd von Zarovich", Rarity.Unique, 5, 10, 20, 5, "Heals HP equal to damage dealt each round"),
            new BossCard(new Guid("00000000-0000-0000-0006-000000000004"), "Acererak the Archlich", Rarity.Unique, 6, 11, 18, 6, "Immune to damage from allies with Strength 3 or less"),
            new BossCard(new Guid("00000000-0000-0000-0006-000000000005"), "Demogorgon", Rarity.Unique, 6, 14, 23, 4, "Attacks two random allies each round instead of one"),
            new BossCard(new Guid("00000000-0000-0000-0006-000000000006"), "Lolth the Spider Queen", Rarity.Unique, 5, 12, 20, 5, "Summons a 3/3 Spider token at the start of each round"),
            new BossCard(new Guid("00000000-0000-0000-0006-000000000007"), "Asmodeus", Rarity.Unique, 6, 13, 22, 6, "Allies that deal damage to Asmodeus lose 1 HP at end of round"),
            new BossCard(new Guid("00000000-0000-0000-0006-000000000008"), "Orcus", Rarity.Unique, 6, 14, 24, 3, "Revives one eliminated monster from the discard pile each round"),
            new BossCard(new Guid("00000000-0000-0000-0006-000000000009"), "Zariel the Fallen", Rarity.Unique, 5, 11, 19, 5, "Gains +1 Strength for each ally eliminated this combat"),
            new BossCard(new Guid("00000000-0000-0000-0006-000000000010"), "Halaster Blackcloak", Rarity.Unique, 5, 9, 17, 6, "At the start of each round, randomly swaps two allies' combat assignments"),
        };
    }
}
