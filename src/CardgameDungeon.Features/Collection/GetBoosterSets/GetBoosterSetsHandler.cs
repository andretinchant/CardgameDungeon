using CardgameDungeon.Domain.Repositories;
using MediatR;

namespace CardgameDungeon.Features.Collection.GetBoosterSets;

public class GetBoosterSetsHandler(ICardSetRepository cardSetRepo)
    : IRequestHandler<GetBoosterSetsQuery, GetBoosterSetsResponse>
{
    private const int DefaultBoosterPrice = 50;

    public async Task<GetBoosterSetsResponse> Handle(GetBoosterSetsQuery request, CancellationToken ct)
    {
        var sets = await cardSetRepo.GetAllAsync(ct);

        var dtos = sets
            .Select(set => new BoosterSetDto(
                set.Id,
                set.Code,
                set.Name,
                set.Description,
                set.ReleaseDate.ToString("yyyy-MM-dd"),
                DefaultBoosterPrice,
                set.TotalCards))
            .ToList();

        return new GetBoosterSetsResponse(dtos);
    }
}
