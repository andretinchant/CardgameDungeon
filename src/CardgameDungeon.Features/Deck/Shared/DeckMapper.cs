using CardgameDungeon.Domain.Entities;

namespace CardgameDungeon.Features.Deck.Shared;

public static class DeckMapper
{
    public static DeckResponse ToResponse(DeckList deck)
        => new(
            deck.Id,
            deck.PlayerId,
            deck.AdventurerCards.Select(ToCardDto).ToList(),
            deck.EnemyCards.Select(ToCardDto).ToList(),
            deck.DungeonRooms.Select(ToDungeonRoomDto).ToList(),
            ToCardDto(deck.Boss));

    private static CardDto ToCardDto(Card card)
        => new(card.Id, card.Name, card.Type, card.Rarity, card.Cost);

    private static DungeonRoomDto ToDungeonRoomDto(DungeonRoomCard room)
        => new(room.Id, room.Name, room.Order, room.MonsterIds, room.TrapIds);
}
