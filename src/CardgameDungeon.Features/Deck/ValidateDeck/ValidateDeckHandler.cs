using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Repositories;
using MediatR;

namespace CardgameDungeon.Features.Deck.ValidateDeck;

public class ValidateDeckHandler(IDeckRepository deckRepo)
    : IRequestHandler<ValidateDeckCommand, ValidateDeckResponse>
{
    public async Task<ValidateDeckResponse> Handle(ValidateDeckCommand request, CancellationToken ct)
    {
        var deck = await deckRepo.GetByIdAsync(request.DeckId, ct)
            ?? throw new KeyNotFoundException($"Deck {request.DeckId} not found.");

        var errors = new List<string>();

        try
        {
            // Re-run domain validation by reconstructing
            _ = new DeckList(
                deck.Id,
                deck.PlayerId,
                deck.AdventurerCards,
                deck.EnemyCards,
                deck.DungeonRooms,
                deck.Boss);
        }
        catch (InvalidOperationException ex)
        {
            errors.AddRange(
                ex.Message.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                    .Skip(1)); // Skip the "Invalid deck list:" header
        }

        return new ValidateDeckResponse(deck.Id, errors.Count == 0, errors);
    }
}
