using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Domain.Repositories;
using MediatR;

namespace CardgameDungeon.Features.Matchmaking.FindMatch;

public class FindMatchHandler(
    IQueueRepository queueRepo,
    IDeckRepository deckRepo,
    IMatchRepository matchRepo)
    : IRequestHandler<FindMatchCommand, FindMatchResponse>
{
    private const int RankedBaseRange = 100;
    private const int RankedExpansion = 100;
    private const int RankedExpansionInterval = 30;

    public async Task<FindMatchResponse> Handle(FindMatchCommand request, CancellationToken ct)
    {
        var entries = await queueRepo.GetByQueueTypeAsync(request.QueueType, ct);
        if (entries.Count < 2)
            return new FindMatchResponse(false, null, null, null);

        var sorted = entries.OrderBy(e => e.JoinedAt).ToList();

        for (var i = 0; i < sorted.Count; i++)
        {
            for (var j = i + 1; j < sorted.Count; j++)
            {
                if (request.QueueType == QueueType.Casual || AreInRange(sorted[i], sorted[j]))
                {
                    return await CreateMatch(sorted[i], sorted[j], ct);
                }
            }
        }

        return new FindMatchResponse(false, null, null, null);
    }

    private static bool AreInRange(QueueEntry a, QueueEntry b)
    {
        var rangeA = a.GetExpandedRange(RankedBaseRange, RankedExpansion, RankedExpansionInterval);
        var rangeB = b.GetExpandedRange(RankedBaseRange, RankedExpansion, RankedExpansionInterval);
        var maxRange = Math.Max(rangeA, rangeB);
        return a.IsInRange(b, maxRange);
    }

    private async Task<FindMatchResponse> CreateMatch(
        QueueEntry entry1, QueueEntry entry2, CancellationToken ct)
    {
        var deck1 = await deckRepo.GetByIdAsync(entry1.DeckId, ct)
            ?? throw new KeyNotFoundException($"Deck {entry1.DeckId} not found.");
        var deck2 = await deckRepo.GetByIdAsync(entry2.DeckId, ct)
            ?? throw new KeyNotFoundException($"Deck {entry2.DeckId} not found.");

        var player1 = new PlayerState(entry1.PlayerId, 20, deck1.AdventurerCards.ToList());
        var player2 = new PlayerState(entry2.PlayerId, 20, deck2.AdventurerCards.ToList());
        player1.RefillHand();
        player2.RefillHand();

        var match = new MatchState(
            Guid.NewGuid(), player1, player2, deck1.DungeonRooms, deck1.Boss);

        await matchRepo.SaveAsync(match, ct);
        await queueRepo.RemoveAsync(entry1.PlayerId, ct);
        await queueRepo.RemoveAsync(entry2.PlayerId, ct);

        return new FindMatchResponse(true, match.Id, entry1.PlayerId, entry2.PlayerId);
    }
}
