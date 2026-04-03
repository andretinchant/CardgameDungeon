using CardgameDungeon.Features.Match.Combat.AdvanceRoom;
using CardgameDungeon.Features.Match.Combat.AssignCombat;
using CardgameDungeon.Features.Match.Combat.ConcedeRoom;
using CardgameDungeon.Features.Match.Combat.ResolveCombatRound;
using CardgameDungeon.Features.Match.Combat.ResolveOpportunityAttack;
using CardgameDungeon.Features.Match.Combat.Retarget;
using CardgameDungeon.Features.Match.CreateMatch;
using CardgameDungeon.Features.Match.GetMatchState;
using CardgameDungeon.Features.Match.PlaceBet;
using CardgameDungeon.Features.Match.ResolveInitiative;
using CardgameDungeon.Features.Match.RevealInitialTeams;
using CardgameDungeon.Features.Match.SetupInitialTeam;
using MediatR;

namespace CardgameDungeon.API.Endpoints;

public static class MatchEndpoints
{
    public static void MapMatchEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/matches").WithTags("Match");

        group.MapPost("/", async (CreateMatchCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Created($"/api/matches/{result.Id}", result);
        });

        group.MapGet("/{id:guid}", async (Guid id, IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetMatchStateQuery(id))));

        group.MapPost("/{id:guid}/setup-team", async (Guid id, SetupInitialTeamCommand command, IMediator mediator) =>
            Results.Ok(await mediator.Send(command with { MatchId = id })));

        group.MapPost("/{id:guid}/reveal-teams", async (Guid id, IMediator mediator) =>
            Results.Ok(await mediator.Send(new RevealInitialTeamsCommand(id))));

        group.MapPost("/{id:guid}/resolve-initiative", async (Guid id, IMediator mediator) =>
            Results.Ok(await mediator.Send(new ResolveInitiativeCommand(id))));

        group.MapPost("/{id:guid}/place-bet", async (Guid id, PlaceBetCommand command, IMediator mediator) =>
            Results.Ok(await mediator.Send(command with { MatchId = id })));

        group.MapPost("/{id:guid}/assign-combat", async (Guid id, AssignCombatCommand command, IMediator mediator) =>
            Results.Ok(await mediator.Send(command with { MatchId = id })));

        group.MapPost("/{id:guid}/resolve-combat", async (Guid id, IMediator mediator) =>
            Results.Ok(await mediator.Send(new ResolveCombatRoundCommand(id))));

        group.MapPost("/{id:guid}/retarget", async (Guid id, RetargetCommand command, IMediator mediator) =>
            Results.Ok(await mediator.Send(command with { MatchId = id })));

        group.MapPost("/{id:guid}/opportunity-attack", async (Guid id, ResolveOpportunityAttackCommand command, IMediator mediator) =>
            Results.Ok(await mediator.Send(command with { MatchId = id })));

        group.MapPost("/{id:guid}/advance-room", async (Guid id, IMediator mediator) =>
            Results.Ok(await mediator.Send(new AdvanceRoomCommand(id))));

        group.MapPost("/{id:guid}/concede-room", async (Guid id, ConcedeRoomCommand command, IMediator mediator) =>
            Results.Ok(await mediator.Send(command with { MatchId = id })));
    }
}
