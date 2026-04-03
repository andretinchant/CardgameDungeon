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

**Rarities:** Unique (1 per deck), Rare, Uncommon, Common.

### Match Flow

**Setup:** Each player fields a starting team with total cost ≤ 5, revealed simultaneously.

**Cost system:** Pay by discarding/exiling from the top of your deck, or by restoring cards from opponent's discard pile.

**Defender draw:** Defender draws up to 8 cards when entering a new room.

**Room without monsters:** Attacker wins without combat.

### Combat

- Sum of strength determines damage to HP. Tie → both sides take damage.
- Normal room: simultaneous elimination = defender wins.
- Boss room: simultaneous elimination = attacking allies win.

### Initiative

- Sum of initiative values. Tie → bid war (discard or exile from top).
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
