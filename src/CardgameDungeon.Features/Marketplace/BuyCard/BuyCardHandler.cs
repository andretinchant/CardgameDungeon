using CardgameDungeon.Domain.Repositories;
using CardgameDungeon.Features.Marketplace.Shared;
using MediatR;

namespace CardgameDungeon.Features.Marketplace.BuyCard;

public class BuyCardHandler(
    IMarketplaceRepository marketRepo,
    IWalletRepository walletRepo,
    ICollectionRepository collectionRepo)
    : IRequestHandler<BuyCardCommand, BuyCardResponse>
{
    public async Task<BuyCardResponse> Handle(BuyCardCommand request, CancellationToken ct)
    {
        var listing = await marketRepo.GetByIdAsync(request.ListingId, ct)
            ?? throw new KeyNotFoundException($"Listing {request.ListingId} not found.");

        if (!listing.IsActive)
            throw new InvalidOperationException("Listing is no longer active.");

        if (listing.SellerId == request.BuyerId)
            throw new InvalidOperationException("Cannot buy your own listing.");

        // Deduct from buyer
        var buyerWallet = await walletRepo.GetByPlayerIdAsync(request.BuyerId, ct)
            ?? throw new KeyNotFoundException($"Wallet for buyer {request.BuyerId} not found.");
        buyerWallet.Deduct(listing.Price);

        // Pay seller (minus fee)
        var sellerWallet = await walletRepo.GetByPlayerIdAsync(listing.SellerId, ct)
            ?? throw new KeyNotFoundException($"Wallet for seller {listing.SellerId} not found.");
        var sellerProceeds = listing.CalculateSellerProceeds();
        sellerWallet.AddFunds(sellerProceeds, Domain.Enums.FundSource.EventPrize,
            DateOnly.FromDateTime(DateTime.UtcNow));

        // Transfer card: release from seller, add to buyer
        var sellerCollection = await collectionRepo.GetByPlayerIdAsync(listing.SellerId, ct)
            ?? throw new KeyNotFoundException($"Collection for seller {listing.SellerId} not found.");
        var ownedCard = sellerCollection.GetOwnedCard(listing.OwnedCardId);
        ownedCard.Release();
        ownedCard.TransferTo(request.BuyerId);
        sellerCollection.RemoveCard(ownedCard);

        var buyerCollection = await collectionRepo.GetByPlayerIdAsync(request.BuyerId, ct)
            ?? throw new KeyNotFoundException($"Collection for buyer {request.BuyerId} not found.");
        buyerCollection.AddCard(listing.CardId);

        listing.Complete();

        await walletRepo.UpdateAsync(buyerWallet, ct);
        await walletRepo.UpdateAsync(sellerWallet, ct);
        await collectionRepo.UpdateAsync(sellerCollection, ct);
        await collectionRepo.UpdateAsync(buyerCollection, ct);
        await marketRepo.UpdateAsync(listing, ct);

        var fee = listing.CalculateFee();

        return new BuyCardResponse(
            new ListingDto(listing.Id, listing.SellerId, listing.CardId,
                listing.Price, fee, listing.IsActive),
            listing.Price,
            sellerProceeds,
            fee);
    }
}
