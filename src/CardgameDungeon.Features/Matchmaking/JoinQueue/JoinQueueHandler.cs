using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Repositories;
using MediatR;

namespace CardgameDungeon.Features.Matchmaking.JoinQueue;

public class JoinQueueHandler(
    IQueueRepository queueRepo,
    IDeckRepository deckRepo,
    IRatingRepository ratingRepo)
    : IRequestHandler<JoinQueueCommand, JoinQueueResponse>
{
    public async Task<JoinQueueResponse> Handle(JoinQueueCommand request, CancellationToken ct)
    {
        // Validate deck exists
        var deck = await deckRepo.GetByIdAsync(request.DeckId, ct)
            ?? throw new KeyNotFoundException($"Deck {request.DeckId} not found.");

        // Check not already in queue
        var existing = await queueRepo.GetByPlayerIdAsync(request.PlayerId, ct);
        if (existing is not null)
            throw new InvalidOperationException("Player is already in a queue.");

        var rating = await ratingRepo.GetByPlayerIdAsync(request.PlayerId, ct);
        var elo = rating?.Elo ?? PlayerRating.DefaultElo;

        var entry = new QueueEntry(request.PlayerId, request.DeckId, request.QueueType, elo);
        await queueRepo.AddAsync(entry, ct);

        return new JoinQueueResponse(request.PlayerId, request.QueueType, true);
    }
}
