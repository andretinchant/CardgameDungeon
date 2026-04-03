using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.API.Data.Seeds;

public static partial class CardSetSeeder
{
    private static readonly IReadOnlyDictionary<string, string> TrapActivationConditions =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["Sphere of Annihilation Trap"] =
                "Activation condition: triggers only if the attacker has 12 or more cards in exile.",
            ["Demiplane Prison"] =
                "Activation condition: triggers only if the attacker played a consumable this round.",
            ["Prismatic Wall Trap"] =
                "Activation condition: triggers only if the attacker changed target this round.",
            ["Symbol of Death"] =
                "Activation condition: triggers only if the attacker has 10 or more cards in exile.",
            ["Reverse Gravity Chamber"] =
                "Activation condition: triggers only if the attacker has Initiative 3 or lower.",
            ["Crushing Walls Room"] =
                "Activation condition: triggers only if 3 or more allies are currently in play.",
            ["Vorpal Blade Pendulum"] =
                "Activation condition: triggers only if the attacker took damage earlier this room.",
            ["Disintegration Ray"] =
                "Activation condition: triggers only if the attacker has at least 1 equipment attached.",
            ["Banishment Sigil"] =
                "Activation condition: triggers only from room 3 onward.",
            ["Power Word Kill Glyph"] =
                "Activation condition: triggers only if the attacker has 11 or more cards in exile.",
            ["Poison Dart Trap"] =
                "Activation condition: triggers only if the attacker has no Shield-slot equipment.",
            ["Spiked Pit"] =
                "Activation condition: triggers only if the attacker has Initiative 4 or higher.",
            ["Explosive Rune"] =
                "Activation condition: triggers only if a Scroll was played this round.",
            ["Flame Jet"] =
                "Activation condition: triggers only if any fire source is active in the room.",
            ["Collapsing Ceiling"] =
                "Activation condition: triggers only if the attacker controls 3 or more allies.",
            ["Swinging Blade"] =
                "Activation condition: triggers only if the attacker used retarget this round.",
            ["Lightning Bolt Trap"] =
                "Activation condition: triggers only if there are at least 2 allies in play.",
            ["Acid Spray Trap"] =
                "Activation condition: triggers only if the target has at least 1 equipment attached.",
            ["Freezing Glyph"] =
                "Activation condition: triggers only if the attacker has Initiative 3 or higher.",
            ["Sleep Gas Vent"] =
                "Activation condition: triggers only if the attacker has no active Potion or Balm.",
            ["Web Trap"] =
                "Activation condition: triggers only if the target is not Ambusher.",
            ["Alarm Rune"] =
                "Activation condition: triggers only if the attacker attempted to ambush this room.",
            ["Teleportation Circle"] =
                "Activation condition: triggers only if 2 or more allies are currently in play.",
            ["Illusion Wall"] =
                "Activation condition: triggers only if no trap was revealed earlier this room.",
            ["Gravity Well"] =
                "Activation condition: triggers only if 3 or more total creatures are in combat.",
            ["Sonic Shriek Trap"] =
                "Activation condition: triggers only if the target has Initiative 4 or higher.",
            ["Blindness Trap"] =
                "Activation condition: triggers only if the target has ranged weapon equipment.",
            ["Fear Totem"] =
                "Activation condition: triggers only if the target has current Strength 4 or higher.",
            ["Necrotic Drain"] =
                "Activation condition: triggers only if an Undead or Fiend monster is in play.",
            ["Petrification Beam"] =
                "Activation condition: triggers only if the target has current Strength 5 or higher.",
            ["Tripwire"] =
                "Activation condition: triggers only if the attacker has Initiative 3 or higher.",
            ["Caltrops Field"] =
                "Activation condition: triggers only if the attacker changed lane or target this round.",
            ["Hidden Pit"] =
                "Activation condition: triggers only if no light source is active in the room.",
            ["Rolling Boulder"] =
                "Activation condition: triggers only if 2 or more allies share the same lane.",
            ["Net Trap"] =
                "Activation condition: triggers only if the target is Ambusher.",
            ["Bear Trap"] =
                "Activation condition: triggers only against the first attacking ally of the round.",
            ["Loose Flagstone"] =
                "Activation condition: triggers only if the attacker has cost 2 or lower.",
            ["Falling Rubble"] =
                "Activation condition: triggers only from room 2 onward.",
            ["Rusty Spikes"] =
                "Activation condition: triggers only if the target has metal equipment attached.",
            ["Oil Slick"] =
                "Activation condition: triggers only if any fire effect was used this room.",
            ["Noisemaker"] =
                "Activation condition: triggers only if the attacker declared an ambush.",
            ["Snare Loop"] =
                "Activation condition: triggers only if the attacker has Initiative 4 or higher.",
            ["Pressure Plate"] =
                "Activation condition: triggers only if the attacker has 2 or more cards in hand.",
            ["Sliding Floor"] =
                "Activation condition: triggers only if 3 or more allies are in play.",
            ["Tipping Bridge"] =
                "Activation condition: triggers only if the attacker has current Strength 5 or higher.",
            ["Sand Trap"] =
                "Activation condition: triggers only if the attacker has no Boots-slot equipment.",
            ["Smoke Vent"] =
                "Activation condition: triggers only if no torch or lantern effect is active.",
            ["Grease Trap"] =
                "Activation condition: triggers only if the target has Weapon-slot equipment.",
            ["Mirror Trap"] =
                "Activation condition: triggers only if a Scroll or spell effect was used this round.",
            ["Water Trap"] =
                "Activation condition: triggers only if a fire source is active in the room.",
            ["Sticky Floor"] =
                "Activation condition: triggers only if the attacker has cost 2 or lower.",
            ["Rotating Wall"] =
                "Activation condition: triggers only if 3 or more allies are in play.",
            ["False Door"] =
                "Activation condition: triggers only when the attacker attempts to loot this room.",
            ["Trapped Chest"] =
                "Activation condition: triggers only when a Treasure gain is about to resolve.",
            ["Trapped Lock"] =
                "Activation condition: triggers only if the target has Accessory-slot equipment.",
            ["Needle Trap"] =
                "Activation condition: triggers only if the attacker has no Balm equipped.",
            ["Spring Blade"] =
                "Activation condition: triggers only if the attacker has Initiative 4 or higher.",
            ["Falling Net"] =
                "Activation condition: triggers only if the target has current Strength 4 or higher.",
            ["Swinging Log"] =
                "Activation condition: triggers only against a front-line attacker.",
            ["Crossbow Trap"] =
                "Activation condition: triggers only if the target has no shield equipped.",
        };

    private static string BuildRestrictedTrapEffect(string trapName, string baseEffect)
    {
        if (!TrapActivationConditions.TryGetValue(trapName, out var condition))
            return baseEffect;

        return $"{condition} {baseEffect}";
    }

    private static List<TrapCard> CreateTraps()
    {
        var traps = new List<TrapCard>
        {
            // ── UNIQUE (2) ──────────────────────────────────────────────────
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000001"),
                "Sphere of Annihilation Trap",
                Rarity.Unique,
                cost: 6,
                damage: 8,
                "Instantly destroys the first creature that fails a Dexterity save, removing it from the encounter entirely"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000002"),
                "Demiplane Prison",
                Rarity.Unique,
                cost: 5,
                damage: 6,
                "Banishes a target to a sealed demiplane for 2 rounds, preventing them from acting or being targeted"),

            // ── RARE (8) ────────────────────────────────────────────────────
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000003"),
                "Prismatic Wall Trap",
                Rarity.Rare,
                cost: 5,
                damage: 6,
                "Erects a prismatic barrier that deals a random elemental damage type each round to anyone who crosses it"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000004"),
                "Symbol of Death",
                Rarity.Rare,
                cost: 5,
                damage: 6,
                "When triggered, forces all creatures in the room to make a Constitution save or drop to half health"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000005"),
                "Reverse Gravity Chamber",
                Rarity.Rare,
                cost: 4,
                damage: 5,
                "Reverses gravity in the room, causing all grounded creatures to take falling damage and lose their next action"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000006"),
                "Crushing Walls Room",
                Rarity.Rare,
                cost: 4,
                damage: 5,
                "Walls close in over 3 rounds, dealing increasing damage each round unless the party finds the hidden switch"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000007"),
                "Vorpal Blade Pendulum",
                Rarity.Rare,
                cost: 4,
                damage: 6,
                "A massive swinging blade that has a chance to instantly slay a target on a critical failure of their dodge roll"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000008"),
                "Disintegration Ray",
                Rarity.Rare,
                cost: 5,
                damage: 5,
                "Fires a beam that reduces the target's maximum health by the damage dealt for the rest of the dungeon"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000009"),
                "Banishment Sigil",
                Rarity.Rare,
                cost: 3,
                damage: 4,
                "Teleports the triggering creature back to the dungeon entrance, forcing them to traverse previous rooms again"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000010"),
                "Power Word Kill Glyph",
                Rarity.Rare,
                cost: 5,
                damage: 6,
                "If the target has fewer than 10 remaining health points when triggered, they are instantly eliminated from combat"),

            // ── UNCOMMON (20) ───────────────────────────────────────────────
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000011"),
                "Poison Dart Trap",
                Rarity.Uncommon,
                cost: 2,
                damage: 3,
                "Launches a volley of poisoned darts that inflict ongoing poison damage of 1 per round for 3 rounds"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000012"),
                "Spiked Pit",
                Rarity.Uncommon,
                cost: 2,
                damage: 4,
                "A concealed pit with iron spikes at the bottom; trapped creatures must spend an action to climb out"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000013"),
                "Explosive Rune",
                Rarity.Uncommon,
                cost: 3,
                damage: 4,
                "Detonates in a burst of arcane fire, dealing damage to all creatures within melee range of the trigger point"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000014"),
                "Flame Jet",
                Rarity.Uncommon,
                cost: 2,
                damage: 3,
                "Shoots a gout of flame from a concealed nozzle, igniting the target and dealing 1 extra fire damage per round"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000015"),
                "Collapsing Ceiling",
                Rarity.Uncommon,
                cost: 3,
                damage: 4,
                "Brings down a section of the ceiling, blocking the path behind the party and preventing retreat"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000016"),
                "Swinging Blade",
                Rarity.Uncommon,
                cost: 2,
                damage: 3,
                "A concealed blade swings across the corridor, reducing the target's armor value by 1 for the encounter"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000017"),
                "Lightning Bolt Trap",
                Rarity.Uncommon,
                cost: 3,
                damage: 3,
                "Releases a bolt of lightning that arcs to a second nearby creature, dealing half damage to them as well"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000018"),
                "Acid Spray Trap",
                Rarity.Uncommon,
                cost: 2,
                damage: 2,
                "Sprays corrosive acid that destroys one random equipment card carried by the affected creature"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000019"),
                "Freezing Glyph",
                Rarity.Uncommon,
                cost: 2,
                damage: 2,
                "Encases the target's legs in ice, reducing their Initiative by 3 for the remainder of the room"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000020"),
                "Sleep Gas Vent",
                Rarity.Uncommon,
                cost: 3,
                damage: 2,
                "Releases a cloud of soporific gas that puts the target to sleep for 1 round unless they pass a Constitution save"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000021"),
                "Web Trap",
                Rarity.Uncommon,
                cost: 2,
                damage: 2,
                "Covers the target in magical spider silk, immobilizing them for 1 round and granting attackers advantage"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000022"),
                "Alarm Rune",
                Rarity.Uncommon,
                cost: 2,
                damage: 2,
                "Emits a piercing shriek that alerts all enemies in adjacent rooms, adding reinforcements to the next encounter"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000023"),
                "Teleportation Circle",
                Rarity.Uncommon,
                cost: 3,
                damage: 2,
                "Randomly teleports the triggering creature to another room in the dungeon, separating them from the party"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000024"),
                "Illusion Wall",
                Rarity.Uncommon,
                cost: 2,
                damage: 2,
                "Conceals a hazard behind a false wall; the next attack against the deceived creature has advantage"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000025"),
                "Gravity Well",
                Rarity.Uncommon,
                cost: 3,
                damage: 3,
                "Creates a localized gravity vortex that pulls all creatures toward the center, grouping them for area attacks"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000026"),
                "Sonic Shriek Trap",
                Rarity.Uncommon,
                cost: 2,
                damage: 3,
                "Unleashes a deafening sonic blast that prevents the target from casting spells on their next turn"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000027"),
                "Blindness Trap",
                Rarity.Uncommon,
                cost: 3,
                damage: 2,
                "A flash of searing light blinds the target, causing their next attack to have disadvantage"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000028"),
                "Fear Totem",
                Rarity.Uncommon,
                cost: 2,
                damage: 2,
                "Projects an aura of supernatural dread, forcing the target to flee from combat for 1 round"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000029"),
                "Necrotic Drain",
                Rarity.Uncommon,
                cost: 3,
                damage: 4,
                "Siphons life force from the target and heals the dungeon boss for an equal amount of health"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000030"),
                "Petrification Beam",
                Rarity.Uncommon,
                cost: 3,
                damage: 3,
                "Partially petrifies the target, halving their movement and reducing all damage they deal by 1 for 2 rounds"),

            // ── COMMON (30) ────────────────────────────────────────────────
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000031"),
                "Tripwire",
                Rarity.Common,
                cost: 1,
                damage: 1,
                "Causes the target to fall prone, losing their next movement action"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000032"),
                "Caltrops Field",
                Rarity.Common,
                cost: 1,
                damage: 1,
                "Scatters sharp caltrops that reduce the movement speed of all creatures passing through the area"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000033"),
                "Hidden Pit",
                Rarity.Common,
                cost: 1,
                damage: 2,
                "A shallow concealed pit that causes the target to stumble and lose their bonus action this turn"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000034"),
                "Rolling Boulder",
                Rarity.Common,
                cost: 2,
                damage: 2,
                "Releases a heavy stone that rolls down the corridor, pushing all creatures in its path back one space"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000035"),
                "Net Trap",
                Rarity.Common,
                cost: 1,
                damage: 1,
                "Drops a heavy net on the target, restraining them until they spend an action cutting free"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000036"),
                "Bear Trap",
                Rarity.Common,
                cost: 1,
                damage: 2,
                "Iron jaws clamp onto the target's leg, immobilizing them and dealing 1 bleed damage at the start of their turn"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000037"),
                "Loose Flagstone",
                Rarity.Common,
                cost: 1,
                damage: 1,
                "A wobbling stone underfoot throws the target off balance, giving the next attacker advantage against them"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000038"),
                "Falling Rubble",
                Rarity.Common,
                cost: 2,
                damage: 2,
                "Dislodged stones rain down from the ceiling, creating difficult terrain in the affected area"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000039"),
                "Rusty Spikes",
                Rarity.Common,
                cost: 1,
                damage: 2,
                "Corroded iron spikes spring from the floor, inflicting a wound that festers and deals 1 damage next round"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000040"),
                "Oil Slick",
                Rarity.Common,
                cost: 1,
                damage: 1,
                "Coats the floor in flammable oil; any fire damage in the area is doubled for the rest of the encounter"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000041"),
                "Noisemaker",
                Rarity.Common,
                cost: 1,
                damage: 1,
                "Triggers a loud alarm that prevents the party from gaining surprise in the next encounter"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000042"),
                "Snare Loop",
                Rarity.Common,
                cost: 1,
                damage: 1,
                "A hidden rope snare yanks the target upside-down, causing them to drop one held item"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000043"),
                "Pressure Plate",
                Rarity.Common,
                cost: 1,
                damage: 1,
                "Activates a connected mechanism that seals the room's exits for 1 round"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000044"),
                "Sliding Floor",
                Rarity.Common,
                cost: 2,
                damage: 1,
                "The floor shifts sideways, forcing all creatures to reposition and disrupting any formations"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000045"),
                "Tipping Bridge",
                Rarity.Common,
                cost: 2,
                damage: 2,
                "A narrow bridge tilts when weight is placed on it, threatening to dump the target into the chasm below"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000046"),
                "Sand Trap",
                Rarity.Common,
                cost: 1,
                damage: 1,
                "Quicksand pulls the target down, reducing their speed to zero until an ally uses an action to pull them free"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000047"),
                "Smoke Vent",
                Rarity.Common,
                cost: 1,
                damage: 1,
                "Fills the room with thick smoke, imposing disadvantage on all ranged attacks for 2 rounds"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000048"),
                "Grease Trap",
                Rarity.Common,
                cost: 1,
                damage: 1,
                "Coats the target in slippery grease, causing them to drop their weapon at the start of their next turn"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000049"),
                "Mirror Trap",
                Rarity.Common,
                cost: 2,
                damage: 1,
                "Reflects the target's own spell back at them, dealing damage equal to the spell's original cost"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000050"),
                "Water Trap",
                Rarity.Common,
                cost: 1,
                damage: 1,
                "Floods the room with ankle-deep water, extinguishing torches and imposing darkness penalties"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000051"),
                "Sticky Floor",
                Rarity.Common,
                cost: 1,
                damage: 1,
                "Adhesive resin on the floor halves the target's movement for the rest of the room"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000052"),
                "Rotating Wall",
                Rarity.Common,
                cost: 2,
                damage: 1,
                "A section of wall rotates, separating one party member from the group into an adjacent corridor"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000053"),
                "False Door",
                Rarity.Common,
                cost: 1,
                damage: 2,
                "An explosive charge detonates when the fake door handle is turned, catching the opener off guard"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000054"),
                "Trapped Chest",
                Rarity.Common,
                cost: 2,
                damage: 2,
                "A spring-loaded blade inside the chest slashes the opener and scatters the chest's contents across the room"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000055"),
                "Trapped Lock",
                Rarity.Common,
                cost: 1,
                damage: 1,
                "A needle coated in mild toxin jabs the lockpicker, reducing their Dexterity-based checks by 1 this encounter"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000056"),
                "Needle Trap",
                Rarity.Common,
                cost: 1,
                damage: 1,
                "A tiny poisoned needle pricks the target, applying a minor debuff that stacks with repeated triggers"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000057"),
                "Spring Blade",
                Rarity.Common,
                cost: 2,
                damage: 2,
                "A concealed blade springs from the wall at waist height, catching unaware adventurers mid-stride"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000058"),
                "Falling Net",
                Rarity.Common,
                cost: 1,
                damage: 1,
                "A weighted net falls from above, entangling the target and reducing their attack rolls by 1 for 1 round"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000059"),
                "Swinging Log",
                Rarity.Common,
                cost: 2,
                damage: 2,
                "A heavy log swings down from the ceiling, knocking the target backward and stunning them for 1 round"),
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000060"),
                "Crossbow Trap",
                Rarity.Common,
                cost: 1,
                damage: 2,
                "A hidden crossbow fires a bolt at the first creature to cross its line of sight, bypassing armor bonuses"),
        };

        var restrictedTraps = new List<TrapCard>(traps.Count);
        foreach (var trap in traps)
        {
            restrictedTraps.Add(
                new TrapCard(
                    trap.Id,
                    trap.Name,
                    trap.Rarity,
                    trap.Cost,
                    trap.Damage,
                    BuildRestrictedTrapEffect(trap.Name, trap.Effect)));
        }

        return restrictedTraps;
    }
}
