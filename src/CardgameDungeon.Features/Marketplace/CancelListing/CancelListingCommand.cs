using MediatR;

namespace CardgameDungeon.Features.Marketplace.CancelListing;

public record CancelListingCommand(Guid SellerId, Guid ListingId) : IRequest<CancelListingResponse>;

public record CancelListingResponse(Guid ListingId, bool Cancelled);
