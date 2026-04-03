using CardgameDungeon.Domain.Repositories;
using MediatR;

namespace CardgameDungeon.Features.Collection.GetCollection;

public class GetCollectionHandler(ICollectionRepository collectionRepo)
    : IRequestHandler<GetCollectionQuery, CollectionResponse>
{
    public async Task<CollectionResponse> Handle(GetCollectionQuery request, CancellationToken ct)
    {
        var collection = await collectionRepo.GetByPlayerIdAsync(request.PlayerId, ct)
            ?? throw new KeyNotFoundException($"Collection for player {request.PlayerId} not found.");

        var cards = collection.Cards.Select(c =>
            new OwnedCardDto(c.Id, c.CardId, c.IsReserved)).ToList();

        return new CollectionResponse(
            collection.PlayerId,
            cards,
            cards.Count,
            cards.Count(c => !c.IsReserved));
    }
}
