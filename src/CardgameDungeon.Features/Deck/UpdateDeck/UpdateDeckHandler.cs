using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Repositories;
using CardgameDungeon.Features.Deck.Shared;
using MediatR;

namespace CardgameDungeon.Features.Deck.UpdateDeck;

public class UpdateDeckHandler(ICardRepository cardRepo, IDeckRepository deckRepo)
    : IRequestHandler<UpdateDeckCommand, DeckResponse>
{
    public async Task<DeckResponse> Handle(UpdateDeckCommand request, CancellationToken ct)
    {
        var existing = await deckRepo.GetByIdAsync(request.DeckId, ct)
            ?? throw new KeyNotFoundException($"Deck {request.DeckId} not found.");

        var adventurerCards = await cardRepo.GetByIdsAsync(request.AdventurerCardIds, ct);
        var enemyCards = await cardRepo.GetByIdsAsync(request.EnemyCardIds, ct);
        var dungeonRooms = await cardRepo.GetDungeonRoomsByIdsAsync(request.DungeonRoomIds, ct);
        var boss = await cardRepo.GetBossByIdAsync(request.BossCardId, ct)
            ?? throw new InvalidOperationException($"Boss card {request.BossCardId} not found.");

        // Reconstruct with same id/player, new cards — runs domain validation
        var updated = new DeckList(
            existing.Id,
            existing.PlayerId,
            adventurerCards,
            enemyCards,
            dungeonRooms,
            boss);

        await deckRepo.UpdateAsync(updated, ct);

        return DeckMapper.ToResponse(updated);
    }
}
