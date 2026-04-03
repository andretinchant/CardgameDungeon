using CardgameDungeon.Features.Elo.GetPlayerRank;
using CardgameDungeon.Features.Elo.RecalculateTiers;
using CardgameDungeon.Features.Elo.UpdateElo;
using MediatR;

namespace CardgameDungeon.API.Endpoints;

public static class RankingEndpoints
{
    public static void MapRankingEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/ranking").WithTags("Ranking").RequireAuthorization();

        group.MapGet("/{playerId:guid}", async (Guid playerId, IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetPlayerRankQuery(playerId))));

        group.MapPost("/update-elo", async (UpdateEloCommand command, IMediator mediator) =>
            Results.Ok(await mediator.Send(command)));

        group.MapPost("/recalculate-tiers", async (IMediator mediator) =>
            Results.Ok(await mediator.Send(new RecalculateTiersCommand())));
    }
}
