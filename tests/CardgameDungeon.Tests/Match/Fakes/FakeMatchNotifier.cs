using CardgameDungeon.Features.Match.Shared;

namespace CardgameDungeon.Tests.Match.Fakes;

public class FakeMatchNotifier : IMatchNotifier
{
    public Task MatchStarted(Guid matchId, MatchResponse state) => Task.CompletedTask;
    public Task TeamRevealed(Guid matchId, MatchResponse state) => Task.CompletedTask;
    public Task InitiativeResolved(Guid matchId, object result) => Task.CompletedTask;
    public Task CombatAssigned(Guid matchId, object assignments) => Task.CompletedTask;
    public Task CombatResolved(Guid matchId, object result) => Task.CompletedTask;
    public Task RoomAdvanced(Guid matchId, MatchResponse state) => Task.CompletedTask;
    public Task PlayerDisconnected(Guid matchId, Guid playerId) => Task.CompletedTask;
    public Task MatchFinished(Guid matchId, Guid winnerId, MatchResponse state) => Task.CompletedTask;
    public Task BetPlaced(Guid matchId, object result) => Task.CompletedTask;
    public Task RoomConceeded(Guid matchId, MatchResponse state) => Task.CompletedTask;
    public Task OpportunityAttackResolved(Guid matchId, object result) => Task.CompletedTask;
    public Task RetargetCompleted(Guid matchId, object result) => Task.CompletedTask;
    public Task SetupTeamSubmitted(Guid matchId, Guid playerId, bool bothReady) => Task.CompletedTask;
}
