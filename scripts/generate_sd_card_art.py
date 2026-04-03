from __future__ import annotations

import argparse
import base64
import json
import re
import sys
from dataclasses import asdict, dataclass
from pathlib import Path
from typing import Any
from urllib import error, request


ROOT = Path(__file__).resolve().parents[1]
STYLE_GUIDE_PATH = ROOT / "STYLE_GUIDE_DND_DUNGEON.md"
SEEDS_ROOT = ROOT / "src" / "CardgameDungeon.API" / "Data" / "Seeds"
OUTPUT_ROOT = ROOT / "art-output" / "stable-diffusion"

CARD_TYPE_ORDER = ["Equipment", "Ally", "Monster", "Trap", "DungeonRoom", "Boss"]

RARITY_DISTRIBUTION_60 = (
    ["Unique"] * 2
    + ["Rare"] * 8
    + ["Uncommon"] * 20
    + ["Common"] * 30
)

TYPE_TO_CARD_KIND = {
    "AllyCard": "Ally",
    "EquipmentCard": "Equipment",
    "MonsterCard": "Monster",
    "TrapCard": "Trap",
    "DungeonRoomCard": "DungeonRoom",
    "BossCard": "Boss",
}

TYPE_FIELD_ORDER = {
    "AllyCard": [
        "_guid",
        "name",
        "rarity",
        "cost",
        "strength",
        "hit_points",
        "initiative",
        "is_ambusher",
        "treasure",
        "effect",
    ],
    "EquipmentCard": [
        "_guid",
        "name",
        "rarity",
        "cost",
        "strength_mod",
        "hit_points_mod",
        "initiative_mod",
    ],
    "MonsterCard": [
        "_guid",
        "name",
        "rarity",
        "cost",
        "strength",
        "hit_points",
        "initiative",
        "treasure",
        "effect",
    ],
    "TrapCard": [
        "_guid",
        "name",
        "rarity",
        "cost",
        "damage",
        "effect",
    ],
    "DungeonRoomCard": [
        "_guid",
        "name",
        "rarity",
        "room_order",
        "_unused_a",
        "_unused_b",
        "monster_cost_budget",
        "effect",
    ],
    "BossCard": [
        "_guid",
        "name",
        "rarity",
        "cost",
        "strength",
        "hit_points",
        "initiative",
        "effect",
    ],
}

TYPE_FIELD_ALIASES = {
    "hitPoints": "hit_points",
    "isAmbusher": "is_ambusher",
    "strMod": "strength_mod",
    "hpMod": "hit_points_mod",
    "initMod": "initiative_mod",
    "monsterCostBudget": "monster_cost_budget",
}

THEME_FIELD_ORDER = {
    "AllyTheme": [
        "name",
        "strength",
        "hit_points",
        "initiative",
        "cost",
        "treasure",
        "is_ambusher",
        "effect",
    ],
    "EquipmentTheme": [
        "name",
        "cost",
        "strength_mod",
        "hit_points_mod",
        "initiative_mod",
    ],
    "MonsterTheme": [
        "name",
        "strength",
        "hit_points",
        "initiative",
        "cost",
        "treasure",
        "effect",
    ],
    "TrapTheme": [
        "name",
        "cost",
        "damage",
        "effect",
    ],
    "RoomTheme": [
        "name",
        "monster_cost_budget",
        "effect",
    ],
    "BossTheme": [
        "name",
        "strength",
        "hit_points",
        "initiative",
        "cost",
        "effect",
    ],
}

THEME_ARRAY_TO_CARD_TYPE = {
    "Allies": ("AllyTheme", "Ally"),
    "Equipment": ("EquipmentTheme", "Equipment"),
    "Monsters": ("MonsterTheme", "Monster"),
    "Traps": ("TrapTheme", "Trap"),
    "Rooms": ("RoomTheme", "DungeonRoom"),
    "Bosses": ("BossTheme", "Boss"),
}

SD_PAYLOAD = {
    "width": 512,
    "height": 896,
    "steps": 30,
    "cfg_scale": 7.5,
    "sampler_name": "DPM++ 2M Karras",
    "batch_size": 1,
}

CLASS_HINTS = {
    "paladin": "paladin",
    "ranger": "ranger",
    "cleric": "cleric",
    "rogue": "rogue",
    "artificer": "artificer",
    "warlock": "warlock",
    "fighter": "fighter",
    "barbarian": "barbarian",
    "monk": "monk",
    "druid": "druid",
    "berserker": "berserker",
    "swashbuckler": "swashbuckler",
    "spy": "spy",
    "bladesinger": "bladesinger",
    "stormcaller": "stormcaller",
    "shaman": "shaman",
    "warlord": "warlord",
    "wizard": "wizard",
    "mage": "mage",
    "healer": "healer",
    "medic": "healer",
    "merchant": "merchant",
    "cook": "cook",
    "handler": "beast handler",
    "teller": "oracle",
    "hunter": "hunter",
    "guardian": "guardian",
    "exorcist": "exorcist",
    "acrobat": "acrobat",
    "knight": "knight",
    "witch": "witch",
    "queen": "witch queen",
    "ringmaster": "ringmaster",
    "priest": "priest",
    "mystic": "mystic",
}

EQUIPMENT_BASE_DESCRIPTIONS = {
    "boots": "enchanted leather boots",
    "sword": "ancient runed sword",
    "staff": "ornate magical staff",
    "wand": "arcane wand",
    "robe": "mystic ceremonial robe",
    "shield": "heavy engraved shield",
    "cloak": "shadowy enchanted cloak",
    "ring": "glowing enchanted ring",
    "amulet": "ornate magical amulet",
    "belt": "massive enchanted belt",
    "bracers": "engraved defensive bracers",
    "buckler": "reinforced iron buckler",
    "caltrops": "spiked iron caltrops",
    "cape": "luxurious enchanted cape",
    "mail": "enchanted chain mail armor",
    "circlet": "glowing jeweled circlet",
    "club": "brutal carved war club",
    "dagger": "razor sharp ritual dagger",
    "powder": "volatile flash powder vial",
    "gauntlets": "massive runed gauntlets",
    "gloves": "enchanted dueling gloves",
    "helm": "ancient iron battle helm",
    "helmet": "ancient iron battle helm",
    "lance": "ceremonial war lance",
    "armor": "enchanted plate armor",
    "plate": "enchanted plate armor",
    "bow": "finely crafted longbow",
    "crossbow": "mechanical crossbow",
    "spear": "barbed war spear",
    "hammer": "rune-carved war hammer",
    "axe": "heavy battle axe",
    "vial": "glowing alchemical vial",
    "orb": "floating magical orb",
    "tome": "ancient spell tome",
    "lantern": "enchanted brass lantern",
    "mask": "ominous enchanted mask",
}

NAME_CLEANUP_PATTERN = re.compile(r"\+\d+|\bof\b|\bthe\b", flags=re.IGNORECASE)
EQUIPMENT_NEGATIVE_PROMPT = "person, character, hands, body, humanoid, anime, cartoon, low detail"
CREATURE_NEGATIVE_PROMPT = "anime, cartoon, low detail, flat lighting, pastel colors"
ENVIRONMENT_NEGATIVE_PROMPT = "person, character, humanoid, anime, cartoon"


@dataclass
class CardDefinition:
    set_code: str
    card_type: str
    name: str
    rarity: str
    cost: int | None = None
    strength: int | None = None
    hit_points: int | None = None
    initiative: int | None = None
    treasure: int | None = None
    damage: int | None = None
    effect: str | None = None
    is_ambusher: bool | None = None
    strength_mod: int | None = None
    hit_points_mod: int | None = None
    initiative_mod: int | None = None
    room_order: int | None = None
    monster_cost_budget: int | None = None


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(
        description="Generate card art via a local AUTOMATIC1111 Stable Diffusion API."
    )
    parser.add_argument("--style-guide", type=Path, default=STYLE_GUIDE_PATH)
    parser.add_argument("--seeds-root", type=Path, default=SEEDS_ROOT)
    parser.add_argument("--output-dir", type=Path, default=OUTPUT_ROOT)
    parser.add_argument("--base-url", default="http://127.0.0.1:7860")
    parser.add_argument("--set-code", help="Filter by set code, e.g. DND1 or DND2.")
    parser.add_argument("--card-type", choices=CARD_TYPE_ORDER)
    parser.add_argument("--name", help="Case-insensitive substring match on card name.")
    parser.add_argument("--limit", type=int, help="Maximum number of cards to process.")
    parser.add_argument("--overwrite", action="store_true")
    parser.add_argument("--parse-only", action="store_true")
    return parser.parse_args()


def collapse_whitespace(value: str) -> str:
    return re.sub(r"\s+", " ", value).strip()


def read_style_guide(style_guide_path: Path) -> tuple[str, str]:
    content = style_guide_path.read_text(encoding="utf-8")

    base_prompt_match = re.search(
        r"Use este prompt como estrutura e personalize por carta:\s*>?\s*\n\s*>\s*(.+?)\n\s*\*\*Negative prompt sugerido:\*\*",
        content,
        flags=re.DOTALL,
    )
    negative_prompt_match = re.search(
        r"\*\*Negative prompt sugerido:\*\*\s*>?\s*\n\s*>\s*(.+?)(?:\n\s*---|\Z)",
        content,
        flags=re.DOTALL,
    )

    if not base_prompt_match or not negative_prompt_match:
        raise ValueError(
            f"Could not extract base prompt and negative prompt from {style_guide_path}."
        )

    base_prompt = collapse_whitespace(base_prompt_match.group(1))
    negative_prompt = collapse_whitespace(negative_prompt_match.group(1))
    return base_prompt, negative_prompt


def extract_balanced(text: str, open_index: int, open_char: str, close_char: str) -> tuple[str, int]:
    depth = 0
    in_string = False
    i = open_index

    while i < len(text):
        char = text[i]

        if char == '"' and (i == 0 or text[i - 1] != "\\"):
            in_string = not in_string
        elif not in_string:
            if char == open_char:
                depth += 1
            elif char == close_char:
                depth -= 1
                if depth == 0:
                    return text[open_index + 1 : i], i

        i += 1

    raise ValueError(f"Unbalanced block starting at index {open_index}.")


def split_top_level_arguments(value: str) -> list[str]:
    items: list[str] = []
    current: list[str] = []
    paren_depth = 0
    brace_depth = 0
    bracket_depth = 0
    in_string = False

    for i, char in enumerate(value):
        if char == '"' and (i == 0 or value[i - 1] != "\\"):
            in_string = not in_string
            current.append(char)
            continue

        if not in_string:
            if char == "(":
                paren_depth += 1
            elif char == ")":
                paren_depth -= 1
            elif char == "{":
                brace_depth += 1
            elif char == "}":
                brace_depth -= 1
            elif char == "[":
                bracket_depth += 1
            elif char == "]":
                bracket_depth -= 1
            elif char == "," and paren_depth == 0 and brace_depth == 0 and bracket_depth == 0:
                items.append("".join(current).strip())
                current = []
                continue

        current.append(char)

    if current:
        items.append("".join(current).strip())

    return [item for item in items if item]


def strip_named_argument(arg: str) -> tuple[str | None, str]:
    match = re.match(r"^\s*([A-Za-z_][A-Za-z0-9_]*)\s*:\s*(.+)$", arg, flags=re.DOTALL)
    if not match:
        return None, arg.strip()
    return match.group(1), match.group(2).strip()


def parse_scalar(raw_value: str) -> Any:
    value = raw_value.strip()

    if value == "null":
        return None
    if value in {"true", "false"}:
        return value == "true"
    if value.startswith("Rarity."):
        return value.split(".", 1)[1]
    if value.startswith('"') and value.endswith('"'):
        return value[1:-1].replace('\\"', '"')
    if re.fullmatch(r"-?\d+", value):
        return int(value)

    return collapse_whitespace(value)


def normalise_field_name(field_name: str) -> str:
    return TYPE_FIELD_ALIASES.get(field_name, field_name)


def parse_typed_constructor_args(card_type_name: str, args_text: str) -> dict[str, Any]:
    raw_args = split_top_level_arguments(args_text)
    ordered_fields = TYPE_FIELD_ORDER[card_type_name]
    parsed: dict[str, Any] = {}

    def next_positional_field() -> str:
        for field_name in ordered_fields:
            if field_name not in parsed:
                return field_name
        raise ValueError(f"Too many positional arguments while parsing {card_type_name}.")

    for raw_arg in raw_args:
        named_field, raw_value = strip_named_argument(raw_arg)
        if named_field:
            parsed[normalise_field_name(named_field)] = parse_scalar(raw_value)
            continue

        field_name = next_positional_field()
        parsed[field_name] = parse_scalar(raw_value)

    return parsed


def parse_theme_constructor_args(theme_type_name: str, args_text: str) -> dict[str, Any]:
    raw_args = split_top_level_arguments(args_text)
    ordered_fields = THEME_FIELD_ORDER[theme_type_name]
    return {
        ordered_fields[index]: parse_scalar(raw_value)
        for index, raw_value in enumerate(raw_args)
        if index < len(ordered_fields)
    }


def build_card_definition(set_code: str, card_type: str, values: dict[str, Any]) -> CardDefinition:
    allowed_fields = {field.name for field in CardDefinition.__dataclass_fields__.values()}
    data = {"set_code": set_code, "card_type": card_type}

    for key, value in values.items():
        if key.startswith("_"):
            continue
        if key in allowed_fields:
            data[key] = value

    return CardDefinition(**data)


def parse_seed_cards(seeds_root: Path) -> list[CardDefinition]:
    cards: list[CardDefinition] = []
    typed_seed_files = sorted(seeds_root.glob("DND1_*.cs"))

    for seed_file in typed_seed_files:
        set_code = "DND1"
        content = seed_file.read_text(encoding="utf-8")

        for constructor_type in TYPE_TO_CARD_KIND:
            pattern = re.compile(rf"new\s+{constructor_type}\s*\(")

            for match in pattern.finditer(content):
                open_index = content.find("(", match.start())
                args_text, _ = extract_balanced(content, open_index, "(", ")")
                parsed = parse_typed_constructor_args(constructor_type, args_text)
                cards.append(
                    build_card_definition(set_code, TYPE_TO_CARD_KIND[constructor_type], parsed)
                )

    for theme_file in sorted((seeds_root / "Themes").glob("*.cs")):
        content = theme_file.read_text(encoding="utf-8")
        theme_pattern = re.compile(r"public\s+static\s+SetTheme\s+\w+\s*\(\)\s*=>\s*new\s*\(")

        for match in theme_pattern.finditer(content):
            open_index = content.find("(", match.end() - 1)
            theme_args_text, _ = extract_balanced(content, open_index, "(", ")")
            theme_args = split_top_level_arguments(theme_args_text)

            if len(theme_args) != 10:
                continue

            set_code = parse_scalar(theme_args[1])
            arrays = {
                "Allies": theme_args[4],
                "Equipment": theme_args[5],
                "Monsters": theme_args[6],
                "Traps": theme_args[7],
                "Rooms": theme_args[8],
                "Bosses": theme_args[9],
            }

            for array_name, array_content in arrays.items():
                theme_type_name, card_type = THEME_ARRAY_TO_CARD_TYPE[array_name]
                brace_index = array_content.find("{")
                if brace_index == -1:
                    continue

                initializer_text, _ = extract_balanced(array_content, brace_index, "{", "}")
                entries: list[dict[str, Any]] = []

                for entry_match in re.finditer(r"new\s*\(", initializer_text):
                    entry_open_index = initializer_text.find("(", entry_match.start())
                    entry_args_text, _ = extract_balanced(initializer_text, entry_open_index, "(", ")")
                    entries.append(parse_theme_constructor_args(theme_type_name, entry_args_text))

                for index, entry in enumerate(entries):
                    if card_type in {"Ally", "Equipment", "Monster", "Trap"}:
                        entry["rarity"] = RARITY_DISTRIBUTION_60[index]
                    elif card_type == "DungeonRoom":
                        entry["rarity"] = "Common"
                        entry["room_order"] = min((index // 10) + 1, 5)
                    elif card_type == "Boss":
                        entry["rarity"] = "Unique"

                    cards.append(build_card_definition(set_code, card_type, entry))

    cards.sort(key=lambda card: (card.set_code, card.card_type, card.name))
    return cards


def slugify(value: str) -> str:
    slug = re.sub(r"[^a-z0-9]+", "-", value.lower()).strip("-")
    return slug or "card"


def title_case_words(value: str) -> list[str]:
    return re.findall(r"[A-Za-z]+", value.lower())


def infer_ally_class(card: CardDefinition) -> str:
    haystack = f"{card.name} {card.effect or ''}".lower()
    for keyword, class_name in CLASS_HINTS.items():
        if keyword in haystack:
            return class_name
    return "adventurer"


def build_effect_hint(card: CardDefinition) -> str:
    parts: list[str] = []

    if card.effect:
        first_sentence = re.split(r"[.!?]", card.effect, maxsplit=1)[0]
        if first_sentence.strip():
            parts.append(collapse_whitespace(first_sentence.strip()))

    if card.card_type == "Equipment":
        if (card.strength_mod or 0) > 0:
            parts.append("glowing with offensive battle enchantment")
        if (card.hit_points_mod or 0) > 0:
            parts.append("radiating protective ward magic")
        if (card.initiative_mod or 0) > 0:
            parts.append("surrounded by swift kinetic runes")
        if not parts:
            parts.append("radiating ancient magical power")
    elif card.card_type == "Trap":
        if card.damage is not None:
            parts.append(f"dealing {card.damage} damage in a burst of arcane force")
        if card.effect and not parts:
            parts.append("dangerous magical energy discharging")
    elif card.card_type == "DungeonRoom":
        if card.monster_cost_budget is not None:
            parts.append(f"built for deadly encounters with looming danger in every shadow")

    return collapse_whitespace(", ".join(parts))


def describe_equipment_item(name: str) -> str:
    lowered = NAME_CLEANUP_PATTERN.sub(" ", name.lower())
    words = [word for word in title_case_words(lowered) if word not in {"a", "an"}]
    if not words:
        return "enchanted magical item"

    base_word = next(
        (word for word in reversed(words) if word in EQUIPMENT_BASE_DESCRIPTIONS),
        words[-1],
    )
    descriptor = EQUIPMENT_BASE_DESCRIPTIONS.get(base_word)

    if descriptor is None:
        descriptor = f"enchanted {' '.join(words)}"

    extra_traits: list[str] = []
    if "speed" in words:
        extra_traits.append("with speed runes")
    if "elvenkind" in words:
        extra_traits.append("with elegant elven filigree")
    if "displacement" in words:
        extra_traits.append("with distortion shimmer")
    if "health" in words:
        extra_traits.append("with restorative life sigils")
    if "giant" in words or "ogre" in words:
        extra_traits.append("with strength glyphs")
    if "defense" in words:
        extra_traits.append("with protective ward engravings")
    if "blasting" in words:
        extra_traits.append("with explosive fire sigils")
    if "waterdeep" in words:
        extra_traits.append("with arcane city glyphs")
    if "orcus" in words:
        extra_traits.append("with sinister necromantic carvings")
    if "mountebank" in words:
        extra_traits.append("with teleportation sigils")
    if "antidote" in words:
        extra_traits.append("filled with glowing antidote liquid")
    if "flash" in words:
        extra_traits.append("releasing blinding alchemical sparks")

    suffix = f" {' '.join(extra_traits)}" if extra_traits else ""
    return collapse_whitespace(f"{descriptor}{suffix}")


def build_prompt(card: CardDefinition) -> str:
    effect_hint = build_effect_hint(card)

    if card.card_type == "Equipment":
        item_name = describe_equipment_item(card.name)
        return collapse_whitespace(
            f"a single {item_name}, fantasy magical item, {effect_hint}, "
            "isolated on dark dungeon stone pedestal, dramatic underlighting from below, "
            "glowing golden runes, antique gold and brown tones, deep purple magical glow, "
            "highly detailed texture, no character, no person, no hands, trading card item art, "
            "dark fantasy digital painting"
        )

    if card.card_type == "Ally":
        hero_class = infer_ally_class(card)
        return collapse_whitespace(
            f"{card.name}, fantasy {hero_class} hero, heroic action pose, determined expression, "
            "worn battle gear with golden trim, dramatic underlighting from dungeon torches below, "
            "epic low-angle perspective, dark dungeon background with stone arches, dominant brown "
            "and antique gold palette, deep purple accents, dark fantasy digital painting, "
            "trading card character art"
        )

    if card.card_type == "Monster":
        return collapse_whitespace(
            f"{card.name}, fearsome fantasy monster, aggressive threatening pose, massive imposing figure, "
            "dramatic purple underlighting from below, ancient dungeon environment, dominant dark brown "
            "and deep purple palette, glowing eyes, dark fantasy digital painting, trading card monster art"
        )

    if card.card_type == "Trap":
        return collapse_whitespace(
            f"{card.name} trap activating, magical mechanism in motion, {effect_hint}, dark dungeon floor, "
            "dramatic purple and gold energy effect, no character, no person, isolated magical effect, "
            "dark fantasy digital painting, trading card trap art"
        )

    if card.card_type == "DungeonRoom":
        return collapse_whitespace(
            f"{card.name} dungeon room, wide establishing shot, ancient stone architecture, dramatic "
            "underlighting from floor torches and runes, oppressive atmosphere, volumetric fog, dungeon "
            "crawl environment, dark fantasy digital painting, trading card location art"
        )

    if card.card_type == "Boss":
        return collapse_whitespace(
            f"{card.name}, legendary boss creature, monumental scale filling entire frame, overwhelming "
            "presence, dramatic purple and gold underlighting, ancient dungeon throne room, inevitable "
            "power aura, dark fantasy digital painting, trading card boss art"
        )

    raise ValueError(f"Unsupported card type for prompt building: {card.card_type}")


def build_negative_prompt(card: CardDefinition) -> str:
    if card.card_type == "Equipment":
        return EQUIPMENT_NEGATIVE_PROMPT
    if card.card_type in {"Ally", "Monster", "Boss"}:
        return CREATURE_NEGATIVE_PROMPT
    if card.card_type in {"Trap", "DungeonRoom"}:
        return ENVIRONMENT_NEGATIVE_PROMPT
    raise ValueError(f"Unsupported card type for negative prompt: {card.card_type}")


def filter_cards(cards: list[CardDefinition], args: argparse.Namespace) -> list[CardDefinition]:
    filtered = cards

    if args.set_code:
        expected = args.set_code.lower()
        filtered = [card for card in filtered if card.set_code.lower() == expected]

    if args.card_type:
        filtered = [card for card in filtered if card.card_type == args.card_type]

    if args.name:
        name_filter = args.name.lower()
        filtered = [card for card in filtered if name_filter in card.name.lower()]

    if args.limit is not None:
        filtered = filtered[: args.limit]

    return filtered


def manifest_payload(cards: list[CardDefinition]) -> list[dict[str, Any]]:
    payload: list[dict[str, Any]] = []

    for card in cards:
        row = asdict(card)
        row["prompt"] = build_prompt(card)
        row["negative_prompt"] = build_negative_prompt(card)
        payload.append(row)

    return payload


def write_manifest(output_dir: Path, cards: list[CardDefinition]) -> Path:
    output_dir.mkdir(parents=True, exist_ok=True)
    manifest_path = output_dir / "manifest.json"
    manifest = manifest_payload(cards)
    manifest_path.write_text(json.dumps(manifest, indent=2, ensure_ascii=False), encoding="utf-8")
    return manifest_path


def post_json(url: str, payload: dict[str, Any]) -> dict[str, Any]:
    encoded = json.dumps(payload).encode("utf-8")
    req = request.Request(
        url,
        data=encoded,
        headers={"Content-Type": "application/json"},
        method="POST",
    )

    try:
        with request.urlopen(req, timeout=600) as response:
            return json.loads(response.read().decode("utf-8"))
    except error.HTTPError as exc:
        body = exc.read().decode("utf-8", errors="replace")
        raise RuntimeError(f"Stable Diffusion API returned HTTP {exc.code}: {body}") from exc
    except error.URLError as exc:
        raise RuntimeError(f"Could not reach Stable Diffusion API at {url}: {exc}") from exc


def save_generated_image(card: CardDefinition, image_data: str, output_dir: Path) -> Path:
    image_bytes = base64.b64decode(image_data.split(",", 1)[-1])
    card_dir = output_dir / card.set_code / card.card_type.lower()
    card_dir.mkdir(parents=True, exist_ok=True)
    file_path = card_dir / f"{slugify(card.name)}.png"
    file_path.write_bytes(image_bytes)
    return file_path


def generate_cards(
    cards: list[CardDefinition],
    output_dir: Path,
    base_url: str,
    overwrite: bool,
) -> None:
    for card in cards:
        target_path = output_dir / card.set_code / card.card_type.lower() / f"{slugify(card.name)}.png"
        if target_path.exists() and not overwrite:
            print(f"Skipping existing file: {target_path}")
            continue

        payload = {
            **SD_PAYLOAD,
            "prompt": build_prompt(card),
            "negative_prompt": build_negative_prompt(card),
        }
        response = post_json(f"{base_url.rstrip('/')}/sdapi/v1/txt2img", payload)
        images = response.get("images") or []

        if not images:
            raise RuntimeError(f"No images returned for {card.name}.")

        saved_path = save_generated_image(card, images[0], output_dir)
        print(f"Generated {card.set_code} {card.card_type} '{card.name}' -> {saved_path}")


def main() -> int:
    args = parse_args()
    # Keep validating the style guide path for compatibility with older workflow usage.
    if args.style_guide.exists():
        read_style_guide(args.style_guide)
    cards = parse_seed_cards(args.seeds_root)
    selected_cards = filter_cards(cards, args)

    if not selected_cards:
        print("No cards matched the provided filters.", file=sys.stderr)
        return 1

    manifest_path = write_manifest(args.output_dir, selected_cards)
    print(f"Selected {len(selected_cards)} cards. Manifest written to {manifest_path}")

    if args.parse_only:
        preview = manifest_payload(selected_cards[:3])
        print(json.dumps(preview, indent=2, ensure_ascii=False))
        return 0

    generate_cards(
        selected_cards,
        args.output_dir,
        args.base_url,
        args.overwrite,
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
