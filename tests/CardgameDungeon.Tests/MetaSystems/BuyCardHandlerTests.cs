using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Features.Marketplace.BuyCard;
using CardgameDungeon.Tests.MetaSystems.Fakes;

namespace CardgameDungeon.Tests.MetaSystems;

public class BuyCardHandlerTests
{
    private readonly FakeMarketplaceRepository _marketRepo = new();
    private readonly FakeWalletRepository _walletRepo = new();
    private readonly FakeCollectionRepository _collectionRepo = new();
    private BuyCardHandler Handler => new(_marketRepo, _walletRepo, _collectionRepo);

    private (MarketplaceListing listing, Guid sellerId, Guid buyerId) SetupListing(
        int price = 100, int sellerBalance = 0, int buyerBalance = 500)
    {
        var sellerId = Guid.NewGuid();
        var buyerId = Guid.NewGuid();
        var cardId = Guid.NewGuid();

        var sellerCollection = new PlayerCollection(sellerId);
        var ownedCard = sellerCollection.AddCard(cardId);
        ownedCard.Reserve();

        var listing = new MarketplaceListing(
            Guid.NewGuid(), sellerId, ownedCard.Id, cardId, price);

        _walletRepo.Seed(new PlayerWallet(sellerId, sellerBalance));
        _walletRepo.Seed(new PlayerWallet(buyerId, buyerBalance));
        _collectionRepo.Seed(sellerCollection);
        _collectionRepo.Seed(new PlayerCollection(buyerId));
        _marketRepo.Seed(listing);

        return (listing, sellerId, buyerId);
    }

    [Fact]
    public async Task ValidPurchase_TransfersCard()
    {
        var (listing, sellerId, buyerId) = SetupListing(price: 100);

        var response = await Handler.Handle(
            new BuyCardCommand(buyerId, listing.Id), CancellationToken.None);

        Assert.Equal(100, response.AmountPaid);
        Assert.Equal(90, response.SellerReceived);  // 100 - 10% fee
        Assert.Equal(10, response.FeePaid);
        Assert.False(response.Listing.IsActive);
    }

    [Fact]
    public async Task BuyerBalanceDeducted()
    {
        var (listing, _, buyerId) = SetupListing(price: 100, buyerBalance: 500);

        await Handler.Handle(new BuyCardCommand(buyerId, listing.Id), CancellationToken.None);

        var wallet = await _walletRepo.GetByPlayerIdAsync(buyerId);
        Assert.Equal(400, wallet!.Balance);
    }

    [Fact]
    public async Task SellerReceivesProceeds()
    {
        var (listing, sellerId, buyerId) = SetupListing(price: 200, sellerBalance: 50);

        await Handler.Handle(new BuyCardCommand(buyerId, listing.Id), CancellationToken.None);

        var wallet = await _walletRepo.GetByPlayerIdAsync(sellerId);
        Assert.Equal(230, wallet!.Balance); // 50 + (200 - 20 fee)
    }

    [Fact]
    public async Task BuyerCardAddedToCollection()
    {
        var (listing, _, buyerId) = SetupListing();

        await Handler.Handle(new BuyCardCommand(buyerId, listing.Id), CancellationToken.None);

        var collection = await _collectionRepo.GetByPlayerIdAsync(buyerId);
        Assert.Single(collection!.Cards);
    }

    [Fact]
    public async Task SellerCardRemovedFromCollection()
    {
        var (listing, sellerId, buyerId) = SetupListing();

        await Handler.Handle(new BuyCardCommand(buyerId, listing.Id), CancellationToken.None);

        var collection = await _collectionRepo.GetByPlayerIdAsync(sellerId);
        Assert.Empty(collection!.Cards);
    }

    [Fact]
    public async Task InsufficientBuyerBalance_Throws()
    {
        var (listing, _, buyerId) = SetupListing(price: 100, buyerBalance: 50);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Handler.Handle(new BuyCardCommand(buyerId, listing.Id), CancellationToken.None));
    }

    [Fact]
    public async Task BuyOwnListing_Throws()
    {
        var (listing, sellerId, _) = SetupListing();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Handler.Handle(new BuyCardCommand(sellerId, listing.Id), CancellationToken.None));
    }

    [Fact]
    public async Task InactiveListing_Throws()
    {
        var (listing, _, buyerId) = SetupListing();
        listing.Cancel();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Handler.Handle(new BuyCardCommand(buyerId, listing.Id), CancellationToken.None));
    }

    [Fact]
    public async Task ListingNotFound_Throws()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            Handler.Handle(new BuyCardCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None));
    }
}
