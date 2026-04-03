using CardgameDungeon.Features.Tournament.AdvanceTournament;
using CardgameDungeon.Features.Tournament.CreateTournament;
using CardgameDungeon.Features.Tournament.FinalizeTournament;
using CardgameDungeon.Features.Tournament.JoinTournament;
using MediatR;

namespace CardgameDungeon.API.Endpoints;

public static class TournamentEndpoints
{
    public static void MapTournamentEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/tournaments").WithTags("Tournament");

        group.MapPost("/", async (CreateTournamentCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Created($"/api/tournaments/{result.Id}", result);
        });

        group.MapPost("/{id:guid}/join", async (Guid id, JoinTournamentCommand command, IMediator mediator) =>
            Results.Ok(await mediator.Send(command with { TournamentId = id })));

        group.MapPost("/{id:guid}/advance", async (Guid id, AdvanceTournamentCommand command, IMediator mediator) =>
            Results.Ok(await mediator.Send(command with { TournamentId = id })));

        group.MapPost("/{id:guid}/finalize", async (Guid id, IMediator mediator) =>
            Results.Ok(await mediator.Send(new FinalizeTournamentCommand(id))));
    }
}
