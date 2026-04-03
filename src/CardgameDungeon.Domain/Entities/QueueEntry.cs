using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.Domain.Entities;

public class QueueEntry
{
    public Guid PlayerId { get; private set; }
    public Guid DeckId { get; private set; }
    public QueueType QueueType { get; private set; }
    public int Elo { get; private set; }
    public DateTime JoinedAt { get; private set; }

    public QueueEntry(Guid playerId, Guid deckId, QueueType queueType, int elo)
    {
        PlayerId = playerId;
        DeckId = deckId;
        QueueType = queueType;
        Elo = elo;
        JoinedAt = DateTime.UtcNow;
    }

    public int GetExpandedRange(int baseRange, int expansionPerInterval, int intervalSeconds)
    {
        var elapsed = (DateTime.UtcNow - JoinedAt).TotalSeconds;
        var expansions = (int)(elapsed / intervalSeconds);
        return baseRange + (expansions * expansionPerInterval);
    }

    public bool IsInRange(QueueEntry other, int range)
    {
        return Math.Abs(Elo - other.Elo) <= range;
    }
}
