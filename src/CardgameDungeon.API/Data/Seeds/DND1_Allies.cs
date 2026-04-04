using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.API.Data.Seeds;

public static partial class CardSetSeeder
{
    private static List<AllyCard> CreateAllies()
    {
        return new List<AllyCard>
        {
            // ═══════════════════════════════════════════════════════════════
            //  WARRIOR (6): 1 Unique, 1 Rare, 2 Uncommon, 2 Common
            // ═══════════════════════════════════════════════════════════════

            // ── UNIQUE ──
            // Thordak the Ironwall: Warrior, Dwarf — Cost 6, Unique Budget = 87
            // ATK 10((10-1)*3=27) + HP 5((5-1)*3=12) + INIT 3(3*1.5=4.5) + Treasure 1(-1) = 42.5. Effect = 44.5
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000061"), "Thordak the Ironwall", Rarity.Unique, 6, 10, 5, 3,
                isAmbusher: false, treasure: 1,
                effect: "The mountain that walks. Discard 1 from hand: all damage dealt to allies in this combat group is redirected to Thordak this round. With Disadvantage: +4 HP. Exile 8 cards from deck: force all enemies in the room to target only Thordak until end of room",
                race: Race.Dwarf, allyClass: AllyClass.Warrior,
                effectTags: "ON_ACTIVATE|COST:DISCARD_HAND:1|REDIRECT_DAMAGE:GROUP\nWITH_DISADVANTAGE|+HP:4\nON_ACTIVATE|COST:EXILE_DECK:8|REDIRECT_DAMAGE:ALL_ENEMIES"),

            // ── RARE ──
            // Bruenor Battlehammer: Warrior, Dwarf — Cost 4, Rare Budget = 53
            // ATK 9((9-1)*3=24) + HP 4((4-1)*3=9) + INIT 4(4*1.5=6) + Treasure 1(-1) = 38. Effect = 15
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000003"), "Bruenor Battlehammer", Rarity.Rare, 4, 9, 4, 4,
                isAmbusher: false, treasure: 1,
                effect: "Reduces all incoming damage to allies in this combat group by 1. With Disadvantage: +3 HP. If another Dwarf ally in play: +2 HP.",
                race: Race.Dwarf, allyClass: AllyClass.Warrior,
                effectTags: "PASSIVE|REDUCE_DAMAGE:1\nWITH_DISADVANTAGE|+HP:3\nPASSIVE|IF_RACE:Dwarf|+HP:2"),

            // ── UNCOMMON ──
            // Dragonborn Fighter: Warrior, Dragonborn — Cost 3, Uncommon Budget = 37
            // ATK 8((8-1)*3=21) + HP 3((3-1)*3=6) + INIT 3(3*1.5=4.5) + Treasure 1(-1) = 30.5. Effect = 6.5
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000017"), "Dragonborn Fighter", Rarity.Uncommon, 3, 8, 3, 3,
                isAmbusher: false, treasure: 1,
                effect: "With Disadvantage: +2 ATK. If equipped with a Weapon: +1 ATK.",
                race: Race.Dragonborn, allyClass: AllyClass.Warrior,
                effectTags: "WITH_DISADVANTAGE|+ATK:2\nPASSIVE|IF_EQUIPPED:Weapon|+ATK:1"),

            // Half-Orc Barbarian: Warrior, Orc — Cost 3, Uncommon Budget = 37
            // ATK 8((8-1)*3=21) + HP 3((3-1)*3=6) + INIT 3(3*1.5=4.5) + Treasure 0(0) = 31.5. Effect = 5.5
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000018"), "Half-Orc Barbarian", Rarity.Uncommon, 3, 8, 3, 3,
                isAmbusher: false, treasure: 0,
                effect: "With Disadvantage: +3 HP. If below half HP: +2 ATK.",
                race: Race.Orc, allyClass: AllyClass.Warrior,
                effectTags: "WITH_DISADVANTAGE|+HP:3\nPASSIVE|+ATK:2"),

            // ── COMMON ──
            // Militia Recruit: Warrior, Human — Cost 1, Common Budget = 15
            // ATK 4((4-1)*3=9) + HP 2((2-1)*3=3) + INIT 1(1*1.5=1.5) + Treasure 0(0) = 13.5. Effect = 1.5
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000031"), "Militia Recruit", Rarity.Common, 1, 4, 2, 1,
                isAmbusher: false, treasure: 0,
                effect: "With Disadvantage: +1 HP.",
                race: Race.Human, allyClass: AllyClass.Warrior,
                effectTags: "WITH_DISADVANTAGE|+HP:1"),

            // Pit Fighter: Warrior, Human — Cost 2, Common Budget = 29
            // ATK 7((7-1)*3=18) + HP 3((3-1)*3=6) + INIT 2(2*1.5=3) + Treasure 1(-1) = 26. Effect = 3
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000043"), "Pit Fighter", Rarity.Common, 2, 7, 3, 2,
                isAmbusher: false, treasure: 1,
                effect: "With Disadvantage: +1 ATK.",
                race: Race.Human, allyClass: AllyClass.Warrior,
                effectTags: "WITH_DISADVANTAGE|+ATK:1"),

            // ═══════════════════════════════════════════════════════════════
            //  ROGUE (6): 1 Unique, 1 Rare, 2 Uncommon, 2 Common
            // ═══════════════════════════════════════════════════════════════

            // ── UNIQUE ──
            // Irvine the Opportunist: Rogue, Halfling — Cost 5, Unique Budget = 75
            // ATK 10((10-1)*3=27) + HP 5((5-1)*3=12) + INIT 3(3*1.5=4.5) + Ambusher(2) + Treasure 1(-1) = 44.5. Effect = 30.5
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000063"), "Irvine the Opportunist", Rarity.Unique, 5, 10, 5, 3,
                isAmbusher: true, treasure: 1,
                effect: "Every opening is a death sentence. This ally's opportunity attacks deal double ATK damage. When any enemy flees or changes combat group: Irvine automatically performs an opportunity attack against them (does not count toward the 1-per-round limit). With Advantage: +3 ATK",
                race: Race.Halfling, allyClass: AllyClass.Rogue,
                effectTags: "PASSIVE|OPP_ATTACK_DOUBLE\nON_ENEMY_FLEE|IGNORE_OPP_ATTACK_LIMIT\nWITH_ADVANTAGE|+ATK:3"),

            // ── RARE ──
            // Regis the Halfling: Rogue, Halfling — Cost 3, Rare Budget = 43
            // ATK 8((8-1)*3=21) + HP 3((3-1)*3=6) + INIT 3(3*1.5=4.5) + Ambusher(2) + Treasure 1(-1) = 32.5. Effect = 10.5
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000006"), "Regis the Halfling", Rarity.Rare, 3, 8, 3, 3,
                isAmbusher: true, treasure: 1,
                effect: "With Advantage: +3 ATK. On kill: exile the defeated enemy and gain +1 treasure. If another Halfling ally in play: +1 INIT.",
                race: Race.Halfling, allyClass: AllyClass.Rogue,
                effectTags: "WITH_ADVANTAGE|+ATK:3\nON_KILL|EXILE_HAND:1\nPASSIVE|IF_RACE:Halfling|+INIT:1"),

            // ── UNCOMMON ──
            // Halfling Rogue: Rogue, Halfling — Cost 2, Uncommon Budget = 32
            // ATK 7((7-1)*3=18) + HP 2((2-1)*3=3) + INIT 2(2*1.5=3) + Ambusher(2) + Treasure 0(0) = 26. Effect = 6
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000014"), "Halfling Rogue", Rarity.Uncommon, 2, 7, 2, 2,
                isAmbusher: true, treasure: 0,
                effect: "With Advantage: +3 ATK. On kill: exile the defeated enemy.",
                race: Race.Halfling, allyClass: AllyClass.Rogue,
                effectTags: "WITH_ADVANTAGE|+ATK:3\nON_KILL|EXILE_HAND:1"),

            // Tabaxi Swashbuckler: Rogue, Beast — Cost 3, Uncommon Budget = 37
            // ATK 8((8-1)*3=21) + HP 3((3-1)*3=6) + INIT 2(2*1.5=3) + Ambusher(2) + Treasure 1(-1) = 31. Effect = 6
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000022"), "Tabaxi Swashbuckler", Rarity.Uncommon, 3, 8, 3, 2,
                isAmbusher: true, treasure: 1,
                effect: "With Advantage: +2 ATK. Opportunity Attacks against this ally deal no damage.",
                race: Race.Beast, allyClass: AllyClass.Rogue,
                effectTags: "WITH_ADVANTAGE|+ATK:2\nPASSIVE|REDUCE_DAMAGE:99"),

            // ── COMMON ──
            // Street Urchin: Rogue, Human — Cost 1, Common Budget = 15
            // ATK 4((4-1)*3=9) + HP 1((1-1)*3=0) + INIT 1(1*1.5=1.5) + Ambusher(2) + Treasure 0(0) = 12.5. Effect = 2.5
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000037"), "Street Urchin", Rarity.Common, 1, 4, 1, 1,
                isAmbusher: true, treasure: 0,
                effect: "With Advantage: +1 ATK.",
                race: Race.Human, allyClass: AllyClass.Rogue,
                effectTags: "WITH_ADVANTAGE|+ATK:1"),

            // Tomb Raider: Rogue, Human — Cost 2, Common Budget = 29
            // ATK 7((7-1)*3=18) + HP 2((2-1)*3=3) + INIT 2(2*1.5=3) + Ambusher(2) + Treasure 0(0) = 26. Effect = 3
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000059"), "Tomb Raider", Rarity.Common, 2, 7, 2, 2,
                isAmbusher: true, treasure: 0,
                effect: "With Advantage: +2 ATK.",
                race: Race.Human, allyClass: AllyClass.Rogue,
                effectTags: "WITH_ADVANTAGE|+ATK:2"),

            // ═══════════════════════════════════════════════════════════════
            //  MAGE (5): 1 Unique, 1 Rare, 2 Uncommon, 1 Common
            // ═══════════════════════════════════════════════════════════════

            // ── UNIQUE ──
            // Elminster Aumar: Mage, Human — Cost 7, Unique Budget = 99
            // ATK 10((10-1)*3=27) + HP 5((5-1)*3=12) + INIT 4(4*1.5=6) + Treasure 1(-1) = 44. Effect = 55
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000002"), "Elminster Aumar", Rarity.Unique, 7, 10, 5, 4,
                isAmbusher: false, treasure: 1,
                effect: "The Chosen of Mystra channels the Weave itself. Exile 2 cards from hand: search your deck for any Scroll, put it in your hand, and reshuffle. When Elminster uses a Scroll: the Scroll is returned to the bottom of the deck instead of being destroyed. Exile 8 cards from deck: search your exile pile for up to 3 Scrolls and return them to hand",
                race: Race.Human, allyClass: AllyClass.Mage,
                effectTags: "ON_ACTIVATE|COST:EXILE_HAND:2|SEARCH_DECK:SCROLL\nON_SCROLL_USED|SCROLL_TO_BOTTOM\nON_ACTIVATE|COST:EXILE_DECK:8|RECOVER_SCROLL_EXILE:3"),

            // ── RARE ──
            // Mordenkainen: Mage, Human — Cost 4, Rare Budget = 53
            // ATK 9((9-1)*3=24) + HP 4((4-1)*3=9) + INIT 4(4*1.5=6) + Treasure 1(-1) = 38. Effect = 15
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000007"), "Mordenkainen", Rarity.Rare, 4, 9, 4, 4,
                isAmbusher: false, treasure: 1,
                effect: "Necromancer: choose 1 monster with cost <= 3 from any discard pile and place it on top of your deck. All allies in this combat group gain +1 ATK",
                race: Race.Human, allyClass: AllyClass.Mage,
                effectTags: "ON_ACTIVATE|SEARCH_DECK:MONSTER\nPASSIVE|+ATK:1"),

            // ── UNCOMMON ──
            // Gnome Artificer: Mage, Halfling — Cost 3, Uncommon Budget = 37
            // ATK 8((8-1)*3=21) + HP 3((3-1)*3=6) + INIT 3(3*1.5=4.5) + Treasure 0(0) = 31.5. Effect = 5.5
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000015"), "Gnome Artificer", Rarity.Uncommon, 3, 8, 3, 3,
                isAmbusher: false, treasure: 0,
                effect: "Equipped Accessories and Totems grant +1 additional ATK to the holder. If a Scroll is equipped on any ally: +2 ATK.",
                race: Race.Halfling, allyClass: AllyClass.Mage,
                effectTags: "PASSIVE|IF_EQUIPPED:Accessory|+ATK:1\nPASSIVE|IF_SCROLL_USED|+ATK:2"),

            // Eladrin Bladesinger: Mage, Elf — Cost 3, Uncommon Budget = 37
            // ATK 8((8-1)*3=21) + HP 3((3-1)*3=6) + INIT 3(3*1.5=4.5) + Treasure 0(0) = 31.5. Effect = 5.5
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000026"), "Eladrin Bladesinger", Rarity.Uncommon, 3, 8, 3, 3,
                isAmbusher: false, treasure: 0,
                effect: "If equipped with a Weapon: +2 ATK and +1 INIT. If another Elf ally in play: +1 HP.",
                race: Race.Elf, allyClass: AllyClass.Mage,
                effectTags: "PASSIVE|IF_EQUIPPED:Weapon|+ATK:2|+INIT:1\nPASSIVE|IF_RACE:Elf|+HP:1"),

            // ── COMMON ──
            // Apprentice Wizard: Mage, Human — Cost 2, Common Budget = 29
            // ATK 7((7-1)*3=18) + HP 2((2-1)*3=3) + INIT 2(2*1.5=3) + Treasure 1(-1) = 23. Effect = 6
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000033"), "Apprentice Wizard", Rarity.Common, 2, 7, 2, 2,
                isAmbusher: false, treasure: 1,
                effect: "If equipped with a Scroll: +1 ATK.",
                race: Race.Human, allyClass: AllyClass.Mage,
                effectTags: "PASSIVE|IF_SCROLL_USED|+ATK:1"),

            // ═══════════════════════════════════════════════════════════════
            //  CLERIC (6): 1 Unique, 1 Rare, 2 Uncommon, 2 Common
            // ═══════════════════════════════════════════════════════════════

            // ── UNIQUE ──
            // Seraphina the Radiant: Cleric, Human — Cost 6, Unique Budget = 87
            // ATK 10((10-1)*3=27) + HP 5((5-1)*3=12) + INIT 4(4*1.5=6) + Treasure 1(-1) = 44. Effect = 43
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000062"), "Seraphina the Radiant", Rarity.Unique, 6, 10, 5, 4,
                isAmbusher: false, treasure: 1,
                effect: "Her light mends what darkness breaks. At start of each round: restore 2 HP to all allies. +3 ATK against Undead and Demon. Exile 8 cards from deck: fully restore all allies to max HP and remove all negative effects",
                race: Race.Human, allyClass: AllyClass.Cleric,
                effectTags: "ON_ROUND_START|HEAL:2:ALL_ALLIES\nPASSIVE|IF_RACE:Undead|+ATK:3\nPASSIVE|IF_RACE:Demon|+ATK:3\nON_ACTIVATE|COST:EXILE_DECK:8|HEAL:99:ALL_ALLIES"),

            // ── RARE ──
            // Jalarloth Baenre: Cleric, Elf — Cost 4, Rare Budget = 53
            // ATK 9((9-1)*3=24) + HP 4((4-1)*3=9) + INIT 3(3*1.5=4.5) + Treasure 1(-1) = 36.5. Effect = 16.5
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000010"), "Jalarloth Baenre", Rarity.Rare, 4, 9, 4, 3,
                isAmbusher: false, treasure: 1,
                effect: "Restore 3 HP to all allies at the start of combat. Against Undead enemies: all allies gain +2 ATK. If a Potion is used: restore 2 additional HP to all allies.",
                race: Race.Elf, allyClass: AllyClass.Cleric,
                effectTags: "ON_COMBAT_START|HEAL:3:ALL_ALLIES\nPASSIVE|FAVORED_ENEMY:Undead|+ATK:2\nPASSIVE|HEAL:2:ALL_ALLIES"),

            // ── UNCOMMON ──
            // Dwarven Cleric: Cleric, Dwarf — Cost 3, Uncommon Budget = 37
            // ATK 8((8-1)*3=21) + HP 3((3-1)*3=6) + INIT 3(3*1.5=4.5) + Treasure 1(-1) = 30.5. Effect = 6.5
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000013"), "Dwarven Cleric", Rarity.Uncommon, 3, 8, 3, 3,
                isAmbusher: false, treasure: 1,
                effect: "Restore 2 HP to the most injured ally at the start of combat. Against Undead: +2 ATK.",
                race: Race.Dwarf, allyClass: AllyClass.Cleric,
                effectTags: "ON_COMBAT_START|HEAL:2:ALLY\nPASSIVE|FAVORED_ENEMY:Undead|+ATK:2"),

            // Firbolg Druid: Cleric, Giant — Cost 3, Uncommon Budget = 37
            // ATK 8((8-1)*3=21) + HP 3((3-1)*3=6) + INIT 3(3*1.5=4.5) + Treasure 1(-1) = 30.5. Effect = 6.5
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000020"), "Firbolg Druid", Rarity.Uncommon, 3, 8, 3, 3,
                isAmbusher: false, treasure: 1,
                effect: "Restore 1 HP to all allies at end of combat. If a Potion or Balm is used this combat: restore 2 additional HP to target.",
                race: Race.Giant, allyClass: AllyClass.Cleric,
                effectTags: "PASSIVE|HEAL:1:ALL_ALLIES\nPASSIVE|HEAL:2:ALLY"),

            // ── COMMON ──
            // Acolyte Healer: Cleric, Human — Cost 2, Common Budget = 29
            // ATK 7((7-1)*3=18) + HP 2((2-1)*3=3) + INIT 2(2*1.5=3) + Treasure 1(-1) = 23. Effect = 6
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000034"), "Acolyte Healer", Rarity.Common, 2, 7, 2, 2,
                isAmbusher: false, treasure: 1,
                effect: "Restore 2 HP to the most injured ally at end of combat.",
                race: Race.Human, allyClass: AllyClass.Cleric,
                effectTags: "PASSIVE|HEAL:2:ALLY"),

            // Camp Cook: Cleric, Human — Cost 1, Common Budget = 15
            // ATK 4((4-1)*3=9) + HP 2((2-1)*3=3) + INIT 1(1*1.5=1.5) + Treasure 1(-1) = 12.5. Effect = 2.5
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000056"), "Camp Cook", Rarity.Common, 1, 4, 2, 1,
                isAmbusher: false, treasure: 1,
                effect: "Between rooms: restore 1 HP to all allies.",
                race: Race.Human, allyClass: AllyClass.Cleric,
                effectTags: "PASSIVE|HEAL:1:ALL_ALLIES"),

            // ═══════════════════════════════════════════════════════════════
            //  RANGER (5): 1 Unique, 1 Rare, 2 Uncommon, 1 Common
            // ═══════════════════════════════════════════════════════════════

            // ── UNIQUE ──
            // Drizzt Do'Urden: Ranger, Drow — Cost 6, Unique Budget = 87
            // ATK 10((10-1)*3=27) + HP 5((5-1)*3=12) + INIT 4(4*1.5=6) + Treasure 1(-1) = 44. Effect = 43
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000001"), "Drizzt Do'Urden", Rarity.Unique, 6, 10, 5, 4,
                isAmbusher: false, treasure: 1,
                effect: "The legendary Drow ranger moves like shadow through the dungeon. Reveal opponent's hand and top 3 of their deck. Favored Enemy (choose Race when played): +4 ATK against that Race this dungeon. If another Drow or Elf in play: +2 INIT",
                race: Race.Drow, allyClass: AllyClass.Ranger,
                effectTags: "ON_PLAY|REVEAL_HAND|REVEAL_DECK:3\nON_PLAY|FAVORED_ENEMY:CHOOSE\nWITH_ADVANTAGE|IF_RACE:FAVORED|+ATK:4\nPASSIVE|IF_CLASS:Ranger|+INIT:2"),

            // ── RARE ──
            // Cattie-brie: Ranger, Human — Cost 3, Rare Budget = 43
            // ATK 8((8-1)*3=21) + HP 3((3-1)*3=6) + INIT 3(3*1.5=4.5) + Treasure 1(-1) = 30.5. Effect = 12.5
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000005"), "Cattie-brie", Rarity.Rare, 3, 8, 3, 3,
                isAmbusher: false, treasure: 1,
                effect: "Favored Enemy (Goblin): +3 ATK against Goblins. Reveal top 2 cards of opponent's deck at the start of combat. If a Ranger ally is in play: +1 INIT.",
                race: Race.Human, allyClass: AllyClass.Ranger,
                effectTags: "PASSIVE|FAVORED_ENEMY:Goblin|+ATK:3\nON_COMBAT_START|REVEAL_DECK:2\nPASSIVE|IF_CLASS:Ranger|+INIT:1"),

            // ── UNCOMMON ──
            // Elven Ranger: Ranger, Elf — Cost 3, Uncommon Budget = 37
            // ATK 8((8-1)*3=21) + HP 3((3-1)*3=6) + INIT 3(3*1.5=4.5) + Treasure 1(-1) = 30.5. Effect = 6.5
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000012"), "Elven Ranger", Rarity.Uncommon, 3, 8, 3, 3,
                isAmbusher: false, treasure: 1,
                effect: "Favored Enemy (Orc): +3 ATK against Orcs. Reveal the next Trap before it triggers.",
                race: Race.Elf, allyClass: AllyClass.Ranger,
                effectTags: "PASSIVE|FAVORED_ENEMY:Orc|+ATK:3\nPASSIVE|REVEAL_DECK:1"),

            // Changeling Spy: Ranger, Human — Cost 2, Uncommon Budget = 32
            // ATK 7((7-1)*3=18) + HP 2((2-1)*3=3) + INIT 2(2*1.5=3) + Ambusher(2) + Treasure 0(0) = 26. Effect = 6
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000025"), "Changeling Spy", Rarity.Uncommon, 2, 7, 2, 2,
                isAmbusher: true, treasure: 0,
                effect: "Reveal opponent's hand at the start of combat. If a Trap is in the current room: all allies gain +1 INIT.",
                race: Race.Human, allyClass: AllyClass.Ranger,
                effectTags: "ON_COMBAT_START|REVEAL_HAND\nPASSIVE|+INIT:1"),

            // ── COMMON ──
            // Scout Tracker: Ranger, Human — Cost 2, Common Budget = 29
            // ATK 7((7-1)*3=18) + HP 3((3-1)*3=6) + INIT 2(2*1.5=3) + Treasure 1(-1) = 26. Effect = 3
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000036"), "Scout Tracker", Rarity.Common, 2, 7, 3, 2,
                isAmbusher: false, treasure: 1,
                effect: "Favored Enemy (Beast): +2 ATK against Beasts.",
                race: Race.Human, allyClass: AllyClass.Ranger,
                effectTags: "PASSIVE|FAVORED_ENEMY:Beast|+ATK:2"),

            // ═══════════════════════════════════════════════════════════════
            //  PALADIN (6): 1 Unique, 1 Rare, 2 Uncommon, 2 Common
            // ═══════════════════════════════════════════════════════════════

            // ── UNIQUE ──
            // Aldric the Oathbound: Paladin, Human — Cost 6, Unique Budget = 87
            // ATK 10((10-1)*3=27) + HP 5((5-1)*3=12) + INIT 3(3*1.5=4.5) + Treasure 1(-1) = 42.5. Effect = 44.5
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000064"), "Aldric the Oathbound", Rarity.Unique, 6, 10, 5, 3,
                isAmbusher: false, treasure: 1,
                effect: "His oath burns brighter than any flame. Sacrifice (exile this ally): eliminate one enemy with HP <= 10 and restore 3 HP to all other allies. +3 ATK against Undead. Exile 8 cards from deck: all allies gain +2 ATK for the rest of the room",
                race: Race.Human, allyClass: AllyClass.Paladin,
                effectTags: "ON_ACTIVATE|COST:EXILE_HAND:99|DAMAGE:10:ENEMY|HEAL:3:ALL_ALLIES\nPASSIVE|IF_RACE:Undead|+ATK:3\nON_ACTIVATE|COST:EXILE_DECK:8|+ATK:2"),

            // ── RARE ──
            // Wulfgar Son of Beornegar: Paladin, Human — Cost 4, Rare Budget = 53
            // ATK 9((9-1)*3=24) + HP 4((4-1)*3=9) + INIT 4(4*1.5=6) + Treasure 0(0) = 39. Effect = 14
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000004"), "Wulfgar Son of Beornegar", Rarity.Rare, 4, 9, 4, 4,
                isAmbusher: false, treasure: 0,
                effect: "Sacrifice 3 HP: exile one enemy card from opponent's discard pile. If a Cleric ally is in play: +3 ATK.",
                race: Race.Human, allyClass: AllyClass.Paladin,
                effectTags: "ON_ACTIVATE|COST:HP:3|EXILE_HAND:1\nPASSIVE|IF_CLASS:Cleric|+ATK:3"),

            // ── UNCOMMON ──
            // Human Paladin: Paladin, Human — Cost 3, Uncommon Budget = 37
            // ATK 8((8-1)*3=21) + HP 3((3-1)*3=6) + INIT 3(3*1.5=4.5) + Treasure 0(0) = 31.5. Effect = 5.5
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000011"), "Human Paladin", Rarity.Uncommon, 3, 8, 3, 3,
                isAmbusher: false, treasure: 0,
                effect: "Sacrifice 2 HP: restore 3 HP to another ally. Against Undead: +2 ATK.",
                race: Race.Human, allyClass: AllyClass.Paladin,
                effectTags: "ON_ACTIVATE|COST:HP:2|HEAL:3:ALLY\nPASSIVE|FAVORED_ENEMY:Undead|+ATK:2"),

            // Tortle Guardian: Paladin, Beast — Cost 3, Uncommon Budget = 37
            // ATK 8((8-1)*3=21) + HP 3((3-1)*3=6) + INIT 3(3*1.5=4.5) + Treasure 1(-1) = 30.5. Effect = 6.5
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000024"), "Tortle Guardian", Rarity.Uncommon, 3, 8, 3, 3,
                isAmbusher: false, treasure: 1,
                effect: "Sacrifice 1 HP: redirect all damage from one ally to this ally for the combat.",
                race: Race.Beast, allyClass: AllyClass.Paladin,
                effectTags: "ON_ACTIVATE|COST:HP:1|REDIRECT_DAMAGE:ALLY"),

            // ── COMMON ──
            // Squire of the Order: Paladin, Human — Cost 1, Common Budget = 15
            // ATK 4((4-1)*3=9) + HP 2((2-1)*3=3) + INIT 1(1*1.5=1.5) + Treasure 0(0) = 13.5. Effect = 1.5
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000035"), "Squire of the Order", Rarity.Common, 1, 4, 2, 1,
                isAmbusher: false, treasure: 0,
                effect: "If a Paladin ally is in play: +1 ATK.",
                race: Race.Human, allyClass: AllyClass.Paladin,
                effectTags: "PASSIVE|IF_CLASS:Paladin|+ATK:1"),

            // Cavalry Rider: Paladin, Human — Cost 2, Common Budget = 29
            // ATK 7((7-1)*3=18) + HP 3((3-1)*3=6) + INIT 2(2*1.5=3) + Treasure 0(0) = 27. Effect = 2
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000053"), "Cavalry Rider", Rarity.Common, 2, 7, 3, 2,
                isAmbusher: false, treasure: 0,
                effect: "Sacrifice 1 HP: +2 ATK this combat.",
                race: Race.Human, allyClass: AllyClass.Paladin,
                effectTags: "ON_ACTIVATE|COST:HP:1|+ATK:2"),

            // ═══════════════════════════════════════════════════════════════
            //  BARD (5): 1 Unique, 1 Rare, 2 Uncommon, 1 Common
            //  Theme: Card recycling — return cards from hand to deck
            // ═══════════════════════════════════════════════════════════════

            // ── UNIQUE ──
            // Volo the Chronicler: Bard, Human — Cost 6, Unique Budget = 87
            // ATK 10((10-1)*3=27) + HP 5((5-1)*3=12) + INIT 4(4*1.5=6) + Treasure 1(-1) = 44. Effect = 43
            new AllyCard(new Guid("00000000-0000-0000-0008-000000000001"),
                "Volo the Chronicler", Rarity.Unique, 6,
                10, 5, 4, treasure: 1,
                effect: "The pen that rewrites fate. Return up to 3 cards from hand to the bottom of the deck, then draw 2 cards. Exile 8 cards from deck: look at opponent's hand and return 1 of their cards to top of their deck. All Bard allies gain +1 INIT",
                race: Race.Human, allyClass: AllyClass.Bard,
                effectTags: "ON_ACTIVATE|RETURN_HAND_BOTTOM:3|DRAW:2\nON_ACTIVATE|COST:EXILE_DECK:8|REVEAL_HAND\nPASSIVE|IF_CLASS:Bard|+INIT:1"),

            // ── RARE ──
            // Danilo Thann: Bard, Human — Cost 4, Rare Budget = 53
            // ATK 9((9-1)*3=24) + HP 4((4-1)*3=9) + INIT 4(4*1.5=6) + Treasure 1(-1) = 38. Effect = 15
            new AllyCard(new Guid("00000000-0000-0000-0008-000000000002"),
                "Danilo Thann", Rarity.Rare, 4,
                9, 4, 4, treasure: 1,
                effect: "Return up to 2 cards from hand to bottom of deck, then draw 2 cards. If a Bard is already in play: draw 3 instead",
                race: Race.Human, allyClass: AllyClass.Bard,
                effectTags: "ON_ACTIVATE|RETURN_HAND_BOTTOM:2|DRAW:2\nON_ACTIVATE|IF_CLASS:Bard|DRAW:3"),

            // ── UNCOMMON ──
            // Half-Elf Minstrel: Bard, Elf — Cost 3, Uncommon Budget = 37
            // ATK 8((8-1)*3=21) + HP 3((3-1)*3=6) + INIT 3(3*1.5=4.5) + Treasure 1(-1) = 30.5. Effect = 6.5
            new AllyCard(new Guid("00000000-0000-0000-0008-000000000004"),
                "Half-Elf Minstrel", Rarity.Uncommon, 3,
                8, 3, 3, treasure: 1,
                effect: "Return 1 card from hand to top of deck, then draw 1 card. With Advantage: return 2 and draw 2 instead",
                race: Race.Elf, allyClass: AllyClass.Bard,
                effectTags: "ON_ACTIVATE|RETURN_HAND_TOP:1|DRAW:1\nWITH_ADVANTAGE|RETURN_HAND_TOP:2|DRAW:2"),

            // Tiefling Vocalist: Bard, Tiefling — Cost 2, Uncommon Budget = 32
            // ATK 7((7-1)*3=18) + HP 2((2-1)*3=3) + INIT 2(2*1.5=3) + Treasure 0(0) = 24. Effect = 8
            new AllyCard(new Guid("00000000-0000-0000-0008-000000000005"),
                "Tiefling Vocalist", Rarity.Uncommon, 2,
                7, 2, 2, treasure: 0,
                effect: "Return 1 card from hand to bottom of deck, then draw 1 card. All allies gain +1 ATK this round",
                race: Race.Tiefling, allyClass: AllyClass.Bard,
                effectTags: "ON_ACTIVATE|RETURN_HAND_BOTTOM:1|DRAW:1|+ATK:1"),

            // ── COMMON ──
            // Halfling Storyteller: Bard, Halfling — Cost 2, Common Budget = 29
            // ATK 7((7-1)*3=18) + HP 2((2-1)*3=3) + INIT 2(2*1.5=3) + Treasure 1(-1) = 23. Effect = 6 (note: old budget was lower; now correct)
            new AllyCard(new Guid("00000000-0000-0000-0008-000000000008"),
                "Halfling Storyteller", Rarity.Common, 2,
                7, 2, 2, treasure: 1,
                effect: "Return 1 card from hand to top of deck. Draw 1 card",
                race: Race.Halfling, allyClass: AllyClass.Bard,
                effectTags: "ON_ACTIVATE|RETURN_HAND_TOP:1|DRAW:1"),

            // ═══════════════════════════════════════════════════════════════
            //  MONK (5): 1 Unique, 1 Rare, 2 Uncommon, 1 Common
            //  Theme: Discard/exile from hand to cancel combat, disengage
            // ═══════════════════════════════════════════════════════════════

            // ── UNIQUE ──
            // Kane, the Unbroken Circle: Monk, Human — Cost 6, Unique Budget = 87
            // ATK 10((10-1)*3=27) + HP 5((5-1)*3=12) + INIT 3(3*1.5=4.5) + Treasure 1(-1) = 42.5. Effect = 44.5
            new AllyCard(new Guid("00000000-0000-0000-0009-000000000001"),
                "Kane, the Unbroken Circle", Rarity.Unique, 6,
                10, 5, 3, treasure: 1,
                effect: "The immovable sentinel who holds the line so others may strike. Exile 3 cards from hand: cancel ALL damage in this combat group this round (both sides deal 0). While Kane holds, allies in other combat groups gain +1 ATK. This hold can be maintained each round by paying the cost again",
                race: Race.Human, allyClass: AllyClass.Monk,
                effectTags: "ON_ACTIVATE|COST:EXILE_HAND:3|CANCEL_COMBAT\nPASSIVE|+ATK:1"),

            // ── RARE ──
            // Shadow Monk: Monk, Human — Cost 4, Rare Budget = 53
            // ATK 9((9-1)*3=24) + HP 4((4-1)*3=9) + INIT 4(4*1.5=6) + Treasure 1(-1) = 38. Effect = 15
            new AllyCard(new Guid("00000000-0000-0000-0009-000000000002"),
                "Shadow Monk", Rarity.Rare, 4,
                9, 4, 4, treasure: 1,
                effect: "Discard 1 from hand: cancel one combat this ally is in. Exile 1 from hand: this ally cannot be targeted until next round. With Advantage: +2 ATK",
                race: Race.Human, allyClass: AllyClass.Monk,
                effectTags: "ON_ACTIVATE|COST:DISCARD_HAND:1|CANCEL_COMBAT\nON_ACTIVATE|COST:EXILE_HAND:1|REDUCE_DAMAGE:99\nWITH_ADVANTAGE|+ATK:2"),

            // ── UNCOMMON ──
            // Open Hand Adept: Monk, Human — Cost 3, Uncommon Budget = 37
            // ATK 8((8-1)*3=21) + HP 3((3-1)*3=6) + INIT 3(3*1.5=4.5) + Treasure 0(0) = 31.5. Effect = 5.5
            new AllyCard(new Guid("00000000-0000-0000-0009-000000000004"),
                "Open Hand Adept", Rarity.Uncommon, 3,
                8, 3, 3, treasure: 0,
                effect: "Discard 1 from hand: cancel one combat this ally is in. With Disadvantage: +2 HP",
                race: Race.Human, allyClass: AllyClass.Monk,
                effectTags: "ON_ACTIVATE|COST:DISCARD_HAND:1|CANCEL_COMBAT\nWITH_DISADVANTAGE|+HP:2"),

            // Drunken Fist Brawler: Monk, Halfling — Cost 2, Uncommon Budget = 32
            // ATK 7((7-1)*3=18) + HP 2((2-1)*3=3) + INIT 2(2*1.5=3) + Treasure 0(0) = 24. Effect = 8
            new AllyCard(new Guid("00000000-0000-0000-0009-000000000005"),
                "Drunken Fist Brawler", Rarity.Uncommon, 2,
                7, 2, 2, treasure: 0,
                effect: "Discard 1 from hand: disengage without triggering opportunity attack. +1 ATK against enemies with higher INIT",
                race: Race.Halfling, allyClass: AllyClass.Monk,
                effectTags: "ON_ACTIVATE|COST:DISCARD_HAND:1|IGNORE_OPP_ATTACK_LIMIT\nPASSIVE|+ATK:1"),

            // ── COMMON ──
            // Temple Initiate: Monk, Human — Cost 1, Common Budget = 15
            // ATK 4((4-1)*3=9) + HP 2((2-1)*3=3) + INIT 1(1*1.5=1.5) + Treasure 0(0) = 13.5. Effect = 1.5
            new AllyCard(new Guid("00000000-0000-0000-0009-000000000007"),
                "Temple Initiate", Rarity.Common, 1,
                4, 2, 1, treasure: 0,
                effect: "Discard 1 from hand: +1 ATK this round",
                race: Race.Human, allyClass: AllyClass.Monk,
                effectTags: "ON_ACTIVATE|COST:DISCARD_HAND:1|+ATK:1"),

            // ═══════════════════════════════════════════════════════════════
            //  WARLOCK (5): 1 Unique, 1 Rare, 2 Uncommon, 1 Common
            //  Theme: Mark enemy during initiative; if marked dies, trigger effects
            // ═══════════════════════════════════════════════════════════════

            // ── UNIQUE ──
            // Magus, the Tormented: Warlock, Drow — Cost 7, Unique Budget = 99
            // ATK 10((10-1)*3=27) + HP 5((5-1)*3=12) + INIT 3(3*1.5=4.5) + Treasure 4(-4) = 39.5. Effect = 59.5
            // Treasure 4 = huge downside: opponent recovers 4 cards when Magus dies
            // Cost 7 = massive deck pressure: discard 7 cards just to play him
            new AllyCard(new Guid("00000000-0000-0000-000A-000000000001"),
                "Magus, the Tormented", Rarity.Unique, 7,
                10, 5, 3, treasure: 4,
                effect: "A Drow whose pact carved runes of suffering into his soul. During initiative: Magus fixes his gaze on one enemy — marking it with dread. If the marked enemy falls, terror ripples through the battlefield: every surviving enemy in that combat group panics and attempts to flee, triggering opportunity attacks from all adjacent allies. This dread can only be unleashed once per combat. If the marked enemy survives, the pact turns inward — exile 1 card from your own deck as the torment feeds",
                race: Race.Drow, allyClass: AllyClass.Warlock,
                effectTags: "ON_INITIATIVE|MARK_ENEMY:1\nON_MARKED_KILL|TRIGGER_OPP_ATTACK:ENEMY_GROUP|ONCE_PER_COMBAT\nON_MARKED_SURVIVE|EXILE_DECK:1"),

            // ── RARE ──
            // Hexblade Champion: Warlock, Human — Cost 4, Rare Budget = 53
            // ATK 9((9-1)*3=24) + HP 4((4-1)*3=9) + INIT 4(4*1.5=6) + Treasure 1(-1) = 38. Effect = 15
            new AllyCard(new Guid("00000000-0000-0000-000A-000000000002"),
                "Hexblade Champion", Rarity.Rare, 4,
                9, 4, 4, treasure: 1,
                effect: "During initiative: mark 1 enemy. If marked enemy is eliminated: +3 ATK until end of room and draw 2 cards. This Warlock's weapon deals +1 damage to marked target",
                race: Race.Human, allyClass: AllyClass.Warlock,
                effectTags: "ON_INITIATIVE|MARK_ENEMY:1\nON_MARKED_KILL|+ATK:3|DRAW:2\nPASSIVE|DAMAGE:1:MARKED"),

            // ── UNCOMMON ──
            // Fiend Pact Adept: Warlock, Tiefling — Cost 3, Uncommon Budget = 37
            // ATK 8((8-1)*3=21) + HP 3((3-1)*3=6) + INIT 3(3*1.5=4.5) + Treasure 1(-1) = 30.5. Effect = 6.5
            new AllyCard(new Guid("00000000-0000-0000-000A-000000000004"),
                "Fiend Pact Adept", Rarity.Uncommon, 3,
                8, 3, 3, treasure: 1,
                effect: "During initiative: mark 1 enemy. If marked enemy is eliminated: deal 2 damage to all enemies in same combat group",
                race: Race.Tiefling, allyClass: AllyClass.Warlock,
                effectTags: "ON_INITIATIVE|MARK_ENEMY:1\nON_MARKED_KILL|DAMAGE:2:ENEMY_GROUP"),

            // Great Old One Cultist: Warlock, Human — Cost 2, Uncommon Budget = 32
            // ATK 7((7-1)*3=18) + HP 2((2-1)*3=3) + INIT 2(2*1.5=3) + Treasure 1(-1) = 23. Effect = 9
            new AllyCard(new Guid("00000000-0000-0000-000A-000000000005"),
                "Great Old One Cultist", Rarity.Uncommon, 2,
                7, 2, 2, treasure: 1,
                effect: "During initiative: mark 1 enemy. If marked enemy is eliminated: exile 1 card from opponent's deck",
                race: Race.Human, allyClass: AllyClass.Warlock,
                effectTags: "ON_INITIATIVE|MARK_ENEMY:1\nON_MARKED_KILL|EXILE_DECK:1"),

            // ── COMMON ──
            // Pact Initiate: Warlock, Human — Cost 1, Common Budget = 15
            // ATK 4((4-1)*3=9) + HP 2((2-1)*3=3) + INIT 1(1*1.5=1.5) + Treasure 0(0) = 13.5. Effect = 1.5
            new AllyCard(new Guid("00000000-0000-0000-000A-000000000007"),
                "Pact Initiate", Rarity.Common, 1,
                4, 2, 1, treasure: 0,
                effect: "During initiative: mark 1 enemy. If marked enemy is eliminated: draw 1 card",
                race: Race.Human, allyClass: AllyClass.Warlock,
                effectTags: "ON_INITIATIVE|MARK_ENEMY:1\nON_MARKED_KILL|DRAW:1"),

            // ═══════════════════════════════════════════════════════════════
            //  SORCERER (5): 1 Unique, 1 Rare, 2 Uncommon, 1 Common
            //  Theme: Amplify, copy, and recover Scrolls
            // ═══════════════════════════════════════════════════════════════

            // ── UNIQUE ──
            // Archmage of the Weave: Sorcerer, Human — Cost 6, Unique Budget = 87
            // ATK 10((10-1)*3=27) + HP 5((5-1)*3=12) + INIT 4(4*1.5=6) + Treasure 1(-1) = 44. Effect = 43
            new AllyCard(new Guid("00000000-0000-0000-000B-000000000001"),
                "Archmage of the Weave", Rarity.Unique, 6,
                10, 5, 4, treasure: 1,
                effect: "When a Scroll is used by any ally: copy its effect (apply it a second time for free). Discard 2 from hand: retrieve 1 Scroll from exile to hand. All Scrolls used by allies cost 1 less",
                race: Race.Human, allyClass: AllyClass.Sorcerer,
                effectTags: "ON_SCROLL_USED|COPY_SCROLL\nON_ACTIVATE|COST:DISCARD_HAND:2|RECOVER_SCROLL:EXILE"),

            // ── RARE ──
            // Wild Magic Sorcerer: Sorcerer, Human — Cost 4, Rare Budget = 53
            // ATK 9((9-1)*3=24) + HP 4((4-1)*3=9) + INIT 4(4*1.5=6) + Treasure 1(-1) = 38. Effect = 15
            new AllyCard(new Guid("00000000-0000-0000-000B-000000000002"),
                "Wild Magic Sorcerer", Rarity.Rare, 4,
                9, 4, 4, treasure: 1,
                effect: "When a Scroll is used: reveal top card of deck. If it's an ally: +2 ATK to all allies. If monster/trap: opponent draws 1 fewer card next room. Discard 1: recover 1 Scroll from discard to hand",
                race: Race.Human, allyClass: AllyClass.Sorcerer,
                effectTags: "ON_SCROLL_USED|REVEAL_DECK:1|+ATK:2\nON_ACTIVATE|COST:DISCARD_HAND:1|RECOVER_SCROLL:DISCARD"),

            // ── UNCOMMON ──
            // Storm Sorcerer: Sorcerer, Human — Cost 3, Uncommon Budget = 37
            // ATK 8((8-1)*3=21) + HP 3((3-1)*3=6) + INIT 3(3*1.5=4.5) + Treasure 1(-1) = 30.5. Effect = 6.5
            new AllyCard(new Guid("00000000-0000-0000-000B-000000000004"),
                "Storm Sorcerer", Rarity.Uncommon, 3,
                8, 3, 3, treasure: 1,
                effect: "When a Scroll is used by this ally: deal 1 damage to all enemies in this combat group. Discard 1: recover 1 Scroll from discard to hand",
                race: Race.Human, allyClass: AllyClass.Sorcerer,
                effectTags: "ON_SCROLL_USED|DAMAGE:1:ENEMY_GROUP\nON_ACTIVATE|COST:DISCARD_HAND:1|RECOVER_SCROLL:DISCARD"),

            // Shadow Sorcerer: Sorcerer, Human — Cost 2, Uncommon Budget = 32
            // ATK 7((7-1)*3=18) + HP 2((2-1)*3=3) + INIT 2(2*1.5=3) + Treasure 1(-1) = 23. Effect = 9
            new AllyCard(new Guid("00000000-0000-0000-000B-000000000005"),
                "Shadow Sorcerer", Rarity.Uncommon, 2,
                7, 2, 2, treasure: 1,
                effect: "Exile 1 from hand: copy the last Scroll used this round (apply its effect again)",
                race: Race.Human, allyClass: AllyClass.Sorcerer,
                effectTags: "ON_ACTIVATE|COST:EXILE_HAND:1|COPY_SCROLL"),

            // ── COMMON ──
            // Arcane Prodigy: Sorcerer, Elf — Cost 2, Common Budget = 29
            // ATK 7((7-1)*3=18) + HP 2((2-1)*3=3) + INIT 2(2*1.5=3) + Treasure 1(-1) = 23. Effect = 6
            new AllyCard(new Guid("00000000-0000-0000-000B-000000000008"),
                "Arcane Prodigy", Rarity.Common, 2,
                7, 2, 2, treasure: 1,
                effect: "When a Scroll is used by this ally: +1 ATK this round",
                race: Race.Elf, allyClass: AllyClass.Sorcerer,
                effectTags: "ON_SCROLL_USED|+ATK:1"),

            // ═══════════════════════════════════════════════════════════════
            //  BARBARIAN (6): 1 Unique, 1 Rare, 2 Uncommon, 2 Common
            //  Theme: Highest ATK/HP, Rage via exile for +ATK, damage reduction
            // ═══════════════════════════════════════════════════════════════

            // ── UNIQUE ──
            // Uthgar, Blood of the Ancestors: Barbarian, Human — Cost 6, Unique Budget = 87
            // ATK 10((10-1)*3=27) + HP 5((5-1)*3=12) + INIT 0(0*1.5=0) + Treasure 1(-1) = 38. Effect = 49
            new AllyCard(new Guid("00000000-0000-0000-000C-000000000001"),
                "Uthgar, Blood of the Ancestors", Rarity.Unique, 6,
                10, 5, 0, treasure: 1,
                effect: "Born from a bloodline that predates the gods. Exile 4 cards from top of deck once per combat: +2 ATK until end of this combat and Uthgar's ATK counts as double for elimination checks. If Uthgar eliminates a monster while raging, he can join another fight immediately. While raging, Uthgar forfeits all Treasure from eliminated enemies",
                effectTags: "ON_ACTIVATE|COST:EXILE_DECK:4|+ATK:2|ELIM_DOUBLE|ONCE_PER_COMBAT\nON_KILL|IF_RAGING|JOIN_COMBAT|FORFEIT_TREASURE",
                race: Race.Human, allyClass: AllyClass.Barbarian),

            // ── RARE ──
            // Frostborn Berserker: Barbarian, Human — Cost 4, Rare Budget = 53
            // ATK 10((10-1)*3=27) + HP 4((4-1)*3=9) + INIT 3(3*1.5=4.5) + Treasure 0(0) = 40.5. Effect = 12.5
            new AllyCard(new Guid("00000000-0000-0000-000C-000000000002"),
                "Frostborn Berserker", Rarity.Rare, 4,
                10, 4, 3, treasure: 0,
                effect: "Exile 1 from hand: enter Rage (+2 ATK, take 1 less damage). While raging, ATK counts as double for elimination checks against enemies with lower HP than this ally",
                race: Race.Human, allyClass: AllyClass.Barbarian,
                effectTags: "ON_ACTIVATE|COST:EXILE_HAND:1|+ATK:2|REDUCE_DAMAGE:1\nIF_RAGING|ELIM_DOUBLE"),

            // ── UNCOMMON ──
            // Half-Orc Rager: Barbarian, Orc — Cost 3, Uncommon Budget = 37
            // ATK 8((8-1)*3=21) + HP 3((3-1)*3=6) + INIT 3(3*1.5=4.5) + Treasure 0(0) = 31.5. Effect = 5.5
            new AllyCard(new Guid("00000000-0000-0000-000C-000000000004"),
                "Half-Orc Rager", Rarity.Uncommon, 3,
                8, 3, 3, treasure: 0,
                effect: "Exile 1 from hand: +2 ATK and take 1 less damage this round. When reduced to 0 HP while raging: survive with 1 HP instead",
                race: Race.Orc, allyClass: AllyClass.Barbarian,
                effectTags: "ON_ACTIVATE|COST:EXILE_HAND:1|+ATK:2|REDUCE_DAMAGE:1\nON_DEATH|IF_RAGING|+HP:1"),

            // Tribal Fury: Barbarian, Human — Cost 2, Uncommon Budget = 32
            // ATK 7((7-1)*3=18) + HP 2((2-1)*3=3) + INIT 2(2*1.5=3) + Treasure 0(0) = 24. Effect = 8
            new AllyCard(new Guid("00000000-0000-0000-000C-000000000005"),
                "Tribal Fury", Rarity.Uncommon, 2,
                7, 2, 2, treasure: 0,
                effect: "Exile 1 from hand: +2 ATK this round. With Disadvantage: +1 HP",
                race: Race.Human, allyClass: AllyClass.Barbarian,
                effectTags: "ON_ACTIVATE|COST:EXILE_HAND:1|+ATK:2\nWITH_DISADVANTAGE|+HP:1"),

            // ── COMMON ──
            // Reckless Brute: Barbarian, Orc — Cost 1, Common Budget = 15
            // ATK 4((4-1)*3=9) + HP 2((2-1)*3=3) + INIT 1(1*1.5=1.5) + Treasure 0(0) = 13.5. Effect = 1.5
            new AllyCard(new Guid("00000000-0000-0000-000C-000000000007"),
                "Reckless Brute", Rarity.Common, 1,
                4, 2, 1, treasure: 0,
                effect: "Exile 1 from hand: +1 ATK this round",
                race: Race.Orc, allyClass: AllyClass.Barbarian,
                effectTags: "ON_ACTIVATE|COST:EXILE_HAND:1|+ATK:1"),

            // Pit Brawler: Barbarian, Human — Cost 2, Common Budget = 29
            // ATK 7((7-1)*3=18) + HP 3((3-1)*3=6) + INIT 2(2*1.5=3) + Treasure 1(-1) = 26. Effect = 3
            new AllyCard(new Guid("00000000-0000-0000-000C-000000000008"),
                "Pit Brawler", Rarity.Common, 2,
                7, 3, 2, treasure: 1,
                effect: "Exile 1 from hand: +2 ATK this round",
                race: Race.Human, allyClass: AllyClass.Barbarian,
                effectTags: "ON_ACTIVATE|COST:EXILE_HAND:1|+ATK:2"),

            // ═══════════════════════════════════════════════════════════════
            //  DRUID — Adaptation, recovery, cost reduction
            // ═══════════════════════════════════════════════════════════════

            // Silvanus' Chosen: Druid, Elf — Cost 6, Unique Budget = 87
            // ATK 10((10-1)*3=27) + HP 5((5-1)*3=12) + INIT 4(4*1.5=6) + Treasure 1(-1) = 44. Effect = 43
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000071"),
                "Silvanus' Chosen", Rarity.Unique, 6, 10, 5, 4, treasure: 1,
                effect: "Voice of the wild that bends nature to her will. Reshuffle your hand into deck and draw the same number of cards. Recover up to 2 cards from exile to discard pile. When an ally dies or a room advances: next card played costs 2 less. Companions gain +1 ATK",
                race: Race.Elf, allyClass: AllyClass.Druid,
                effectTags: "ON_ACTIVATE|RESHUFFLE_HAND_REDRAW\nON_ACTIVATE|RECOVER_FROM_EXILE:2\nON_ALLY_DEATH|REDUCE_NEXT_COST:2\nON_ROOM_ADVANCE|REDUCE_NEXT_COST:2\nPASSIVE|IF_CLASS:Companion|+ATK:1"),

            // Circle of the Land Druid: Druid, Human — Cost 3, Rare Budget = 43
            // ATK 8((8-1)*3=21) + HP 3((3-1)*3=6) + INIT 3(3*1.5=4.5) + Treasure 1(-1) = 30.5. Effect = 12.5
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000072"),
                "Circle of the Land Druid", Rarity.Rare, 3, 8, 3, 3, treasure: 1,
                effect: "Reshuffle hand into deck and draw the same number. When trap triggers: next card costs 1 less. +1 HP to all Companions",
                race: Race.Human, allyClass: AllyClass.Druid,
                effectTags: "ON_ACTIVATE|RESHUFFLE_HAND_REDRAW\nON_TRAP_TRIGGER|REDUCE_NEXT_COST:1\nPASSIVE|IF_CLASS:Companion|+HP:1"),

            // Wildshape Initiate: Druid, Human — Cost 2, Uncommon Budget = 32
            // ATK 7((7-1)*3=18) + HP 2((2-1)*3=3) + INIT 2(2*1.5=3) + Treasure 0(0) = 24. Effect = 8
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000073"),
                "Wildshape Initiate", Rarity.Uncommon, 2, 7, 2, 2,
                effect: "Recover 1 card from exile to discard pile. When an ally dies: draw 1 card",
                race: Race.Human, allyClass: AllyClass.Druid,
                effectTags: "ON_ACTIVATE|RECOVER_FROM_EXILE:1\nON_ALLY_DEATH|DRAW:1"),

            // Moon Druid: Druid, Elf — Cost 3, Uncommon Budget = 37
            // ATK 8((8-1)*3=21) + HP 3((3-1)*3=6) + INIT 3(3*1.5=4.5) + Treasure 0(0) = 31.5. Effect = 5.5
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000074"),
                "Moon Druid", Rarity.Uncommon, 3, 8, 3, 3,
                effect: "Reshuffle hand into deck and draw same number. With Disadvantage: +2 HP. Companions gain +1 INIT",
                race: Race.Elf, allyClass: AllyClass.Druid,
                effectTags: "ON_ACTIVATE|RESHUFFLE_HAND_REDRAW\nWITH_DISADVANTAGE|+HP:2\nPASSIVE|IF_CLASS:Companion|+INIT:1"),

            // Nature Apprentice: Druid, Halfling — Cost 1, Common Budget = 15
            // ATK 4((4-1)*3=9) + HP 2((2-1)*3=3) + INIT 1(1*1.5=1.5) + Treasure 0(0) = 13.5. Effect = 1.5
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000075"),
                "Nature Apprentice", Rarity.Common, 1, 4, 2, 1,
                effect: "When a room advances: draw 1 card",
                race: Race.Halfling, allyClass: AllyClass.Druid,
                effectTags: "ON_ROOM_ADVANCE|DRAW:1"),
        };
    }
}
