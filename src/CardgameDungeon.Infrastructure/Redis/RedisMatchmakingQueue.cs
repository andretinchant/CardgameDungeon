using System.Text.Json;
using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Domain.Repositories;
using StackExchange.Redis;

namespace CardgameDungeon.Infrastructure.Redis;

public class RedisMatchmakingQueue(IConnectionMultiplexer redis) : IQueueRepository
{
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    private const string KeyPrefix = "matchmaking:queue:";
    private const string PlayerIndex = "matchmaking:players";

    private IDatabase Db => redis.GetDatabase();

    public async Task<QueueEntry?> GetByPlayerIdAsync(Guid playerId, CancellationToken ct = default)
    {
        var json = await Db.HashGetAsync(PlayerIndex, playerId.ToString());
        if (json.IsNullOrEmpty) return null;
        return Deserialize(json!);
    }

    public async Task<IReadOnlyList<QueueEntry>> GetByQueueTypeAsync(QueueType queueType, CancellationToken ct = default)
    {
        var key = KeyPrefix + queueType.ToString().ToLowerInvariant();
        var entries = await Db.SortedSetRangeByScoreAsync(key);

        var result = new List<QueueEntry>();
        foreach (var entry in entries)
        {
            if (entry.IsNullOrEmpty) continue;
            var qe = Deserialize(entry!);
            if (qe != null) result.Add(qe);
        }
        return result;
    }

    public async Task AddAsync(QueueEntry entry, CancellationToken ct = default)
    {
        var json = Serialize(entry);
        var key = KeyPrefix + entry.QueueType.ToString().ToLowerInvariant();

        // Sorted set by ELO for range-based matching
        await Db.SortedSetAddAsync(key, json, entry.Elo);

        // Player index for quick lookup
        await Db.HashSetAsync(PlayerIndex, entry.PlayerId.ToString(), json);
    }

    public async Task RemoveAsync(Guid playerId, CancellationToken ct = default)
    {
        var json = await Db.HashGetAsync(PlayerIndex, playerId.ToString());
        if (json.IsNullOrEmpty) return;

        var entry = Deserialize(json!);
        if (entry == null) return;

        var key = KeyPrefix + entry.QueueType.ToString().ToLowerInvariant();
        await Db.SortedSetRemoveAsync(key, json);
        await Db.HashDeleteAsync(PlayerIndex, playerId.ToString());
    }

    private static string Serialize(QueueEntry entry)
    {
        var dto = new QueueEntryDto
        {
            PlayerId = entry.PlayerId,
            DeckId = entry.DeckId,
            QueueType = (int)entry.QueueType,
            Elo = entry.Elo,
            JoinedAt = entry.JoinedAt
        };
        return JsonSerializer.Serialize(dto, JsonOpts);
    }

    private static QueueEntry? Deserialize(string json)
    {
        var dto = JsonSerializer.Deserialize<QueueEntryDto>(json, JsonOpts);
        if (dto == null) return null;
        return new QueueEntry(dto.PlayerId, dto.DeckId, (QueueType)dto.QueueType, dto.Elo);
    }

    private class QueueEntryDto
    {
        public Guid PlayerId { get; set; }
        public Guid DeckId { get; set; }
        public int QueueType { get; set; }
        public int Elo { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}
