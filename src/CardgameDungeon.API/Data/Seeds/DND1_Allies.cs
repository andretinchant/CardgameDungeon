using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.API.Data.Seeds;

public static partial class CardSetSeeder
{
    private static List<AllyCard> CreateAllies()
    {
        return new List<AllyCard>
        {
            // ===== UNIQUE =====

            // Drizzt Do'Urden: RANGER (not Rogue), Drow — Cost 6, Unique Budget = 30
            // STR6(6) + HP6(6) + INIT4(6) + Treasure1(1) = 19. Effect = 8 ✓
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000001"), "Drizzt Do'Urden", Rarity.Unique, 6, 6, 6, 4,
                isAmbusher: false, treasure: 1,
                effect: "The legendary Drow ranger moves like shadow through the dungeon. Reveal opponent's hand and top 3 of their deck. Favored Enemy (choose Race when played): +4 STR against that Race this dungeon. If another Drow or Elf in play: +2 INIT",
                race: Race.Drow, allyClass: AllyClass.Ranger,
                effectTags: "ON_PLAY|REVEAL_HAND|REVEAL_DECK:3\nON_PLAY|FAVORED_ENEMY:CHOOSE\nWITH_ADVANTAGE|IF_RACE:FAVORED|+STR:4\nPASSIVE|IF_CLASS:Ranger|+INIT:2"),

            // Elminster Aumar: Mage, Human — Cost 7, Unique Budget = 34
            // STR5(5) + HP8(8) + INIT3(4.5) + Treasure1(1) = 18.5. Effect = 8 ✓
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000002"), "Elminster Aumar", Rarity.Unique, 7, 5, 8, 3,
                isAmbusher: false, treasure: 1,
                effect: "The Chosen of Mystra channels the Weave itself. Exile 2 cards from hand: search your deck for any Scroll, put it in your hand, and reshuffle. When Elminster uses a Scroll: the Scroll is returned to the bottom of the deck instead of being destroyed. Exile 8 cards from deck: search your exile pile for up to 3 Scrolls and return them to hand",
                race: Race.Human, allyClass: AllyClass.Mage,
                effectTags: "ON_ACTIVATE|COST:EXILE_HAND:2|SEARCH_DECK:SCROLL\nON_SCROLL_USED|SCROLL_TO_BOTTOM\nON_ACTIVATE|COST:EXILE_DECK:8|RECOVER_SCROLL_EXILE:3"),

            // Thordak the Ironwall: Warrior, Dwarf — Cost 6, Unique Budget = 30
            // STR7(7) + HP8(8) + INIT1(1.5) + Treasure1(1) = 17.5. Effect = 8 ✓
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000061"), "Thordak the Ironwall", Rarity.Unique, 6, 7, 8, 1,
                isAmbusher: false, treasure: 1,
                effect: "The mountain that walks. Discard 1 from hand: all damage dealt to allies in this combat group is redirected to Thordak this round. With Disadvantage: +4 HP. Exile 8 cards from deck: force all enemies in the room to target only Thordak until end of room",
                race: Race.Dwarf, allyClass: AllyClass.Warrior,
                effectTags: "ON_ACTIVATE|COST:DISCARD_HAND:1|REDIRECT_DAMAGE:GROUP\nWITH_DISADVANTAGE|+HP:4\nON_ACTIVATE|COST:EXILE_DECK:8|REDIRECT_DAMAGE:ALL_ENEMIES"),

            // Seraphina the Radiant: Cleric, Human — Cost 6, Unique Budget = 30
            // STR4(4) + HP9(9) + INIT2(3) + Treasure1(1) = 17. Effect = 8 ✓
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000062"), "Seraphina the Radiant", Rarity.Unique, 6, 4, 9, 2,
                isAmbusher: false, treasure: 1,
                effect: "Her light mends what darkness breaks. At start of each round: restore 2 HP to all allies. +3 STR against Undead and Demon. Exile 8 cards from deck: fully restore all allies to max HP and remove all negative effects",
                race: Race.Human, allyClass: AllyClass.Cleric,
                effectTags: "ON_ROUND_START|HEAL:2:ALL_ALLIES\nPASSIVE|IF_RACE:Undead|+STR:3\nPASSIVE|IF_RACE:Demon|+STR:3\nON_ACTIVATE|COST:EXILE_DECK:8|HEAL:99:ALL_ALLIES"),

            // Irvine the Opportunist: Rogue, Halfling — Cost 5, Unique Budget = 26
            // STR5(5) + HP4(4) + INIT3(4.5) + Ambusher(2) + Treasure1(1) = 16.5. Effect = 8 ✓
            // The ONLY Unique at cost 5 — can enter the field earlier than other Uniques
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000063"), "Irvine the Opportunist", Rarity.Unique, 5, 5, 4, 3,
                isAmbusher: true, treasure: 1,
                effect: "Every opening is a death sentence. This ally's opportunity attacks deal double STR damage. When any enemy flees or changes combat group: Irvine automatically performs an opportunity attack against them (does not count toward the 1-per-round limit). With Advantage: +3 STR",
                race: Race.Halfling, allyClass: AllyClass.Rogue,
                effectTags: "PASSIVE|OPP_ATTACK_DOUBLE\nON_ENEMY_FLEE|IGNORE_OPP_ATTACK_LIMIT\nWITH_ADVANTAGE|+STR:3"),

            // Aldric the Oathbound: Paladin, Human — Cost 6, Unique Budget = 30
            // STR6(6) + HP7(7) + INIT2(3) + Treasure1(1) = 17. Effect = 8 ✓
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000064"), "Aldric the Oathbound", Rarity.Unique, 6, 6, 7, 2,
                isAmbusher: false, treasure: 1,
                effect: "His oath burns brighter than any flame. Sacrifice (exile this ally): eliminate one enemy with HP <= 10 and restore 3 HP to all other allies. +3 STR against Undead. Exile 8 cards from deck: all allies gain +2 STR for the rest of the room",
                race: Race.Human, allyClass: AllyClass.Paladin,
                effectTags: "ON_ACTIVATE|COST:EXILE_HAND:99|DAMAGE:10:ENEMY|HEAL:3:ALL_ALLIES\nPASSIVE|IF_RACE:Undead|+STR:3\nON_ACTIVATE|COST:EXILE_DECK:8|+STR:2"),

            // ===== RARE (8) =====

            // Bruenor Battlehammer: Warrior, Dwarf — Cost 4, Budget = 18
            // Stats: STR 4(4) + HP 5(5) + INIT 2(3) + Treasure 1(1) = 13
            // Effect budget: 18 - 13 = 5 (Rare 5-6 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000003"), "Bruenor Battlehammer", Rarity.Rare, 4, 4, 5, 2,
                isAmbusher: false, treasure: 1,
                effect: "Reduces all incoming damage to allies in this combat group by 1. With Disadvantage: +3 HP. If another Dwarf ally in play: +2 HP.",
                race: Race.Dwarf, allyClass: AllyClass.Warrior,
                effectTags: "PASSIVE|REDUCE_DAMAGE:1\nWITH_DISADVANTAGE|+HP:3\nPASSIVE|IF_RACE:Dwarf|+HP:2"),

            // Wulfgar Son of Beornegar: Paladin, Human — Cost 4, Budget = 18
            // Stats: STR 5(5) + HP 5(5) + INIT 2(3) + Treasure 0(0) = 13
            // Effect budget: 18 - 13 = 5 (Rare 5-6 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000004"), "Wulfgar Son of Beornegar", Rarity.Rare, 4, 5, 5, 2,
                isAmbusher: false, treasure: 0,
                effect: "Sacrifice 3 HP: exile one enemy card from opponent's discard pile. If a Cleric ally is in play: +3 STR.",
                race: Race.Human, allyClass: AllyClass.Paladin,
                effectTags: "ON_ACTIVATE|COST:HP:3|EXILE_HAND:1\nPASSIVE|IF_CLASS:Cleric|+STR:3"),

            // Cattie-brie: Ranger, Human — Cost 3, Budget = 14
            // Stats: STR 3(3) + HP 3(3) + INIT 1(1.5) + Treasure 1(1) = 8.5
            // Effect budget: 14 - 8.5 = 5.5 (Rare 5-6 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000005"), "Cattie-brie", Rarity.Rare, 3, 3, 3, 1,
                isAmbusher: false, treasure: 1,
                effect: "Favored Enemy (Goblin): +3 STR against Goblins. Reveal top 2 cards of opponent's deck at the start of combat. If a Ranger ally is in play: +1 INIT.",
                race: Race.Human, allyClass: AllyClass.Ranger,
                effectTags: "PASSIVE|FAVORED_ENEMY:Goblin|+STR:3\nON_COMBAT_START|REVEAL_DECK:2\nPASSIVE|IF_CLASS:Ranger|+INIT:1"),

            // Regis the Halfling: Rogue, Halfling — Cost 3, Budget = 14
            // Stats: STR 2(2) + HP 3(3) + INIT 1(1.5) + Ambusher(2) + Treasure 1(1) = 9.5
            // Effect budget: 14 - 9.5 = 4.5 -> 5 (Rare 5-6 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000006"), "Regis the Halfling", Rarity.Rare, 3, 2, 3, 1,
                isAmbusher: true, treasure: 1,
                effect: "With Advantage: +3 STR. On kill: exile the defeated enemy and gain +1 treasure. If another Halfling ally in play: +1 INIT.",
                race: Race.Halfling, allyClass: AllyClass.Rogue,
                effectTags: "WITH_ADVANTAGE|+STR:3\nON_KILL|EXILE_HAND:1\nPASSIVE|IF_RACE:Halfling|+INIT:1"),

            // Mordenkainen: Mage, Human — Cost 4, Budget = 18
            // Stats: STR 3(3) + HP 4(4) + INIT 2(3) + Treasure 1(1) = 11
            // Effect budget: 18 - 11 = 7 -> 6 (Rare 5-6 range), push stat up
            // Readjust: STR 4(4) + HP 4(4) + INIT 2(3) + Treasure 1(1) = 12
            // Effect budget: 18 - 12 = 6 (Rare 5-6 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000007"), "Mordenkainen", Rarity.Rare, 4, 4, 4, 2,
                isAmbusher: false, treasure: 1,
                effect: "Necromancer: choose 1 monster with cost <= 3 from any discard pile and place it on top of your deck. All allies in this combat group gain +1 STR",
                race: Race.Human, allyClass: AllyClass.Mage,
                effectTags: "ON_ACTIVATE|SEARCH_DECK:MONSTER\nPASSIVE|+STR:1"),

            // Tasha the Witch Queen: Mage, Human — Cost 4, Budget = 18
            // Stats: STR 3(3) + HP 5(5) + INIT 2(3) + Treasure 1(1) = 12
            // Effect budget: 18 - 12 = 6 (Rare 5-6 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000008"), "Tasha the Witch Queen", Rarity.Rare, 4, 3, 5, 2,
                isAmbusher: false, treasure: 1,
                effect: "Necromancer: choose 1 monster with cost <= 4 from any discard pile and place it on top of your deck. Opponent exiles 1 card from top of their deck",
                race: Race.Human, allyClass: AllyClass.Mage,
                effectTags: "ON_ACTIVATE|SEARCH_DECK:MONSTER\nPASSIVE|EXILE_DECK:1"),

            // Minsc and Boo: Warrior, Human — Cost 4, Budget = 18
            // Stats: STR 5(5) + HP 5(5) + INIT 1(1.5) + Treasure 1(1) = 12.5
            // Effect budget: 18 - 12.5 = 5.5 (Rare 5-6 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000009"), "Minsc and Boo", Rarity.Rare, 4, 5, 5, 1,
                isAmbusher: false, treasure: 1,
                effect: "With Disadvantage: +4 STR. Redirect all damage from one ally to this ally this combat. Traps in this room deal 2 less damage to all allies.",
                race: Race.Human, allyClass: AllyClass.Warrior,
                effectTags: "WITH_DISADVANTAGE|+STR:4\nON_ACTIVATE|REDIRECT_DAMAGE:ALLY\nPASSIVE|REDUCE_DAMAGE:2"),

            // Jalarloth Baenre: Cleric, Elf — Cost 4, Budget = 18
            // Stats: STR 3(3) + HP 5(5) + INIT 2(3) + Treasure 1(1) = 12
            // Effect budget: 18 - 12 = 6 (Rare 5-6 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000010"), "Jalarloth Baenre", Rarity.Rare, 4, 3, 5, 2,
                isAmbusher: false, treasure: 1,
                effect: "Restore 3 HP to all allies at the start of combat. Against Undead enemies: all allies gain +2 STR. If a Potion is used: restore 2 additional HP to all allies.",
                race: Race.Elf, allyClass: AllyClass.Cleric,
                effectTags: "ON_COMBAT_START|HEAL:3:ALL_ALLIES\nPASSIVE|FAVORED_ENEMY:Undead|+STR:2\nPASSIVE|HEAL:2:ALL_ALLIES"),

            // ===== UNCOMMON (20) =====

            // --- Warrior Uncommon (4) ---

            // Dragonborn Fighter: Warrior, Dragonborn — Cost 3, Budget = 14
            // Stats: STR 4(4) + HP 4(4) + INIT 1(1.5) + Treasure 1(1) = 10.5
            // Effect budget: 14 - 10.5 = 3.5 (Uncommon 3-4 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000017"), "Dragonborn Fighter", Rarity.Uncommon, 3, 4, 4, 1,
                isAmbusher: false, treasure: 1,
                effect: "With Disadvantage: +2 STR. If equipped with a Weapon: +1 STR.",
                race: Race.Dragonborn, allyClass: AllyClass.Warrior,
                effectTags: "WITH_DISADVANTAGE|+STR:2\nPASSIVE|IF_EQUIPPED:Weapon|+STR:1"),

            // Half-Orc Barbarian: Warrior, Orc — Cost 3, Budget = 14
            // Stats: STR 4(4) + HP 5(5) + INIT 1(1.5) + Treasure 0(0) = 10.5
            // Effect budget: 14 - 10.5 = 3.5 (Uncommon 3-4 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000018"), "Half-Orc Barbarian", Rarity.Uncommon, 3, 4, 5, 1,
                isAmbusher: false, treasure: 0,
                effect: "With Disadvantage: +3 HP. If below half HP: +2 STR.",
                race: Race.Orc, allyClass: AllyClass.Warrior,
                effectTags: "WITH_DISADVANTAGE|+HP:3\nPASSIVE|+STR:2"),

            // Goliath Berserker: Warrior, Giant — Cost 3, Budget = 14
            // Stats: STR 4(4) + HP 5(5) + INIT 1(1.5) + Treasure 0(0) = 10.5
            // Effect budget: 14 - 10.5 = 3.5 (Uncommon 3-4 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000021"), "Goliath Berserker", Rarity.Uncommon, 3, 4, 5, 1,
                isAmbusher: false, treasure: 0,
                effect: "Reduces damage from one attack per combat by 2. With Disadvantage: +2 STR.",
                race: Race.Giant, allyClass: AllyClass.Warrior,
                effectTags: "PASSIVE|REDUCE_DAMAGE:2|ONCE_PER_COMBAT\nWITH_DISADVANTAGE|+STR:2"),

            // Hobgoblin Warlord: Warrior, Goblin — Cost 3, Budget = 14
            // Stats: STR 3(3) + HP 4(4) + INIT 2(3) + Treasure 0(0) = 10
            // Effect budget: 14 - 10 = 4 (Uncommon 3-4 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000030"), "Hobgoblin Warlord", Rarity.Uncommon, 3, 3, 4, 2,
                isAmbusher: false, treasure: 0,
                effect: "All allies in this combat group gain +1 STR. One ally may Retarget without paying cost.",
                race: Race.Goblin, allyClass: AllyClass.Warrior,
                effectTags: "PASSIVE|+STR:1\nPASSIVE|JOIN_COMBAT"),

            // --- Rogue Uncommon (3) ---

            // Halfling Rogue: Rogue, Halfling — Cost 2, Budget = 10
            // Stats: STR 2(2) + HP 2(2) + INIT 1(1.5) + Ambusher(2) + Treasure 0(0) = 7.5
            // Effect budget: 10 - 7.5 = 2.5 -> 3 (Uncommon 3-4 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000014"), "Halfling Rogue", Rarity.Uncommon, 2, 2, 2, 1,
                isAmbusher: true, treasure: 0,
                effect: "With Advantage: +3 STR. On kill: exile the defeated enemy.",
                race: Race.Halfling, allyClass: AllyClass.Rogue,
                effectTags: "WITH_ADVANTAGE|+STR:3\nON_KILL|EXILE_HAND:1"),

            // Tabaxi Swashbuckler: Rogue, Beast — Cost 3, Budget = 14
            // Stats: STR 3(3) + HP 3(3) + INIT 1(1.5) + Ambusher(2) + Treasure 1(1) = 10.5
            // Effect budget: 14 - 10.5 = 3.5 (Uncommon 3-4 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000022"), "Tabaxi Swashbuckler", Rarity.Uncommon, 3, 3, 3, 1,
                isAmbusher: true, treasure: 1,
                effect: "With Advantage: +2 STR. Opportunity Attacks against this ally deal no damage.",
                race: Race.Beast, allyClass: AllyClass.Rogue,
                effectTags: "WITH_ADVANTAGE|+STR:2\nPASSIVE|REDUCE_DAMAGE:99"),

            // Kenku Shadow: Rogue, Beast — Cost 2, Budget = 10
            // Stats: STR 2(2) + HP 2(2) + INIT 1(1.5) + Ambusher(2) + Treasure 0(0) = 7.5
            // Effect budget: 10 - 7.5 = 2.5 -> 3 (Uncommon 3-4 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000023"), "Kenku Shadow", Rarity.Uncommon, 2, 2, 2, 1,
                isAmbusher: true, treasure: 0,
                effect: "With Advantage: +2 STR. If another Rogue ally in play: +1 STR.",
                race: Race.Beast, allyClass: AllyClass.Rogue,
                effectTags: "WITH_ADVANTAGE|+STR:2\nPASSIVE|IF_CLASS:Rogue|+STR:1"),

            // --- Mage Uncommon (4) ---

            // Gnome Artificer: Mage, Halfling — Cost 3, Budget = 14
            // Stats: STR 3(3) + HP 4(4) + INIT 1(1.5) + Treasure 1(1) = 9.5
            // Effect budget: 14 - 9.5 = 4.5 -> 4 (Uncommon 3-4 range), push stat
            // Readjust: STR 3(3) + HP 4(4) + INIT 2(3) + Treasure 0(0) = 10
            // Effect budget: 14 - 10 = 4 (Uncommon 3-4 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000015"), "Gnome Artificer", Rarity.Uncommon, 3, 3, 4, 2,
                isAmbusher: false, treasure: 0,
                effect: "Equipped Accessories and Totems grant +1 additional STR to the holder. If a Scroll is equipped on any ally: +2 STR.",
                race: Race.Halfling, allyClass: AllyClass.Mage,
                effectTags: "PASSIVE|IF_EQUIPPED:Accessory|+STR:1\nPASSIVE|IF_SCROLL_USED|+STR:2"),

            // Tiefling Warlock: Mage, Tiefling — Cost 3, Budget = 14
            // Stats: STR 4(4) + HP 4(4) + INIT 1(1.5) + Treasure 1(1) = 10.5
            // Effect budget: 14 - 10.5 = 3.5 (Uncommon 3-4 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000016"), "Tiefling Warlock", Rarity.Uncommon, 3, 4, 4, 1,
                isAmbusher: false, treasure: 1,
                effect: "Necromancer: choose 1 monster with cost <= 2 from your discard pile and place it on top of your deck. +1 STR this combat",
                race: Race.Tiefling, allyClass: AllyClass.Mage,
                effectTags: "ON_ACTIVATE|SEARCH_DECK:MONSTER\nPASSIVE|+STR:1"),

            // Eladrin Bladesinger: Mage, Elf — Cost 3, Budget = 14
            // Stats: STR 3(3) + HP 4(4) + INIT 2(3) + Treasure 0(0) = 10
            // Effect budget: 14 - 10 = 4 (Uncommon 3-4 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000026"), "Eladrin Bladesinger", Rarity.Uncommon, 3, 3, 4, 2,
                isAmbusher: false, treasure: 0,
                effect: "If equipped with a Weapon: +2 STR and +1 INIT. If another Elf ally in play: +1 HP.",
                race: Race.Elf, allyClass: AllyClass.Mage,
                effectTags: "PASSIVE|IF_EQUIPPED:Weapon|+STR:2|+INIT:1\nPASSIVE|IF_RACE:Elf|+HP:1"),

            // Genasi Stormcaller: Mage, Elemental — Cost 3, Budget = 14
            // Stats: STR 4(4) + HP 3(3) + INIT 1(1.5) + Treasure 1(1) = 9.5
            // Effect budget: 14 - 9.5 = 4.5 -> 4 (Uncommon 3-4 range)
            // Readjust: STR 4(4) + HP 4(4) + INIT 1(1.5) + Treasure 1(1) = 10.5
            // Effect budget: 14 - 10.5 = 3.5 (Uncommon 3-4 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000027"), "Genasi Stormcaller", Rarity.Uncommon, 3, 4, 4, 1,
                isAmbusher: false, treasure: 1,
                effect: "All enemies in this combat group lose 1 STR. If entering room 3 or later: enemies also lose 1 INIT.",
                race: Race.Elemental, allyClass: AllyClass.Mage,
                effectTags: "PASSIVE|-STR:1:ALL_ENEMIES\nPASSIVE|-INIT:1:ALL_ENEMIES"),

            // --- Cleric Uncommon (3) ---

            // Dwarven Cleric: Cleric, Dwarf — Cost 3, Budget = 14
            // Stats: STR 3(3) + HP 5(5) + INIT 1(1.5) + Treasure 1(1) = 10.5
            // Effect budget: 14 - 10.5 = 3.5 (Uncommon 3-4 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000013"), "Dwarven Cleric", Rarity.Uncommon, 3, 3, 5, 1,
                isAmbusher: false, treasure: 1,
                effect: "Restore 2 HP to the most injured ally at the start of combat. Against Undead: +2 STR.",
                race: Race.Dwarf, allyClass: AllyClass.Cleric,
                effectTags: "ON_COMBAT_START|HEAL:2:ALLY\nPASSIVE|FAVORED_ENEMY:Undead|+STR:2"),

            // Firbolg Druid: Cleric, Giant — Cost 3, Budget = 14
            // Stats: STR 3(3) + HP 5(5) + INIT 1(1.5) + Treasure 0(0) = 9.5
            // Effect budget: 14 - 9.5 = 4.5 -> 4 (Uncommon 3-4 range)
            // Readjust: STR 3(3) + HP 5(5) + INIT 1(1.5) + Treasure 1(1) = 10.5
            // Effect budget: 14 - 10.5 = 3.5 (Uncommon 3-4 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000020"), "Firbolg Druid", Rarity.Uncommon, 3, 3, 5, 1,
                isAmbusher: false, treasure: 1,
                effect: "Restore 1 HP to all allies at end of combat. If a Potion or Balm is used this combat: restore 2 additional HP to target.",
                race: Race.Giant, allyClass: AllyClass.Cleric,
                effectTags: "PASSIVE|HEAL:1:ALL_ALLIES\nPASSIVE|HEAL:2:ALLY"),

            // Lizardfolk Shaman: Cleric, Beast — Cost 3, Budget = 14
            // Stats: STR 4(4) + HP 4(4) + INIT 1(1.5) + Treasure 1(1) = 10.5
            // Effect budget: 14 - 10.5 = 3.5 (Uncommon 3-4 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000028"), "Lizardfolk Shaman", Rarity.Uncommon, 3, 4, 4, 1,
                isAmbusher: false, treasure: 1,
                effect: "Restore 1 HP to all allies at start of combat. If an ally is defeated this combat: gain +2 STR for the rest of the room.",
                race: Race.Beast, allyClass: AllyClass.Cleric,
                effectTags: "ON_COMBAT_START|HEAL:1:ALL_ALLIES\nON_DEATH|+STR:2"),

            // --- Ranger Uncommon (3) ---

            // Elven Ranger: Ranger, Elf — Cost 3, Budget = 14
            // Stats: STR 3(3) + HP 3(3) + INIT 2(3) + Treasure 1(1) = 10
            // Effect budget: 14 - 10 = 4 (Uncommon 3-4 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000012"), "Elven Ranger", Rarity.Uncommon, 3, 3, 3, 2,
                isAmbusher: false, treasure: 1,
                effect: "Favored Enemy (Orc): +3 STR against Orcs. Reveal the next Trap before it triggers.",
                race: Race.Elf, allyClass: AllyClass.Ranger,
                effectTags: "PASSIVE|FAVORED_ENEMY:Orc|+STR:3\nPASSIVE|REVEAL_DECK:1"),

            // Changeling Spy: Ranger, Human — Cost 2, Budget = 10
            // Stats: STR 1(1) + HP 2(2) + INIT 1(1.5) + Ambusher(2) + Treasure 0(0) = 6.5
            // Effect budget: 10 - 6.5 = 3.5 (Uncommon 3-4 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000025"), "Changeling Spy", Rarity.Uncommon, 2, 1, 2, 1,
                isAmbusher: true, treasure: 0,
                effect: "Reveal opponent's hand at the start of combat. If a Trap is in the current room: all allies gain +1 INIT.",
                race: Race.Human, allyClass: AllyClass.Ranger,
                effectTags: "ON_COMBAT_START|REVEAL_HAND\nPASSIVE|+INIT:1"),

            // Aasimar Monk: Ranger, Human — Cost 3, Budget = 14
            // Stats: STR 3(3) + HP 4(4) + INIT 2(3) + Treasure 0(0) = 10
            // Effect budget: 14 - 10 = 4 (Uncommon 3-4 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000019"), "Aasimar Monk", Rarity.Uncommon, 3, 3, 4, 2,
                isAmbusher: false, treasure: 0,
                effect: "Reveal top 3 cards of opponent's deck. Favored Enemy (Demon): +3 STR against Demons.",
                race: Race.Human, allyClass: AllyClass.Ranger,
                effectTags: "ON_PLAY|REVEAL_DECK:3\nPASSIVE|FAVORED_ENEMY:Demon|+STR:3"),

            // --- Paladin Uncommon (3) ---

            // Human Paladin: Paladin, Human — Cost 3, Budget = 14
            // Stats: STR 4(4) + HP 5(5) + INIT 1(1.5) + Treasure 0(0) = 10.5
            // Effect budget: 14 - 10.5 = 3.5 (Uncommon 3-4 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000011"), "Human Paladin", Rarity.Uncommon, 3, 4, 5, 1,
                isAmbusher: false, treasure: 0,
                effect: "Sacrifice 2 HP: restore 3 HP to another ally. Against Undead: +2 STR.",
                race: Race.Human, allyClass: AllyClass.Paladin,
                effectTags: "ON_ACTIVATE|COST:HP:2|HEAL:3:ALLY\nPASSIVE|FAVORED_ENEMY:Undead|+STR:2"),

            // Tortle Guardian: Paladin, Beast — Cost 3, Budget = 14
            // Stats: STR 2(2) + HP 6(6) + INIT 1(1.5) + Treasure 1(1) = 10.5
            // Effect budget: 14 - 10.5 = 3.5 (Uncommon 3-4 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000024"), "Tortle Guardian", Rarity.Uncommon, 3, 2, 6, 1,
                isAmbusher: false, treasure: 1,
                effect: "Sacrifice 1 HP: redirect all damage from one ally to this ally for the combat.",
                race: Race.Beast, allyClass: AllyClass.Paladin,
                effectTags: "ON_ACTIVATE|COST:HP:1|REDIRECT_DAMAGE:ALLY"),

            // Bugbear Bruiser: Paladin, Goblin — Cost 3, Budget = 14
            // Stats: STR 3(3) + HP 3(3) + INIT 1(1.5) + Ambusher(2) + Treasure 1(1) = 10.5
            // Effect budget: 14 - 10.5 = 3.5 (Uncommon 3-4 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000029"), "Bugbear Bruiser", Rarity.Uncommon, 3, 3, 3, 1,
                isAmbusher: true, treasure: 1,
                effect: "Sacrifice 2 HP: +3 STR this combat. If a Paladin ally is in play: +1 INIT.",
                race: Race.Goblin, allyClass: AllyClass.Paladin,
                effectTags: "ON_ACTIVATE|COST:HP:2|+STR:3\nPASSIVE|IF_CLASS:Paladin|+INIT:1"),

            // ===== COMMON (30) =====

            // --- Warrior Common (5) ---

            // Militia Recruit: Warrior, Human — Cost 1, Budget = 6
            // Stats: STR 2(2) + HP 3(3) + INIT 0(0) + Treasure 0(0) = 5
            // Effect budget: 6 - 5 = 1 (Common 0-2 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000031"), "Militia Recruit", Rarity.Common, 1, 2, 3, 0,
                isAmbusher: false, treasure: 0,
                effect: "With Disadvantage: +1 HP.",
                race: Race.Human, allyClass: AllyClass.Warrior,
                effectTags: "WITH_DISADVANTAGE|+HP:1"),

            // Town Guard: Warrior, Human — Cost 1, Budget = 6
            // Stats: STR 1(1) + HP 4(4) + INIT 0(0) + Treasure 0(0) = 5
            // Effect budget: 6 - 5 = 1 (Common 0-2 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000032"), "Town Guard", Rarity.Common, 1, 1, 4, 0,
                isAmbusher: false, treasure: 0,
                effect: "With Disadvantage: +2 HP.",
                race: Race.Human, allyClass: AllyClass.Warrior,
                effectTags: "WITH_DISADVANTAGE|+HP:2"),

            // Caravan Guard: Warrior, Human — Cost 1, Budget = 6
            // Stats: STR 3(3) + HP 2(2) + INIT 0(0) + Treasure 0(0) = 5
            // Effect budget: 6 - 5 = 1 (Common 0-2 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000042"), "Caravan Guard", Rarity.Common, 1, 3, 2, 0,
                isAmbusher: false, treasure: 0,
                effect: "If equipped with a Weapon: +1 STR.",
                race: Race.Human, allyClass: AllyClass.Warrior,
                effectTags: "PASSIVE|IF_EQUIPPED:Weapon|+STR:1"),

            // Shield Bearer: Warrior, Human — Cost 1, Budget = 6
            // Stats: STR 1(1) + HP 3(3) + INIT 1(1.5) + Treasure 0(0) = 5.5
            // Effect budget: 6 - 5.5 = 0.5 (Common 0-2 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000051"), "Shield Bearer", Rarity.Common, 1, 1, 3, 1,
                isAmbusher: false, treasure: 0,
                effect: "With Disadvantage: +1 HP.",
                race: Race.Human, allyClass: AllyClass.Warrior,
                effectTags: "WITH_DISADVANTAGE|+HP:1"),

            // Pit Fighter: Warrior, Human — Cost 2, Budget = 10
            // Stats: STR 4(4) + HP 4(4) + INIT 0(0) + Treasure 1(1) = 9
            // Effect budget: 10 - 9 = 1 (Common 0-2 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000043"), "Pit Fighter", Rarity.Common, 2, 4, 4, 0,
                isAmbusher: false, treasure: 1,
                effect: "With Disadvantage: +1 STR.",
                race: Race.Human, allyClass: AllyClass.Warrior,
                effectTags: "WITH_DISADVANTAGE|+STR:1"),

            // --- Rogue Common (5) ---

            // Street Urchin: Rogue, Human — Cost 1, Budget = 6
            // Stats: STR 1(1) + HP 1(1) + INIT 1(1.5) + Ambusher(2) + Treasure 0(0) = 5.5
            // Effect budget: 6 - 5.5 = 0.5 (Common 0-2 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000037"), "Street Urchin", Rarity.Common, 1, 1, 1, 1,
                isAmbusher: true, treasure: 0,
                effect: "With Advantage: +1 STR.",
                race: Race.Human, allyClass: AllyClass.Rogue,
                effectTags: "WITH_ADVANTAGE|+STR:1"),

            // Lockpick Expert: Rogue, Human — Cost 1, Budget = 6
            // Stats: STR 1(1) + HP 2(2) + INIT 0(0) + Ambusher(2) + Treasure 0(0) = 5
            // Effect budget: 6 - 5 = 1 (Common 0-2 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000049"), "Lockpick Expert", Rarity.Common, 1, 1, 2, 0,
                isAmbusher: true, treasure: 0,
                effect: "With Advantage: +2 STR.",
                race: Race.Human, allyClass: AllyClass.Rogue,
                effectTags: "WITH_ADVANTAGE|+STR:2"),

            // Tomb Raider: Rogue, Human — Cost 2, Budget = 10
            // Stats: STR 3(3) + HP 2(2) + INIT 1(1.5) + Ambusher(2) + Treasure 0(0) = 8.5
            // Effect budget: 10 - 8.5 = 1.5 (Common 0-2 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000059"), "Tomb Raider", Rarity.Common, 2, 3, 2, 1,
                isAmbusher: true, treasure: 0,
                effect: "With Advantage: +2 STR.",
                race: Race.Human, allyClass: AllyClass.Rogue,
                effectTags: "WITH_ADVANTAGE|+STR:2"),

            // Throwing Knife Specialist: Rogue, Human — Cost 2, Budget = 10
            // Stats: STR 3(3) + HP 3(3) + INIT 1(1.5) + Treasure 1(1) = 8.5
            // Effect budget: 10 - 8.5 = 1.5 (Common 0-2 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000052"), "Throwing Knife Specialist", Rarity.Common, 2, 3, 3, 1,
                isAmbusher: false, treasure: 1,
                effect: "With Advantage: +2 STR.",
                race: Race.Human, allyClass: AllyClass.Rogue,
                effectTags: "WITH_ADVANTAGE|+STR:2"),

            // Tavern Brawler: Rogue, Human — Cost 1, Budget = 6
            // Stats: STR 2(2) + HP 2(2) + INIT 1(1.5) + Treasure 0(0) = 5.5
            // Effect budget: 6 - 5.5 = 0.5 (Common 0-2 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000038"), "Tavern Brawler", Rarity.Common, 1, 2, 2, 1,
                isAmbusher: false, treasure: 0,
                effect: "With Advantage: +1 STR.",
                race: Race.Human, allyClass: AllyClass.Rogue,
                effectTags: "WITH_ADVANTAGE|+STR:1"),

            // --- Mage Common (5) ---

            // Apprentice Wizard: Mage, Human — Cost 2, Budget = 10
            // Stats: STR 3(3) + HP 3(3) + INIT 1(1.5) + Treasure 1(1) = 8.5
            // Effect budget: 10 - 8.5 = 1.5 (Common 0-2 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000033"), "Apprentice Wizard", Rarity.Common, 2, 3, 3, 1,
                isAmbusher: false, treasure: 1,
                effect: "If equipped with a Scroll: +1 STR.",
                race: Race.Human, allyClass: AllyClass.Mage,
                effectTags: "PASSIVE|IF_SCROLL_USED|+STR:1"),

            // Hedge Mage: Mage, Human — Cost 2, Budget = 10
            // Stats: STR 2(2) + HP 3(3) + INIT 2(3) + Treasure 1(1) = 9
            // Effect budget: 10 - 9 = 1 (Common 0-2 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000040"), "Hedge Mage", Rarity.Common, 2, 2, 3, 2,
                isAmbusher: false, treasure: 1,
                effect: "All allies in this combat group gain +1 INIT.",
                race: Race.Human, allyClass: AllyClass.Mage,
                effectTags: "PASSIVE|+INIT:1"),

            // Fortune Teller: Mage, Human — Cost 2, Budget = 10
            // Stats: STR 2(2) + HP 4(4) + INIT 1(1.5) + Treasure 1(1) = 8.5
            // Effect budget: 10 - 8.5 = 1.5 (Common 0-2 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000060"), "Fortune Teller", Rarity.Common, 2, 2, 4, 1,
                isAmbusher: false, treasure: 1,
                effect: "Reveal top card of opponent's deck at the start of combat.",
                race: Race.Human, allyClass: AllyClass.Mage,
                effectTags: "ON_COMBAT_START|REVEAL_DECK:1"),

            // War Drummer: Mage, Human — Cost 1, Budget = 6
            // Stats: STR 1(1) + HP 2(2) + INIT 1(1.5) + Treasure 0(0) = 4.5
            // Effect budget: 6 - 4.5 = 1.5 (Common 0-2 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000054"), "War Drummer", Rarity.Common, 1, 1, 2, 1,
                isAmbusher: false, treasure: 0,
                effect: "All allies in this combat group gain +1 INIT.",
                race: Race.Human, allyClass: AllyClass.Mage,
                effectTags: "PASSIVE|+INIT:1"),

            // Torchbearer: Mage, Human — Cost 1, Budget = 6
            // Stats: STR 1(1) + HP 3(3) + INIT 0(0) + Treasure 1(1) = 5
            // Effect budget: 6 - 5 = 1 (Common 0-2 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000047"), "Torchbearer", Rarity.Common, 1, 1, 3, 0,
                isAmbusher: false, treasure: 1,
                effect: "Enemies in this combat group lose Ambusher.",
                race: Race.Human, allyClass: AllyClass.Mage,
                effectTags: "PASSIVE|REVEAL_HAND"),

            // --- Cleric Common (5) ---

            // Acolyte Healer: Cleric, Human — Cost 2, Budget = 10
            // Stats: STR 2(2) + HP 4(4) + INIT 1(1.5) + Treasure 1(1) = 8.5
            // Effect budget: 10 - 8.5 = 1.5 (Common 0-2 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000034"), "Acolyte Healer", Rarity.Common, 2, 2, 4, 1,
                isAmbusher: false, treasure: 1,
                effect: "Restore 2 HP to the most injured ally at end of combat.",
                race: Race.Human, allyClass: AllyClass.Cleric,
                effectTags: "PASSIVE|HEAL:2:ALLY"),

            // Shrine Maiden: Cleric, Human — Cost 2, Budget = 10
            // Stats: STR 2(2) + HP 4(4) + INIT 1(1.5) + Treasure 1(1) = 8.5
            // Effect budget: 10 - 8.5 = 1.5 (Common 0-2 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000045"), "Shrine Maiden", Rarity.Common, 2, 2, 4, 1,
                isAmbusher: false, treasure: 1,
                effect: "Against Undead: +2 STR.",
                race: Race.Human, allyClass: AllyClass.Cleric,
                effectTags: "PASSIVE|FAVORED_ENEMY:Undead|+STR:2"),

            // Battle Medic: Cleric, Human — Cost 2, Budget = 10
            // Stats: STR 3(3) + HP 4(4) + INIT 0(0) + Treasure 1(1) = 8
            // Effect budget: 10 - 8 = 2 (Common 0-2 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000050"), "Battle Medic", Rarity.Common, 2, 3, 4, 0,
                isAmbusher: false, treasure: 1,
                effect: "If a Balm or Potion is used this combat: restore 2 HP to target ally.",
                race: Race.Human, allyClass: AllyClass.Cleric,
                effectTags: "PASSIVE|HEAL:2:ALLY"),

            // Camp Cook: Cleric, Human — Cost 1, Budget = 6
            // Stats: STR 1(1) + HP 3(3) + INIT 0(0) + Treasure 1(1) = 5
            // Effect budget: 6 - 5 = 1 (Common 0-2 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000056"), "Camp Cook", Rarity.Common, 1, 1, 3, 0,
                isAmbusher: false, treasure: 1,
                effect: "Between rooms: restore 1 HP to all allies.",
                race: Race.Human, allyClass: AllyClass.Cleric,
                effectTags: "PASSIVE|HEAL:1:ALL_ALLIES"),

            // Farmhand Fighter: Cleric, Human — Cost 1, Budget = 6
            // Stats: STR 2(2) + HP 3(3) + INIT 0(0) + Treasure 0(0) = 5
            // Effect budget: 6 - 5 = 1 (Common 0-2 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000039"), "Farmhand Fighter", Rarity.Common, 1, 2, 3, 0,
                isAmbusher: false, treasure: 0,
                effect: "If another Cleric is in play: +1 HP.",
                race: Race.Human, allyClass: AllyClass.Cleric,
                effectTags: "PASSIVE|IF_CLASS:Cleric|+HP:1"),

            // --- Ranger Common (5) ---

            // Scout Tracker: Ranger, Human — Cost 2, Budget = 10
            // Stats: STR 3(3) + HP 3(3) + INIT 1(1.5) + Treasure 1(1) = 8.5
            // Effect budget: 10 - 8.5 = 1.5 (Common 0-2 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000036"), "Scout Tracker", Rarity.Common, 2, 3, 3, 1,
                isAmbusher: false, treasure: 1,
                effect: "Favored Enemy (Beast): +2 STR against Beasts.",
                race: Race.Human, allyClass: AllyClass.Ranger,
                effectTags: "PASSIVE|FAVORED_ENEMY:Beast|+STR:2"),

            // Forest Warden: Ranger, Human — Cost 2, Budget = 10
            // Stats: STR 3(3) + HP 3(3) + INIT 1(1.5) + Treasure 1(1) = 8.5
            // Effect budget: 10 - 8.5 = 1.5 (Common 0-2 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000044"), "Forest Warden", Rarity.Common, 2, 3, 3, 1,
                isAmbusher: false, treasure: 1,
                effect: "Favored Enemy (Beast): +2 STR against Beasts.",
                race: Race.Human, allyClass: AllyClass.Ranger,
                effectTags: "PASSIVE|FAVORED_ENEMY:Beast|+STR:2"),

            // Beast Handler: Ranger, Human — Cost 2, Budget = 10
            // Stats: STR 2(2) + HP 3(3) + INIT 2(3) + Treasure 1(1) = 9
            // Effect budget: 10 - 9 = 1 (Common 0-2 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000057"), "Beast Handler", Rarity.Common, 2, 2, 3, 2,
                isAmbusher: false, treasure: 1,
                effect: "Favored Enemy (Beast): +2 STR against Beasts.",
                race: Race.Human, allyClass: AllyClass.Ranger,
                effectTags: "PASSIVE|FAVORED_ENEMY:Beast|+STR:2"),

            // Map Reader: Ranger, Human — Cost 1, Budget = 6
            // Stats: STR 1(1) + HP 2(2) + INIT 1(1.5) + Treasure 0(0) = 4.5
            // Effect budget: 6 - 4.5 = 1.5 (Common 0-2 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000058"), "Map Reader", Rarity.Common, 1, 1, 2, 1,
                isAmbusher: false, treasure: 0,
                effect: "Reveal the next room's Trap cards.",
                race: Race.Human, allyClass: AllyClass.Ranger,
                effectTags: "PASSIVE|REVEAL_DECK:1"),

            // Dungeon Delver: Ranger, Human — Cost 2, Budget = 10
            // Stats: STR 2(2) + HP 3(3) + INIT 1(1.5) + Treasure 2(2) = 8.5
            // Effect budget: 10 - 8.5 = 1.5 (Common 0-2 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000046"), "Dungeon Delver", Rarity.Common, 2, 2, 3, 1,
                isAmbusher: false, treasure: 2,
                effect: "Traps in this room deal 1 less damage to all allies.",
                race: Race.Human, allyClass: AllyClass.Ranger,
                effectTags: "PASSIVE|REDUCE_DAMAGE:1"),

            // --- Paladin Common (5) ---

            // Squire of the Order: Paladin, Human — Cost 1, Budget = 6
            // Stats: STR 2(2) + HP 3(3) + INIT 0(0) + Treasure 0(0) = 5
            // Effect budget: 6 - 5 = 1 (Common 0-2 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000035"), "Squire of the Order", Rarity.Common, 1, 2, 3, 0,
                isAmbusher: false, treasure: 0,
                effect: "If a Paladin ally is in play: +1 STR.",
                race: Race.Human, allyClass: AllyClass.Paladin,
                effectTags: "PASSIVE|IF_CLASS:Paladin|+STR:1"),

            // Flag Bearer: Paladin, Human — Cost 1, Budget = 6
            // Stats: STR 1(1) + HP 3(3) + INIT 0(0) + Treasure 1(1) = 5
            // Effect budget: 6 - 5 = 1 (Common 0-2 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000055"), "Flag Bearer", Rarity.Common, 1, 1, 3, 0,
                isAmbusher: false, treasure: 1,
                effect: "If this ally is defeated: all allies gain +1 STR for the rest of the room.",
                race: Race.Human, allyClass: AllyClass.Paladin,
                effectTags: "ON_DEATH|+STR:1"),

            // Cavalry Rider: Paladin, Human — Cost 2, Budget = 10
            // Stats: STR 3(3) + HP 4(4) + INIT 1(1.5) + Treasure 0(0) = 8.5
            // Effect budget: 10 - 8.5 = 1.5 (Common 0-2 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000053"), "Cavalry Rider", Rarity.Common, 2, 3, 4, 1,
                isAmbusher: false, treasure: 0,
                effect: "Sacrifice 1 HP: +2 STR this combat.",
                race: Race.Human, allyClass: AllyClass.Paladin,
                effectTags: "ON_ACTIVATE|COST:HP:1|+STR:2"),

            // Trapfinder: Paladin, Human — Cost 2, Budget = 10
            // Stats: STR 2(2) + HP 4(4) + INIT 1(1.5) + Treasure 1(1) = 8.5
            // Effect budget: 10 - 8.5 = 1.5 (Common 0-2 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000048"), "Trapfinder", Rarity.Common, 2, 2, 4, 1,
                isAmbusher: false, treasure: 1,
                effect: "Sacrifice 1 HP: disarm one Trap in this room.",
                race: Race.Human, allyClass: AllyClass.Paladin,
                effectTags: "ON_ACTIVATE|COST:HP:1|CANCEL_COMBAT"),

            // Crossbow Specialist: Paladin, Human — Cost 2, Budget = 10
            // Stats: STR 4(4) + HP 3(3) + INIT 1(1.5) + Treasure 0(0) = 8.5
            // Effect budget: 10 - 8.5 = 1.5 (Common 0-2 range)
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000041"), "Crossbow Specialist", Rarity.Common, 2, 4, 3, 1,
                isAmbusher: false, treasure: 0,
                effect: "Sacrifice 2 HP: deal 3 STR damage to any enemy in any combat group.",
                race: Race.Human, allyClass: AllyClass.Paladin,
                effectTags: "ON_ACTIVATE|COST:HP:2|DAMAGE:3:ENEMY")
        };
    }
}
