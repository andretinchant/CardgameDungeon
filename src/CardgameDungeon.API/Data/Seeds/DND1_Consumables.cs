using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.API.Data.Seeds;

public static partial class CardSetSeeder
{
    // Consumable budget (equipment +1 for single-use): Cost 1→4, 2→6, 3→7, 4→9, 5→11, Unique 5→13
    // Formula: ATK + HP + INIT*1.5 + EffectPts = Budget
    // All consumables MUST have an effect.
    private static List<EquipmentCard> CreateConsumables()
    {
        return new List<EquipmentCard>
        {
            // ═══════════════════════════════════════════════════════════════
            //  UNIQUE (2) — Cost 5, Budget 13
            // ═══════════════════════════════════════════════════════════════

            // Budget: STR0 + HP0 + INIT0×1.5 + Effect13 = 13 (modal choice is worth ~13)
            new(new Guid("00000000-0000-0000-0007-000000000001"),
                "Scroll of Wish", Rarity.Unique, 5, 0, 0, 0, EquipmentSlot.Scroll,
                "Search your deck, discard pile, or exile: take any 1 card and put it in your hand. Exile this scroll after use",
                effectTags: "ON_ACTIVATE|SEARCH_ANY:1|EXILE_SELF"),

            // Budget: STR0 + HP4 + INIT0×1.5 + Effect9 = 13 (cannot-die effect ~8, self-damage -1 offset)
            new(new Guid("00000000-0000-0000-0007-000000000002"),
                "Elixir of Immortality", Rarity.Unique, 5, 0, 4, 0, EquipmentSlot.Potion,
                "Target ally cannot be eliminated this round and gains +4 HP. At end of round: lose 1 HP",
                effectTags: "ON_ACTIVATE|IMMUNE_ELIM:ALLY|+HP:4:ALLY\nON_ROUND_START|-HP:1:ALLY"),

            // ═══════════════════════════════════════════════════════════════
            //  RARE (8) — Cost 3→Budget 7, Cost 4→Budget 9
            // ═══════════════════════════════════════════════════════════════

            // Budget: STR1 + HP0 + INIT0×1.5 + Effect8 = 9 (AoE split damage + Advantage bonus)
            new(new Guid("00000000-0000-0000-0007-000000000003"),
                "Scroll of Fireball", Rarity.Rare, 4, 1, 0, 0, EquipmentSlot.Scroll,
                "Deal 1 damage split among all enemies in play. With Advantage: deal 2 damage instead",
                effectTags: "ON_ACTIVATE|DAMAGE:1:ALL_ENEMIES\nWITH_ADVANTAGE|DAMAGE:2:ALL_ENEMIES"),

            // Budget: STR4 + HP0 + INIT0×1.5 + Effect3 = 7
            new(new Guid("00000000-0000-0000-0007-000000000004"),
                "Potion of Giant Attack", Rarity.Rare, 3, 4, 0, 0, EquipmentSlot.Potion,
                "Target ally gains +4 ATK for this combat round",
                effectTags: "ON_ACTIVATE|+ATK:4:ALLY"),

            // Budget: STR0 + HP0 + INIT0×1.5 + Effect9 = 9 (recovery from discard ~9)
            new(new Guid("00000000-0000-0000-0007-000000000005"),
                "Scroll of Resurrection", Rarity.Rare, 4, 0, 0, 0, EquipmentSlot.Scroll,
                "Return one ally from your discard pile to your hand. That ally enters play with -1 ATK",
                effectTags: "ON_ACTIVATE|RESTORE_ALLY_FROM_DISCARD|-ATK:1:ALLY"),

            // Budget: STR1 + HP0 + INIT0×1.5 + Effect8 = 9 (ignores Shield ~2, direct damage ~1)
            new(new Guid("00000000-0000-0000-0007-000000000006"),
                "Bomb of Devastation", Rarity.Rare, 4, 1, 0, 0, EquipmentSlot.Bomb,
                "Deal 1 damage to target enemy, ignoring Shield equipment bonuses",
                effectTags: "ON_ACTIVATE|DAMAGE:1:ENEMY|IGNORE_SHIELD"),

            // Budget: STR0 + HP3 + INIT0×1.5 + Effect4 = 7 (AoE heal ~4)
            new(new Guid("00000000-0000-0000-0007-000000000007"),
                "Sacred Balm of Lathander", Rarity.Rare, 3, 0, 3, 0, EquipmentSlot.Balm,
                "Restore 3 HP to all allies in play",
                effectTags: "ON_ACTIVATE|HEAL:3:ALL_ALLIES"),

            // Budget: STR0 + HP2 + INIT2×1.5 + Effect2 = 7
            new(new Guid("00000000-0000-0000-0007-000000000008"),
                "Totem of Ancestral Guard", Rarity.Rare, 3, 0, 2, 2, EquipmentSlot.Totem,
                "All allies gain +2 HP and +2 INIT this round. With Disadvantage: also grants +1 ATK",
                effectTags: "ON_ACTIVATE|+HP:2:ALL_ALLIES|+INIT:2:ALL_ALLIES\nWITH_DISADVANTAGE|+ATK:1:ALL_ALLIES"),

            // Budget: STR0 + HP0 + INIT3×1.5 + Effect4.5 = 9 (guaranteed first-strike ~4.5)
            new(new Guid("00000000-0000-0000-0007-000000000009"),
                "Scroll of Time Stop", Rarity.Rare, 4, 0, 0, 3, EquipmentSlot.Scroll,
                "Your team acts first regardless of INIT this round",
                effectTags: "ON_ACTIVATE|FIRST_STRIKE:ALL_ALLIES"),

            // Budget: STR0 + HP3 + INIT0×1.5 + Effect4 = 7 (no-damage effect ~4)
            new(new Guid("00000000-0000-0000-0007-000000000010"),
                "Potion of Invulnerability", Rarity.Rare, 3, 0, 3, 0, EquipmentSlot.Potion,
                "Target ally takes no damage this combat round",
                effectTags: "ON_ACTIVATE|IMMUNE_DAMAGE:ALLY"),

            // ═══════════════════════════════════════════════════════════════
            //  UNCOMMON (20) — Cost 2, Budget 6
            // ═══════════════════════════════════════════════════════════════

            // Budget: STR1 + HP0 + INIT0×1.5 + Effect5 = 6
            new(new Guid("00000000-0000-0000-0007-000000000011"),
                "Scroll of Lightning Bolt", Rarity.Uncommon, 2, 1, 0, 0, EquipmentSlot.Scroll,
                "Deal 1 damage to target enemy",
                effectTags: "ON_ACTIVATE|DAMAGE:1:ENEMY"),

            // Budget: STR0 + HP3 + INIT0×1.5 + Effect3 = 6
            new(new Guid("00000000-0000-0000-0007-000000000012"),
                "Potion of Healing", Rarity.Uncommon, 2, 0, 3, 0, EquipmentSlot.Potion,
                "Restore 3 HP to target ally",
                effectTags: "ON_ACTIVATE|HEAL:3:ALLY"),

            // Budget: STR0 + HP2 + INIT0×1.5 + Effect4 = 6 (temporary HP + defensive)
            new(new Guid("00000000-0000-0000-0007-000000000013"),
                "Scroll of Shield", Rarity.Uncommon, 2, 0, 2, 0, EquipmentSlot.Scroll,
                "Target ally gains +2 HP until end of round and cannot be targeted by traps",
                effectTags: "ON_ACTIVATE|+HP:2:ALLY|IMMUNE_TRAP:ALLY"),

            // Budget: STR0 + HP0 + INIT3×1.5 + Effect1.5 = 6
            new(new Guid("00000000-0000-0000-0007-000000000014"),
                "Potion of Speed", Rarity.Uncommon, 2, 0, 0, 3, EquipmentSlot.Potion,
                "Target ally gains +3 INIT this round",
                effectTags: "ON_ACTIVATE|+INIT:3:ALLY"),

            // Budget: STR1 + HP0 + INIT0×1.5 + Effect5 = 6 (single target damage)
            new(new Guid("00000000-0000-0000-0007-000000000015"),
                "Alchemist Fire", Rarity.Uncommon, 2, 1, 0, 0, EquipmentSlot.Bomb,
                "Deal 1 damage to target enemy",
                effectTags: "ON_ACTIVATE|DAMAGE:1:ENEMY"),

            // Budget: STR1 + HP0 + INIT0×1.5 + Effect5 = 6 (type-restricted bonus ~1)
            new(new Guid("00000000-0000-0000-0007-000000000016"),
                "Holy Water", Rarity.Uncommon, 2, 1, 0, 0, EquipmentSlot.Bomb,
                "Deal 1 damage to target Undead or Fiend enemy. Exiles that enemy if defeated",
                effectTags: "ON_ACTIVATE|IF_RACE:Undead|DAMAGE:1:ENEMY|EXILE_TARGET\nON_ACTIVATE|IF_RACE:Demon|DAMAGE:1:ENEMY|EXILE_TARGET"),

            // Budget: STR0 + HP2 + INIT0×1.5 + Effect4 = 6 (heal + cleanse ~4)
            new(new Guid("00000000-0000-0000-0007-000000000017"),
                "Balm of Restoration", Rarity.Uncommon, 2, 0, 2, 0, EquipmentSlot.Balm,
                "Restore 2 HP to target ally and remove all Disadvantage effects",
                effectTags: "ON_ACTIVATE|HEAL:2:ALLY|REMOVE_DISADVANTAGE:ALLY"),

            // Budget: STR2 + HP0 + INIT0×1.5 + Effect4 = 6 (AoE ATK ~4)
            new(new Guid("00000000-0000-0000-0007-000000000018"),
                "Totem of War", Rarity.Uncommon, 2, 2, 0, 0, EquipmentSlot.Totem,
                "All allies gain +2 ATK this combat round",
                effectTags: "ON_ACTIVATE|+ATK:2:ALL_ALLIES"),

            // Budget: STR1 + HP0 + INIT2×1.5 + Effect2 = 6
            new(new Guid("00000000-0000-0000-0007-000000000019"),
                "Scroll of Haste", Rarity.Uncommon, 2, 1, 0, 2, EquipmentSlot.Scroll,
                "Target ally gains +1 ATK and +2 INIT this round",
                effectTags: "ON_ACTIVATE|+ATK:1:ALLY|+INIT:2:ALLY"),

            // Budget: STR0 + HP2 + INIT1×1.5 + Effect2.5 = 6
            new(new Guid("00000000-0000-0000-0007-000000000020"),
                "Potion of Fortitude", Rarity.Uncommon, 2, 0, 2, 1, EquipmentSlot.Potion,
                "Target ally gains +2 HP and +1 INIT this round",
                effectTags: "ON_ACTIVATE|+HP:2:ALLY|+INIT:1:ALLY"),

            // Budget: STR0 + HP0 + INIT0×1.5 + Effect6 = 6 (INIT reduction AoE ~6)
            new(new Guid("00000000-0000-0000-0007-000000000021"),
                "Tanglefoot Bag", Rarity.Uncommon, 2, 0, 0, 0, EquipmentSlot.Bomb,
                "All enemies lose 2 INIT this round",
                effectTags: "ON_ACTIVATE|-INIT:2:ALL_ENEMIES"),

            // Budget: STR1 + HP0 + INIT0×1.5 + Effect5 = 6 (AoE damage ~5)
            new(new Guid("00000000-0000-0000-0007-000000000022"),
                "Thunderstone", Rarity.Uncommon, 2, 1, 0, 0, EquipmentSlot.Bomb,
                "Deal 1 damage to all enemies in play",
                effectTags: "ON_ACTIVATE|DAMAGE:1:ALL_ENEMIES"),

            // Budget: STR1 + HP1 + INIT0×1.5 + Effect4 = 6
            new(new Guid("00000000-0000-0000-0007-000000000023"),
                "Balm of Heroes", Rarity.Uncommon, 2, 1, 1, 0, EquipmentSlot.Balm,
                "Target ally gains +1 ATK and +1 HP. Remove one negative effect",
                effectTags: "ON_ACTIVATE|+ATK:1:ALLY|+HP:1:ALLY|REMOVE_NEGATIVE:ALLY"),

            // Budget: STR0 + HP2 + INIT0×1.5 + Effect4 = 6 (AoE HP ~4)
            new(new Guid("00000000-0000-0000-0007-000000000024"),
                "Totem of Protection", Rarity.Uncommon, 2, 0, 2, 0, EquipmentSlot.Totem,
                "All allies gain +2 HP this combat round",
                effectTags: "ON_ACTIVATE|+HP:2:ALL_ALLIES"),

            // Budget: STR0 + HP0 + INIT0×1.5 + Effect6 = 6 (negate ~6)
            new(new Guid("00000000-0000-0000-0007-000000000025"),
                "Scroll of Counterspell", Rarity.Uncommon, 2, 0, 0, 0, EquipmentSlot.Scroll,
                "Negate one enemy trap or consumable effect this round",
                effectTags: "ON_ACTIVATE|NEGATE_EFFECT:ENEMY"),

            // Budget: STR3 + HP-1 + INIT0×1.5 + Effect4 = 6 (self-damage drawback)
            new(new Guid("00000000-0000-0000-0007-000000000026"),
                "Potion of Rage", Rarity.Uncommon, 2, 3, -1, 0, EquipmentSlot.Potion,
                "Target ally gains +3 ATK but loses 1 HP",
                effectTags: "ON_ACTIVATE|+ATK:3:ALLY|-HP:1:ALLY"),

            // Budget: STR0 + HP0 + INIT0×1.5 + Effect6 = 6 (permanent HP loss ~6)
            new(new Guid("00000000-0000-0000-0007-000000000027"),
                "Poison Vial", Rarity.Uncommon, 2, 0, 0, 0, EquipmentSlot.Bomb,
                "Target enemy loses 1 HP permanently for the rest of the dungeon",
                effectTags: "ON_ACTIVATE|-HP:1:ENEMY"),

            // Budget: STR0 + HP0 + INIT2×1.5 + Effect3 = 6 (AoE INIT ~3)
            new(new Guid("00000000-0000-0000-0007-000000000028"),
                "Totem of Swiftness", Rarity.Uncommon, 2, 0, 0, 2, EquipmentSlot.Totem,
                "All allies gain +2 INIT this round",
                effectTags: "ON_ACTIVATE|+INIT:2:ALL_ALLIES"),

            // Budget: STR0 + HP1 + INIT1×1.5 + Effect3 = 6 (race-restricted AoE ~3)
            new(new Guid("00000000-0000-0000-0007-000000000029"),
                "Balm of the Wild", Rarity.Uncommon, 2, 0, 1, 1, EquipmentSlot.Balm,
                "Restore 1 HP and grant +1 INIT to all Beast race allies",
                effectTags: "ON_ACTIVATE|IF_RACE:Beast|HEAL:1:ALL_ALLIES|+INIT:1:ALL_ALLIES"),

            // Budget: STR0 + HP0 + INIT0×1.5 + Effect6 = 6 (untargetable ~6)
            new(new Guid("00000000-0000-0000-0007-000000000030"),
                "Scroll of Mirror Image", Rarity.Uncommon, 2, 0, 0, 0, EquipmentSlot.Scroll,
                "Target ally gains Ambusher trait and cannot be targeted this round",
                effectTags: "ON_ACTIVATE|AMBUSHER:ALLY|UNTARGETABLE:ALLY"),

            // ═══════════════════════════════════════════════════════════════
            //  COMMON (30) — Cost 1, Budget 4
            //  All consumables must have effects.
            // ═══════════════════════════════════════════════════════════════

            // Budget: STR0 + HP1 + INIT0×1.5 + Effect3 = 4
            new(new Guid("00000000-0000-0000-0007-000000000031"),
                "Minor Healing Potion", Rarity.Common, 1, 0, 1, 0, EquipmentSlot.Potion,
                "Restore 1 HP to target ally",
                effectTags: "ON_ACTIVATE|HEAL:1:ALLY"),

            // Budget: STR1 + HP0 + INIT0×1.5 + Effect3 = 4
            new(new Guid("00000000-0000-0000-0007-000000000032"),
                "Scroll of Magic Missile", Rarity.Common, 1, 1, 0, 0, EquipmentSlot.Scroll,
                "Deal 1 damage to target enemy that cannot be prevented",
                effectTags: "ON_ACTIVATE|DAMAGE:1:ENEMY|UNPREVENTABLE"),

            // Budget: STR1 + HP0 + INIT0×1.5 + Effect3 = 4
            new(new Guid("00000000-0000-0000-0007-000000000033"),
                "Firebomb", Rarity.Common, 1, 1, 0, 0, EquipmentSlot.Bomb,
                "Deal 1 damage to target enemy",
                effectTags: "ON_ACTIVATE|DAMAGE:1:ENEMY"),

            // Budget: STR0 + HP1 + INIT0×1.5 + Effect3 = 4
            new(new Guid("00000000-0000-0000-0007-000000000034"),
                "Herbal Balm", Rarity.Common, 1, 0, 1, 0, EquipmentSlot.Balm,
                "Restore 1 HP to target ally",
                effectTags: "ON_ACTIVATE|HEAL:1:ALLY"),

            // Budget: STR1 + HP0 + INIT0×1.5 + Effect3 = 4
            new(new Guid("00000000-0000-0000-0007-000000000035"),
                "Hunting Totem", Rarity.Common, 1, 1, 0, 0, EquipmentSlot.Totem,
                "One ally gains +1 ATK this round",
                effectTags: "ON_ACTIVATE|+ATK:1:ALLY"),

            // Budget: STR1 + HP0 + INIT0×1.5 + Effect3 = 4
            new(new Guid("00000000-0000-0000-0007-000000000036"),
                "Potion of Courage", Rarity.Common, 1, 1, 0, 0, EquipmentSlot.Potion,
                "Target ally gains +1 ATK this round",
                effectTags: "ON_ACTIVATE|+ATK:1:ALLY"),

            // Budget: STR0 + HP0 + INIT1×1.5 + Effect2.5 = 4
            new(new Guid("00000000-0000-0000-0007-000000000037"),
                "Scroll of Bless", Rarity.Common, 1, 0, 0, 1, EquipmentSlot.Scroll,
                "Target ally gains +1 INIT this round and gains Advantage on next attack",
                effectTags: "ON_ACTIVATE|+INIT:1:ALLY|GRANT_ADVANTAGE:ALLY"),

            // Budget: STR1 + HP0 + INIT0×1.5 + Effect3 = 4 (ignores Armor ~1)
            new(new Guid("00000000-0000-0000-0007-000000000038"),
                "Acid Flask", Rarity.Common, 1, 1, 0, 0, EquipmentSlot.Bomb,
                "Deal 1 damage to target enemy, ignoring Armor HP bonus",
                effectTags: "ON_ACTIVATE|DAMAGE:1:ENEMY|IGNORE_ARMOR"),

            // Budget: STR0 + HP0 + INIT0×1.5 + Effect4 = 4 (cleanse ~4)
            new(new Guid("00000000-0000-0000-0007-000000000039"),
                "Soothing Salve", Rarity.Common, 1, 0, 0, 0, EquipmentSlot.Balm,
                "Remove one Disadvantage effect from target ally and restore 1 HP",
                effectTags: "ON_ACTIVATE|REMOVE_DISADVANTAGE:ALLY|HEAL:1:ALLY"),

            // Budget: STR0 + HP1 + INIT0×1.5 + Effect3 = 4
            new(new Guid("00000000-0000-0000-0007-000000000040"),
                "Ward Totem", Rarity.Common, 1, 0, 1, 0, EquipmentSlot.Totem,
                "One ally gains +1 HP this round",
                effectTags: "ON_ACTIVATE|+HP:1:ALLY"),

            // Budget: STR0 + HP0 + INIT1×1.5 + Effect2.5 = 4
            new(new Guid("00000000-0000-0000-0007-000000000041"),
                "Potion of Swiftness", Rarity.Common, 1, 0, 0, 1, EquipmentSlot.Potion,
                "Target ally gains +1 INIT this round",
                effectTags: "ON_ACTIVATE|+INIT:1:ALLY"),

            // Budget: STR0 + HP0 + INIT0×1.5 + Effect4 = 4 (reveal ~4)
            new(new Guid("00000000-0000-0000-0007-000000000042"),
                "Scroll of Detect Magic", Rarity.Common, 1, 0, 0, 0, EquipmentSlot.Scroll,
                "Reveal the next trap in the dungeon deck",
                effectTags: "ON_ACTIVATE|REVEAL_TRAP"),

            // Budget: STR0 + HP0 + INIT0×1.5 + Effect4 = 4 (enemy INIT reduction ~4)
            new(new Guid("00000000-0000-0000-0007-000000000043"),
                "Stink Bomb", Rarity.Common, 1, 0, 0, 0, EquipmentSlot.Bomb,
                "Target enemy loses 2 INIT this round",
                effectTags: "ON_ACTIVATE|-INIT:2:ENEMY"),

            // Budget: STR0 + HP1 + INIT0×1.5 + Effect3 = 4 (conditional heal ~3)
            new(new Guid("00000000-0000-0000-0007-000000000044"),
                "Field Dressing", Rarity.Common, 1, 0, 1, 0, EquipmentSlot.Balm,
                "Restore 1 HP to ally that took damage this round",
                effectTags: "ON_ACTIVATE|HEAL:1:ALLY"),

            // Budget: STR0 + HP0 + INIT1×1.5 + Effect2.5 = 4
            new(new Guid("00000000-0000-0000-0007-000000000045"),
                "Spirit Totem", Rarity.Common, 1, 0, 0, 1, EquipmentSlot.Totem,
                "One ally gains +1 INIT this round",
                effectTags: "ON_ACTIVATE|+INIT:1:ALLY"),

            // Budget: STR0 + HP0 + INIT1×1.5 + Effect2.5 = 4
            new(new Guid("00000000-0000-0000-0007-000000000046"),
                "Elixir of Focus", Rarity.Common, 1, 0, 0, 1, EquipmentSlot.Potion,
                "Target ally gains +1 INIT this round",
                effectTags: "ON_ACTIVATE|+INIT:1:ALLY"),

            // Budget: STR0 + HP1 + INIT0×1.5 + Effect3 = 4
            new(new Guid("00000000-0000-0000-0007-000000000047"),
                "Scroll of Mending", Rarity.Common, 1, 0, 1, 0, EquipmentSlot.Scroll,
                "Restore 1 HP to target ally",
                effectTags: "ON_ACTIVATE|HEAL:1:ALLY"),

            // Budget: STR0 + HP0 + INIT0×1.5 + Effect4 = 4 (enemy ATK reduction ~4)
            new(new Guid("00000000-0000-0000-0007-000000000048"),
                "Frost Bomb", Rarity.Common, 1, 0, 0, 0, EquipmentSlot.Bomb,
                "Target enemy loses 1 ATK this round",
                effectTags: "ON_ACTIVATE|-ATK:1:ENEMY"),

            // Budget: STR0 + HP0 + INIT1×1.5 + Effect2.5 = 4
            new(new Guid("00000000-0000-0000-0007-000000000049"),
                "Warming Balm", Rarity.Common, 1, 0, 0, 1, EquipmentSlot.Balm,
                "Target ally gains +1 INIT and takes 1 less trap damage this round",
                effectTags: "ON_ACTIVATE|+INIT:1:ALLY|REDUCE_TRAP_DAMAGE:1:ALLY"),

            // Budget: STR0 + HP0 + INIT0×1.5 + Effect4 = 4 (type-restricted debuff ~4)
            new(new Guid("00000000-0000-0000-0007-000000000050"),
                "Bone Totem", Rarity.Common, 1, 0, 0, 0, EquipmentSlot.Totem,
                "Reduce one Undead enemy's ATK by 2 this round",
                effectTags: "ON_ACTIVATE|IF_RACE:Undead|-ATK:2:ENEMY"),

            // Budget: STR0 + HP1 + INIT0×1.5 + Effect3 = 4
            new(new Guid("00000000-0000-0000-0007-000000000051"),
                "Potion of Toughness", Rarity.Common, 1, 0, 1, 0, EquipmentSlot.Potion,
                "Target ally gains +1 HP this round",
                effectTags: "ON_ACTIVATE|+HP:1:ALLY"),

            // Budget: STR0 + HP0 + INIT0×1.5 + Effect4 = 4 (negate room effects ~4)
            new(new Guid("00000000-0000-0000-0007-000000000052"),
                "Scroll of Light", Rarity.Common, 1, 0, 0, 0, EquipmentSlot.Scroll,
                "Negate darkness and visibility penalties in the current room",
                effectTags: "ON_ACTIVATE|NEGATE_VISIBILITY_PENALTY"),

            // Budget: STR0 + HP0 + INIT0×1.5 + Effect4 = 4 (disable retarget ~4)
            new(new Guid("00000000-0000-0000-0007-000000000053"),
                "Tar Bomb", Rarity.Common, 1, 0, 0, 0, EquipmentSlot.Bomb,
                "Target enemy cannot use retarget this round",
                effectTags: "ON_ACTIVATE|DISABLE_RETARGET:ENEMY"),

            // Budget: STR1 + HP0 + INIT-1×1.5 + Effect4.5 = 4 (drawback offsets)
            new(new Guid("00000000-0000-0000-0007-000000000054"),
                "Dwarven Spirits", Rarity.Common, 1, 1, 0, -1, EquipmentSlot.Potion,
                "Target ally gains +1 ATK but loses 1 INIT this round",
                effectTags: "ON_ACTIVATE|+ATK:1:ALLY|-INIT:1:ALLY"),

            // Budget: STR0 + HP0 + INIT0×1.5 + Effect4 = 4 (exile prevention ~4)
            new(new Guid("00000000-0000-0000-0007-000000000055"),
                "Scroll of Feather Fall", Rarity.Common, 1, 0, 0, 0, EquipmentSlot.Scroll,
                "Prevent exile of 1 card from your deck this round",
                effectTags: "ON_ACTIVATE|PREVENT_EXILE:1"),

            // Budget: STR0 + HP0 + INIT0×1.5 + Effect4 = 4 (opportunity attack immunity ~4)
            new(new Guid("00000000-0000-0000-0007-000000000056"),
                "Smoke Pellet", Rarity.Common, 1, 0, 0, 0, EquipmentSlot.Bomb,
                "One ally cannot be targeted by opportunity attacks this round",
                effectTags: "ON_ACTIVATE|IMMUNE_OPP_ATTACK:ALLY"),

            // Budget: STR0 + HP0 + INIT0×1.5 + Effect4 = 4 (field cleanse ~4)
            new(new Guid("00000000-0000-0000-0007-000000000057"),
                "Cleansing Balm", Rarity.Common, 1, 0, 0, 0, EquipmentSlot.Balm,
                "Remove one negative effect from the field",
                effectTags: "ON_ACTIVATE|REMOVE_NEGATIVE:ALL_ALLIES"),

            // Budget: STR0 + HP0 + INIT0×1.5 + Effect4 = 4 (exile prevention ~4)
            new(new Guid("00000000-0000-0000-0007-000000000058"),
                "Ancestor Totem", Rarity.Common, 1, 0, 0, 0, EquipmentSlot.Totem,
                "Prevent 1 card from being exiled this round",
                effectTags: "ON_ACTIVATE|PREVENT_EXILE:1"),

            // Budget: STR0 + HP0 + INIT0×1.5 + Effect4 = 4 (Ambusher grant ~4)
            new(new Guid("00000000-0000-0000-0007-000000000059"),
                "Potion of Invisibility", Rarity.Common, 1, 0, 0, 0, EquipmentSlot.Potion,
                "Target ally gains Ambusher trait this round",
                effectTags: "ON_ACTIVATE|AMBUSHER:ALLY"),

            // Budget: STR0 + HP0 + INIT0×1.5 + Effect4 = 4 (untargetable ~4)
            new(new Guid("00000000-0000-0000-0007-000000000060"),
                "Scroll of Sanctuary", Rarity.Common, 1, 0, 0, 0, EquipmentSlot.Scroll,
                "One ally cannot be targeted this round",
                effectTags: "ON_ACTIVATE|UNTARGETABLE:ALLY"),
        };
    }
}
