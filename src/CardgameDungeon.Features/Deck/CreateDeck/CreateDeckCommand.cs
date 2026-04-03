using CardgameDungeon.Features.Deck.Shared;
using MediatR;

namespace CardgameDungeon.Features.Deck.CreateDeck;

public record CreateDeckCommand(
    Guid PlayerId,
    IReadOnlyList<Guid> AdventurerCardIds,
    IReadOnlyList<Guid> EnemyCardIds,
    IReadOnlyList<Guid> DungeonRoomIds,
    Guid BossCardId) : IRequest<DeckResponse>;
