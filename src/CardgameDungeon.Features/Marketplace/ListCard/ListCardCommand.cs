using CardgameDungeon.Features.Marketplace.Shared;
using MediatR;

namespace CardgameDungeon.Features.Marketplace.ListCard;

public record ListCardCommand(
    Guid SellerId,
    Guid OwnedCardId,
    int Price) : IRequest<ListingDto>;
