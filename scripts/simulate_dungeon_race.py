from __future__ import annotations

import argparse
import random
import re
from dataclasses import dataclass
from pathlib import Path

import generate_sd_card_art as seed_parser


@dataclass
class PlayerState:
    health: int = 100
    exile_count: int = 0
    progress: int = 0
    attack_turns: int = 0
    rooms_advanced: int = 0
    entered_boss_on_turn: int | None = None
    took_damage_last_turn: bool = False
    trap_revealed_last_turn: bool = False
    hand_size: int = 5


@dataclass
class EncounterContext:
    room_order: int
    attackers_count: int
    total_creatures: int
    attacker_initiative: int
    attacker_attack: int
    attacker_cost: int
    attacker_exile: int
    played_consumable: bool
    changed_target: bool
    used_retarget: bool
    took_damage_earlier: bool
    has_equipment: bool
    has_shield: bool
    has_boots: bool
    has_balm: bool
    has_weapon: bool
    has_accessory: bool
    has_ranged_weapon: bool
    target_is_ambusher: bool
    fire_source_active: bool
    light_source_active: bool
    cards_in_hand: int
    attempted_ambush: bool
    trap_revealed_earlier: bool
    loot_attempt: bool
    treasure_gain_pending: bool
    undead_or_fiend_present: bool
    front_line_attacker: bool


class DeckState:
    def __init__(self, cards: list[seed_parser.CardDefinition], rng: random.Random) -> None:
        self.rng = rng
        self.draw_pile = list(cards)
        self.rng.shuffle(self.draw_pile)
        self.hand: list[seed_parser.CardDefinition] = []
        self.discard: list[seed_parser.CardDefinition] = []

    def draw_to(self, target_size: int) -> None:
        while len(self.hand) < target_size:
            if not self.draw_pile:
                if not self.discard:
                    return
                self.draw_pile = self.discard
                self.discard = []
                self.rng.shuffle(self.draw_pile)
            self.hand.append(self.draw_pile.pop())

    def play_cards(self, cards: list[seed_parser.CardDefinition]) -> None:
        for card in cards:
            if card in self.hand:
                self.hand.remove(card)
                self.discard.append(card)

    def hand_size(self) -> int:
        return len(self.hand)


def card_score_attacker(card: seed_parser.CardDefinition) -> float:
    if card.card_type == "Ally":
        return float((card.attack or 0) + (card.hit_points or 0) + 0.35 * (card.initiative or 0))
    if card.card_type == "Equipment":
        return float((card.attack_mod or 0) + (card.hit_points_mod or 0) + 0.25 * (card.initiative_mod or 0))
    return 0.0


def card_score_defender(card: seed_parser.CardDefinition) -> float:
    if card.card_type == "Monster":
        return float((card.attack or 0) + (card.hit_points or 0) + 0.25 * (card.initiative or 0))
    if card.card_type == "Trap":
        return float((card.damage or 0) * 1.1)
    return 0.0


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(
        description="Simulate dungeon race matches for DND1 balance telemetry."
    )
    parser.add_argument("--set-code", default="DND1")
    parser.add_argument("--matches", type=int, default=8000)
    parser.add_argument("--seed", type=int, default=20260403)
    parser.add_argument(
        "--trap-mode",
        choices=["always", "conditional", "off", "all"],
        default="all",
        help="always: trap always applies; conditional: use trap text activation checks; off: disable trap.",
    )
    return parser.parse_args()


def load_trap_activation_conditions(seeds_root: Path) -> dict[str, str]:
    traps_file = seeds_root / "DND1_Traps.cs"
    if not traps_file.exists():
        return {}

    content = traps_file.read_text(encoding="utf-8")
    pattern = re.compile(r'\["([^"]+)"\]\s*=\s*"([^"]+)"')
    return {name: condition for name, condition in pattern.findall(content)}


def load_cards(set_code: str) -> dict[str, list[seed_parser.CardDefinition]]:
    cards = [c for c in seed_parser.parse_seed_cards(seed_parser.SEEDS_ROOT) if c.set_code == set_code]
    trap_conditions = load_trap_activation_conditions(seed_parser.SEEDS_ROOT)

    # Boss cards are declared in DND1_Dungeon.cs and may not be captured by parse_seed_cards.
    dungeon_file = seed_parser.SEEDS_ROOT / "DND1_Dungeon.cs"
    if dungeon_file.exists():
        content = dungeon_file.read_text(encoding="utf-8")
        pattern = re.compile(r"new\s+BossCard\s*\(")
        for match in pattern.finditer(content):
            open_index = content.find("(", match.start())
            args_text, _ = seed_parser.extract_balanced(content, open_index, "(", ")")
            parsed = seed_parser.parse_typed_constructor_args("BossCard", args_text)
            cards.append(seed_parser.build_card_definition(set_code, "Boss", parsed))

    grouped: dict[str, list[seed_parser.CardDefinition]] = {
        "ally": [],
        "monster": [],
        "trap": [],
        "equipment": [],
        "room": [],
        "boss": [],
    }

    for card in cards:
        if card.card_type == "Ally" and all(
            isinstance(v, int) for v in (card.attack, card.hit_points, card.initiative, card.cost)
        ):
            grouped["ally"].append(card)
        elif card.card_type == "Monster" and all(
            isinstance(v, int) for v in (card.attack, card.hit_points, card.initiative, card.cost)
        ):
            grouped["monster"].append(card)
        elif card.card_type == "Trap" and isinstance(card.damage, int) and isinstance(card.cost, int):
            # parser can produce synthetic rows from foreach variables; skip non-literal names.
            if "." not in card.name:
                if card.name in trap_conditions and "Activation condition:" not in (card.effect or ""):
                    card.effect = f"{trap_conditions[card.name]} {card.effect or ''}".strip()
                grouped["trap"].append(card)
        elif card.card_type == "Equipment" and all(
            isinstance(v, int) for v in (card.attack_mod, card.hit_points_mod, card.initiative_mod, card.cost)
        ):
            grouped["equipment"].append(card)
        elif card.card_type == "DungeonRoom" and isinstance(card.room_order, int) and isinstance(
            card.monster_cost_budget, int
        ):
            grouped["room"].append(card)
        elif card.card_type == "Boss" and all(
            isinstance(v, int) for v in (card.attack, card.hit_points, card.initiative, card.cost)
        ):
            grouped["boss"].append(card)

    return grouped


def pick_defenders(
    monsters: list[seed_parser.CardDefinition],
    room_budget: int,
    rng: random.Random,
) -> list[seed_parser.CardDefinition]:
    defenders: list[seed_parser.CardDefinition] = []
    remaining = max(1, room_budget)
    max_monsters = 4

    for _ in range(max_monsters):
        affordable = [m for m in monsters if m.cost and m.cost <= max(remaining, 2)]
        if not affordable:
            break
        pick = rng.choice(affordable)
        defenders.append(pick)
        remaining -= pick.cost or 0
        if remaining <= 0:
            break

    if not defenders:
        defenders.append(rng.choice(monsters))

    return defenders


def sample_room_path(
    rooms: list[seed_parser.CardDefinition],
    rng: random.Random,
) -> list[seed_parser.CardDefinition]:
    by_order: dict[int, list[seed_parser.CardDefinition]] = {i: [] for i in range(1, 6)}
    for room in rooms:
        if room.room_order in by_order:
            by_order[room.room_order].append(room)
    return [rng.choice(by_order[i]) for i in range(1, 6)]


def is_eliminated(own_attack: int, opponent_attack: int, own_hp: int) -> bool:
    return opponent_attack >= own_attack * 2 or opponent_attack >= own_hp


def attacker_wins(att_str: int, att_hp: int, def_str: int, def_hp: int) -> bool:
    attacker_eliminated = is_eliminated(att_str, def_str, att_hp)
    defender_eliminated = is_eliminated(def_str, att_str, def_hp)

    if attacker_eliminated and not defender_eliminated:
        return False
    if not attacker_eliminated and defender_eliminated:
        return True
    if attacker_eliminated and defender_eliminated:
        return False
    if att_str == def_str:
        return False
    return att_str > def_str


def trap_triggers(
    trap: seed_parser.CardDefinition,
    context: EncounterContext,
    trap_mode: str,
) -> bool:
    if trap_mode == "off":
        return False
    if trap_mode == "always":
        return True

    effect = trap.effect or ""
    marker = "Activation condition:"
    if marker not in effect:
        return True

    condition = effect.split(marker, 1)[1].split(".", 1)[0].strip().lower()

    if "12 or more cards in exile" in condition:
        return context.attacker_exile >= 12
    if "11 or more cards in exile" in condition:
        return context.attacker_exile >= 11
    if "10 or more cards in exile" in condition:
        return context.attacker_exile >= 10
    if "initiative 4 or higher" in condition:
        return context.attacker_initiative >= 4
    if "initiative 3 or higher" in condition:
        return context.attacker_initiative >= 3
    if "initiative 3 or lower" in condition:
        return context.attacker_initiative <= 3
    if "current attack 5 or higher" in condition:
        return context.attacker_attack >= 5
    if "current attack 4 or higher" in condition:
        return context.attacker_attack >= 4
    if "cost 2 or lower" in condition:
        return context.attacker_cost <= 2
    if "target is ambusher" in condition:
        return context.target_is_ambusher
    if "target is not ambusher" in condition:
        return not context.target_is_ambusher
    if "played a consumable this round" in condition:
        return context.played_consumable
    if "changed target this round" in condition or "changed lane or target this round" in condition:
        return context.changed_target
    if "used retarget this round" in condition:
        return context.used_retarget
    if "took damage earlier this room" in condition:
        return context.took_damage_earlier
    if "at least 1 equipment attached" in condition:
        return context.has_equipment
    if "from room 3 onward" in condition:
        return context.room_order >= 3
    if "from room 2 onward" in condition:
        return context.room_order >= 2
    if "3 or more allies are currently in play" in condition or "3 or more allies are in play" in condition:
        return context.attackers_count >= 3
    if "2 or more allies are currently in play" in condition:
        return context.attackers_count >= 2
    if "3 or more total creatures are in combat" in condition:
        return context.total_creatures >= 3
    if "attacker attempted to ambush this room" in condition or "attacker declared an ambush" in condition:
        return context.attempted_ambush
    if "no trap was revealed earlier this room" in condition:
        return not context.trap_revealed_earlier
    if "target has ranged weapon equipment" in condition:
        return context.has_ranged_weapon
    if "undead or fiend monster is in play" in condition:
        return context.undead_or_fiend_present
    if "target has metal equipment attached" in condition:
        return context.has_equipment
    if "fire source is active in the room" in condition or "any fire effect was used this room" in condition:
        return context.fire_source_active
    if "attacker has no boots-slot equipment" in condition:
        return not context.has_boots
    if "no torch or lantern effect is active" in condition or "no light source is active in the room" in condition:
        return not context.light_source_active
    if "target has weapon-slot equipment" in condition:
        return context.has_weapon
    if "target has no shield equipped" in condition:
        return not context.has_shield
    if "target has accessory-slot equipment" in condition:
        return context.has_accessory
    if "attacker has no balm equipped" in condition or "no active potion or balm" in condition:
        return not context.has_balm
    if "attacker has 2 or more cards in hand" in condition:
        return context.cards_in_hand >= 2
    if "target has no shield-slot equipment" in condition:
        return not context.has_shield
    if "first attacking ally of the round" in condition:
        return True
    if "target has current attack 4 or higher" in condition:
        return context.attacker_attack >= 4
    if "target has current attack 5 or higher" in condition:
        return context.attacker_attack >= 5
    if "front-line attacker" in condition:
        return context.front_line_attacker
    if "attacker attempts to loot this room" in condition:
        return context.loot_attempt
    if "treasure gain is about to resolve" in condition:
        return context.treasure_gain_pending

    # fallback: if condition is unknown, keep trap harder to trigger rather than always-on.
    return False


def run_duel_estimate(
    cards: dict[str, list[seed_parser.CardDefinition]],
    rng: random.Random,
    trap_mode: str,
    iterations: int = 150000,
) -> float:
    allies = cards["ally"]
    monsters = cards["monster"]
    traps = cards["trap"]
    gear = [e for e in cards["equipment"] if e.slot not in {"Scroll", "Potion", "Balm", "Bomb", "Totem"}]
    consumables = [e for e in cards["equipment"] if e.slot in {"Scroll", "Potion", "Balm", "Bomb", "Totem"}]

    wins = 0
    for _ in range(iterations):
        ally = rng.choice(allies)
        monster = rng.choice(monsters)
        g = rng.choice(gear)
        c = rng.choice(consumables)
        trap = rng.choice(traps)

        att_str = (ally.attack or 0) + (g.attack_mod or 0) + (c.attack_mod or 0)
        att_hp = (ally.hit_points or 0) + (g.hit_points_mod or 0) + (c.hit_points_mod or 0)
        att_init = (ally.initiative or 0) + (g.initiative_mod or 0) + (c.initiative_mod or 0)
        def_str = monster.attack or 0
        def_hp = monster.hit_points or 0

        context = EncounterContext(
            room_order=3,
            attackers_count=1,
            total_creatures=2,
            attacker_initiative=att_init,
            attacker_attack=att_str,
            attacker_cost=ally.cost or 1,
            attacker_exile=6,
            played_consumable=True,
            changed_target=False,
            used_retarget=False,
            took_damage_earlier=False,
            has_equipment=True,
            has_shield=(g.slot == "Shield"),
            has_boots=(g.slot == "Boots"),
            has_balm=(c.slot == "Balm"),
            has_weapon=(g.slot == "Weapon"),
            has_accessory=(g.slot == "Accessory"),
            has_ranged_weapon=any(k in (g.name or "").lower() for k in ("bow", "crossbow", "sling")),
            target_is_ambusher=bool(ally.is_ambusher),
            fire_source_active=False,
            light_source_active=True,
            cards_in_hand=4,
            attempted_ambush=bool(ally.is_ambusher),
            trap_revealed_earlier=False,
            loot_attempt=False,
            treasure_gain_pending=False,
            undead_or_fiend_present=False,
            front_line_attacker=True,
        )

        if trap_triggers(trap, context, trap_mode):
            att_hp -= trap.damage or 0

        if attacker_wins(max(1, att_str), max(1, att_hp), max(1, def_str), max(1, def_hp)):
            wins += 1

    return wins / iterations


def run_match_simulation(
    cards: dict[str, list[seed_parser.CardDefinition]],
    rng: random.Random,
    trap_mode: str,
    matches: int,
) -> dict[str, float]:
    allies = cards["ally"]
    monsters = cards["monster"]
    traps = cards["trap"]
    rooms = cards["room"]
    bosses = cards["boss"]
    gear = [e for e in cards["equipment"] if e.slot not in {"Scroll", "Potion", "Balm", "Bomb", "Totem"}]
    consumables = [e for e in cards["equipment"] if e.slot in {"Scroll", "Potion", "Balm", "Bomb", "Totem"}]

    total_attack_turns = 0
    total_rooms_advanced = 0
    boss_entry_turns: list[int] = []
    missed_opportunity_players = 0
    missed_opportunity_then_win = 0
    player0_wins = 0
    attacker_wins_total = 0
    total_attack_cards_played = 0
    total_defense_cards_played = 0
    total_defense_monsters_played = 0
    total_defense_traps_played = 0
    trap_heavy_encounters = 0
    trap_heavy_attacker_wins = 0

    for _ in range(matches):
        players = [PlayerState(), PlayerState()]
        dungeons = [sample_room_path(rooms, rng), sample_room_path(rooms, rng)]
        bosses_for_match = [rng.choice(bosses), rng.choice(bosses)]
        first_missed = [False, False]
        winner: int | None = None
        # Each player owns an adventurer deck (ally+equipment) and an enemy deck (monster+trap).
        player_adv_decks: list[DeckState] = []
        player_enemy_decks: list[DeckState] = []
        for _player in (0, 1):
            ally_count = rng.randint(22, 28)
            equip_count = 40 - ally_count
            trap_count = rng.randint(10, 22)
            monster_count = 40 - trap_count

            adv_cards = [rng.choice(allies) for _i in range(ally_count)] + [
                rng.choice(cards["equipment"]) for _i in range(equip_count)
            ]
            enemy_cards = [rng.choice(monsters) for _i in range(monster_count)] + [
                rng.choice(traps) for _i in range(trap_count)
            ]
            player_adv_decks.append(DeckState(adv_cards, rng))
            player_enemy_decks.append(DeckState(enemy_cards, rng))

        max_turns = 80
        attacker_index = rng.randint(0, 1)

        for _turn in range(max_turns):
            defender_index = 1 - attacker_index
            attacker = players[attacker_index]
            attacker.attack_turns += 1
            total_attack_turns += 1

            # Simple rest policy: if badly hurt, recover instead of pushing.
            room_order = min(attacker.progress + 1, 6)
            rest_threshold = 26 + room_order * 2
            if attacker.health <= rest_threshold:
                attacker.health = min(100, attacker.health + 18)
                attacker.hand_size = min(8, attacker.hand_size + 2)
                attacker.took_damage_last_turn = False
                attacker_index = defender_index
                continue

            adv_deck = player_adv_decks[attacker_index]
            enemy_deck = player_enemy_decks[defender_index]
            adv_deck.draw_to(8)
            enemy_deck.draw_to(8)
            attacker.hand_size = adv_deck.hand_size()

            attack_slots = min(5, adv_deck.hand_size(), rng.randint(3, 5))
            ranked_attack = sorted(adv_deck.hand, key=card_score_attacker, reverse=True)
            selected_attack = ranked_attack[:attack_slots]
            ally_in_selection = [c for c in selected_attack if c.card_type == "Ally"]
            ally_in_hand = [c for c in adv_deck.hand if c.card_type == "Ally"]

            # Guarantee at least one ally card if available in hand.
            if not ally_in_selection and ally_in_hand and selected_attack:
                best_ally = max(ally_in_hand, key=card_score_attacker)
                selected_attack[-1] = best_ally
                ally_in_selection = [best_ally]

            if not ally_in_selection:
                if not first_missed[attacker_index]:
                    first_missed[attacker_index] = True
                attacker_index = defender_index
                continue

            # Keep ally count bounded by field limit and selected slots.
            attacking_team = ally_in_selection[:5]
            selected_equipment = [c for c in selected_attack if c.card_type == "Equipment"]
            selected_gear = [e for e in selected_equipment if e.slot not in {"Scroll", "Potion", "Balm", "Bomb", "Totem"}]
            selected_consumables = [
                e for e in selected_equipment if e.slot in {"Scroll", "Potion", "Balm", "Bomb", "Totem"}
            ]
            # Cap support cards by allies in combat to avoid unrealistic full-stacking from hand.
            max_support = len(attacking_team) + 1
            selected_gear = selected_gear[:max_support]
            selected_consumables = selected_consumables[:max_support]
            cards_to_play = attacking_team + selected_gear + selected_consumables
            adv_deck.play_cards(cards_to_play)
            total_attack_cards_played += len(cards_to_play)

            team_size = len(attacking_team)
            base_att_str = sum(a.attack or 0 for a in attacking_team)
            base_att_hp = sum(a.hit_points or 0 for a in attacking_team)
            base_att_init = sum(a.initiative or 0 for a in attacking_team)
            avg_att_cost = max(1, sum(a.cost or 1 for a in attacking_team) // max(1, team_size))
            target_is_ambusher = bool(rng.choice(attacking_team).is_ambusher)

            gear_str = sum(e.attack_mod or 0 for e in selected_gear)
            gear_hp = sum(e.hit_points_mod or 0 for e in selected_gear)
            gear_init = sum(e.initiative_mod or 0 for e in selected_gear)
            cons_str = sum(e.attack_mod or 0 for e in selected_consumables)
            cons_hp = sum(e.hit_points_mod or 0 for e in selected_consumables)
            cons_init = sum(e.initiative_mod or 0 for e in selected_consumables)

            att_str = base_att_str + gear_str + cons_str
            att_hp = base_att_hp + gear_hp + cons_hp
            att_init = base_att_init + gear_init + cons_init

            if attacker.progress < 5:
                current_room = dungeons[attacker_index][attacker.progress]
                defense_slots = min(5, enemy_deck.hand_size(), rng.randint(3, 5))
                ranked_defense = sorted(enemy_deck.hand, key=card_score_defender, reverse=True)
                selected_defense = ranked_defense[:defense_slots]
                selected_monsters = [c for c in selected_defense if c.card_type == "Monster"]
                selected_traps = [c for c in selected_defense if c.card_type == "Trap"]

                # Ensure at least one monster if hand has one.
                monsters_in_hand = [c for c in enemy_deck.hand if c.card_type == "Monster"]
                if not selected_monsters and monsters_in_hand:
                    best_monster = max(monsters_in_hand, key=card_score_defender)
                    if selected_defense:
                        selected_defense[-1] = best_monster
                    else:
                        selected_defense.append(best_monster)
                    selected_monsters = [best_monster]
                    selected_traps = [c for c in selected_defense if c.card_type == "Trap"]

                # Constrain monster deployment by room budget.
                room_budget = current_room.monster_cost_budget or 6
                defenders: list[seed_parser.CardDefinition] = []
                used_budget = 0
                for monster in sorted(selected_monsters, key=card_score_defender, reverse=True):
                    m_cost = monster.cost or 1
                    if not defenders or used_budget + m_cost <= room_budget:
                        defenders.append(monster)
                        used_budget += m_cost
                if not defenders and selected_monsters:
                    defenders.append(min(selected_monsters, key=lambda m: m.cost or 1))

                enemy_deck.play_cards(selected_defense)
                total_defense_cards_played += len(selected_defense)
                total_defense_monsters_played += len(defenders)
                total_defense_traps_played += len(selected_traps)

                def_str = sum(m.attack or 0 for m in defenders)
                def_hp = sum(m.hit_points or 0 for m in defenders)
                def_init = sum(m.initiative or 0 for m in defenders)
                trap = max(selected_traps, key=lambda t: t.damage or 0) if selected_traps else None

                has_shield = any(e.slot == "Shield" for e in selected_gear)
                has_boots = any(e.slot == "Boots" for e in selected_gear)
                has_weapon = any(e.slot == "Weapon" for e in selected_gear)
                has_accessory = any(e.slot == "Accessory" for e in selected_gear)
                has_balm = any(e.slot == "Balm" for e in selected_consumables)
                has_equipment = bool(selected_gear or selected_consumables)
                has_ranged_weapon = any(
                    any(tag in (e.name or "").lower() for tag in ("bow", "crossbow", "sling"))
                    for e in selected_gear
                )
                fire_source_active = any(
                    any(tag in (e.name or "").lower() for tag in ("fire", "flame"))
                    for e in selected_consumables
                )
                light_source_active = not any(
                    any(tag in (current_room.effect or "").lower() for tag in ("dark", "shadow", "night"))
                    for _ in (0,)
                )
                undead_or_fiend_present = any(
                    any(tag in (m.name or "").lower() for tag in ("undead", "demon", "devil", "wraith", "lich"))
                    for m in defenders
                )

                context = EncounterContext(
                    room_order=current_room.room_order or room_order,
                    attackers_count=team_size,
                    total_creatures=team_size + len(defenders),
                    attacker_initiative=att_init,
                    attacker_attack=att_str,
                    attacker_cost=avg_att_cost,
                    attacker_exile=attacker.exile_count,
                    played_consumable=len(selected_consumables) > 0,
                    changed_target=rng.random() < 0.30,
                    used_retarget=rng.random() < 0.25,
                    took_damage_earlier=attacker.took_damage_last_turn,
                    has_equipment=has_equipment,
                    has_shield=has_shield,
                    has_boots=has_boots,
                    has_balm=has_balm,
                    has_weapon=has_weapon,
                    has_accessory=has_accessory,
                    has_ranged_weapon=has_ranged_weapon,
                    target_is_ambusher=target_is_ambusher,
                    fire_source_active=fire_source_active,
                    light_source_active=light_source_active,
                    cards_in_hand=attacker.hand_size,
                    attempted_ambush=rng.random() < 0.25,
                    trap_revealed_earlier=attacker.trap_revealed_last_turn,
                    loot_attempt=rng.random() < 0.35,
                    treasure_gain_pending=rng.random() < 0.30,
                    undead_or_fiend_present=undead_or_fiend_present,
                    front_line_attacker=True,
                )

                trap_applied = bool(trap) and trap_triggers(trap, context, trap_mode)
                if trap_applied:
                    att_hp -= trap.damage or 0
                    if (trap.damage or 0) >= 3:
                        att_init -= 1
                attacker.trap_revealed_last_turn = trap_applied

                trap_heavy = len(selected_traps) > len(defenders)
                if trap_heavy:
                    trap_heavy_encounters += 1

                # Initiative edge as a light swing.
                if att_init > def_init:
                    att_str += 1
                elif att_init < def_init:
                    def_str += 1

                if not defenders:
                    did_advance = att_hp > 0
                else:
                    did_advance = attacker_wins(
                        max(1, att_str),
                        max(1, att_hp),
                        max(1, def_str),
                        max(1, def_hp),
                    )

                attacker.hand_size = adv_deck.hand_size()
                exile_spend = max(1, (sum(a.cost or 1 for a in attacking_team) + len(selected_gear) + len(selected_consumables)) // 3)
                attacker.exile_count += exile_spend + (1 if trap_applied else 0) + (2 if not did_advance else 0)

                if did_advance:
                    attacker.progress += 1
                    attacker.rooms_advanced += 1
                    total_rooms_advanced += 1
                    attacker_wins_total += 1
                    attacker.health -= max(2, def_str // 3)
                    attacker.took_damage_last_turn = True
                    if trap_heavy:
                        trap_heavy_attacker_wins += 1
                    if attacker.progress >= 5 and attacker.entered_boss_on_turn is None:
                        attacker.entered_boss_on_turn = attacker.attack_turns
                        boss_entry_turns.append(attacker.attack_turns)
                else:
                    attacker.health -= max(4, def_str // 2)
                    attacker.took_damage_last_turn = True
                    if not first_missed[attacker_index]:
                        first_missed[attacker_index] = True

            else:
                boss = bosses_for_match[attacker_index]
                def_str = boss.attack or 0
                def_hp = boss.hit_points or 0
                def_init = boss.initiative or 0
                if att_init > def_init:
                    att_str += 1
                elif att_init < def_init:
                    def_str += 1

                beat_boss = attacker_wins(
                    max(1, att_str),
                    max(1, att_hp),
                    max(1, def_str),
                    max(1, def_hp),
                )
                attacker.hand_size = adv_deck.hand_size()
                attacker.exile_count += max(
                    2,
                    (sum(a.cost or 1 for a in attacking_team) + len(selected_gear) + len(selected_consumables)) // 3,
                )
                if beat_boss:
                    winner = attacker_index
                    attacker_wins_total += 1
                    break
                attacker.health -= max(6, def_str // 2)
                attacker.took_damage_last_turn = True
                if not first_missed[attacker_index]:
                    first_missed[attacker_index] = True

            if attacker.health <= 0:
                # collapse and forced rest by opposition pressure.
                attacker.health = 18
                attacker.hand_size = min(8, attacker.hand_size + 2)

            attacker_index = defender_index

        if winner is None:
            if players[0].progress != players[1].progress:
                winner = 0 if players[0].progress > players[1].progress else 1
            elif players[0].health != players[1].health:
                winner = 0 if players[0].health > players[1].health else 1
            else:
                winner = rng.randint(0, 1)

        if winner == 0:
            player0_wins += 1

        for player_index in (0, 1):
            if first_missed[player_index]:
                missed_opportunity_players += 1
                if winner == player_index:
                    missed_opportunity_then_win += 1

    rooms_advanced_per_turn = total_rooms_advanced / max(1, total_attack_turns)
    mean_turns_to_boss = (sum(boss_entry_turns) / len(boss_entry_turns)) if boss_entry_turns else 0.0
    missed_then_win_rate = missed_opportunity_then_win / max(1, missed_opportunity_players)
    avg_attack_cards = total_attack_cards_played / max(1, total_attack_turns)
    avg_def_cards = total_defense_cards_played / max(1, total_attack_turns)
    avg_def_monsters = total_defense_monsters_played / max(1, total_attack_turns)
    avg_def_traps = total_defense_traps_played / max(1, total_attack_turns)
    trap_heavy_rate = trap_heavy_encounters / max(1, total_attack_turns)
    trap_heavy_attacker_win_rate = trap_heavy_attacker_wins / max(1, trap_heavy_encounters)

    return {
        "matches": float(matches),
        "player0_win_rate": player0_wins / matches,
        "attacker_win_rate": attacker_wins_total / max(1, total_attack_turns),
        "rooms_advanced_per_turn": rooms_advanced_per_turn,
        "mean_attack_turns_to_boss_room": mean_turns_to_boss,
        "missed_opportunity_pass_winrate": missed_then_win_rate,
        "players_with_missed_opportunity": float(missed_opportunity_players),
        "avg_attack_cards_played": avg_attack_cards,
        "avg_defense_cards_played": avg_def_cards,
        "avg_defense_monsters_played": avg_def_monsters,
        "avg_defense_traps_played": avg_def_traps,
        "trap_heavy_encounter_rate": trap_heavy_rate,
        "trap_heavy_attacker_win_rate": trap_heavy_attacker_win_rate,
    }


def print_report(
    trap_mode: str,
    duel_estimate: float,
    match_metrics: dict[str, float],
) -> None:
    print(f"=== Trap Mode: {trap_mode} ===")
    print(f"Duel estimate (ally winrate, gear+cons+trap mode): {duel_estimate:.4f}")
    print(f"Match simulation player0 winrate: {match_metrics['player0_win_rate']:.4f}")
    print(f"Encounter attacker winrate: {match_metrics['attacker_win_rate']:.4f}")
    print(f"Rooms advanced per attacking turn: {match_metrics['rooms_advanced_per_turn']:.4f}")
    print(
        "Mean attacking turns to reach boss room: "
        f"{match_metrics['mean_attack_turns_to_boss_room']:.2f}"
    )
    print(
        "Winrate after missed opportunity (pass turn after failing to advance): "
        f"{match_metrics['missed_opportunity_pass_winrate']:.4f}"
    )
    print(
        "Players with at least one missed opportunity event: "
        f"{int(match_metrics['players_with_missed_opportunity'])}"
    )
    print(
        "Avg cards played per attack/defense turn: "
        f"{match_metrics['avg_attack_cards_played']:.2f} / {match_metrics['avg_defense_cards_played']:.2f}"
    )
    print(
        "Avg defense composition (monsters/traps): "
        f"{match_metrics['avg_defense_monsters_played']:.2f} / {match_metrics['avg_defense_traps_played']:.2f}"
    )
    print(
        "Trap-heavy defense encounter rate: "
        f"{match_metrics['trap_heavy_encounter_rate']:.4f}"
    )
    print(
        "Attacker winrate when defense is trap-heavy: "
        f"{match_metrics['trap_heavy_attacker_win_rate']:.4f}"
    )
    print("")


def main() -> int:
    args = parse_args()
    cards = load_cards(args.set_code)
    rng = random.Random(args.seed)

    modes = [args.trap_mode] if args.trap_mode != "all" else ["always", "conditional", "off"]

    if not all(cards[key] for key in ("ally", "monster", "trap", "equipment", "room", "boss")):
        raise SystemExit("Missing required DND1 pools for simulation.")

    for mode in modes:
        duel_rng = random.Random(rng.randint(1, 10_000_000))
        match_rng = random.Random(rng.randint(1, 10_000_000))
        duel_est = run_duel_estimate(cards, duel_rng, mode)
        metrics = run_match_simulation(cards, match_rng, mode, args.matches)
        print_report(mode, duel_est, metrics)

    return 0


if __name__ == "__main__":
    raise SystemExit(main())
