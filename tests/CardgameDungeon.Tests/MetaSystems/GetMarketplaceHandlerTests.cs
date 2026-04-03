using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Features.Marketplace.GetMarketplace;
using CardgameDungeon.Tests.MetaSystems.Fakes;

namespace CardgameDungeon.Tests.MetaSystems;

public class GetMarketplaceHandlerTests
{
    private readonly FakeMarketplaceRepository _marketRepo = new();
    private GetMarketplaceHandler Handler => new(_marketRepo);

    [Fact]
    public async Task ActiveListings_Returned()
    {
        _marketRepo.Seed(new MarketplaceListing(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 100));
        _marketRepo.Seed(new MarketplaceListing(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 200));

        var response = await Handler.Handle(new GetMarketplaceQuery(), CancellationToken.None);

        Assert.Equal(2, response.TotalCount);
        Assert.Equal(2, response.Listings.Count);
    }

    [Fact]
    public async Task InactiveListings_Excluded()
    {
        var active = new MarketplaceListing(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 100);
        var cancelled = new MarketplaceListing(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 200);
        cancelled.Cancel();

        _marketRepo.Seed(active);
        _marketRepo.Seed(cancelled);

        var response = await Handler.Handle(new GetMarketplaceQuery(), CancellationToken.None);

        Assert.Equal(1, response.TotalCount);
    }

    [Fact]
    public async Task EmptyMarketplace_ReturnsEmpty()
    {
        var response = await Handler.Handle(new GetMarketplaceQuery(), CancellationToken.None);

        Assert.Empty(response.Listings);
        Assert.Equal(0, response.TotalCount);
    }
}
