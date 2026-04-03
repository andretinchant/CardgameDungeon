using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Features.Marketplace.Shared;
using MediatR;

namespace CardgameDungeon.Features.Marketplace.GetMarketplace;

public record GetMarketplaceQuery(
    CardType? CardTypeFilter = null,
    Rarity? RarityFilter = null) : IRequest<GetMarketplaceResponse>;

public record GetMarketplaceResponse(IReadOnlyList<ListingDto> Listings, int TotalCount);
