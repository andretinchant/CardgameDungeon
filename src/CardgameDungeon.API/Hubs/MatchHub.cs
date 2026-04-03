using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CardgameDungeon.API.Hubs;

[Authorize]
public class MatchHub : Hub
{
    public async Task JoinMatch(string matchId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, matchId);
        await Clients.OthersInGroup(matchId).SendAsync("PlayerConnected", new
        {
            PlayerId = Context.UserIdentifier,
            MatchId = matchId
        });
    }

    public async Task LeaveMatch(string matchId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, matchId);
        await Clients.OthersInGroup(matchId).SendAsync("PlayerDisconnected", new
        {
            PlayerId = Context.UserIdentifier,
            MatchId = matchId
        });
    }

    public async Task SendMatchAction(string matchId, string action, string payload)
    {
        await Clients.OthersInGroup(matchId).SendAsync("MatchActionReceived", new
        {
            PlayerId = Context.UserIdentifier,
            MatchId = matchId,
            Action = action,
            Payload = payload
        });
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // Hub groups are cleaned up automatically by SignalR
        await base.OnDisconnectedAsync(exception);
    }
}
