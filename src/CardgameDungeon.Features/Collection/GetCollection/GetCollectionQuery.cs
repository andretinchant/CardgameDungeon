using MediatR;

namespace CardgameDungeon.Features.Collection.GetCollection;

public record GetCollectionQuery(Guid PlayerId) : IRequest<CollectionResponse>;

public record CollectionResponse(
    Guid PlayerId,
    IReadOnlyList<OwnedCardDto> Cards,
    int TotalCards,
    int AvailableCards);

public record OwnedCardDto(
    Guid OwnedCardId,
    Guid CardId,
    bool IsReserved,
    string CardName,
    string CardType,
    string Rarity,
    int Cost,
    string DetailText);
