using CardgameDungeon.Features.Matchmaking.JoinQueue;
using CardgameDungeon.Features.Matchmaking.LeaveQueue;
using MediatR;

namespace CardgameDungeon.API.Endpoints;

public static class QueueEndpoints
{
    public static void MapQueueEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/queue").WithTags("Queue");

        group.MapPost("/join", async (JoinQueueCommand command, IMediator mediator) =>
            Results.Ok(await mediator.Send(command)));

        group.MapPost("/leave", async (LeaveQueueCommand command, IMediator mediator) =>
            Results.Ok(await mediator.Send(command)));
    }
}
