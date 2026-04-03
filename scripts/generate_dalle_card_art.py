#!/usr/bin/env python3
"""Generate DALL-E 3 card artwork from C# seed files.

Usage:
  OPENAI_API_KEY=... python scripts/generate_dalle_card_art.py
"""

from __future__ import annotations

import argparse
import base64
import csv
import os
import re
import sys
import time
from dataclasses import dataclass
from pathlib import Path
from typing import Optional

from urllib.request import urlopen

PROMPT_BASE = (
    "dark fantasy digital painting of [SUBJECT], inside an ancient dungeon, "
    "dramatic underlighting from below, epic low-angle perspective, dominant "
    "brown and antique gold with deep purple accents, painterly texture, "
    "high contrast chiaroscuro, volumetric fog, embers and dust particles, "
    "detailed armor/stone/metal, cinematic composition, trading card illustration, "
    "no modern elements, no sci-fi, no cartoon style"
)

NEGATIVE_PROMPT = (
    "bright daylight, flat lighting, pastel palette, modern city, sci-fi armor, "
    "anime style, low detail, washed colors, cute expression, comedic tone"
)

TYPE_DIRECTIONS = {
    "Monster": "postura agressiva, volume grande, luz roxa de baixo, sensação de ameaça",
    "Ally": "dourado dominante, expressão determinada, contraste desgaste/nobreza",
    "Equipment": "item em close dramático, luz dourada realçando detalhes do material",
    "Trap": "composição focada no efeito em ação, cor de acento para leitura do efeito",
    "DungeonRoom": "ambiente wide shot, atmosfera opressiva, escala arquitetônica",
    "Boss": "escala monumental, dominância total do frame, aura de poder inevitável",
}

CLASS_TO_TYPE = {
    "AllyCard": "Ally",
    "MonsterCard": "Monster",
    "EquipmentCard": "Equipment",
    "TrapCard": "Trap",
    "DungeonRoomCard": "DungeonRoom",
    "BossCard": "Boss",
}


@dataclass
class CardRecord:
    name: str
    card_type: str
    effect_text: str
    strength: Optional[int]
    initiative: Optional[int]
    hit_points: Optional[int]
    treasure: Optional[int]
    cost: Optional[int]
    rarity: str
    set_code: str


def sanitize_filename(value: str) -> str:
    value = re.sub(r'[\\/:*?"<>|]+', "_", value)
    value = value.strip().rstrip(".")
    return value or "unnamed_card"


def parse_rarity(args: list[str]) -> str:
    for arg in args:
        match = re.search(r"Rarity\.(\w+)", arg)
        if match:
            return match.group(1)
    return "Unknown"


def parse_name(args: list[str]) -> str:
    for arg in args:
        arg = arg.strip()
        if arg.startswith('"') and arg.endswith('"'):
            return unquote(arg)
    return "Unknown Card"


def parse_effect(args: list[str]) -> str:
    for arg in reversed(args):
        arg = arg.strip()
        if arg.startswith('"') and arg.endswith('"'):
            return unquote(arg)
    return ""


def unquote(s: str) -> str:
    s = s.strip()
    if len(s) >= 2 and s[0] == '"' and s[-1] == '"':
        s = s[1:-1]
    return s.replace('\\"', '"')


def parse_int(value: str) -> Optional[int]:
    value = value.strip()
    if value.lower() == "null":
        return None
    if re.fullmatch(r"-?\d+", value):
        return int(value)
    return None


def split_top_level_args(arg_blob: str) -> list[str]:
    parts: list[str] = []
    buf: list[str] = []
    depth = 0
    in_string = False
    escape = False

    for ch in arg_blob:
        if in_string:
            buf.append(ch)
            if escape:
                escape = False
            elif ch == "\\":
                escape = True
            elif ch == '"':
                in_string = False
            continue

        if ch == '"':
            in_string = True
            buf.append(ch)
            continue

        if ch == "(":
            depth += 1
            buf.append(ch)
            continue

        if ch == ")":
            depth -= 1
            buf.append(ch)
            continue

        if ch == "," and depth == 0:
            parts.append("".join(buf).strip())
            buf = []
            continue

        buf.append(ch)

    if buf:
        parts.append("".join(buf).strip())

    return parts


def find_card_blocks(content: str) -> list[tuple[str, str]]:
    blocks: list[tuple[str, str]] = []
    pattern = re.compile(r"new\s+(\w+Card)\s*\(")

    for match in pattern.finditer(content):
        class_name = match.group(1)
        i = match.end() - 1
        depth = 0
        in_string = False
        escape = False

        while i < len(content):
            ch = content[i]
            if in_string:
                if escape:
                    escape = False
                elif ch == "\\":
                    escape = True
                elif ch == '"':
                    in_string = False
            else:
                if ch == '"':
                    in_string = True
                elif ch == "(":
                    depth += 1
                elif ch == ")":
                    depth -= 1
                    if depth == 0:
                        inner = content[match.end():i]
                        blocks.append((class_name, inner))
                        break
            i += 1

    return blocks


def parse_card_args(class_name: str, args: list[str], set_code: str) -> CardRecord:
    card_type = CLASS_TO_TYPE.get(class_name, class_name)
    name = parse_name(args)
    rarity = parse_rarity(args)
    effect_text = parse_effect(args)

    strength = initiative = hit_points = treasure = cost = None

    named: dict[str, str] = {}
    positional: list[str] = []
    for arg in args:
        if ":" in arg and not arg.strip().startswith('"'):
            key, value = arg.split(":", 1)
            named[key.strip()] = value.strip()
        else:
            positional.append(arg.strip())

    if class_name == "MonsterCard":
        cost = parse_int(named.get("cost", ""))
        strength = parse_int(named.get("strength", ""))
        hit_points = parse_int(named.get("hitPoints", ""))
        initiative = parse_int(named.get("initiative", ""))
        treasure = parse_int(named.get("treasure", ""))
        effect_text = unquote(named.get("effect", f'"{effect_text}"'))

    elif class_name == "TrapCard":
        cost = parse_int(named.get("cost", ""))
        strength = parse_int(named.get("damage", ""))

    elif class_name == "AllyCard":
        if len(positional) >= 10:
            cost = parse_int(positional[3])
            strength = parse_int(positional[4])
            hit_points = parse_int(positional[5])
            initiative = parse_int(positional[6])
            treasure = parse_int(positional[8])
            effect_text = unquote(positional[9])

    elif class_name == "EquipmentCard":
        if len(positional) >= 7:
            cost = parse_int(positional[3])
            strength = parse_int(positional[4])
            hit_points = parse_int(positional[5])
            initiative = parse_int(positional[6])

    elif class_name == "DungeonRoomCard":
        if len(positional) >= 8:
            cost = parse_int(positional[3])
            treasure = parse_int(positional[6])
            effect_text = unquote(positional[7])

    elif class_name == "BossCard":
        if len(positional) >= 8:
            cost = parse_int(positional[3])
            strength = parse_int(positional[4])
            hit_points = parse_int(positional[5])
            initiative = parse_int(positional[6])
            effect_text = unquote(positional[7])

    return CardRecord(
        name=name,
        card_type=card_type,
        effect_text=effect_text,
        strength=strength,
        initiative=initiative,
        hit_points=hit_points,
        treasure=treasure,
        cost=cost,
        rarity=rarity,
        set_code=set_code,
    )


def build_prompt(card: CardRecord) -> str:
    direction = TYPE_DIRECTIONS.get(card.card_type, "")
    stats = (
        f"Força {card.strength if card.strength is not None else '-'}, "
        f"Iniciativa {card.initiative if card.initiative is not None else '-'}, "
        f"HP {card.hit_points if card.hit_points is not None else '-'}, "
        f"Tesouro {card.treasure if card.treasure is not None else '-'}, "
        f"Custo {card.cost if card.cost is not None else '-'}"
    )

    subject = (
        f"{card.name} ({card.card_type})" if card.effect_text.strip() == "" else f"{card.name} ({card.card_type}) depicting: {card.effect_text}"
    )

    prompt_base = PROMPT_BASE.replace("[SUBJECT]", subject)
    return (
        f"{prompt_base}. "
        f"Direção por tipo: {direction}. "
        f"Raridade: {card.rarity}. Set: {card.set_code}. Stats: {stats}. "
        f"Negative prompt reference: {NEGATIVE_PROMPT}."
    )


def get_seed_files(seed_dir: Path) -> list[Path]:
    return sorted(seed_dir.glob("DND*_*.cs"))


def extract_set_code(path: Path) -> str:
    match = re.match(r"(DND\d+)_", path.name)
    return match.group(1) if match else "DND_UNKNOWN"


def parse_cards(seed_dir: Path) -> list[CardRecord]:
    cards: list[CardRecord] = []
    for file in get_seed_files(seed_dir):
        set_code = extract_set_code(file)
        content = file.read_text(encoding="utf-8")
        for class_name, arg_blob in find_card_blocks(content):
            args = split_top_level_args(arg_blob)
            cards.append(parse_card_args(class_name, args, set_code))
    return cards


def save_image_from_result(result, output_path: Path) -> None:
    data = result.data[0]
    output_path.parent.mkdir(parents=True, exist_ok=True)

    if getattr(data, "b64_json", None):
        image_bytes = base64.b64decode(data.b64_json)
        output_path.write_bytes(image_bytes)
        return

    image_url = getattr(data, "url", None)
    if not image_url:
        raise RuntimeError("Image response has no b64_json or url")

    with urlopen(image_url, timeout=60) as response:
        output_path.write_bytes(response.read())


def generate_images(cards: list[CardRecord], output_root: Path, csv_path: Path) -> None:
    if not os.getenv("OPENAI_API_KEY"):
        raise RuntimeError("OPENAI_API_KEY não encontrada no ambiente.")

    from openai import OpenAI

    client = OpenAI()
    total = len(cards)

    csv_path.parent.mkdir(parents=True, exist_ok=True)
    with csv_path.open("w", newline="", encoding="utf-8") as f:
        writer = csv.DictWriter(
            f,
            fieldnames=["CardName", "Set", "Type", "Prompt", "ImagePath", "Status"],
        )
        writer.writeheader()

        for idx, card in enumerate(cards, start=1):
            print(f"Gerando carta {idx} de {total}: {card.name}")
            prompt = build_prompt(card)
            output_path = output_root / card.set_code / card.card_type / f"{sanitize_filename(card.name)}.png"
            status = "failed"

            for attempt in range(1, 4):
                try:
                    result = client.images.generate(
                        model="dall-e-3",
                        prompt=prompt,
                        size="1024x1792",
                        n=1,
                    )
                    save_image_from_result(result, output_path)
                    status = "success"
                    break
                except Exception as exc:
                    print(f"  Tentativa {attempt}/3 falhou para {card.name}: {exc}")
                    if attempt < 3:
                        time.sleep(2)
                finally:
                    time.sleep(1)

            writer.writerow(
                {
                    "CardName": card.name,
                    "Set": card.set_code,
                    "Type": card.card_type,
                    "Prompt": prompt,
                    "ImagePath": str(output_path),
                    "Status": status,
                }
            )
            f.flush()


def main() -> int:
    parser = argparse.ArgumentParser(description="Generate DALL-E 3 art for CardgameDungeon cards")
    parser.add_argument(
        "--seed-dir",
        type=Path,
        default=Path("src/CardgameDungeon.API/Data/Seeds"),
        help="Path to C# seed files directory",
    )
    parser.add_argument(
        "--output-root",
        type=Path,
        default=Path("unity-client/Assets/Art/Cards"),
        help="Output root for generated images",
    )
    parser.add_argument(
        "--csv-log",
        type=Path,
        default=Path("unity-client/Assets/Art/Cards/generation_log.csv"),
        help="CSV log output path",
    )
    parser.add_argument(
        "--parse-only",
        action="store_true",
        help="Only parse seed files and print card count",
    )
    args = parser.parse_args()

    if not args.seed_dir.exists():
        print(f"Diretório de seeds não encontrado: {args.seed_dir}", file=sys.stderr)
        return 1

    cards = parse_cards(args.seed_dir)
    print(f"Cartas encontradas: {len(cards)}")

    if args.parse_only:
        return 0

    generate_images(cards, args.output_root, args.csv_log)
    print("Concluído.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
