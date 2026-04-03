using CardgameDungeon.Domain.Services;
using CardgameDungeon.Features.Behaviors;
using CardgameDungeon.Features.Deck.CreateDeck;
using CardgameDungeon.Features.Deck.GetDeck;
using CardgameDungeon.Features.Deck.UpdateDeck;
using CardgameDungeon.Features.Deck.ValidateDeck;
using CardgameDungeon.Features.Match.CreateMatch;
using CardgameDungeon.Features.Match.GetMatchState;
using CardgameDungeon.Features.Match.PlaceBet;
using CardgameDungeon.Features.Match.ResolveInitiative;
using CardgameDungeon.Features.Match.RevealInitialTeams;
using CardgameDungeon.Features.Match.SetupInitialTeam;
using FluentValidation;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<CardgameDungeon.Features.AssemblyReference>());

builder.Services.AddValidatorsFromAssemblyContaining<CardgameDungeon.Features.AssemblyReference>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddSingleton<CombatResolver>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/health", () => Results.Ok("healthy"));

// Deck endpoints
app.MapPost("/api/decks", async (CreateDeckCommand command, IMediator mediator) =>
    Results.Created($"/api/decks/{command.PlayerId}", await mediator.Send(command)));

app.MapGet("/api/decks/{id:guid}", async (Guid id, IMediator mediator) =>
    Results.Ok(await mediator.Send(new GetDeckQuery(id))));

app.MapPut("/api/decks/{id:guid}", async (Guid id, UpdateDeckCommand command, IMediator mediator) =>
    Results.Ok(await mediator.Send(command with { DeckId = id })));

app.MapPost("/api/decks/{id:guid}/validate", async (Guid id, IMediator mediator) =>
    Results.Ok(await mediator.Send(new ValidateDeckCommand(id))));

// Match endpoints
app.MapPost("/api/matches", async (CreateMatchCommand command, IMediator mediator) =>
    Results.Created($"/api/matches", await mediator.Send(command)));

app.MapGet("/api/matches/{id:guid}", async (Guid id, IMediator mediator) =>
    Results.Ok(await mediator.Send(new GetMatchStateQuery(id))));

app.MapPost("/api/matches/{id:guid}/setup-team", async (Guid id, SetupInitialTeamCommand command, IMediator mediator) =>
    Results.Ok(await mediator.Send(command with { MatchId = id })));

app.MapPost("/api/matches/{id:guid}/reveal-teams", async (Guid id, IMediator mediator) =>
    Results.Ok(await mediator.Send(new RevealInitialTeamsCommand(id))));

app.MapPost("/api/matches/{id:guid}/resolve-initiative", async (Guid id, IMediator mediator) =>
    Results.Ok(await mediator.Send(new ResolveInitiativeCommand(id))));

app.MapPost("/api/matches/{id:guid}/place-bet", async (Guid id, PlaceBetCommand command, IMediator mediator) =>
    Results.Ok(await mediator.Send(command with { MatchId = id })));

app.Run();
