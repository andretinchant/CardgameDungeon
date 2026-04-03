using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Repositories;
using CardgameDungeon.Features.Deck.Shared;
using MediatR;

namespace CardgameDungeon.Features.Deck.CreateDeck;

public class CreateDeckHandler(ICardRepository cardRepo, IDeckRepository deckRepo)
    : IRequestHandler<CreateDeckCommand, DeckResponse>
{
    public async Task<DeckResponse> Handle(CreateDeckCommand request, CancellationToken ct)
    {
        var adventurerCards = await cardRepo.GetByIdsAsync(request.AdventurerCardIds, ct);
        var enemyCards = await cardRepo.GetByIdsAsync(request.EnemyCardIds, ct);
        var dungeonRooms = await cardRepo.GetDungeonRoomsByIdsAsync(request.DungeonRoomIds, ct);
        var boss = await cardRepo.GetBossByIdAsync(request.BossCardId, ct)
            ?? throw new InvalidOperationException($"Boss card {request.BossCardId} not found.");

        // DeckList constructor runs all domain validations
        var deck = new DeckList(
            Guid.NewGuid(),
            request.PlayerId,
            adventurerCards,
            enemyCards,
            dungeonRooms,
            boss);

        await deckRepo.SaveAsync(deck, ct);

        return DeckMapper.ToResponse(deck);
    }
}
