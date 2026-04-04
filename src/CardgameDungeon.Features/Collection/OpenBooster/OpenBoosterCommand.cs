using CardgameDungeon.Domain.Enums;
using MediatR;

namespace CardgameDungeon.Features.Collection.OpenBooster;

public record OpenBoosterCommand(Guid PlayerId, int BoosterPrice, string SetCode) : IRequest<OpenBoosterResponse>;

public record OpenBoosterResponse(
    Guid PlayerId,
    string SetCode,
    IReadOnlyList<BoosterCardDto> Cards);

public record BoosterCardDto(Guid CardId, string Name, Rarity Rarity, CardType Type, string SetCode);
