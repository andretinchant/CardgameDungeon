namespace CardgameDungeon.Features.Match.Shared;

public interface IMatchNotifier
{
    Task MatchStarted(Guid matchId, MatchResponse state);
    Task TeamRevealed(Guid matchId, MatchResponse state);
    Task InitiativeResolved(Guid matchId, object result);
    Task CombatAssigned(Guid matchId, object assignments);
    Task CombatResolved(Guid matchId, object result);
    Task RoomAdvanced(Guid matchId, MatchResponse state);
    Task PlayerDisconnected(Guid matchId, Guid playerId);
    Task MatchFinished(Guid matchId, Guid winnerId, MatchResponse state);
    Task BetPlaced(Guid matchId, object result);
    Task RoomConceeded(Guid matchId, MatchResponse state);
    Task OpportunityAttackResolved(Guid matchId, object result);
    Task RetargetCompleted(Guid matchId, object result);
    Task SetupTeamSubmitted(Guid matchId, Guid playerId, bool bothReady);
}
