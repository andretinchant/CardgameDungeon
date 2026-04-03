using CardgameDungeon.Features.Deck.Shared;
using MediatR;

namespace CardgameDungeon.Features.Deck.GetDeck;

public record GetDeckQuery(Guid DeckId) : IRequest<DeckResponse>;
