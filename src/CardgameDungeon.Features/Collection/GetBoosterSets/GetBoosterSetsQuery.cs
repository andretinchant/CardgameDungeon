using MediatR;

namespace CardgameDungeon.Features.Collection.GetBoosterSets;

public record GetBoosterSetsQuery() : IRequest<GetBoosterSetsResponse>;

public record GetBoosterSetsResponse(IReadOnlyList<BoosterSetDto> Sets);

public record BoosterSetDto(
    Guid SetId,
    string SetCode,
    string SetName,
    string Description,
    string ReleaseDate,
    int BoosterPrice,
    int TotalCards);
