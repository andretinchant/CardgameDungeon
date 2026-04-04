# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

Competitive card game dungeon crawler. Two players race to defeat the dungeon boss. Backend in C#/.NET 10, Unity client consumes the API. Meta systems: collection, ELO ranking, marketplace, and boosters.

## Build & Run

```bash
dotnet build
dotnet run --project CardgameDungeon.API
dotnet test                              # all tests
dotnet test --filter "FullyQualifiedName~FeatureName"  # single feature tests
```

## Architecture — Vertical Slice

Each feature is self-contained with its own handler, request/response DTOs, and validation. Slices do NOT cross-reference each other; shared logic lives in Domain.

```
CardgameDungeon.API        — HTTP entry point, endpoints, middleware
CardgameDungeon.Features   — one folder per slice (Match, Deck, Collection, Marketplace, etc.)
CardgameDungeon.Domain     — shared entities, value objects, game rules
CardgameDungeon.Tests      — xUnit tests mirroring Features structure
```

Unity client is a separate project that consumes the API.

## Game Rules (reference for domain logic)

### Deck Composition

40 adventurer cards (allies + equipment) + 40 enemy cards (monsters + traps) + 5 dungeon room cards (ordered) + 1 boss (separate). Piles: Deck, Discard, Exile.

**Card types:** Ally, Equipment, Monster, Trap, Dungeon Room, Boss.

**Rarities:** Unique (1 per deck, minimum cost 6 for allies — cannot be used in starting team), Rare, Uncommon, Common.

**Ally Classes:** Warrior (controls combat, redirects damage), Rogue (Advantage bonuses, evasion), Mage (conditional multi-combat effects; Necromancer variant recycles monsters from discard to deck top), Cleric (heals, anti-Undead, conditional buffs), Ranger (reveals hand/deck/traps, favored enemy), Paladin (combat+support hybrid, sacrifice for effects), Bard (returns cards from hand to deck top/bottom/shuffle to control future draws), Monk (discards/exiles from hand to cancel combats, attack twice, or disengage), Warlock (marks enemy during initiative; if marked target dies triggers heal/draw/AOE/exile), Sorcerer (amplifies Scrolls: copy effects, recover from discard via discard/exile cost, empower potency). Each ally has exactly one class.

**Races:** Human, Elf, Dwarf, Orc, Halfling, Dragonborn, Tiefling, Goblin, Undead, Demon, Beast, Construct, Elemental, Giant, Aberration, Dragon. Allies, Monsters, and Bosses each have a Race.

**Equipment Slots (Gear):** Weapon, Armor, Shield, Helmet, Boots, Accessory. Limited to 1 per slot per ally (e.g., no 2 boots, no 2 armors).

**Consumables:** Scroll, Potion, Balm, Bomb, Totem. No equip limit — an ally can carry multiple. Single-use: destroyed after activation. Typically have stronger effects than gear to compensate for being one-time-only. Have an Effect text describing their activation.

### Match Flow

**Setup:** Each player fields a starting team with total cost ≤ 5, revealed simultaneously.

**Cost system:** Pay by discarding/exiling from the top of your deck, or by restoring cards from opponent's discard pile.

**Defender draw:** Defender draws up to 8 cards when entering a new room.

**Room without monsters:** Attacker wins without combat.

### Victory Condition

**Players do NOT have HP.** Victory is achieved by conquering all 5 dungeon rooms and defeating the Boss. Allies and Monsters have individual HP. Exile is the central pressure/punishment mechanic — exiled cards are permanently removed from the game.

### Combat

- Sum of strength determines damage to individual unit HP. Tie → both sides take damage.
- Normal room: simultaneous elimination = defender wins.
- Boss room: simultaneous elimination = attacking allies win.

### Advantage & Disadvantage

Numerical superiority in a combat group determines Advantage/Disadvantage **states**. These states have **no automatic STR bonus** — they only trigger effects described on cards.

- **Advantage**: your side has more units in this combat group (e.g., 3v1, 2v1).
- **Disadvantage**: your side has fewer units (e.g., 1v2, 1v3).
- **Neutral**: equal unit counts (1v1, 2v2).

Cards reference these states in their Effect text: "With Advantage: +2 STR", "With Disadvantage: gains Ambusher", etc. This creates tactical depth around combat assignment — concentrate forces for Advantage effects or spread to cover more targets.

### Initiative

- Sum of initiative values. Tie → bid war (discard or exile from top).
- **Winner chooses role:** The initiative winner decides whether to be the **attacker** or **defender**. This makes high initiative a tactical advantage — you control the tempo.
- **Retarget:** After initiative sequence resolves, paying a cost lets a unit reposition to participate in two combats but deal damage in only one.

### Field & Targeting

- Max 5 allies in play at once.
- **Ambusher:** Cannot be targeted if a non-ambusher ally is available.
- **Attack of opportunity:** 1 per player per round when abandoning a combat.

### Exile

Count is public, contents are hidden. Central pressure mechanic — exile depletes resources permanently.

## Meta Systems

### Economy

Single currency obtained via: real-money purchase, daily rewards (capped), event prizes.

### Boosters

Fixed composition by rarity, purchased with in-game currency.

### Marketplace (P2P)

- Fixed-price listings, 1 copy per listing, 10% transaction fee.
- Listed copy is reserved and unavailable for deck building.

### Collection & Deck Building

- Collection based on actual card copies owned.
- Decklists may only use owned, non-reserved copies.

## Matchmaking & Events

### Ranked & Casual Queues

- Classic ELO with fixed K-factor, progressive range search.
- Dynamic tiers: Bronze (bottom 50%), Silver (middle 30%), Gold (top 20%).
- Tier cutoffs recalculated periodically from active rating distribution.

### Tournaments

- Automatic 8-player brackets segmented by tier.
- Entry fee paid in game currency.
- Prizes: 50% / 30% / 20% split + boosters for top 3.

## Code Conventions

- C# with async/await for all I/O
- English naming throughout
- Rich domain entities with behavior (not anemic models)
- Each slice: Handler + Request + Response + Validation, self-contained
- xUnit tests for every critical business rule
