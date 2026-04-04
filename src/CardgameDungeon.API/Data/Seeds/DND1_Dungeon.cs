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
            // ═════════════════════════════════════════════════════════
            // BOSSES (10) — Diverse races, progression mechanics,
            // room interactions, class-like abilities
            // Budget: Cost 5=26, 6=30, 7=34, 8=37, 9=40
            // Races: Dragon x2, Undead x2, Demon x1, Beast x1,
            //        Giant x1, Human x1, Elemental x1, Monstrosity x1
            // ═════════════════════════════════════════════════════════

            // 1. TIAMAT — Dragon — Caverna Vulcanica
            // Budget 34 (cost 7): STR 8(8) + HP 14(14) + INIT 1(1.5) = 23.5. Effect = 8 ✓
            new BossCard(new Guid("00000000-0000-0000-0006-000000000001"),
                "Tiamat, Queen of Dragons", Rarity.Unique, 7, 8, 14, 1,
                "Sala: Caverna Vulcanica — aliados sem Armor tomam +1 dano por round. Progressao: +1 STR por aliado morto no descarte do oponente. Breath: deal 2 damage to all attackers at start of each round. Immune to Dragon-race allies' effects",
                race: Race.Dragon,
                effectTags: "ON_ROUND_START|DAMAGE:2:ALL_ENEMIES\nPASSIVE|+STR:1:PER_DEAD_ALLY\nPASSIVE|IF_RACE:Dragon|IMMUNE"),

            // 2. VECNA — Undead — Cripta Arcana
            // Budget 30 (cost 6): STR 7(7) + HP 12(12) + INIT 1(1.5) = 20.5. Effect = 8 ✓
            new BossCard(new Guid("00000000-0000-0000-0006-000000000002"),
                "Vecna, the Whispered One", Rarity.Unique, 6, 7, 12, 1,
                "Sala: Cripta Arcana — traps nesta sala ativam 2x. Progressao: cada 3 cartas exiladas pelo oponente ao longo da partida: +1 STR permanente. During initiative: marca 1 atacante — se atacar Vecna, exile 2 do deck em vez de 1",
                race: Race.Undead,
                effectTags: "ON_INITIATIVE|MARK_ENEMY:1\nPASSIVE|+STR:1:PER_3_EXILED\nPASSIVE|TRAP_DOUBLE"),

            // 3. STRAHD — Undead — Castelo Sombrio
            // Budget 26 (cost 5): STR 7(7) + HP 10(10) + INIT 1(1.5) = 18.5. Effect = 7.5 ✓
            new BossCard(new Guid("00000000-0000-0000-0006-000000000003"),
                "Strahd von Zarovich", Rarity.Unique, 5, 7, 10, 1,
                "Sala: Castelo Sombrio — todos atacantes -1 STR (escuridao). Progressao: restaura HP igual ao numero de aliados mortos no descarte do oponente (no inicio de cada round). With Advantage: -2 STR ao aliado mais forte. Imune a Scrolls (resistencia magica vampirica)",
                race: Race.Undead,
                effectTags: "PASSIVE|-STR:1:ALL_ENEMIES\nON_ROUND_START|HEAL:1:SELF:PER_DEAD_ALLY\nWITH_ADVANTAGE|-STR:2:ENEMY\nPASSIVE|IMMUNE_SCROLL"),

            // 4. XANATHAR — Aberration — Covil Subterraneo
            // Budget 30 (cost 6): STR 7(7) + HP 13(13) + INIT 1(1.5) = 21.5. Effect = 8 ✓
            new BossCard(new Guid("00000000-0000-0000-0006-000000000004"),
                "Xanathar, the Eye Tyrant", Rarity.Unique, 6, 7, 13, 1,
                "Sala: Covil Subterraneo — aliados nao podem usar retarget. Revela a mao do oponente permanentemente. A cada round: desativa 1 equipamento aleatorio de um atacante (o equipamento volta pra mao). Imune a Scrolls (anti-magia do Beholder)",
                race: Race.Aberration,
                effectTags: "PASSIVE|REVEAL_HAND\nON_ROUND_START|DISABLE_EQUIPMENT:1\nPASSIVE|IMMUNE_SCROLL\nPASSIVE|PREVENT_RETARGET"),

            // 5. DEMOGORGON — Demon — Abismo
            // Budget 34 (cost 7): STR 8(8) + HP 14(14) + INIT 1(1.5) = 23.5. Effect = 8 ✓
            new BossCard(new Guid("00000000-0000-0000-0006-000000000005"),
                "Demogorgon, Prince of Demons", Rarity.Unique, 7, 8, 14, 1,
                "Sala: Abismo — aliados com INIT 0 nao podem atacar (desespero). Progressao: se oponente tem >8 cartas exiladas: imune a consumiveis. Duas cabecas: ataca 2 alvos diferentes por round (divide STR entre eles). Cada round sem ser danificado: +1 STR",
                race: Race.Demon,
                effectTags: "PASSIVE|ATTACK_TWO_TARGETS\nPASSIVE|IF_EXILED_GT_8|IMMUNE_CONSUMABLE\nON_ROUND_START|IF_NO_DAMAGE|+STR:1"),

            // 6. THE KRAKEN — Beast — Oceano Submerso
            // Budget 37 (cost 8): STR 10(10) + HP 16(16) + INIT 0(0) = 26. Effect = 8 ✓
            new BossCard(new Guid("00000000-0000-0000-0006-000000000006"),
                "The Kraken of the Depths", Rarity.Unique, 8, 10, 16, 0,
                "Sala: Oceano Submerso — aliados sem Boots perdem 1 INIT. Afogamento: cada round, oponente descarta 1 da mao ou o aliado com menor HP e exilado. Tentaculos: no inicio do combate, 1 aliado aleatorio e removido do campo e volta pra mao (sem opportunity attack). Tinta: com Desvantagem, ninguem pode usar retarget",
                race: Race.Beast,
                effectTags: "ON_ROUND_START|FORCE_DISCARD_OR_EXILE_WEAKEST\nON_COMBAT_START|RETURN_ALLY_TO_HAND:1\nWITH_DISADVANTAGE|PREVENT_RETARGET\nPASSIVE|IF_NO_BOOTS|-INIT:1"),

            // 7. SNURRE IRONBELLY — Giant — Salao de Fogo
            // Budget 34 (cost 7): STR 9(9) + HP 14(14) + INIT 1(1.5) = 24.5. Effect = 8 ✓
            new BossCard(new Guid("00000000-0000-0000-0006-000000000007"),
                "Snurre Ironbelly, Fire Giant King", Rarity.Unique, 7, 9, 14, 1,
                "Sala: Salao de Fogo — aliados sem Armor ou Shield tomam +1 dano por round. Progressao: cada aliado eliminado neste combate: +2 STR permanente (fica mais forte conforme mata). Forja: equipamentos destruidos neste combate concedem +1 HP a Snurre",
                race: Race.Giant,
                effectTags: "ON_KILL|+STR:2\nPASSIVE|IF_NO_ARMOR|DAMAGE:1:ALL_ENEMIES\nON_EQUIPMENT_DESTROYED|+HP:1"),

            // 8. HALASTER BLACKCLOAK — Human — Labirinto Magico
            // Budget 37 (cost 8): STR 8(8) + HP 15(15) + INIT 2(3) = 26. Effect = 8 ✓
            new BossCard(new Guid("00000000-0000-0000-0006-000000000008"),
                "Halaster Blackcloak", Rarity.Unique, 8, 8, 15, 2,
                "Sala: Labirinto Magico — assignments sao embaralhados no inicio de cada round. Copia: o ultimo consumivel usado pelo oponente e copiado e aplicado contra os atacantes. Progressao: se oponente tem <3 cartas na mao: +3 STR. Teleporte: uma vez por combate, troca de posicao com 1 monstro da pilha de descarte (substitui-se no campo)",
                race: Race.Human,
                effectTags: "ON_ROUND_START|SWAP_ASSIGNMENTS\nON_SCROLL_USED|COPY_SCROLL:REVERSE\nPASSIVE|IF_HAND_LT_3|+STR:3\nON_ACTIVATE|ONCE_PER_COMBAT|SWAP_WITH_MONSTER"),

            // 9. ZARATAN — Elemental — Pantano Primordial
            // Budget 40 (cost 9): STR 8(8) + HP 20(20) + INIT 0(0) = 28. Effect = 8 ✓
            new BossCard(new Guid("00000000-0000-0000-0006-000000000009"),
                "Zaratan, the World Turtle", Rarity.Unique, 9, 8, 20, 0,
                "Sala: Pantano Primordial — aliados com custo 1 nao podem entrar neste combate (afundam no pantano). Carapaca: reduz TODO dano recebido em 3. So pode ser eliminado pela regra de STR >= 2x (nao por dano >= HP). Terremoto: a cada 2 rounds, exile 2 do topo do deck de AMBOS os jogadores. Lento: INIT 0 — nunca ganha iniciativa",
                race: Race.Elemental,
                effectTags: "PASSIVE|REDUCE_DAMAGE:3\nPASSIVE|ONLY_ELIM_BY_DOUBLE\nON_ROUND_START|IF_EVERY_2_ROUNDS|EXILE_DECK:2:BOTH\nPASSIVE|IF_COST_1|PREVENT_ATTACK"),

            // 10. ARAKTHISS — Monstrosity — Ninho de Aranhas
            // Budget 30 (cost 6): STR 6(6) + HP 12(12) + INIT 2(3) = 21. Effect = 8 ✓
            new BossCard(new Guid("00000000-0000-0000-0006-000000000010"),
                "Arakthiss, the Broodmother", Rarity.Unique, 6, 6, 12, 2,
                "Sala: Ninho de Aranhas — teias: aliados com INIT < 2 nao podem atacar. Ninhada: spawna 1 monstro Spider (STR 2, HP 2) no inicio de cada round. Veneno: aliados sem Potion ou Balm na mao perdem 1 HP por round. Progressao: cada Spider viva concede +1 STR a Arakthiss",
                race: Race.Monstrosity,
                effectTags: "PASSIVE|IF_INIT_LT_2|PREVENT_ATTACK\nON_ROUND_START|SPAWN_SPIDER:1\nON_ROUND_START|IF_NO_POTION_BALM|DAMAGE:1:ALL_ENEMIES\nPASSIVE|+STR:1:PER_SPIDER"),
        };
    }
}
