using CardgameDungeon.Features.Collection.GetCollection;
using CardgameDungeon.Features.Collection.OpenBooster;
using MediatR;

namespace CardgameDungeon.API.Endpoints;

public static class CollectionEndpoints
{
    public static void MapCollectionEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/collection").WithTags("Collection").RequireAuthorization();

        group.MapGet("/{playerId:guid}", async (Guid playerId, IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetCollectionQuery(playerId))));

        group.MapPost("/open-booster", async (OpenBoosterCommand command, IMediator mediator) =>
            Results.Ok(await mediator.Send(command)));
    }
}
