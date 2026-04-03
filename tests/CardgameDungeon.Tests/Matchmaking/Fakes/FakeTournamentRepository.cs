using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Repositories;

namespace CardgameDungeon.Tests.Matchmaking.Fakes;

public class FakeTournamentRepository : ITournamentRepository
{
    private readonly Dictionary<Guid, Tournament> _tournaments = new();

    public Task<Tournament?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(_tournaments.GetValueOrDefault(id));

    public Task SaveAsync(Tournament tournament, CancellationToken ct = default)
    {
        _tournaments[tournament.Id] = tournament;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Tournament tournament, CancellationToken ct = default)
    {
        _tournaments[tournament.Id] = tournament;
        return Task.CompletedTask;
    }

    public void Seed(Tournament tournament) => _tournaments[tournament.Id] = tournament;
}
