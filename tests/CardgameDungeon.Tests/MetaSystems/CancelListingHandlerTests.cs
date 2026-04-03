using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Features.Marketplace.CancelListing;
using CardgameDungeon.Tests.MetaSystems.Fakes;

namespace CardgameDungeon.Tests.MetaSystems;

public class CancelListingHandlerTests
{
    private readonly FakeMarketplaceRepository _marketRepo = new();
    private readonly FakeCollectionRepository _collectionRepo = new();
    private CancelListingHandler Handler => new(_marketRepo, _collectionRepo);

    [Fact]
    public async Task ValidCancel_ReleasesCard()
    {
        var sellerId = Guid.NewGuid();
        var collection = new PlayerCollection(sellerId);
        var owned = collection.AddCard(Guid.NewGuid());
        owned.Reserve();
        _collectionRepo.Seed(collection);

        var listing = new MarketplaceListing(Guid.NewGuid(), sellerId, owned.Id, owned.CardId, 100);
        _marketRepo.Seed(listing);

        var response = await Handler.Handle(
            new CancelListingCommand(sellerId, listing.Id), CancellationToken.None);

        Assert.True(response.Cancelled);
        Assert.False(owned.IsReserved);
        Assert.False(listing.IsActive);
    }

    [Fact]
    public async Task NonSellerCancel_Throws()
    {
        var sellerId = Guid.NewGuid();
        var collection = new PlayerCollection(sellerId);
        var owned = collection.AddCard(Guid.NewGuid());
        owned.Reserve();
        _collectionRepo.Seed(collection);

        var listing = new MarketplaceListing(Guid.NewGuid(), sellerId, owned.Id, owned.CardId, 100);
        _marketRepo.Seed(listing);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Handler.Handle(new CancelListingCommand(Guid.NewGuid(), listing.Id), CancellationToken.None));
    }

    [Fact]
    public async Task AlreadyCancelledListing_Throws()
    {
        var sellerId = Guid.NewGuid();
        var collection = new PlayerCollection(sellerId);
        var owned = collection.AddCard(Guid.NewGuid());
        owned.Reserve();
        _collectionRepo.Seed(collection);

        var listing = new MarketplaceListing(Guid.NewGuid(), sellerId, owned.Id, owned.CardId, 100);
        listing.Cancel();
        _marketRepo.Seed(listing);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Handler.Handle(new CancelListingCommand(sellerId, listing.Id), CancellationToken.None));
    }

    [Fact]
    public async Task ListingNotFound_Throws()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            Handler.Handle(new CancelListingCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None));
    }
}
