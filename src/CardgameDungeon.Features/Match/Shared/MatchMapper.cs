using CardgameDungeon.Domain.Entities;

namespace CardgameDungeon.Features.Match.Shared;

public static class MatchMapper
{
    public static MatchResponse ToResponse(MatchState match)
        => new(
            match.Id,
            match.Phase,
            match.CurrentRoom,
            ToPlayerDto(match.Player1),
            ToPlayerDto(match.Player2),
            match.InitiativeWinnerId,
            match.WinnerId,
            match.CurrentDungeonRoom is { } room ? ToRoomDto(room) : null,
            match.IsBossRoom);

    private static PlayerStateDto ToPlayerDto(PlayerState player)
        => new(
            player.PlayerId,
            player.HitPoints,
            player.Deck.Count,
            player.Hand.Count,
            player.Discard.Count,
            player.ExileCount,
            player.AlliesInPlay.Select(ToAllyDto).ToList());

    private static AllyDto ToAllyDto(AllyCard ally)
        => new(ally.Id, ally.Name, ally.Strength, ally.HitPoints,
            ally.Initiative, ally.Cost, ally.IsAmbusher);

    private static DungeonRoomDto ToRoomDto(DungeonRoomCard room)
        => new(room.Id, room.Name, room.Order, room.HasMonsters);
}
