using System.Text.Json;
using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Infrastructure.Data.Persistence;
using StackExchange.Redis;

namespace CardgameDungeon.Infrastructure.Redis;

/// <summary>
/// Caches active MatchState as JSON in Redis for fast read/write during live matches.
/// Falls back to the database repository for cold reads.
/// </summary>
public class RedisSessionCache(IConnectionMultiplexer redis)
{
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    private const string KeyPrefix = "match:session:";
    private static readonly TimeSpan Expiry = TimeSpan.FromHours(2);

    private IDatabase Db => redis.GetDatabase();

    public async Task<MatchState?> GetAsync(Guid matchId)
    {
        var json = await Db.StringGetAsync(KeyPrefix + matchId);
        if (json.IsNullOrEmpty) return null;

        var dto = JsonSerializer.Deserialize<MatchStateDto>((string)json!, JsonOpts);
        return dto != null ? MatchStateMapper.FromDto(dto) : null;
    }

    public async Task SetAsync(Guid matchId, MatchState match)
    {
        var dto = MatchStateMapper.ToDto(match);
        var json = JsonSerializer.Serialize(dto, JsonOpts);
        await Db.StringSetAsync(KeyPrefix + matchId, json, Expiry);
    }

    public async Task RemoveAsync(Guid matchId)
    {
        await Db.KeyDeleteAsync(KeyPrefix + matchId);
    }

    public async Task<bool> ExistsAsync(Guid matchId)
    {
        return await Db.KeyExistsAsync(KeyPrefix + matchId);
    }
}
