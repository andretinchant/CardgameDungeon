using CardgameDungeon.API.Hubs;
using CardgameDungeon.Features.Match.Shared;
using Microsoft.AspNetCore.SignalR;

namespace CardgameDungeon.API.Services;

public class MatchNotificationService(IHubContext<MatchHub> hubContext) : IMatchNotifier
{
    private IClientProxy Group(Guid matchId) => hubContext.Clients.Group(matchId.ToString());

    public Task MatchStarted(Guid matchId, MatchResponse state)
        => Group(matchId).SendAsync("MatchStarted", state);

    public Task TeamRevealed(Guid matchId, MatchResponse state)
        => Group(matchId).SendAsync("TeamRevealed", state);

    public Task InitiativeResolved(Guid matchId, object result)
        => Group(matchId).SendAsync("InitiativeResolved", result);

    public Task CombatAssigned(Guid matchId, object assignments)
        => Group(matchId).SendAsync("CombatAssigned", assignments);

    public Task CombatResolved(Guid matchId, object result)
        => Group(matchId).SendAsync("CombatResolved", result);

    public Task RoomAdvanced(Guid matchId, MatchResponse state)
        => Group(matchId).SendAsync("RoomAdvanced", state);

    public Task PlayerDisconnected(Guid matchId, Guid playerId)
        => Group(matchId).SendAsync("PlayerDisconnected", new { PlayerId = playerId });

    public Task MatchFinished(Guid matchId, Guid winnerId, MatchResponse state)
        => Group(matchId).SendAsync("MatchFinished", new { WinnerId = winnerId, State = state });

    public Task BetPlaced(Guid matchId, object result)
        => Group(matchId).SendAsync("BetPlaced", result);

    public Task RoomConceeded(Guid matchId, MatchResponse state)
        => Group(matchId).SendAsync("RoomConceeded", state);

    public Task OpportunityAttackResolved(Guid matchId, object result)
        => Group(matchId).SendAsync("OpportunityAttackResolved", result);

    public Task RetargetCompleted(Guid matchId, object result)
        => Group(matchId).SendAsync("RetargetCompleted", result);

    public Task SetupTeamSubmitted(Guid matchId, Guid playerId, bool bothReady)
        => Group(matchId).SendAsync("SetupTeamSubmitted", new { PlayerId = playerId, BothReady = bothReady });
}
