using CardgameDungeon.Features.Marketplace.Shared;
using MediatR;

namespace CardgameDungeon.Features.Marketplace.BuyCard;

public record BuyCardCommand(Guid BuyerId, Guid ListingId) : IRequest<BuyCardResponse>;

public record BuyCardResponse(
    ListingDto Listing,
    int AmountPaid,
    int SellerReceived,
    int FeePaid);
