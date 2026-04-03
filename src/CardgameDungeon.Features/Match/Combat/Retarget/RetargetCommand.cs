using MediatR;

namespace CardgameDungeon.Features.Match.Combat.Retarget;

public record RetargetCommand(
    Guid MatchId,
    Guid PlayerId,
    Guid AllyId,
    Guid NewDefenderId,
    int Cost,
    bool ExileCost) : IRequest<RetargetResponse>;

public record RetargetResponse(
    Guid MatchId,
    Guid AllyId,
    int PrimaryDamageContribution,
    int SecondaryDamageContribution,
    int CostPaid);
