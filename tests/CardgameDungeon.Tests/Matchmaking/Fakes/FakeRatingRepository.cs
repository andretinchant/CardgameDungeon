using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Repositories;

namespace CardgameDungeon.Tests.Matchmaking.Fakes;

public class FakeRatingRepository : IRatingRepository
{
    private readonly Dictionary<Guid, PlayerRating> _ratings = new();

    public Task<PlayerRating?> GetByPlayerIdAsync(Guid playerId, CancellationToken ct = default)
        => Task.FromResult(_ratings.GetValueOrDefault(playerId));

    public Task<IReadOnlyList<PlayerRating>> GetAllActiveAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<PlayerRating>>(_ratings.Values.ToList());

    public Task SaveAsync(PlayerRating rating, CancellationToken ct = default)
    {
        _ratings[rating.PlayerId] = rating;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(PlayerRating rating, CancellationToken ct = default)
    {
        _ratings[rating.PlayerId] = rating;
        return Task.CompletedTask;
    }

    public Task UpdateManyAsync(IEnumerable<PlayerRating> ratings, CancellationToken ct = default)
    {
        foreach (var r in ratings) _ratings[r.PlayerId] = r;
        return Task.CompletedTask;
    }

    public void Seed(PlayerRating rating) => _ratings[rating.PlayerId] = rating;
}
