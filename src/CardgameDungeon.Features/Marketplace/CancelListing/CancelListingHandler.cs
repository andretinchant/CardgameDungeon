using CardgameDungeon.Domain.Repositories;
using MediatR;

namespace CardgameDungeon.Features.Marketplace.CancelListing;

public class CancelListingHandler(IMarketplaceRepository marketRepo, ICollectionRepository collectionRepo)
    : IRequestHandler<CancelListingCommand, CancelListingResponse>
{
    public async Task<CancelListingResponse> Handle(CancelListingCommand request, CancellationToken ct)
    {
        var listing = await marketRepo.GetByIdAsync(request.ListingId, ct)
            ?? throw new KeyNotFoundException($"Listing {request.ListingId} not found.");

        if (listing.SellerId != request.SellerId)
            throw new InvalidOperationException("Only the seller can cancel a listing.");

        listing.Cancel();

        // Release reserved card
        var collection = await collectionRepo.GetByPlayerIdAsync(request.SellerId, ct)
            ?? throw new KeyNotFoundException($"Collection for seller {request.SellerId} not found.");
        var ownedCard = collection.GetOwnedCard(listing.OwnedCardId);
        ownedCard.Release();

        await marketRepo.UpdateAsync(listing, ct);
        await collectionRepo.UpdateAsync(collection, ct);

        return new CancelListingResponse(listing.Id, true);
    }
}
