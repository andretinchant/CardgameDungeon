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
OVERRIDES_PATH = ROOT / "scripts" / "card_art_prompt_overrides.json"

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
        "attack",
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
        "attack_mod",
        "hit_points_mod",
        "initiative_mod",
        "slot",
        "effect",
    ],
    "MonsterCard": [
        "_guid",
        "name",
        "rarity",
        "cost",
        "attack",
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
        "attack",
        "hit_points",
        "initiative",
        "effect",
    ],
}

TYPE_FIELD_ALIASES = {
    "hitPoints": "hit_points",
    "isAmbusher": "is_ambusher",
    "strMod": "attack_mod",
    "hpMod": "hit_points_mod",
    "initMod": "initiative_mod",
    "monsterCostBudget": "monster_cost_budget",
}

THEME_FIELD_ORDER = {
    "AllyTheme": [
        "name",
        "attack",
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
        "attack_mod",
        "hit_points_mod",
        "initiative_mod",
    ],
    "MonsterTheme": [
        "name",
        "attack",
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
        "attack",
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

TYPED_SEED_FILE_TO_CONSTRUCTOR = {
    "DND1_Allies.cs": "AllyCard",
    "DND1_Equipment.cs": "EquipmentCard",
    "DND1_Consumables.cs": "EquipmentCard",
    "DND1_Monsters.cs": "MonsterCard",
    "DND1_Traps.cs": "TrapCard",
    "DND1_Dungeon.cs": "DungeonRoomCard",
    "DND1_Boss.cs": "BossCard",
}

SD_PAYLOAD = {
    "width": 512,
    "height": 896,
    "steps": 30,
    "cfg_scale": 7.5,
    "sampler_name": "DPM++ 2M Karras",
    "batch_size": 1,
}

CHECKPOINT_DREAMSHAPER = "dreamshaper_8.safetensors [879db523c3]"
CHECKPOINT_SD15 = "v1-5-pruned-emaonly.safetensors [6ce0161689]"

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

CONSUMABLE_SLOTS = {"Scroll", "Potion", "Balm", "Bomb", "Totem"}

CONSUMABLE_SLOT_KEYWORDS = {
    "scroll": "Scroll",
    "potion": "Potion",
    "elixir": "Potion",
    "spirits": "Potion",
    "vial": "Bomb",
    "flask": "Bomb",
    "bomb": "Bomb",
    "pellet": "Bomb",
    "balm": "Balm",
    "salve": "Balm",
    "dressing": "Balm",
    "totem": "Totem",
}

CONSUMABLE_SLOT_BASE_SUBJECT = {
    "Scroll": "ancient spell scroll of aged parchment with wax seal",
    "Potion": "glass potion bottle filled with luminous alchemical liquid",
    "Bomb": "volatile alchemical bomb flask with reinforced stopper",
    "Balm": "small ceramic balm jar filled with glowing herbal salve",
    "Totem": "carved ritual totem idol wrapped with talisman cords",
}

CONSUMABLE_SLOT_NEGATIVES = {
    "Scroll": "potion bottle, vial, flask, bomb, totem idol, weapon, armor, person holding scroll",
    "Potion": "scroll parchment, bomb fuse, totem idol, weapon, armor, person drinking potion",
    "Bomb": "scroll parchment, healing potion bottle, totem idol, weapon, armor, person throwing bomb",
    "Balm": "scroll parchment, potion bottle, bomb flask, totem idol, weapon, armor, person applying balm",
    "Totem": "scroll parchment, potion bottle, bomb flask, balm jar, weapon, armor, person holding idol",
}

CONSUMABLE_VISUAL_PATTERN_RULES = [
    (r"\bfire(ball|bomb)?\b", "bright ember glow, heat distortion, and burning sigils"),
    (r"\blightning\b", "branching lightning arcs wrapping around the item"),
    (r"\bthunder(stone)?\b", "shockwave rings and crackling electric sparks"),
    (r"\bfrost\b", "icy rime, cold mist, and frozen crystalline edges"),
    (r"\bacid\b", "corrosive green fluid and smoking etched metal"),
    (r"\bpoison\b", "venomous green fumes and toxic droplets"),
    (r"\bsmoke\b", "dense swirling smoke plume around the object"),
    (r"\bstink\b", "sickly yellow vapor cloud and nausea aura"),
    (r"\btar\b", "thick black resin dripping with sticky texture"),
    (r"\bheal(ing)?\b", "emerald restorative glow and gentle life sigils"),
    (r"\brestor(ation|e)\b", "restorative runes pulsing around the item"),
    (r"\bmending\b", "repair glyphs and stitched arcane seams"),
    (r"\bresurrection\b", "golden revival halo and phoenix-like motes"),
    (r"\bimmortality\b", "timeless aura and suspended glowing particles"),
    (r"\bwish\b", "cosmic star-like runes and reality-bending shimmer"),
    (r"\btime\s*stop\b", "clockwork sigils frozen in mid-air"),
    (r"\binvisibility\b", "refractive distortion and translucent silhouette edges"),
    (r"\bmirror\b", "mirrored afterimages orbiting the item"),
    (r"\bcounterspell\b", "anti-magic runes canceling nearby glyphs"),
    (r"\bdetect magic\b", "revealing glyph-lines and spectral detection rays"),
    (r"\bshield\b", "protective barrier glyphs and layered ward sigils"),
    (r"\bsanctuary\b", "sacred ward circle and protective halo"),
    (r"\bprotection\b", "defensive runic lattice around the object"),
    (r"\binvulnerability\b", "impenetrable shimmering shell around the item"),
    (r"\bhaste\b", "speed streaks and spiraling kinetic runes"),
    (r"\bspeed\b", "motion trails and wind-like arcane ribbons"),
    (r"\bswiftness\b", "swift airflow glyphs and kinetic sparks"),
    (r"\bfeather\b", "floating feathers and levitation-like weightlessness"),
    (r"\brage\b", "crimson rage aura and jagged war sigils"),
    (r"\bcourage\b", "steady golden courage glow and rally sigils"),
    (r"\bwar\b", "martial runes and aggressive battle aura"),
    (r"\bgiant attack\b", "massive attack glyphs carved in heavy strokes"),
    (r"\bfortitude\b", "fortifying stone-like runes and endurance aura"),
    (r"\btoughness\b", "dense protective glow emphasizing durability"),
    (r"\bbless\b", "soft divine glow and blessing sigils"),
    (r"\bholy\b", "radiant sacred light and sanctified glyphs"),
]

RACE_VISUALS = {
    "aasimar": "radiant celestial features and faint halo glow",
    "dragonborn": "draconic snout, scales, and proud warrior posture",
    "dwarven": "stout dwarven build, braided beard, and heavy armor",
    "dwarf": "stout dwarven build, braided beard, and heavy armor",
    "elven": "sharp elven features, elegant silhouette, and refined gear",
    "elf": "sharp elven features, elegant silhouette, and refined gear",
    "half-orc": "broad muscular frame, tusks, and brutal scarred armor",
    "halfling": "small nimble body and practical adventuring clothes",
    "gnome": "small clever inventor silhouette and tool-laden gear",
    "tiefling": "curved horns, infernal eyes, and arcane clothing",
    "firbolg": "towering forest giant build and druidic ornamentation",
    "goliath": "massive stone-like musculature and tribal markings",
    "tabaxi": "feline features, agile body, and swashbuckling attire",
    "kenku": "crow-like beak, feathered silhouette, and shadowy gear",
    "tortle": "armored shell and stoic guardian posture",
    "changeling": "ambiguous shifting facial features and spy attire",
    "eladrin": "fey elegance, luminous eyes, and flowing bladesinger robes",
    "genasi": "elemental aura and windswept magical clothing",
    "lizardfolk": "reptilian scales and primal bone ornaments",
    "bugbear": "hulking hairy goblinoid frame and savage weapons",
    "hobgoblin": "disciplined martial armor and commanding stance",
    "human": "human adventurer proportions and grounded heroic presence",
}

CLASS_VISUALS = {
    "paladin": "wielding a blessed weapon and shield",
    "ranger": "with bow or dual hunting blades ready",
    "cleric": "holding a holy symbol and radiant mace",
    "rogue": "with daggers drawn and stealthy footwork",
    "artificer": "surrounded by arcane gadgets and crafted tools",
    "warlock": "channeling eldritch magic through one hand",
    "fighter": "in disciplined combat stance with battle weapon",
    "barbarian": "mid-charge with raw fury and heavy weapon",
    "monk": "mid-strike with flowing martial motion",
    "druid": "surrounded by primal nature magic and animal motifs",
    "berserker": "roaring in savage melee momentum",
    "swashbuckler": "dueling gracefully with rapier-like precision",
    "spy": "half-hidden, watchful, and ready to deceive",
    "bladesinger": "mixing swordplay and arcane energy in one motion",
    "stormcaller": "calling lightning around an outstretched arm",
    "shaman": "with bone charms and primal ritual power",
    "warlord": "commanding the battlefield with tactical authority",
    "wizard": "casting a precise arcane spell",
    "mage": "surrounded by controlled magical sigils",
    "healer": "projecting restorative holy light",
    "merchant": "carrying valuable gear and relics",
    "beast handler": "with trained companion cues and command posture",
    "oracle": "reading fate through mystical cards and visions",
    "hunter": "tracking prey with lethal focus",
    "guardian": "interposing defensively with shielded stance",
    "exorcist": "brandishing relics against evil forces",
    "acrobat": "caught in dynamic agile movement",
    "knight": "in heavy plate with noble martial bearing",
    "witch": "casting sinister or whimsical hex magic",
    "witch queen": "radiating regal arcane menace",
    "ringmaster": "with theatrical yet dangerous flourish",
    "priest": "invoking sacred power through ritual gesture",
    "mystic": "shrouded in prophecy and spirit magic",
    "adventurer": "armed and ready for dungeon combat",
}

MONSTER_BASE_VISUALS = {
    "dragon": "single dragon only, colossal reptilian body, huge wings, horned head, long tail, and dominating claws",
    "brain": "single elder brain only, enormous exposed brain mass with hanging neural tendrils in a grotesque vat-like setting",
    "beholder": "single beholder only, floating spherical body, giant central eye, many eyestalks, and levitating menace",
    "lich": "single lich only, skeletal undead sorcerer in decayed regal robes with necromantic aura",
    "mind flayer": "single mind flayer only, tall robed aberration with squid-like face tentacles and psychic presence",
    "vampire": "single vampire noble only, pale aristocratic predator with fangs, dark cape, and blood magic",
    "death knight": "single undead knight only, blackened cursed armor, burning eye sockets, and runed greatsword",
    "fiend": "single infernal fiend only, towering demonic musculature, horns, burning skin, and hellish wings",
    "demon": "single demon only, massive horned abyssal body with fiery whip-like energy and monstrous claws",
    "golem": "single construct only, immense metal body, heavy forged plates, and unstoppable weight",
    "hydra": "single hydra only, one giant beast with multiple serpentine heads from one shared body",
    "ooze": "single ooze creature only, amorphous translucent slime mass with corrosive sheen",
    "spider": "single giant spider only, oversized arachnid body, many legs, fangs, and web-coated silhouette",
    "wolf": "single dire wolf only, huge lupine body, snarling jaws, and predatory eyes",
    "giant": "single giant only, enormous humanoid frame with oversized weapon and brutal scale",
    "troll": "single troll only, lanky regenerating brute with long limbs and claws",
    "basilisk": "single basilisk only, low reptilian body, crown-like horns, and petrifying gaze",
    "cockatrice": "single cockatrice only, twisted rooster-lizard hybrid with sharp beak and stone curse aura",
    "ghost": "single ghost only, spectral humanoid silhouette with trailing ectoplasmic robes",
    "wraith": "single wraith only, shadowy reaper-like apparition with hollow glowing eyes",
    "skeleton": "single skeleton warrior only, animated bones, rusted armor, and undead menace",
    "zombie": "single zombie only, rotting corpse body and shambling aggression",
    "mummy": "single mummy only, wrapped ancient corpse with cursed burial regalia",
    "devil": "single devil only, infernal aristocratic monster with horns, tail, and hellfire aura",
    "imp": "single imp only, small winged devilish creature with sharp silhouette",
    "elemental": "single elemental entity only, body formed from raw fire, ice, stone, or storm energy",
    "hag": "single hag only, twisted witch-like monster with clawed hands and curse magic",
    "kobold": "single kobold only, small draconic reptilian scavenger with cunning stance",
    "goblin": "single goblin only, wiry green raider with crude weapons and vicious grin",
    "orc": "single orc only, brutal tusked warrior with heavy musculature and savage gear",
    "gnoll": "single gnoll only, hyena-headed marauder with ragged armor and feral hunger",
    "minotaur": "single minotaur only, bull-headed giant labyrinth brute with huge horns",
    "medusa": "single medusa only, humanoid serpent-haired monster with petrifying stare",
    "wyvern": "single wyvern only, winged reptile with stinger tail and predatory profile",
    "gargoyle": "single gargoyle only, winged stone demon perched in predatory crouch",
    "mimic": "single mimic only, monstrous treasure chest with teeth, tongue, and transformed wood flesh",
}

MONSTER_ADJECTIVE_VISUALS = {
    "ancient": "scarred by ages, primeval, and immensely old",
    "red": "glowing crimson scales or red infernal light",
    "elder": "elder intellect and grotesque ancient dominance",
    "tyrant": "regal tyrannical presence and domineering posture",
    "lord": "regal and oppressive high-status menace",
    "arcanist": "arcane sigils and spellcasting energy",
    "pit": "hellish fire and infernal smoke",
    "iron": "forged iron surfaces and metallic weight",
    "death": "necrotic aura and deathly gloom",
    "vampire": "blood-drinking elegance and predatory nobility",
    "balor": "flaming abyssal energy and demonic grandeur",
}

MONSTER_EFFECT_VISUALS = {
    "fire damage": "fire streaming from maw, claws, or aura",
    "explodes": "volatile energy swelling inside the body",
    "eye rays": "multiple magical beams firing from eyes",
    "petrify": "stone curse energy gathering around the gaze",
    "charm": "hypnotic eyes and enthralling magic swirl",
    "phylactery": "necromantic soul-binding aura hinting at immortality",
    "devours their intellect": "psychic tendrils and mind-rending pressure",
    "heals hp": "blood or life essence flowing back into the body",
    "immune to fire": "fire curling harmlessly across the skin",
    "extra damage": "weapon or claws crackling with lethal energy",
    "takes control": "domination tendrils or psychic domination energy",
}

MONSTER_IDENTITY_RULES = {
    "dragon": {
        "identity": "unmistakably a dragon, reptilian not humanoid",
        "negative": "humanoid, human body, demon man, devil man, tiefling body, warrior torso, person, mammal face",
    },
    "elder brain": {
        "identity": "unmistakably an elder brain, giant exposed brain mass with long wet tentacles and no humanoid body",
        "negative": "humanoid, person, human body, torso, legs, arms, armor, demon, knight, warrior, giant man",
    },
    "brain": {
        "identity": "unmistakably an exposed aberrant brain creature, not humanoid",
        "negative": "humanoid, person, armor, torso, legs, demon warrior",
    },
    "beholder": {
        "identity": "unmistakably a beholder, floating orb body with one giant eye and many eyestalks",
        "negative": "humanoid, person, dragon, demon, warrior, torso, two creatures",
    },
    "mind flayer": {
        "identity": "unmistakably a mind flayer, squid-faced aberration with face tentacles",
        "negative": "normal human face, demon brute, dragon, beast, two creatures",
    },
    "lich": {
        "identity": "unmistakably an undead lich, skeletal face and decayed sorcerer body",
        "negative": "living healthy skin, handsome human, demon brute, dragon, two characters",
    },
    "death knight": {
        "identity": "unmistakably an undead knight, cursed armor with skeletal or deathly face",
        "negative": "two knights, living clean hero, angel, dragon",
    },
    "vampire": {
        "identity": "unmistakably a vampire lord, pale aristocratic predator with fangs",
        "negative": "normal hero, werewolf, demon brute, two characters",
    },
    "golem": {
        "identity": "unmistakably a golem or construct, artificial body not flesh humanoid",
        "negative": "human skin, demon flesh, normal person, superhero body, two creatures",
    },
    "hydra": {
        "identity": "unmistakably one hydra, one shared body with many serpent heads",
        "negative": "separate monsters, humanoid, dragon rider, two creatures",
    },
    "ooze": {
        "identity": "unmistakably one ooze, amorphous slime body with no limbs",
        "negative": "humanoid, person, demon, armored body, skeleton",
    },
    "spider": {
        "identity": "unmistakably one giant spider, arachnid anatomy with eight legs",
        "negative": "humanoid, woman, man, drider, demon warrior, two spiders",
    },
    "wolf": {
        "identity": "unmistakably one dire wolf, lupine anatomy not humanoid",
        "negative": "werewolf humanoid, person, demon, dog pack, rider",
    },
    "basilisk": {
        "identity": "unmistakably one basilisk, low reptilian monster with petrifying gaze",
        "negative": "humanoid, person, dragon knight, demon",
    },
    "cockatrice": {
        "identity": "unmistakably one cockatrice, rooster-lizard hybrid monster",
        "negative": "humanoid, person, dragon, demon warrior",
    },
    "ghost": {
        "identity": "unmistakably one ghostly spirit, spectral figure made of ectoplasm",
        "negative": "solid living hero, demon brute, two ghosts, crowd",
    },
    "wraith": {
        "identity": "unmistakably one wraith, shadow spirit with flowing spectral form",
        "negative": "solid armor knight, normal human, demon brute, crowd",
    },
    "skeleton": {
        "identity": "unmistakably one animated skeleton warrior, visible bones",
        "negative": "living body, demon brute, zombie flesh, two skeletons",
    },
    "zombie": {
        "identity": "unmistakably one zombie, rotting corpse body",
        "negative": "clean human, skeleton only, demon brute, crowd",
    },
    "mummy": {
        "identity": "unmistakably one mummy, wrapped cursed corpse",
        "negative": "clean human, demon brute, crowd",
    },
    "devil": {
        "identity": "unmistakably one infernal devil creature",
        "negative": "angel, dragon, normal human hero, multiple devils",
    },
    "demon": {
        "identity": "unmistakably one abyssal demon creature",
        "negative": "two demons, crowd, normal human, dragon unless named dragon",
    },
    "giant": {
        "identity": "unmistakably one giant, enormous scale compared to the room",
        "negative": "normal human size, crowd, duplicate giant",
    },
    "troll": {
        "identity": "unmistakably one troll, lanky regenerating monster",
        "negative": "orc warrior, demon brute, two creatures",
    },
    "hag": {
        "identity": "unmistakably one hag, twisted witch-monster",
        "negative": "beautiful heroine, young woman, crowd",
    },
    "mimic": {
        "identity": "unmistakably one mimic, monstrous transformed chest with teeth and tongue",
        "negative": "person, humanoid, simple treasure chest only, crowd",
    },
}

ALLY_NAME_VISUAL_RULES = {
    "drizzt": "dual scimitars, dark elf features, white hair, panther-like agility",
    "elminster": "elder wizard beard, weathered archmage robes, staff and spellbook aura",
    "bruenor": "dwarven kingly beard, heavy axe, battered plate and shield motifs",
    "wulfgar": "massive barbarian with legendary war hammer mid-throw",
    "cattie": "skilled archer with drawn bow and precise ranged focus",
    "regis": "small halfling rogue with jeweled pendant and cunning stance",
    "mordenkainen": "archwizard with layered robes, arcane sigils, and controlled spellcasting",
    "tasha": "imperious witch queen with dangerous laughter magic",
    "minsc": "muscular ranger hero with tiny hamster companion clearly visible",
    "boo": "small heroic hamster companion clearly visible near the hero",
}

EQUIPMENT_NAME_VISUAL_RULES = {
    "blackstaff": "black mystical staff topped with an ornate arcane headpiece",
    "wand of orcus": "sinister necromantic wand with skull motifs and bone texture",
    "vorpal sword": "long sword with impossibly sharp glowing edge and executioner aura",
    "holy avenger": "blessed longsword radiating sacred golden light",
    "robe of the archmagi": "luxurious archmage robe spread to show rich enchanted fabric",
    "shield of the sentinel": "broad sentinel shield with vigilant eye-like crest",
    "cloak of displacement": "cloak rippling with mirrored distortion around its edges",
    "boots of speed": "pair of enchanted leather boots with speed runes and motion streaks",
    "ring of regeneration": "single ring with vivid green healing gem and living energy",
    "staff of power": "ornate staff crowned by a powerful crystal focus",
}

EQUIPMENT_TYPE_NEGATIVES = {
    "boots": "staff, wand, pillar, totem, sword, spear, person wearing boots",
    "ring": "boots, gloves, sword, staff, amulet, crown",
    "amulet": "ring, staff, sword, boots, armor, person wearing necklace",
    "staff": "person holding staff, spear, sword, wand-only, pillar",
    "wand": "staff, sword, spear, person casting spell",
    "sword": "staff, wand, spear, dagger cluster, person holding weapon",
    "dagger": "sword, spear, staff, person holding weapon",
    "cloak": "person wearing cloak, full body, cape on character",
    "robe": "person wearing robe, full body, wizard portrait",
    "shield": "person holding shield, knight portrait, full body",
    "gauntlets": "person wearing gauntlets, hands attached to body, full body",
    "gloves": "person wearing gloves, hands attached to body, full body",
    "bracers": "person wearing bracers, arms attached to body, full body",
    "vial": "staff, sword, orb, potion held in hand",
    "powder": "staff, sword, potion bottle held in hand",
    "circlet": "crown on person, portrait, head wearing circlet",
    "mail": "person wearing armor, full body warrior",
    "armor": "person wearing armor, full body warrior",
}

NON_HUMANOID_LITERAL_KEYWORDS = {
    "dragon",
    "elder brain",
    "brain",
    "beholder",
    "golem",
    "hydra",
    "ooze",
    "spider",
    "wolf",
    "basilisk",
    "cockatrice",
    "ghost",
    "wraith",
    "skeleton",
    "zombie",
    "mummy",
    "mimic",
}

ROOM_VISUALS = {
    "crypt": "funerary stone tombs, sarcophagi, and sepulchral gloom",
    "forge": "molten channels, anvils, chains, and hot iron glow",
    "chapel": "ruined altar, cracked icons, and desecrated sacred stone",
    "library": "towering shelves, chained books, and arcane dust",
    "cavern": "jagged natural rock, echoing darkness, and mineral glow",
    "throne": "raised dais, broken throne, and ominous ceremonial layout",
    "garden": "twisted roots, poisonous foliage, and overgrown ruins",
    "arena": "combat pit, scattered weapons, and bloodstained stone",
    "workshop": "alchemical devices, shattered glass, and unstable apparatus",
    "observatory": "celestial machinery, lenses, and cosmic architecture",
    "catacombs": "long bone-lined corridors and burial alcoves",
    "hatchery": "eggs, nest debris, and draconic heat",
    "rift": "reality tear, abyssal light, and fractured stone",
    "web": "thick webs, cocoons, and clinging shadows",
    "tunnel": "narrow collapsed stone passages and rubble",
    "gate": "portcullis, iron bars, and choke-point corridor",
}


@dataclass
class CardDefinition:
    set_code: str
    card_type: str
    name: str
    rarity: str
    cost: int | None = None
    attack: int | None = None
    hit_points: int | None = None
    initiative: int | None = None
    treasure: int | None = None
    damage: int | None = None
    effect: str | None = None
    is_ambusher: bool | None = None
    attack_mod: int | None = None
    hit_points_mod: int | None = None
    initiative_mod: int | None = None
    slot: str | None = None
    room_order: int | None = None
    monster_cost_budget: int | None = None


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(
        description="Generate card art via a local AUTOMATIC1111 Stable Diffusion API."
    )
    parser.add_argument("--style-guide", type=Path, default=STYLE_GUIDE_PATH)
    parser.add_argument("--seeds-root", type=Path, default=SEEDS_ROOT)
    parser.add_argument("--output-dir", type=Path, default=OUTPUT_ROOT)
    parser.add_argument("--overrides", type=Path, default=OVERRIDES_PATH)
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
    if value.startswith("EquipmentSlot."):
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


def card_override_key(card: CardDefinition) -> str:
    return f"{card.set_code}:{card.card_type}:{card.name}".lower()


def load_overrides(path: Path) -> dict[str, dict[str, Any]]:
    if not path.exists():
        return {}

    data = json.loads(path.read_text(encoding="utf-8"))
    loaded: dict[str, dict[str, Any]] = {}
    for raw_key, value in data.items():
        loaded[raw_key.lower()] = value
    return loaded


def parse_seed_cards(seeds_root: Path) -> list[CardDefinition]:
    cards: list[CardDefinition] = []
    typed_seed_files = sorted(seeds_root.glob("DND1_*.cs"))

    for seed_file in typed_seed_files:
        set_code = "DND1"
        content = seed_file.read_text(encoding="utf-8")
        constructor_type = TYPED_SEED_FILE_TO_CONSTRUCTOR.get(seed_file.name)
        if not constructor_type:
            continue

        explicit_pattern = re.compile(rf"new\s+{constructor_type}\s*\(")
        target_typed_pattern = re.compile(r"new\s*\(\s*new\s+Guid\s*\(")
        seen_open_indices: set[int] = set()

        for pattern in (explicit_pattern, target_typed_pattern):
            for match in pattern.finditer(content):
                open_index = content.find("(", match.start())
                if open_index in seen_open_indices:
                    continue
                seen_open_indices.add(open_index)
                args_text, _ = extract_balanced(content, open_index, "(", ")")
                parsed = parse_typed_constructor_args(constructor_type, args_text)
                cards.append(build_card_definition(set_code, TYPE_TO_CARD_KIND[constructor_type], parsed))

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


def extract_keywords(value: str) -> list[str]:
    return title_case_words(value)


def collect_matching_phrases(text: str, mapping: dict[str, str]) -> list[str]:
    haystack = text.lower()
    matches: list[str] = []
    for keyword, phrase in mapping.items():
        if keyword in haystack and phrase not in matches:
            matches.append(phrase)
    return matches


def collect_identity_rules(text: str) -> list[dict[str, str]]:
    haystack = text.lower()
    matches: list[dict[str, str]] = []
    for keyword, rule in MONSTER_IDENTITY_RULES.items():
        if keyword in haystack:
            matches.append(rule)
    return matches


def collect_regex_phrases(text: str, pattern_rules: list[tuple[str, str]]) -> list[str]:
    matches: list[str] = []
    for pattern, phrase in pattern_rules:
        if re.search(pattern, text, flags=re.IGNORECASE):
            matches.append(phrase)
    return matches


def select_checkpoint(card: CardDefinition) -> str:
    source = f"{card.name} {card.effect or ''}".lower()

    if card.card_type in {"Equipment", "Trap", "DungeonRoom"}:
        return CHECKPOINT_SD15

    if card.card_type in {"Monster", "Boss"}:
        if any(keyword in source for keyword in NON_HUMANOID_LITERAL_KEYWORDS):
            return CHECKPOINT_SD15
        return CHECKPOINT_DREAMSHAPER

    return CHECKPOINT_DREAMSHAPER


def infer_equipment_slot(card: CardDefinition) -> str | None:
    if card.slot:
        normalized = str(card.slot).strip()
        if normalized.startswith("EquipmentSlot."):
            normalized = normalized.split(".", 1)[1]
        normalized = normalized[:1].upper() + normalized[1:].lower()
        if normalized:
            return normalized

    source = f"{card.name} {card.effect or ''}".lower()
    for keyword, slot in CONSUMABLE_SLOT_KEYWORDS.items():
        if keyword in source:
            return slot
    return None


def is_consumable_equipment(card: CardDefinition) -> bool:
    if card.card_type != "Equipment":
        return False
    slot = infer_equipment_slot(card)
    return slot in CONSUMABLE_SLOTS


def build_equipment_type_negative(card: CardDefinition) -> str:
    if is_consumable_equipment(card):
        slot = infer_equipment_slot(card)
        if slot in CONSUMABLE_SLOT_NEGATIVES:
            return CONSUMABLE_SLOT_NEGATIVES[slot]
        return "wrong consumable type, unrelated object, person holding item"

    source = card.name.lower()
    for keyword, negative in EQUIPMENT_TYPE_NEGATIVES.items():
        if keyword in source:
            return negative
    return "wrong object type, unrelated object, person holding item"


def join_phrases(phrases: list[str]) -> str:
    unique: list[str] = []
    for phrase in phrases:
        cleaned = collapse_whitespace(phrase)
        if cleaned and cleaned not in unique:
            unique.append(cleaned)
    return ", ".join(unique)


def infer_ally_class(card: CardDefinition) -> str:
    haystack = f"{card.name} {card.effect or ''}".lower()
    for keyword, class_name in CLASS_HINTS.items():
        if keyword in haystack:
            return class_name
    return "adventurer"


def build_ally_visual_profile(card: CardDefinition, hero_class: str) -> str:
    source = f"{card.name} {card.effect or ''}"
    phrases = collect_matching_phrases(source, RACE_VISUALS)
    phrases.extend(collect_matching_phrases(source, ALLY_NAME_VISUAL_RULES))
    class_phrase = CLASS_VISUALS.get(hero_class)
    if class_phrase:
        phrases.append(class_phrase)

    if card.is_ambusher:
        phrases.append("subtle stealth posture and hidden attack readiness")
    if card.initiative and card.initiative >= 5:
        phrases.append("quick agile movement frozen mid-action")
    if card.attack and card.attack >= 6:
        phrases.append("powerful athletic physique and forceful strike")
    if card.hit_points and card.hit_points >= 7:
        phrases.append("battle-worn resilience and heavy protective gear")

    effect_text = (card.effect or "").lower()
    if "wings" in effect_text:
        phrases.append("spectral wings unfurling behind the hero")
    if "lightning" in effect_text:
        phrases.append("arcs of lightning around the weapon or hands")
    if "heal" in effect_text or "restores" in effect_text:
        phrases.append("holy or restorative energy gathering around one hand")
    if "trap" in effect_text:
        phrases.append("tools or tactical gear hinting at trap mastery")
    if "shadow" in source.lower():
        phrases.append("deep shadow magic curling around the figure")
    if "fire" in effect_text:
        phrases.append("embers and flame-lit highlights around the attack")

    if not phrases:
        phrases.append("distinctive race and class silhouette clearly readable at card size")

    return join_phrases(phrases)


def build_monster_visual_profile(card: CardDefinition) -> str:
    source = f"{card.name} {card.effect or ''}".lower()
    phrases = collect_matching_phrases(source, MONSTER_BASE_VISUALS)
    identity_rules = collect_identity_rules(source)
    phrases.extend(rule["identity"] for rule in identity_rules)
    phrases.extend(collect_matching_phrases(source, MONSTER_ADJECTIVE_VISUALS))
    phrases.extend(collect_matching_phrases(source, MONSTER_EFFECT_VISUALS))

    if card.attack and card.attack >= 8:
        phrases.append("extreme physical power and crushing presence")
    if card.hit_points and card.hit_points >= 9:
        phrases.append("thick armor, hide, or supernatural durability")
    if card.initiative and card.initiative >= 5:
        phrases.append("predatory speed and alert attack posture")

    if "single" not in " ".join(phrases):
        phrases.insert(0, "single monster only, one creature centered in frame")

    return join_phrases(phrases)


def build_boss_visual_profile(card: CardDefinition) -> str:
    source = f"{card.name} {card.effect or ''}".lower()
    phrases = collect_matching_phrases(source, MONSTER_BASE_VISUALS)
    identity_rules = collect_identity_rules(source)
    phrases.extend(rule["identity"] for rule in identity_rules)
    phrases.extend(collect_matching_phrases(source, MONSTER_ADJECTIVE_VISUALS))
    phrases.extend(collect_matching_phrases(source, MONSTER_EFFECT_VISUALS))
    phrases.append("single boss only, one central subject dominating the entire image")
    phrases.append("mythic scale with the environment dwarfed around it")
    if card.attack and card.attack >= 12:
        phrases.append("apocalyptic destructive power")
    if card.hit_points and card.hit_points >= 20:
        phrases.append("near-immortal resilience and overwhelming durability")
    return join_phrases(phrases)


def build_trap_visual_profile(card: CardDefinition) -> str:
    effect_text = (card.effect or "").lower()
    phrases: list[str] = []
    if "acid" in effect_text or "acid" in card.name.lower():
        phrases.append("corrosive acid jets spraying outward")
    if "rune" in card.name.lower():
        phrases.append("runic glyphs igniting across carved stone")
    if "spike" in effect_text or "spike" in card.name.lower():
        phrases.append("razor spikes thrusting from floor or walls")
    if "fire" in effect_text or "flame" in effect_text:
        phrases.append("blazing fire burst and molten sparks")
    if "poison" in effect_text:
        phrases.append("sickly toxic mist and venom glow")
    if "chain" in effect_text:
        phrases.append("enchanted chains snapping taut in mid-air")
    if "alarm" in card.name.lower():
        phrases.append("sonic rune pulse rippling through the chamber")
    if "pit" in card.name.lower():
        phrases.append("collapsing stone floor opening into darkness")
    if not phrases:
        phrases.append("one trap mechanism clearly readable with the magical effect frozen at activation")
    return join_phrases(phrases)


def build_room_visual_profile(card: CardDefinition) -> str:
    source = f"{card.name} {card.effect or ''}".lower()
    phrases = collect_matching_phrases(source, ROOM_VISUALS)
    if card.room_order == 5:
        phrases.append("endgame menace and climactic oppressive scale")
    elif card.room_order == 1:
        phrases.append("entry-level dungeon threshold with immediate danger")
    if "fog" in source or "mist" in source:
        phrases.append("thick low fog obscuring the floor")
    if "lava" in source or "molten" in source:
        phrases.append("molten light reflecting across stone surfaces")
    if "shadow" in source or "darkness" in source:
        phrases.append("deep shadow pools swallowing the edges of the room")
    if not phrases:
        phrases.append("unique environmental storytelling landmarks that identify this chamber instantly")
    return join_phrases(phrases)


def build_effect_hint(card: CardDefinition) -> str:
    parts: list[str] = []

    if card.effect:
        first_sentence = re.split(r"[.!?]", card.effect, maxsplit=1)[0]
        if first_sentence.strip():
            parts.append(collapse_whitespace(first_sentence.strip()))

    if card.card_type == "Equipment":
        if (card.attack_mod or 0) > 0:
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


def build_consumable_item_subject(card: CardDefinition, slot: str) -> str:
    base = CONSUMABLE_SLOT_BASE_SUBJECT.get(slot, "single-use magical consumable item")
    name = card.name.lower()
    traits: list[str] = []

    if "healing" in name or "restoration" in name:
        traits.append("etched with restorative sigils")
    if "fire" in name:
        traits.append("radiating volatile fire runes")
    if "lightning" in name or "thunder" in name:
        traits.append("wrapped in electric rune bands")
    if "acid" in name or "poison" in name:
        traits.append("containing corrosive toxic compounds")
    if "invisibility" in name or "mirror" in name:
        traits.append("surrounded by refractive illusion shimmer")
    if "time stop" in name:
        traits.append("marked by frozen clockwork glyphs")
    if "wish" in name or "immortality" in name:
        traits.append("engraved with mythic legendary iconography")

    suffix = f" {', '.join(traits)}" if traits else ""
    return collapse_whitespace(f"{base}{suffix}")


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
        extra_traits.append("with attack glyphs")
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


def build_consumable_visual_profile(card: CardDefinition, slot: str) -> str:
    source = f"{card.name} {card.effect or ''}".lower()
    phrases: list[str] = []

    if slot == "Scroll":
        phrases.append("rolled parchment body, wax seal, and visible arcane script lines")
    elif slot == "Potion":
        phrases.append("thick glass bottle, cork stopper, and swirling luminous liquid level")
    elif slot == "Bomb":
        phrases.append("throwable flask silhouette with reinforced stopper and unstable pressure glow")
    elif slot == "Balm":
        phrases.append("apothecary jar form with textured salve and healer marks")
    elif slot == "Totem":
        phrases.append("upright carved idol silhouette with ritual carvings and hanging charms")

    phrases.extend(collect_regex_phrases(source, CONSUMABLE_VISUAL_PATTERN_RULES))

    if (card.attack_mod or 0) > 0:
        phrases.append("offensive battle runes pulsing from the item core")
    if (card.hit_points_mod or 0) > 0:
        phrases.append("protective restorative aura wrapping the object")
    if (card.initiative_mod or 0) > 0:
        phrases.append("swift kinetic glyph trails orbiting the object")
    if (card.initiative_mod or 0) < 0:
        phrases.append("hindering debuff sigils with heavy slowing energy")

    phrases.append("single object only, centered, fully visible silhouette with no secondary subject")
    return join_phrases(phrases)


def build_equipment_visual_profile(card: CardDefinition) -> str:
    source = card.name.lower()
    words = extract_keywords(card.name)
    phrases: list[str] = []
    phrases.extend(collect_matching_phrases(source, EQUIPMENT_NAME_VISUAL_RULES))
    if "sword" in words or "dagger" in words or "axe" in words or "hammer" in words or "spear" in words:
        phrases.append("weapon angled to show blade or striking head clearly")
    if "wand" in words or "staff" in words:
        phrases.append("arcane focus covered in glowing runes and magical focal gem")
    if "boots" in words or "gloves" in words or "gauntlets" in words:
        phrases.append("pair of matching items displayed cleanly and symmetrically")
    if "robe" in words or "cloak" in words or "cape" in words or "mail" in words or "armor" in words:
        phrases.append("fabric or armor surfaces fully visible with ornate trim and wear")
    if "ring" in words or "amulet" in words or "circlet" in words:
        phrases.append("jewelry centerpiece shown close enough to read engravings and gemstones")
    if "vial" in words or "powder" in words:
        phrases.append("alchemical contents visibly glowing inside the container")
    if "shield" in words or "buckler" in words or "bracers" in words:
        phrases.append("defensive surfaces facing the viewer with engraved symbols")
    if "orb" in words:
        phrases.append("floating object suspended above the pedestal")
    if "lantern" in words:
        phrases.append("internal magical flame illuminating the metal frame")
    if not phrases:
        phrases.append("item silhouette perfectly isolated and unmistakable at card size")
    return join_phrases(phrases)


def build_prompt(card: CardDefinition) -> str:
    effect_hint = build_effect_hint(card)

    if card.card_type == "Equipment":
        if is_consumable_equipment(card):
            slot = infer_equipment_slot(card) or "Consumable"
            item_subject = build_consumable_item_subject(card, slot)
            visual_profile = build_consumable_visual_profile(card, slot)
            return collapse_whitespace(
                f"{card.name}, a single {item_subject}, single-use fantasy {slot.lower()} consumable, unmistakably matching the card name, {visual_profile}, {effect_hint}, "
                "isolated on dark dungeon stone pedestal, dramatic underlighting from below, glowing golden runes, "
                "antique gold and brown tones, deep purple magical glow, centered composition, object fills most of frame, "
                "highly detailed texture, no character, no person, no hands, no wearer, no model, no secondary object, "
                "trading card item art, dark fantasy digital painting"
            )

        item_name = describe_equipment_item(card.name)
        visual_profile = build_equipment_visual_profile(card)
        return collapse_whitespace(
            f"a single {item_name}, object-only still life, fantasy magical item, unmistakably matching the card name, {visual_profile}, {effect_hint}, "
            "isolated on dark dungeon stone pedestal, dramatic underlighting from below, "
            "glowing golden runes, antique gold and brown tones, deep purple magical glow, centered composition, object fills most of frame, "
            "highly detailed texture, no character, no person, no hands, no wearer, no model, trading card item art, "
            "dark fantasy digital painting"
        )

    if card.card_type == "Ally":
        hero_class = infer_ally_class(card)
        visual_profile = build_ally_visual_profile(card, hero_class)
        return collapse_whitespace(
            f"{card.name}, fantasy {hero_class} hero, {visual_profile}, heroic action pose, determined expression, "
            "worn battle gear with golden trim, dramatic underlighting from dungeon torches below, "
            "epic low-angle perspective, dark dungeon background with stone arches, dominant brown "
            "and antique gold palette, deep purple accents, dark fantasy digital painting, "
            "trading card character art"
        )

    if card.card_type == "Monster":
        visual_profile = build_monster_visual_profile(card)
        return collapse_whitespace(
            f"{card.name}, fearsome fantasy monster, {visual_profile}, one monster only, centered single-subject composition, aggressive threatening pose, massive imposing figure, "
            "dramatic purple underlighting from below, ancient dungeon environment, dominant dark brown "
            "and deep purple palette, glowing eyes, no secondary figures, dark fantasy digital painting, trading card monster art"
        )

    if card.card_type == "Trap":
        visual_profile = build_trap_visual_profile(card)
        return collapse_whitespace(
            f"{card.name} trap activating, magical mechanism in motion, {visual_profile}, {effect_hint}, dark dungeon floor, "
            "dramatic purple and gold energy effect, no character, no person, isolated magical effect, "
            "dark fantasy digital painting, trading card trap art"
        )

    if card.card_type == "DungeonRoom":
        visual_profile = build_room_visual_profile(card)
        return collapse_whitespace(
            f"{card.name} dungeon room, wide establishing shot, {visual_profile}, ancient stone architecture, dramatic "
            "underlighting from floor torches and runes, oppressive atmosphere, volumetric fog, dungeon "
            "crawl environment, dark fantasy digital painting, trading card location art"
        )

    if card.card_type == "Boss":
        visual_profile = build_boss_visual_profile(card)
        return collapse_whitespace(
            f"{card.name}, legendary boss creature, {visual_profile}, one boss only, centered single-subject composition, monumental scale filling entire frame, overwhelming "
            "presence, dramatic purple and gold underlighting, ancient dungeon throne room, inevitable "
            "power aura, no secondary figures, dark fantasy digital painting, trading card boss art"
        )

    raise ValueError(f"Unsupported card type for prompt building: {card.card_type}")


def build_negative_prompt(card: CardDefinition) -> str:
    if card.card_type == "Equipment":
        extra = "full body, portrait, model wearing item, fashion shot, hero pose"
        source = card.name.lower()
        if "boots" in source or "gloves" in source or "gauntlets" in source:
            extra += ", feet in boots, person wearing boots, hands wearing gloves"
        if is_consumable_equipment(card):
            extra += ", hand holding item, drinking pose, throwing pose, multiple items, item bundle, inventory layout, tabletop clutter"
        extra += f", {build_equipment_type_negative(card)}"
        return f"{EQUIPMENT_NEGATIVE_PROMPT}, {extra}"
    if card.card_type in {"Ally", "Monster", "Boss"}:
        if card.card_type in {"Monster", "Boss"}:
            source = f"{card.name} {card.effect or ''}".lower()
            rule_negatives = [rule["negative"] for rule in collect_identity_rules(source)]
            extra = "multiple creatures, duplicate monster, crowd, extra heads, extra limbs, two subjects, group shot"
            if rule_negatives:
                extra = f"{extra}, {', '.join(rule_negatives)}"
            return f"{CREATURE_NEGATIVE_PROMPT}, {extra}"
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


def manifest_payload(cards: list[CardDefinition], overrides: dict[str, dict[str, Any]]) -> list[dict[str, Any]]:
    payload: list[dict[str, Any]] = []

    for card in cards:
        override = overrides.get(card_override_key(card), {})
        row = asdict(card)
        row["prompt"] = override.get("prompt", build_prompt(card))
        row["negative_prompt"] = override.get("negative_prompt", build_negative_prompt(card))
        row["checkpoint"] = override.get("checkpoint", select_checkpoint(card))
        if "notes" in override:
            row["override_notes"] = override["notes"]
        payload.append(row)

    return payload


def write_manifest(output_dir: Path, cards: list[CardDefinition], overrides: dict[str, dict[str, Any]]) -> Path:
    output_dir.mkdir(parents=True, exist_ok=True)
    manifest_path = output_dir / "manifest.json"
    manifest = manifest_payload(cards, overrides)
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
    overrides: dict[str, dict[str, Any]],
    output_dir: Path,
    base_url: str,
    overwrite: bool,
) -> None:
    for card in cards:
        override = overrides.get(card_override_key(card), {})
        target_path = output_dir / card.set_code / card.card_type.lower() / f"{slugify(card.name)}.png"
        if target_path.exists() and not overwrite:
            print(f"Skipping existing file: {target_path}")
            continue

        payload = {
            **SD_PAYLOAD,
            "prompt": override.get("prompt", build_prompt(card)),
            "negative_prompt": override.get("negative_prompt", build_negative_prompt(card)),
            "override_settings": {
                "sd_model_checkpoint": override.get("checkpoint", select_checkpoint(card)),
            },
            "override_settings_restore_afterwards": True,
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
    overrides = load_overrides(args.overrides)
    cards = parse_seed_cards(args.seeds_root)
    selected_cards = filter_cards(cards, args)

    if not selected_cards:
        print("No cards matched the provided filters.", file=sys.stderr)
        return 1

    manifest_path = write_manifest(args.output_dir, selected_cards, overrides)
    print(f"Selected {len(selected_cards)} cards. Manifest written to {manifest_path}")

    if args.parse_only:
        preview = manifest_payload(selected_cards[:3], overrides)
        print(json.dumps(preview, indent=2, ensure_ascii=False))
        return 0

    generate_cards(
        selected_cards,
        overrides,
        args.output_dir,
        args.base_url,
        args.overwrite,
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
