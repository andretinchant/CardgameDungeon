using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.Features.Deck.Shared;

public record DeckResponse(
    Guid Id,
    Guid PlayerId,
    IReadOnlyList<CardDto> AdventurerCards,
    IReadOnlyList<CardDto> EnemyCards,
    IReadOnlyList<DungeonRoomDto> DungeonRooms,
    CardDto Boss);

public record CardDto(
    Guid Id,
    string Name,
    CardType Type,
    Rarity Rarity,
    int Cost);

public record DungeonRoomDto(
    Guid Id,
    string Name,
    int Order,
    IReadOnlyList<Guid> MonsterIds,
    IReadOnlyList<Guid> TrapIds);
