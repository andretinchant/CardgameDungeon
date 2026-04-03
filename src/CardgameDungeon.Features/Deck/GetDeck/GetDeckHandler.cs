using CardgameDungeon.Domain.Repositories;
using CardgameDungeon.Features.Deck.Shared;
using MediatR;

namespace CardgameDungeon.Features.Deck.GetDeck;

public class GetDeckHandler(IDeckRepository deckRepo)
    : IRequestHandler<GetDeckQuery, DeckResponse>
{
    public async Task<DeckResponse> Handle(GetDeckQuery request, CancellationToken ct)
    {
        var deck = await deckRepo.GetByIdAsync(request.DeckId, ct)
            ?? throw new KeyNotFoundException($"Deck {request.DeckId} not found.");

        return DeckMapper.ToResponse(deck);
    }
}
