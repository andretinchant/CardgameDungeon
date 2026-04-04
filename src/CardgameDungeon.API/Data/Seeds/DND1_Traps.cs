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
                "Activation condition: triggers only if the target has current Attack 4 or higher.",
            ["Necrotic Drain"] =
                "Activation condition: triggers only if an Undead or Fiend monster is in play.",
            ["Petrification Beam"] =
                "Activation condition: triggers only if the target has current Attack 5 or higher.",
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
                "Activation condition: triggers only if the attacker has current Attack 5 or higher.",
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
                "Activation condition: triggers only if the target has current Attack 4 or higher.",
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

    // Trap budget: Damage + EffectPts = Budget
    // Cost 1→4, 2→6, 3→8, 4→10, 5→12, 6→14, Unique 5→15, Unique 6→17
    // Damage costs 1pt per point. Remaining points go to effect.
    private static List<TrapCard> CreateTraps()
    {
        var traps = new List<TrapCard>
        {
            // ═══════════════════════════════════════════════════════════════
            //  UNIQUE (2)
            // ═══════════════════════════════════════════════════════════════

            // Budget 17: Damage2 + Effect11 + single-use downside = 17 (Cost 6, Unique)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000001"),
                "Sphere of Annihilation Trap",
                Rarity.Unique,
                cost: 6,
                damage: 2,
                "Exiles the target ally permanently. Equipped gear is exiled with them. This trap is exiled after use (single use)",
                effectTags: "ON_ACTIVATE|EXILE_TARGET|EXILE_EQUIPMENT|EXILE_SELF"),

            // Budget 15: Damage2 + Effect13 = 15 (Cost 5, Unique)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000002"),
                "Demiplane Prison",
                Rarity.Unique,
                cost: 5,
                damage: 2,
                "Target ally is removed from combat for 2 rounds and cannot be targeted or deal damage. Returns with -2 ATK permanently",
                effectTags: "ON_ACTIVATE|REMOVE_FROM_COMBAT:2:ENEMY|-ATK:2:ENEMY"),

            // ═══════════════════════════════════════════════════════════════
            //  RARE (8) — Cost 3→8, Cost 4→10, Cost 5→12
            // ═══════════════════════════════════════════════════════════════

            // Budget 12: Damage1 + Effect11 = 12 (Cost 5)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000003"),
                "Prismatic Wall Trap",
                Rarity.Rare,
                cost: 5,
                damage: 1,
                "Deals 1 damage to each attacking ally at the start of combat for the remainder of this room",
                effectTags: "ON_COMBAT_START|DAMAGE:1:ALL_ENEMIES"),

            // Budget 12: Damage1 + Effect11 = 12 (Cost 5)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000004"),
                "Symbol of Death",
                Rarity.Rare,
                cost: 5,
                damage: 1,
                "All attacking allies with current HP equal to or below 5 are reduced to 1 HP",
                effectTags: "ON_ACTIVATE|IF_HP_BELOW:5|SET_HP:1:ALL_ENEMIES"),

            // Budget 10: Damage1 + Effect9 = 10 (Cost 4)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000005"),
                "Reverse Gravity Chamber",
                Rarity.Rare,
                cost: 4,
                damage: 1,
                "All attacking allies lose their INIT bonus this round. Allies with Boots equipment are immune",
                effectTags: "ON_ACTIVATE|SET_INIT:0:ALL_ENEMIES|IF_HAS_BOOTS|IMMUNE"),

            // Budget 10: Damage1 + Effect9 = 10 (Cost 4)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000006"),
                "Crushing Walls Room",
                Rarity.Rare,
                cost: 4,
                damage: 1,
                "Each round this trap remains active, deal 1 additional damage to all allies. Discard 3 cards from your deck to disable it",
                effectTags: "ON_ROUND_START|DAMAGE:1:ALL_ENEMIES\nCOST:DISCARD_HAND:3|DISABLE_TRAP"),

            // Budget 10: Damage1 + Effect9 = 10 (Cost 4)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000007"),
                "Vorpal Blade Pendulum",
                Rarity.Rare,
                cost: 4,
                damage: 1,
                "Target the ally with the lowest HP. If defeated, that ally is exiled instead of discarded",
                effectTags: "ON_ACTIVATE|TARGET_LOWEST_HP|ON_KILL|EXILE_TARGET"),

            // Budget 12: Damage1 + Effect11 = 12 (Cost 5)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000008"),
                "Disintegration Ray",
                Rarity.Rare,
                cost: 5,
                damage: 1,
                "Target ally permanently loses 1 HP for the rest of the dungeon. Equipment cannot prevent this",
                effectTags: "ON_ACTIVATE|-HP:1:ENEMY|UNPREVENTABLE"),

            // Budget 8: Damage1 + Effect7 = 8 (Cost 3)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000009"),
                "Banishment Sigil",
                Rarity.Rare,
                cost: 3,
                damage: 1,
                "Target ally is returned to the owner's hand. The owner must pay its cost again to replay it",
                effectTags: "ON_ACTIVATE|RETURN_TO_HAND:ENEMY"),

            // Budget 12: Damage1 + Effect11 = 12 (Cost 5)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000010"),
                "Power Word Kill Glyph",
                Rarity.Rare,
                cost: 5,
                damage: 1,
                "If the target ally has 3 or fewer HP remaining after damage, it is immediately exiled",
                effectTags: "ON_ACTIVATE|IF_HP_BELOW:3|EXILE_TARGET"),

            // ═══════════════════════════════════════════════════════════════
            //  UNCOMMON (20) — Cost 2→6, Cost 3→8
            // ═══════════════════════════════════════════════════════════════

            // Budget 6: Damage1 + Effect5 = 6 (Cost 2)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000011"),
                "Poison Dart Trap",
                Rarity.Uncommon,
                cost: 2,
                damage: 1,
                "Deals damage and reduces target ally's ATK by 1 for the next 2 rounds",
                effectTags: "ON_ACTIVATE|-ATK:1:ENEMY"),

            // Budget 6: Damage1 + Effect5 = 6 (Cost 2)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000012"),
                "Spiked Pit",
                Rarity.Uncommon,
                cost: 2,
                damage: 1,
                "Target ally cannot participate in combat next round while climbing out",
                effectTags: "ON_ACTIVATE|STUN:ENEMY"),

            // Budget 8: Damage1 + Effect7 = 8 (Cost 3)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000013"),
                "Explosive Rune",
                Rarity.Uncommon,
                cost: 3,
                damage: 1,
                "Deals damage to the target and 1 damage to all other attacking allies",
                effectTags: "ON_ACTIVATE|DAMAGE:1:ALL_ENEMIES"),

            // Budget 6: Damage1 + Effect5 = 6 (Cost 2)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000014"),
                "Flame Jet",
                Rarity.Uncommon,
                cost: 2,
                damage: 1,
                "Deals damage and 1 additional damage at the start of the next round",
                effectTags: "ON_ACTIVATE|DAMAGE:1:ENEMY"),

            // Budget 8: Damage1 + Effect7 = 8 (Cost 3)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000015"),
                "Collapsing Ceiling",
                Rarity.Uncommon,
                cost: 3,
                damage: 1,
                "Deals damage to the target. The attacker cannot use opportunity attacks this room",
                effectTags: "ON_ACTIVATE|DISABLE_OPP_ATTACK:OPPONENT"),

            // Budget 6: Damage1 + Effect5 = 6 (Cost 2)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000016"),
                "Swinging Blade",
                Rarity.Uncommon,
                cost: 2,
                damage: 1,
                "Deals damage and destroys one Armor-slot equipment on the target ally",
                effectTags: "ON_ACTIVATE|DESTROY_ARMOR:ENEMY"),

            // Budget 8: Damage1 + Effect7 = 8 (Cost 3)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000017"),
                "Lightning Bolt Trap",
                Rarity.Uncommon,
                cost: 3,
                damage: 1,
                "Deals damage to the target ally and 1 damage to one adjacent ally in play",
                effectTags: "ON_ACTIVATE|DAMAGE:1:ENEMY"),

            // Budget 6: Damage1 + Effect5 = 6 (Cost 2)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000018"),
                "Acid Spray Trap",
                Rarity.Uncommon,
                cost: 2,
                damage: 1,
                "Deals damage and destroys one random equipment card attached to the target ally",
                effectTags: "ON_ACTIVATE|DESTROY_EQUIPMENT:ENEMY"),

            // Budget 6: Damage1 + Effect5 = 6 (Cost 2)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000019"),
                "Freezing Glyph",
                Rarity.Uncommon,
                cost: 2,
                damage: 1,
                "Deals damage and reduces target ally's INIT by 3 for the remainder of this room",
                effectTags: "ON_ACTIVATE|-INIT:3:ENEMY"),

            // Budget 8: Damage1 + Effect7 = 8 (Cost 3)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000020"),
                "Sleep Gas Vent",
                Rarity.Uncommon,
                cost: 3,
                damage: 1,
                "Target ally cannot attack or use consumables next round. A Potion or Balm can wake them early",
                effectTags: "ON_ACTIVATE|STUN:ENEMY|DISABLE_CONSUMABLE:ENEMY"),

            // Budget 6: Damage1 + Effect5 = 6 (Cost 2)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000021"),
                "Web Trap",
                Rarity.Uncommon,
                cost: 2,
                damage: 1,
                "Target ally cannot retarget or use opportunity attacks this round",
                effectTags: "ON_ACTIVATE|DISABLE_RETARGET:ENEMY|DISABLE_OPP_ATTACK:ENEMY"),

            // Budget 6: Damage2 + Effect4 = 6 (Cost 2)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000022"),
                "Alarm Rune",
                Rarity.Uncommon,
                cost: 2,
                damage: 2,
                "Deals damage and the defender draws 2 additional cards from their enemy deck for this room",
                effectTags: "ON_ACTIVATE|DEFENDER_DRAW:2"),

            // Budget 8: Damage3 + Effect5 = 8 (Cost 3)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000023"),
                "Teleportation Circle",
                Rarity.Uncommon,
                cost: 3,
                damage: 3,
                "Target ally is returned to the owner's hand without refunding its cost",
                effectTags: "ON_ACTIVATE|RETURN_TO_HAND:ENEMY"),

            // Budget 6: Damage2 + Effect4 = 6 (Cost 2)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000024"),
                "Illusion Wall",
                Rarity.Uncommon,
                cost: 2,
                damage: 2,
                "The next attack against the target ally deals +2 bonus damage",
                effectTags: "ON_ACTIVATE|VULNERABILITY:2:ENEMY"),

            // Budget 8: Damage4 + Effect4 = 8 (Cost 3)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000025"),
                "Gravity Well",
                Rarity.Uncommon,
                cost: 3,
                damage: 4,
                "All attacking allies are forced into a single combat group, sharing damage from one defender",
                effectTags: "ON_ACTIVATE|FORCE_GROUP:ALL_ENEMIES"),

            // Budget 6: Damage3 + Effect3 = 6 (Cost 2)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000026"),
                "Sonic Shriek Trap",
                Rarity.Uncommon,
                cost: 2,
                damage: 3,
                "Deals damage and prevents the target ally from using Scroll consumables this round",
                effectTags: "ON_ACTIVATE|DISABLE_SCROLL:ENEMY"),

            // Budget 8: Damage3 + Effect5 = 8 (Cost 3)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000027"),
                "Blindness Trap",
                Rarity.Uncommon,
                cost: 3,
                damage: 3,
                "Target ally's ATK is halved (rounded down) for their next attack",
                effectTags: "ON_ACTIVATE|HALVE_STR:ENEMY"),

            // Budget 6: Damage2 + Effect4 = 6 (Cost 2)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000028"),
                "Fear Totem",
                Rarity.Uncommon,
                cost: 2,
                damage: 2,
                "Target ally is forced to abandon combat. The attacker may use an opportunity attack",
                effectTags: "ON_ACTIVATE|FORCE_ABANDON:ENEMY"),

            // Budget 8: Damage5 + Effect3 = 8 (Cost 3)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000029"),
                "Necrotic Drain",
                Rarity.Uncommon,
                cost: 3,
                damage: 5,
                "Deals damage to the target ally and restores HP equal to half damage dealt to one defending monster",
                effectTags: "ON_ACTIVATE|HEAL:2:ALLY"),

            // Budget 8: Damage4 + Effect4 = 8 (Cost 3)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000030"),
                "Petrification Beam",
                Rarity.Uncommon,
                cost: 3,
                damage: 4,
                "Target ally's ATK is reduced by 1 permanently for the rest of the dungeon",
                effectTags: "ON_ACTIVATE|-ATK:1:ENEMY"),

            // ═══════════════════════════════════════════════════════════════
            //  COMMON (30) — Cost 1→4, Cost 2→6
            // ═══════════════════════════════════════════════════════════════

            // Budget 4: Damage1 + Effect3 = 4 (Cost 1)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000031"),
                "Tripwire",
                Rarity.Common,
                cost: 1,
                damage: 1,
                "Target ally loses all INIT bonuses this round",
                effectTags: "ON_ACTIVATE|SET_INIT:0:ENEMY"),

            // Budget 4: Damage1 + Effect3 = 4 (Cost 1)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000032"),
                "Caltrops Field",
                Rarity.Common,
                cost: 1,
                damage: 1,
                "All attacking allies lose 1 INIT this round",
                effectTags: "ON_ACTIVATE|-INIT:1:ALL_ENEMIES"),

            // Budget 4: Damage2 + Effect2 = 4 (Cost 1)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000033"),
                "Hidden Pit",
                Rarity.Common,
                cost: 1,
                damage: 2,
                "Target ally cannot use retarget this round",
                effectTags: "ON_ACTIVATE|DISABLE_RETARGET:ENEMY"),

            // Budget 6: Damage3 + Effect3 = 6 (Cost 2)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000034"),
                "Rolling Boulder",
                Rarity.Common,
                cost: 2,
                damage: 3,
                "Deals damage to target ally and 1 damage to one other attacking ally",
                effectTags: "ON_ACTIVATE|DAMAGE:1:ENEMY"),

            // Budget 4: Damage1 + Effect3 = 4 (Cost 1)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000035"),
                "Net Trap",
                Rarity.Common,
                cost: 1,
                damage: 1,
                "Target ally cannot attack this round. Owner must discard 1 card to free them next round",
                effectTags: "ON_ACTIVATE|STUN:ENEMY|COST:DISCARD_HAND:1"),

            // Budget 4: Damage2 + Effect2 = 4 (Cost 1)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000036"),
                "Bear Trap",
                Rarity.Common,
                cost: 1,
                damage: 2,
                "Deals damage and 1 additional damage at the start of the next round",
                effectTags: "ON_ACTIVATE|DAMAGE:1:ENEMY"),

            // Budget 4: Damage1 + Effect3 = 4 (Cost 1)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000037"),
                "Loose Flagstone",
                Rarity.Common,
                cost: 1,
                damage: 1,
                "Target ally receives +2 damage from the next attack against it this round",
                effectTags: "ON_ACTIVATE|VULNERABILITY:2:ENEMY"),

            // Budget 6: Damage3 + Effect3 = 6 (Cost 2)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000038"),
                "Falling Rubble",
                Rarity.Common,
                cost: 2,
                damage: 3,
                "Deals damage split among all attacking allies (rounded up per target)",
                effectTags: "ON_ACTIVATE|DAMAGE:3:ALL_ENEMIES"),

            // Budget 4: Damage2 + Effect2 = 4 (Cost 1)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000039"),
                "Rusty Spikes",
                Rarity.Common,
                cost: 1,
                damage: 2,
                "Deals damage. If target has Armor equipment, deal 1 extra damage",
                effectTags: "ON_ACTIVATE|IF_HAS_ARMOR|DAMAGE:1:ENEMY"),

            // Budget 4: Damage1 + Effect3 = 4 (Cost 1)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000040"),
                "Oil Slick",
                Rarity.Common,
                cost: 1,
                damage: 1,
                "All Bomb consumables deal double damage this room",
                effectTags: "ON_ACTIVATE|DOUBLE_BOMB_DAMAGE"),

            // Budget 4: Damage1 + Effect3 = 4 (Cost 1)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000041"),
                "Noisemaker",
                Rarity.Common,
                cost: 1,
                damage: 1,
                "Defender draws 1 additional card from their enemy deck this room",
                effectTags: "ON_ACTIVATE|DEFENDER_DRAW:1"),

            // Budget 4: Damage1 + Effect3 = 4 (Cost 1)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000042"),
                "Snare Loop",
                Rarity.Common,
                cost: 1,
                damage: 1,
                "Target ally's equipped Weapon is unequipped and returned to owner's hand",
                effectTags: "ON_ACTIVATE|UNEQUIP_WEAPON:ENEMY"),

            // Budget 4: Damage1 + Effect3 = 4 (Cost 1)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000043"),
                "Pressure Plate",
                Rarity.Common,
                cost: 1,
                damage: 1,
                "No allies can retreat from combat this round. Opportunity attacks cannot be used",
                effectTags: "ON_ACTIVATE|DISABLE_RETREAT:ALL_ENEMIES|DISABLE_OPP_ATTACK:ALL_ENEMIES"),

            // Budget 6: Damage2 + Effect4 = 6 (Cost 2)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000044"),
                "Sliding Floor",
                Rarity.Common,
                cost: 2,
                damage: 2,
                "All combat assignments are cleared. Attacker must reassign targets",
                effectTags: "ON_ACTIVATE|CLEAR_ASSIGNMENTS"),

            // Budget 6: Damage3 + Effect3 = 6 (Cost 2)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000045"),
                "Tipping Bridge",
                Rarity.Common,
                cost: 2,
                damage: 3,
                "Target ally must exile 1 card from the top of owner's deck or take 2 additional damage",
                effectTags: "ON_ACTIVATE|COST:EXILE_DECK:1|DAMAGE:2:ENEMY"),

            // Budget 4: Damage1 + Effect3 = 4 (Cost 1)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000046"),
                "Sand Trap",
                Rarity.Common,
                cost: 1,
                damage: 1,
                "Target ally's INIT is reduced to 0 this round",
                effectTags: "ON_ACTIVATE|SET_INIT:0:ENEMY"),

            // Budget 4: Damage1 + Effect3 = 4 (Cost 1)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000047"),
                "Smoke Vent",
                Rarity.Common,
                cost: 1,
                damage: 1,
                "All allies with INIT below 3 lose 1 ATK this round",
                effectTags: "ON_ACTIVATE|IF_INIT_BELOW:3|-ATK:1:ALL_ENEMIES"),

            // Budget 4: Damage1 + Effect3 = 4 (Cost 1)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000048"),
                "Grease Trap",
                Rarity.Common,
                cost: 1,
                damage: 1,
                "Target ally's Weapon equipment provides no ATK bonus this round",
                effectTags: "ON_ACTIVATE|DISABLE_WEAPON_STR:ENEMY"),

            // Budget 6: Damage2 + Effect4 = 6 (Cost 2)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000049"),
                "Mirror Trap",
                Rarity.Common,
                cost: 2,
                damage: 2,
                "The last consumable played by the attacker has its effect reversed onto an attacking ally",
                effectTags: "ON_ACTIVATE|REFLECT_CONSUMABLE:ENEMY"),

            // Budget 4: Damage1 + Effect3 = 4 (Cost 1)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000050"),
                "Water Trap",
                Rarity.Common,
                cost: 1,
                damage: 1,
                "All Totem consumable effects are cancelled this room",
                effectTags: "ON_ACTIVATE|CANCEL_TOTEM"),

            // Budget 4: Damage1 + Effect3 = 4 (Cost 1)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000051"),
                "Sticky Floor",
                Rarity.Common,
                cost: 1,
                damage: 1,
                "Target ally cannot retarget or change combat assignment this round",
                effectTags: "ON_ACTIVATE|DISABLE_RETARGET:ENEMY"),

            // Budget 6: Damage2 + Effect4 = 6 (Cost 2)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000052"),
                "Rotating Wall",
                Rarity.Common,
                cost: 2,
                damage: 2,
                "One random ally is removed from combat this round and cannot be targeted or deal damage",
                effectTags: "ON_ACTIVATE|REMOVE_FROM_COMBAT:1:ENEMY"),

            // Budget 4: Damage2 + Effect2 = 4 (Cost 1)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000053"),
                "False Door",
                Rarity.Common,
                cost: 1,
                damage: 2,
                "Owner must exile 1 card from the top of their deck",
                effectTags: "ON_ACTIVATE|EXILE_DECK:1"),

            // Budget 6: Damage3 + Effect3 = 6 (Cost 2)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000054"),
                "Trapped Chest",
                Rarity.Common,
                cost: 2,
                damage: 3,
                "Any Treasure gained this room is reduced by half (rounded down)",
                effectTags: "ON_ACTIVATE|HALVE_TREASURE"),

            // Budget 4: Damage1 + Effect3 = 4 (Cost 1)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000055"),
                "Trapped Lock",
                Rarity.Common,
                cost: 1,
                damage: 1,
                "Target ally's INIT is reduced by 2 for the rest of this room",
                effectTags: "ON_ACTIVATE|-INIT:2:ENEMY"),

            // Budget 4: Damage1 + Effect3 = 4 (Cost 1)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000056"),
                "Needle Trap",
                Rarity.Common,
                cost: 1,
                damage: 1,
                "Deals damage. Effect stacks: each additional Needle Trap this room deals +1 extra damage",
                effectTags: "ON_ACTIVATE|STACK_DAMAGE:1"),

            // Budget 6: Damage3 + Effect3 = 6 (Cost 2)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000057"),
                "Spring Blade",
                Rarity.Common,
                cost: 2,
                damage: 3,
                "Deals damage to the target ally and destroys one Accessory equipment if present",
                effectTags: "ON_ACTIVATE|DESTROY_ACCESSORY:ENEMY"),

            // Budget 4: Damage1 + Effect3 = 4 (Cost 1)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000058"),
                "Falling Net",
                Rarity.Common,
                cost: 1,
                damage: 1,
                "Target ally's ATK is reduced by 2 this round",
                effectTags: "ON_ACTIVATE|-ATK:2:ENEMY"),

            // Budget 6: Damage3 + Effect3 = 6 (Cost 2)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000059"),
                "Swinging Log",
                Rarity.Common,
                cost: 2,
                damage: 3,
                "Target ally is knocked out of combat. Cannot attack or be assigned a target this round",
                effectTags: "ON_ACTIVATE|REMOVE_FROM_COMBAT:1:ENEMY"),

            // Budget 4: Damage2 + Effect2 = 4 (Cost 1)
            new TrapCard(
                new Guid("00000000-0000-0000-0004-000000000060"),
                "Crossbow Trap",
                Rarity.Common,
                cost: 1,
                damage: 2,
                "Deals damage ignoring Armor and Shield equipment bonuses",
                effectTags: "ON_ACTIVATE|IGNORE_ARMOR|IGNORE_SHIELD"),
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
                    BuildRestrictedTrapEffect(trap.Name, trap.Effect),
                    trap.EffectTags));
        }

        return restrictedTraps;
    }
}
