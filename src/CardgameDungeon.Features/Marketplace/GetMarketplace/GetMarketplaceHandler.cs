using CardgameDungeon.Domain.Repositories;
using CardgameDungeon.Features.Marketplace.Shared;
using MediatR;

namespace CardgameDungeon.Features.Marketplace.GetMarketplace;

public class GetMarketplaceHandler(IMarketplaceRepository marketRepo)
    : IRequestHandler<GetMarketplaceQuery, GetMarketplaceResponse>
{
    public async Task<GetMarketplaceResponse> Handle(GetMarketplaceQuery request, CancellationToken ct)
    {
        var listings = await marketRepo.GetActiveListingsAsync(
            request.CardTypeFilter, request.RarityFilter, ct);

        var dtos = listings.Select(l =>
            new ListingDto(l.Id, l.SellerId, l.CardId, l.Price, l.CalculateFee(), l.IsActive))
            .ToList();

        return new GetMarketplaceResponse(dtos, dtos.Count);
    }
}
