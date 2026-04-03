using MediatR;

namespace CardgameDungeon.Features.Match.Combat.AssignCombat;

public record AssignCombatCommand(
    Guid MatchId,
    Guid PlayerId,
    IReadOnlyList<CombatPairing> Pairings) : IRequest<AssignCombatResponse>;

public record CombatPairing(Guid AttackerAllyId, Guid DefenderAllyId);

public record AssignCombatResponse(
    Guid MatchId,
    int TotalAssignments,
    IReadOnlyList<CombatPairing> Pairings);
