using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.Features.Match.Shared;

public record MatchResponse(
    Guid Id,
    MatchPhase Phase,
    int CurrentRoom,
    PlayerStateDto Player1,
    PlayerStateDto Player2,
    Guid? InitiativeWinnerId,
    Guid? WinnerId,
    DungeonRoomDto? CurrentDungeonRoom,
    bool IsBossRoom);

public record PlayerStateDto(
    Guid PlayerId,
    int HitPoints,
    int DeckCount,
    int HandCount,
    int DiscardCount,
    int ExileCount,
    IReadOnlyList<AllyDto> AlliesInPlay);

public record AllyDto(
    Guid Id,
    string Name,
    int Strength,
    int HitPoints,
    int Initiative,
    int Cost,
    bool IsAmbusher);

public record DungeonRoomDto(
    Guid Id,
    string Name,
    int Order,
    bool HasMonsters);
