using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Repositories;
using CardgameDungeon.Infrastructure.Data;
using CardgameDungeon.Infrastructure.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CardgameDungeon.Infrastructure.Repositories;

public class EfDeckRepository(CardgameDungeonDbContext db) : IDeckRepository
{
    public async Task<DeckList?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.DeckLists.FirstOrDefaultAsync(d => d.Id == id, ct);
        if (entity is null) return null;

        return await ReconstructDeckList(entity, ct);
    }

    public async Task SaveAsync(DeckList deck, CancellationToken ct = default)
    {
        var entity = ToEntity(deck);
        db.DeckLists.Add(entity);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(DeckList deck, CancellationToken ct = default)
    {
        var entity = await db.DeckLists.FirstOrDefaultAsync(d => d.Id == deck.Id, ct);
        if (entity is null)
        {
            entity = ToEntity(deck);
            db.DeckLists.Add(entity);
        }
        else
        {
            entity.PlayerId = deck.PlayerId;
            entity.BossCardId = deck.Boss.Id;
            entity.AdventurerCardIds = deck.AdventurerCards.Select(c => c.Id).ToList();
            entity.EnemyCardIds = deck.EnemyCards.Select(c => c.Id).ToList();
            entity.DungeonRoomCardIds = deck.DungeonRooms.Select(c => c.Id).ToList();
        }
        await db.SaveChangesAsync(ct);
    }

    private async Task<DeckList> ReconstructDeckList(DeckListEntity entity, CancellationToken ct)
    {
        var allCardIds = entity.AdventurerCardIds
            .Concat(entity.EnemyCardIds)
            .Concat(entity.DungeonRoomCardIds)
            .Append(entity.BossCardId)
            .Distinct()
            .ToList();

        var cards = await db.Cards.Where(c => allCardIds.Contains(c.Id)).ToDictionaryAsync(c => c.Id, ct);

        var adventurerCards = entity.AdventurerCardIds.Select(id => cards[id]).ToList();
        var enemyCards = entity.EnemyCardIds.Select(id => cards[id]).ToList();
        var dungeonRooms = entity.DungeonRoomCardIds.Select(id => (DungeonRoomCard)cards[id]).ToList();
        var boss = (BossCard)cards[entity.BossCardId];

        return new DeckList(entity.Id, entity.PlayerId, adventurerCards, enemyCards, dungeonRooms, boss, skipValidation: true);
    }

    private static DeckListEntity ToEntity(DeckList deck) => new()
    {
        Id = deck.Id,
        PlayerId = deck.PlayerId,
        BossCardId = deck.Boss.Id,
        AdventurerCardIds = deck.AdventurerCards.Select(c => c.Id).ToList(),
        EnemyCardIds = deck.EnemyCards.Select(c => c.Id).ToList(),
        DungeonRoomCardIds = deck.DungeonRooms.Select(c => c.Id).ToList()
    };
}
