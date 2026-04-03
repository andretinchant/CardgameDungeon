using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Repositories;
using CardgameDungeon.Features.Marketplace.Shared;
using MediatR;

namespace CardgameDungeon.Features.Marketplace.ListCard;

public class ListCardHandler(ICollectionRepository collectionRepo, IMarketplaceRepository marketRepo)
    : IRequestHandler<ListCardCommand, ListingDto>
{
    public async Task<ListingDto> Handle(ListCardCommand request, CancellationToken ct)
    {
        var collection = await collectionRepo.GetByPlayerIdAsync(request.SellerId, ct)
            ?? throw new KeyNotFoundException($"Collection for player {request.SellerId} not found.");

        var ownedCard = collection.GetOwnedCard(request.OwnedCardId);

        if (ownedCard.PlayerId != request.SellerId)
            throw new InvalidOperationException("Card does not belong to seller.");

        ownedCard.Reserve();

        var listing = new MarketplaceListing(
            Guid.NewGuid(),
            request.SellerId,
            ownedCard.Id,
            ownedCard.CardId,
            request.Price);

        await collectionRepo.UpdateAsync(collection, ct);
        await marketRepo.SaveAsync(listing, ct);

        return new ListingDto(
            listing.Id, listing.SellerId, listing.CardId,
            listing.Price, listing.CalculateFee(), listing.IsActive);
    }
}
