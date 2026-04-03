using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Repositories;
using CardgameDungeon.Features.Match.Shared;
using MediatR;

namespace CardgameDungeon.Features.Match.CreateMatch;

public class CreateMatchHandler(IDeckRepository deckRepo, IMatchRepository matchRepo, IMatchNotifier notifier)
    : IRequestHandler<CreateMatchCommand, MatchResponse>
{
    public async Task<MatchResponse> Handle(CreateMatchCommand request, CancellationToken ct)
    {
        var deck1 = await deckRepo.GetByIdAsync(request.Player1DeckId, ct)
            ?? throw new KeyNotFoundException($"Deck {request.Player1DeckId} not found.");
        var deck2 = await deckRepo.GetByIdAsync(request.Player2DeckId, ct)
            ?? throw new KeyNotFoundException($"Deck {request.Player2DeckId} not found.");

        // Shuffle adventurer cards for each player's draw deck
        var p1Deck = Shuffle(deck1.AdventurerCards);
        var p2Deck = Shuffle(deck2.AdventurerCards);

        var player1 = new PlayerState(request.Player1Id, request.StartingHitPoints, p1Deck);
        var player2 = new PlayerState(request.Player2Id, request.StartingHitPoints, p2Deck);

        // Draw initial hands
        player1.RefillHand();
        player2.RefillHand();

        // Use player 1's dungeon rooms and boss (shared dungeon)
        var match = new MatchState(
            Guid.NewGuid(),
            player1,
            player2,
            deck1.DungeonRooms,
            deck1.Boss);

        await matchRepo.SaveAsync(match, ct);

        var response = MatchMapper.ToResponse(match);
        await notifier.MatchStarted(match.Id, response);
        return response;
    }

    private static List<Card> Shuffle(IReadOnlyList<Card> cards)
    {
        var list = cards.ToList();
        var rng = Random.Shared;
        for (var i = list.Count - 1; i > 0; i--)
        {
            var j = rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
        return list;
    }
}
