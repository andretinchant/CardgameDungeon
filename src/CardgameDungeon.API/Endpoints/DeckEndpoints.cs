using CardgameDungeon.Features.Deck.CreateDeck;
using CardgameDungeon.Features.Deck.GetDeck;
using CardgameDungeon.Features.Deck.UpdateDeck;
using CardgameDungeon.Features.Deck.ValidateDeck;
using MediatR;

namespace CardgameDungeon.API.Endpoints;

public static class DeckEndpoints
{
    public static void MapDeckEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/decks").WithTags("Deck");

        group.MapPost("/", async (CreateDeckCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Created($"/api/decks/{result.Id}", result);
        });

        group.MapGet("/{id:guid}", async (Guid id, IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetDeckQuery(id))));

        group.MapPut("/{id:guid}", async (Guid id, UpdateDeckCommand command, IMediator mediator) =>
            Results.Ok(await mediator.Send(command with { DeckId = id })));

        group.MapPost("/{id:guid}/validate", async (Guid id, IMediator mediator) =>
            Results.Ok(await mediator.Send(new ValidateDeckCommand(id))));
    }
}
