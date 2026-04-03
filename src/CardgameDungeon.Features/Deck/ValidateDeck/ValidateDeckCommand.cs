using MediatR;

namespace CardgameDungeon.Features.Deck.ValidateDeck;

public record ValidateDeckCommand(Guid DeckId) : IRequest<ValidateDeckResponse>;

public record ValidateDeckResponse(
    Guid DeckId,
    bool IsValid,
    IReadOnlyList<string> Errors);
