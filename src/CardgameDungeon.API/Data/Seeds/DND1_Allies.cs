using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.API.Data.Seeds;

public static partial class CardSetSeeder
{
    private static List<AllyCard> CreateAllies()
    {
        return new List<AllyCard>
        {
            // ===== UNIQUE (2) =====
            // Drizzt: Rogue, Elf — cost 5, stats 7/7/7=21, ratio 4.2
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000001"), "Drizzt Do'Urden", Rarity.Unique, 5, 7, 7, 7,
                isAmbusher: true, treasure: 3,
                effect: "With Advantage: +3 STR. If another Elf ally in play: +2 INIT.",
                race: Race.Elf, allyClass: AllyClass.Rogue),

            // Elminster: Mage, Human — cost 5, stats 5/8/5=18, ratio 3.6 but massive effect
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000002"), "Elminster Aumar", Rarity.Unique, 5, 5, 8, 5,
                isAmbusher: false, treasure: 3,
                effect: "All allies gain +1 HP per room. Once per dungeon: exile top 3 cards from opponent's deck.",
                race: Race.Human, allyClass: AllyClass.Mage),

            // ===== RARE (8) =====
            // Bruenor: Warrior, Dwarf — cost 4, stats 6/7/4=17, ratio 4.25
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000003"), "Bruenor Battlehammer", Rarity.Rare, 4, 6, 7, 4,
                isAmbusher: false, treasure: 2,
                effect: "Reduces all incoming damage to allies in this combat group by 1. If another Dwarf ally in play: +2 HP.",
                race: Race.Dwarf, allyClass: AllyClass.Warrior),

            // Wulfgar: Paladin, Human — cost 4, stats 7/6/4=17, ratio 4.25
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000004"), "Wulfgar Son of Beornegar", Rarity.Rare, 4, 7, 6, 4,
                isAmbusher: false, treasure: 2,
                effect: "Sacrifice 2 HP: exile one enemy card from opponent's discard pile. If a Cleric ally is in play: +2 STR.",
                race: Race.Human, allyClass: AllyClass.Paladin),

            // Cattie-brie: Ranger, Human — cost 3, stats 5/5/6=16, ratio 5.33
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000005"), "Cattie-brie", Rarity.Rare, 3, 5, 5, 6,
                isAmbusher: false, treasure: 2,
                effect: "Favored Enemy (Goblin): +3 STR against Goblins. Reveal top 2 cards of opponent's deck at the start of combat.",
                race: Race.Human, allyClass: AllyClass.Ranger),

            // Regis: Rogue, Halfling — cost 3, stats 4/5/6=15, ratio 5.0
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000006"), "Regis the Halfling", Rarity.Rare, 3, 4, 5, 6,
                isAmbusher: true, treasure: 3,
                effect: "With Advantage: +2 STR and gain +1 treasure. Ambusher.",
                race: Race.Halfling, allyClass: AllyClass.Rogue),

            // Mordenkainen: Mage, Human — cost 4, stats 5/6/6=17, ratio 4.25
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000007"), "Mordenkainen", Rarity.Rare, 4, 5, 6, 6,
                isAmbusher: false, treasure: 2,
                effect: "All allies in this combat group gain +1 STR. If entering room 4 or 5: all allies also gain +1 INIT.",
                race: Race.Human, allyClass: AllyClass.Mage),

            // Tasha: Mage, Human — cost 4, stats 5/5/7=17, ratio 4.25
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000008"), "Tasha the Witch Queen", Rarity.Rare, 4, 5, 5, 7,
                isAmbusher: false, treasure: 2,
                effect: "Opponent discards 1 card at the start of combat. If a Scroll is equipped on any ally: opponent discards 2 instead.",
                race: Race.Human, allyClass: AllyClass.Mage),

            // Minsc: Warrior, Human — cost 4, stats 7/7/3=17, ratio 4.25
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000009"), "Minsc and Boo", Rarity.Rare, 4, 7, 7, 3,
                isAmbusher: false, treasure: 2,
                effect: "With Disadvantage: +3 STR. Traps in this room deal 2 less damage to all allies.",
                race: Race.Human, allyClass: AllyClass.Warrior),

            // Jalarloth: Cleric, Elf — cost 4, stats 4/6/6=16, ratio 4.0
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000010"), "Jalarloth Baenre", Rarity.Rare, 4, 4, 6, 6,
                isAmbusher: false, treasure: 2,
                effect: "Restore 2 HP to all allies at the start of combat. Against Undead enemies: all allies gain +2 STR.",
                race: Race.Elf, allyClass: AllyClass.Cleric),

            // ===== UNCOMMON (20) =====

            // --- Warrior Uncommon (4) ---
            // Dragonborn Fighter: Warrior, Dragonborn — cost 3, stats 5/6/3=14, ratio 4.67
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000017"), "Dragonborn Fighter", Rarity.Uncommon, 3, 5, 6, 3,
                isAmbusher: false, treasure: 1,
                effect: "With Disadvantage: +2 STR. If equipped with a Weapon: +1 STR.",
                race: Race.Dragonborn, allyClass: AllyClass.Warrior),

            // Half-Orc Barbarian: Warrior, Orc — cost 3, stats 6/6/2=14, ratio 4.67
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000018"), "Half-Orc Barbarian", Rarity.Uncommon, 3, 6, 6, 2,
                isAmbusher: false, treasure: 1,
                effect: "First time this ally would be reduced to 0 HP: survives with 1 HP instead. With Disadvantage: +2 STR.",
                race: Race.Orc, allyClass: AllyClass.Warrior),

            // Goliath Berserker: Warrior, Giant — cost 3, stats 5/6/3=14, ratio 4.67
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000021"), "Goliath Berserker", Rarity.Uncommon, 3, 5, 6, 3,
                isAmbusher: false, treasure: 1,
                effect: "Reduces damage from one attack per room by 3. When below half HP: +2 STR.",
                race: Race.Giant, allyClass: AllyClass.Warrior),

            // Hobgoblin Warlord: Warrior, Goblin — cost 3, stats 4/5/4=13, ratio 4.33
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000030"), "Hobgoblin Warlord", Rarity.Uncommon, 3, 4, 5, 4,
                isAmbusher: false, treasure: 1,
                effect: "All allies in this combat group gain +1 STR. One ally may Retarget without paying cost.",
                race: Race.Goblin, allyClass: AllyClass.Warrior),

            // --- Rogue Uncommon (3) ---
            // Halfling Rogue: Rogue, Halfling — cost 2, stats 4/4/5=13, ratio 6.5
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000014"), "Halfling Rogue", Rarity.Uncommon, 2, 4, 4, 5,
                isAmbusher: true, treasure: 2,
                effect: "With Advantage: +3 STR. Traps targeting this ally deal half damage.",
                race: Race.Halfling, allyClass: AllyClass.Rogue),

            // Tabaxi Swashbuckler: Rogue, Beast — cost 3, stats 5/4/5=14, ratio 4.67
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000022"), "Tabaxi Swashbuckler", Rarity.Uncommon, 3, 5, 4, 5,
                isAmbusher: true, treasure: 2,
                effect: "With Advantage: +2 INIT. Opportunity Attacks against this ally deal no damage.",
                race: Race.Beast, allyClass: AllyClass.Rogue),

            // Kenku Shadow: Rogue, Beast — cost 2, stats 3/4/5=12, ratio 6.0
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000023"), "Kenku Shadow", Rarity.Uncommon, 2, 3, 4, 5,
                isAmbusher: true, treasure: 1,
                effect: "With Advantage: +2 STR. If another Rogue ally in play: gains Ambusher if not already.",
                race: Race.Beast, allyClass: AllyClass.Rogue),

            // --- Mage Uncommon (4) ---
            // Gnome Artificer: Mage, Halfling — cost 3, stats 3/5/4=12, ratio 4.0
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000015"), "Gnome Artificer", Rarity.Uncommon, 3, 3, 5, 4,
                isAmbusher: false, treasure: 2,
                effect: "Equipped Accessories and Totems grant +1 additional STR to the holder. If a Bomb is used this combat: +2 STR to all allies.",
                race: Race.Halfling, allyClass: AllyClass.Mage),

            // Tiefling Warlock: Mage, Tiefling — cost 3, stats 5/5/4=14, ratio 4.67
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000016"), "Tiefling Warlock", Rarity.Uncommon, 3, 5, 5, 4,
                isAmbusher: false, treasure: 1,
                effect: "Exile 1 card from your discard: +3 STR this combat. Once per dungeon: exile top 2 cards from opponent's deck.",
                race: Race.Tiefling, allyClass: AllyClass.Mage),

            // Eladrin Bladesinger: Mage, Elf — cost 3, stats 4/4/5=13, ratio 4.33
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000026"), "Eladrin Bladesinger", Rarity.Uncommon, 3, 4, 4, 5,
                isAmbusher: false, treasure: 1,
                effect: "If equipped with a Weapon: +2 STR and +1 INIT. If another Elf ally in play: +1 HP.",
                race: Race.Elf, allyClass: AllyClass.Mage),

            // Genasi Stormcaller: Mage, Elemental — cost 3, stats 5/4/4=13, ratio 4.33
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000027"), "Genasi Stormcaller", Rarity.Uncommon, 3, 5, 4, 4,
                isAmbusher: false, treasure: 1,
                effect: "All enemies in this combat group lose 1 STR. If entering room 3 or later: enemies also lose 1 INIT.",
                race: Race.Elemental, allyClass: AllyClass.Mage),

            // --- Cleric Uncommon (3) ---
            // Dwarven Cleric: Cleric, Dwarf — cost 3, stats 4/6/2=12, ratio 4.0
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000013"), "Dwarven Cleric", Rarity.Uncommon, 3, 4, 6, 2,
                isAmbusher: false, treasure: 1,
                effect: "Restore 2 HP to the most injured ally at the start of combat. Against Undead: +2 STR.",
                race: Race.Dwarf, allyClass: AllyClass.Cleric),

            // Firbolg Druid: Cleric, Giant — cost 3, stats 4/5/3=12, ratio 4.0
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000020"), "Firbolg Druid", Rarity.Uncommon, 3, 4, 5, 3,
                isAmbusher: false, treasure: 1,
                effect: "Restore 1 HP to all allies at end of combat. If a Potion or Balm is used this combat: restore 1 additional HP to all allies.",
                race: Race.Giant, allyClass: AllyClass.Cleric),

            // Lizardfolk Shaman: Cleric, Beast — cost 3, stats 4/5/3=12, ratio 4.0
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000028"), "Lizardfolk Shaman", Rarity.Uncommon, 3, 4, 5, 3,
                isAmbusher: false, treasure: 1,
                effect: "Restore 1 HP to all allies at start of combat. If an ally is defeated this combat: gain +2 STR for the rest of the room.",
                race: Race.Beast, allyClass: AllyClass.Cleric),

            // --- Ranger Uncommon (3) ---
            // Elven Ranger: Ranger, Elf — cost 3, stats 4/4/5=13, ratio 4.33
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000012"), "Elven Ranger", Rarity.Uncommon, 3, 4, 4, 5,
                isAmbusher: false, treasure: 1,
                effect: "Favored Enemy (Orc): +3 STR against Orcs. Reveal the next Trap before it triggers.",
                race: Race.Elf, allyClass: AllyClass.Ranger),

            // Changeling Spy: Ranger, Human — cost 2, stats 3/4/5=12, ratio 6.0
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000025"), "Changeling Spy", Rarity.Uncommon, 2, 3, 4, 5,
                isAmbusher: true, treasure: 2,
                effect: "Reveal opponent's hand at the start of combat. If a Trap is in the current room: all allies gain +1 INIT.",
                race: Race.Human, allyClass: AllyClass.Ranger),

            // Aasimar Monk: Ranger, Human — cost 3, stats 4/4/5=13, ratio 4.33
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000019"), "Aasimar Monk", Rarity.Uncommon, 3, 4, 4, 5,
                isAmbusher: false, treasure: 1,
                effect: "Reveal top 3 cards of opponent's deck. Favored Enemy (Demon): +3 STR against Demons.",
                race: Race.Human, allyClass: AllyClass.Ranger),

            // --- Paladin Uncommon (3) ---
            // Human Paladin: Paladin, Human — cost 3, stats 5/6/3=14, ratio 4.67
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000011"), "Human Paladin", Rarity.Uncommon, 3, 5, 6, 3,
                isAmbusher: false, treasure: 1,
                effect: "Sacrifice 2 HP: restore 3 HP to another ally. Against Undead: +2 STR.",
                race: Race.Human, allyClass: AllyClass.Paladin),

            // Tortle Guardian: Paladin, Beast — cost 3, stats 3/7/2=12, ratio 4.0
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000024"), "Tortle Guardian", Rarity.Uncommon, 3, 3, 7, 2,
                isAmbusher: false, treasure: 1,
                effect: "Sacrifice 1 HP: redirect all damage from one ally to this ally for the combat. If equipped with a Shield: +2 HP.",
                race: Race.Beast, allyClass: AllyClass.Paladin),

            // Bugbear Bruiser: Paladin, Goblin — cost 3, stats 5/5/3=13, ratio 4.33
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000029"), "Bugbear Bruiser", Rarity.Uncommon, 3, 5, 5, 3,
                isAmbusher: true, treasure: 1,
                effect: "Sacrifice 1 HP: +3 STR this combat. If entering combat With Advantage: also gains Ambusher.",
                race: Race.Goblin, allyClass: AllyClass.Paladin),

            // ===== COMMON (30) =====

            // --- Warrior Common (5) ---
            // Militia Recruit: Warrior, Human — cost 1, stats 2/3/2=7, ratio 7.0
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000031"), "Militia Recruit", Rarity.Common, 1, 2, 3, 2,
                isAmbusher: false, treasure: 1,
                effect: "With Advantage: +1 STR for each other ally in this combat group.",
                race: Race.Human, allyClass: AllyClass.Warrior),

            // Town Guard: Warrior, Human — cost 1, stats 2/4/2=8, ratio 8.0
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000032"), "Town Guard", Rarity.Common, 1, 2, 4, 2,
                isAmbusher: false, treasure: 1,
                effect: "With Disadvantage: +2 HP. If equipped with a Shield: +1 HP.",
                race: Race.Human, allyClass: AllyClass.Warrior),

            // Caravan Guard: Warrior, Human — cost 1, stats 3/3/2=8, ratio 8.0
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000042"), "Caravan Guard", Rarity.Common, 1, 3, 3, 2,
                isAmbusher: false, treasure: 1,
                effect: "If a Warrior ally is in play: +1 STR. Reduces damage to one other ally by 1 per combat.",
                race: Race.Human, allyClass: AllyClass.Warrior),

            // Shield Bearer: Warrior, Human — cost 1, stats 2/4/1=7, ratio 7.0
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000051"), "Shield Bearer", Rarity.Common, 1, 2, 4, 1,
                isAmbusher: false, treasure: 1,
                effect: "All allies in this combat group gain +1 HP. If equipped with Armor: +1 HP.",
                race: Race.Human, allyClass: AllyClass.Warrior),

            // Pit Fighter: Warrior, Human — cost 2, stats 4/3/2=9, ratio 4.5
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000043"), "Pit Fighter", Rarity.Common, 2, 4, 3, 2,
                isAmbusher: false, treasure: 1,
                effect: "With Advantage: +2 STR. If an enemy in this combat has higher STR: +1 STR.",
                race: Race.Human, allyClass: AllyClass.Warrior),

            // --- Rogue Common (5) ---
            // Street Urchin: Rogue, Human — cost 1, stats 1/2/3=6, ratio 6.0
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000037"), "Street Urchin", Rarity.Common, 1, 1, 2, 3,
                isAmbusher: true, treasure: 2,
                effect: "With Advantage: +2 STR. Gains +1 treasure per room.",
                race: Race.Human, allyClass: AllyClass.Rogue),

            // Lockpick Expert: Rogue, Human — cost 1, stats 1/2/3=6, ratio 6.0
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000049"), "Lockpick Expert", Rarity.Common, 1, 1, 2, 3,
                isAmbusher: true, treasure: 2,
                effect: "With Advantage: +2 STR. Gains +1 treasure from rooms containing Traps.",
                race: Race.Human, allyClass: AllyClass.Rogue),

            // Tomb Raider: Rogue, Human — cost 2, stats 3/3/3=9, ratio 4.5
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000059"), "Tomb Raider", Rarity.Common, 2, 3, 3, 3,
                isAmbusher: true, treasure: 2,
                effect: "With Advantage: +2 STR. Against Undead enemies: +1 treasure.",
                race: Race.Human, allyClass: AllyClass.Rogue),

            // Throwing Knife Specialist (renamed from Crossbow Specialist at 0052): Rogue, Human — cost 2, stats 3/2/3=8, ratio 4.0
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000052"), "Throwing Knife Specialist", Rarity.Common, 2, 3, 2, 3,
                isAmbusher: false, treasure: 1,
                effect: "With Advantage: +3 STR. May Retarget once per combat without paying cost.",
                race: Race.Human, allyClass: AllyClass.Rogue),

            // Tavern Brawler: Rogue, Human — cost 1, stats 3/3/2=8, ratio 8.0
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000038"), "Tavern Brawler", Rarity.Common, 1, 3, 3, 2,
                isAmbusher: false, treasure: 1,
                effect: "With Advantage: +2 STR. If no Equipment is equipped: +1 STR.",
                race: Race.Human, allyClass: AllyClass.Rogue),

            // --- Mage Common (5) ---
            // Apprentice Wizard: Mage, Human — cost 2, stats 3/3/3=9, ratio 4.5
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000033"), "Apprentice Wizard", Rarity.Common, 2, 3, 3, 3,
                isAmbusher: false, treasure: 1,
                effect: "If a Mage ally is in play: +1 STR. If equipped with a Scroll: +1 STR.",
                race: Race.Human, allyClass: AllyClass.Mage),

            // Hedge Mage: Mage, Human — cost 2, stats 2/3/3=8, ratio 4.0
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000040"), "Hedge Mage", Rarity.Common, 2, 2, 3, 3,
                isAmbusher: false, treasure: 1,
                effect: "All allies in this combat group gain +1 INIT. If a Trap is in this room: all allies also gain +1 STR.",
                race: Race.Human, allyClass: AllyClass.Mage),

            // Fortune Teller: Mage, Human — cost 2, stats 2/3/3=8, ratio 4.0
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000060"), "Fortune Teller", Rarity.Common, 2, 2, 3, 3,
                isAmbusher: false, treasure: 2,
                effect: "Reveal top card of opponent's deck at the start of combat. All allies gain +1 STR against the revealed card's type.",
                race: Race.Human, allyClass: AllyClass.Mage),

            // War Drummer: Mage, Human — cost 1, stats 1/3/2=6, ratio 6.0
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000054"), "War Drummer", Rarity.Common, 1, 1, 3, 2,
                isAmbusher: false, treasure: 1,
                effect: "All allies in this combat group gain +1 INIT.",
                race: Race.Human, allyClass: AllyClass.Mage),

            // Torchbearer: Mage, Human — cost 1, stats 1/2/2=5, ratio 5.0
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000047"), "Torchbearer", Rarity.Common, 1, 1, 2, 2,
                isAmbusher: false, treasure: 1,
                effect: "Enemies in this combat group lose Ambusher. Exile this ally: deal 3 STR damage to one enemy.",
                race: Race.Human, allyClass: AllyClass.Mage),

            // --- Cleric Common (5) ---
            // Acolyte Healer: Cleric, Human — cost 2, stats 2/4/2=8, ratio 4.0
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000034"), "Acolyte Healer", Rarity.Common, 2, 2, 4, 2,
                isAmbusher: false, treasure: 1,
                effect: "Restore 2 HP to the most injured ally at end of combat. If a Cleric ally is in play: also restore 1 HP to all other allies.",
                race: Race.Human, allyClass: AllyClass.Cleric),

            // Shrine Maiden: Cleric, Human — cost 2, stats 2/3/2=7, ratio 3.5
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000045"), "Shrine Maiden", Rarity.Common, 2, 2, 3, 2,
                isAmbusher: false, treasure: 1,
                effect: "At start of combat: one ally gains +1 STR and +1 HP for this room. Against Undead: +2 STR.",
                race: Race.Human, allyClass: AllyClass.Cleric),

            // Battle Medic: Cleric, Human — cost 2, stats 2/4/2=8, ratio 4.0
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000050"), "Battle Medic", Rarity.Common, 2, 2, 4, 2,
                isAmbusher: false, treasure: 1,
                effect: "Once per room: restore an ally reduced to 1 HP to full HP. If a Balm or Potion is used: +1 HP to all allies.",
                race: Race.Human, allyClass: AllyClass.Cleric),

            // Camp Cook: Cleric, Human — cost 1, stats 2/3/2=7, ratio 7.0
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000056"), "Camp Cook", Rarity.Common, 1, 2, 3, 2,
                isAmbusher: false, treasure: 1,
                effect: "Between rooms: restore 1 HP to all allies. If a Cleric ally is in play: restore 2 HP instead.",
                race: Race.Human, allyClass: AllyClass.Cleric),

            // Farmhand Fighter: Cleric, Human — cost 1, stats 2/3/1=6, ratio 6.0
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000039"), "Farmhand Fighter", Rarity.Common, 1, 2, 3, 1,
                isAmbusher: false, treasure: 1,
                effect: "Traps deal 1 less damage to this ally. If another Cleric is in play: +1 HP.",
                race: Race.Human, allyClass: AllyClass.Cleric),

            // --- Ranger Common (5) ---
            // Scout Tracker: Ranger, Human — cost 2, stats 3/3/3=9, ratio 4.5
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000036"), "Scout Tracker", Rarity.Common, 2, 3, 3, 3,
                isAmbusher: false, treasure: 1,
                effect: "Reveal the next room's enemies before entering. Favored Enemy (Beast): +2 STR against Beasts.",
                race: Race.Human, allyClass: AllyClass.Ranger),

            // Forest Warden: Ranger, Human — cost 2, stats 3/3/3=9, ratio 4.5
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000044"), "Forest Warden", Rarity.Common, 2, 3, 3, 3,
                isAmbusher: false, treasure: 1,
                effect: "Favored Enemy (Beast): +2 STR against Beasts. If a Ranger ally is in play: +1 INIT.",
                race: Race.Human, allyClass: AllyClass.Ranger),

            // Beast Handler: Ranger, Human — cost 2, stats 3/3/3=9, ratio 4.5
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000057"), "Beast Handler", Rarity.Common, 2, 3, 3, 3,
                isAmbusher: false, treasure: 1,
                effect: "Favored Enemy (Beast): +3 STR against Beasts. Traps targeting this ally are revealed before triggering.",
                race: Race.Human, allyClass: AllyClass.Ranger),

            // Map Reader: Ranger, Human — cost 1, stats 1/2/3=6, ratio 6.0
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000058"), "Map Reader", Rarity.Common, 1, 1, 2, 3,
                isAmbusher: false, treasure: 1,
                effect: "Reveal the next 2 rooms' Trap cards. If a Ranger ally is in play: +1 INIT.",
                race: Race.Human, allyClass: AllyClass.Ranger),

            // Dungeon Delver: Ranger, Human — cost 2, stats 2/3/3=8, ratio 4.0
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000046"), "Dungeon Delver", Rarity.Common, 2, 2, 3, 3,
                isAmbusher: false, treasure: 2,
                effect: "Traps in this room deal 2 less damage to all allies. Reveal all Traps in the current room.",
                race: Race.Human, allyClass: AllyClass.Ranger),

            // --- Paladin Common (5) ---
            // Squire of the Order: Paladin, Human — cost 1, stats 2/3/2=7, ratio 7.0
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000035"), "Squire of the Order", Rarity.Common, 1, 2, 3, 2,
                isAmbusher: false, treasure: 1,
                effect: "If a Paladin ally is in play: +2 STR and +1 INIT.",
                race: Race.Human, allyClass: AllyClass.Paladin),

            // Flag Bearer: Paladin, Human — cost 1, stats 2/3/2=7, ratio 7.0
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000055"), "Flag Bearer", Rarity.Common, 1, 2, 3, 2,
                isAmbusher: false, treasure: 1,
                effect: "All allies in this combat group gain +1 STR. If this ally is defeated: all allies gain +2 STR for the rest of the room.",
                race: Race.Human, allyClass: AllyClass.Paladin),

            // Cavalry Rider: Paladin, Human — cost 2, stats 3/3/3=9, ratio 4.5
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000053"), "Cavalry Rider", Rarity.Common, 2, 3, 3, 3,
                isAmbusher: false, treasure: 1,
                effect: "Sacrifice 1 HP: +2 STR this combat. If a Paladin ally is in play: +1 INIT.",
                race: Race.Human, allyClass: AllyClass.Paladin),

            // Trapfinder: Paladin, Human — cost 2, stats 2/3/3=8, ratio 4.0
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000048"), "Trapfinder", Rarity.Common, 2, 2, 3, 3,
                isAmbusher: false, treasure: 1,
                effect: "Sacrifice 1 HP: disarm one Trap in this room before it triggers. If a Trap is disarmed: +1 treasure.",
                race: Race.Human, allyClass: AllyClass.Paladin),

            // Crossbow Specialist: Paladin, Human — cost 2, stats 3/3/3=9, ratio 4.5
            new AllyCard(new Guid("00000000-0000-0000-0001-000000000041"), "Crossbow Specialist", Rarity.Common, 2, 3, 3, 3,
                isAmbusher: false, treasure: 1,
                effect: "Sacrifice 2 HP: deal 3 STR damage to any enemy in any combat group. If equipped with a Weapon: +1 STR.",
                race: Race.Human, allyClass: AllyClass.Paladin)
        };
    }
}
