using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Repositories;

namespace CardgameDungeon.Tests.Deck.Fakes;

public class FakeCardRepository : ICardRepository
{
    private readonly Dictionary<Guid, Card> _cards = new();
    private readonly Dictionary<Guid, DungeonRoomCard> _rooms = new();
    private readonly Dictionary<Guid, BossCard> _bosses = new();

    public void Add(Card card) => _cards[card.Id] = card;
    public void Add(DungeonRoomCard room) => _rooms[room.Id] = room;
    public void Add(BossCard boss) => _bosses[boss.Id] = boss;

    public Task<IReadOnlyList<Card>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var result = ids.Select(id => _cards.GetValueOrDefault(id))
            .Where(c => c is not null)
            .Cast<Card>()
            .ToList();
        return Task.FromResult<IReadOnlyList<Card>>(result);
    }

    public Task<IReadOnlyList<DungeonRoomCard>> GetDungeonRoomsByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var result = ids.Select(id => _rooms.GetValueOrDefault(id))
            .Where(r => r is not null)
            .Cast<DungeonRoomCard>()
            .ToList();
        return Task.FromResult<IReadOnlyList<DungeonRoomCard>>(result);
    }

    public Task<BossCard?> GetBossByIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(_bosses.GetValueOrDefault(id));
}
