using CardgameDungeon.Features.Deck.Shared;
using MediatR;

namespace CardgameDungeon.Features.Deck.UpdateDeck;

public record UpdateDeckCommand(
    Guid DeckId,
    IReadOnlyList<Guid> AdventurerCardIds,
    IReadOnlyList<Guid> EnemyCardIds,
    IReadOnlyList<Guid> DungeonRoomIds,
    Guid BossCardId) : IRequest<DeckResponse>;
