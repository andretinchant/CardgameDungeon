namespace CardgameDungeon.Features.Marketplace.Shared;

public record ListingDto(
    Guid ListingId,
    Guid SellerId,
    Guid CardId,
    int Price,
    int Fee,
    bool IsActive);
