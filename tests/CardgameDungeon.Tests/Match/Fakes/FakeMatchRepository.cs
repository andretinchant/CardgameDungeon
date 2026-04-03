using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Repositories;

namespace CardgameDungeon.Tests.Match.Fakes;

public class FakeMatchRepository : IMatchRepository
{
    private readonly Dictionary<Guid, MatchState> _matches = new();

    public MatchState? LastSaved { get; private set; }
    public MatchState? LastUpdated { get; private set; }

    public Task<MatchState?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(_matches.GetValueOrDefault(id));

    public Task SaveAsync(MatchState match, CancellationToken ct = default)
    {
        _matches[match.Id] = match;
        LastSaved = match;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(MatchState match, CancellationToken ct = default)
    {
        _matches[match.Id] = match;
        LastUpdated = match;
        return Task.CompletedTask;
    }

    public void Seed(MatchState match) => _matches[match.Id] = match;
}
