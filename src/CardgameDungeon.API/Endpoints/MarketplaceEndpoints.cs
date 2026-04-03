using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Features.Marketplace.BuyCard;
using CardgameDungeon.Features.Marketplace.CancelListing;
using CardgameDungeon.Features.Marketplace.GetMarketplace;
using CardgameDungeon.Features.Marketplace.ListCard;
using MediatR;

namespace CardgameDungeon.API.Endpoints;

public static class MarketplaceEndpoints
{
    public static void MapMarketplaceEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/marketplace").WithTags("Marketplace").RequireAuthorization();

        group.MapGet("/", async (CardType? cardType, Rarity? rarity, IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetMarketplaceQuery(cardType, rarity))));

        group.MapPost("/list", async (ListCardCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Created($"/api/marketplace/{result.ListingId}", result);
        });

        group.MapPost("/{id:guid}/buy", async (Guid id, BuyCardCommand command, IMediator mediator) =>
            Results.Ok(await mediator.Send(command with { ListingId = id })));

        group.MapDelete("/{id:guid}", async (Guid id, [AsParameters] CancelListingRequest request, IMediator mediator) =>
            Results.Ok(await mediator.Send(new CancelListingCommand(request.SellerId, id))));
    }
}

public record CancelListingRequest(Guid SellerId);
