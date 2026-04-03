using MediatR;

namespace CardgameDungeon.Features.Match.Combat.ResolveOpportunityAttack;

public record ResolveOpportunityAttackCommand(
    Guid MatchId,
    Guid AttackingPlayerId,
    Guid AttackerAllyId,
    Guid FleeingAllyId) : IRequest<ResolveOpportunityAttackResponse>;

public record ResolveOpportunityAttackResponse(
    Guid MatchId,
    Guid AttackerAllyId,
    Guid FleeingAllyId,
    int Damage);
