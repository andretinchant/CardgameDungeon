using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Repositories;
using CardgameDungeon.Domain.Services;
using MediatR;

namespace CardgameDungeon.Features.Match.Combat.ResolveOpportunityAttack;

public class ResolveOpportunityAttackHandler(IMatchRepository matchRepo, CombatResolver combatResolver)
    : IRequestHandler<ResolveOpportunityAttackCommand, ResolveOpportunityAttackResponse>
{
    public async Task<ResolveOpportunityAttackResponse> Handle(
        ResolveOpportunityAttackCommand request, CancellationToken ct)
    {
        var match = await matchRepo.GetByIdAsync(request.MatchId, ct)
            ?? throw new KeyNotFoundException($"Match {request.MatchId} not found.");

        var attackingPlayer = match.GetPlayer(request.AttackingPlayerId);
        var opponent = match.GetOpponent(request.AttackingPlayerId);

        var attacker = attackingPlayer.AlliesInPlay.FirstOrDefault(a => a.Id == request.AttackerAllyId)
            ?? throw new InvalidOperationException($"Attacker ally {request.AttackerAllyId} is not in play.");

        var fleeing = opponent.AlliesInPlay.FirstOrDefault(a => a.Id == request.FleeingAllyId)
            ?? throw new InvalidOperationException($"Fleeing ally {request.FleeingAllyId} is not in play.");

        // Use combat board to enforce 1-per-player-per-round
        match.CombatBoard.UseOpportunityAttack(request.AttackingPlayerId);

        var damage = combatResolver.ResolveOpportunityAttack(
            attacker, fleeing, opponent.AlliesInPlay,
            new HashSet<Guid>()); // Already enforced via CombatBoard

        // Remove the fleeing ally's existing assignment
        var fleeingAssignments = match.CombatBoard.GetAssignmentsForAttacker(request.FleeingAllyId);
        foreach (var assignment in fleeingAssignments.ToList())
            match.CombatBoard.RemoveAssignment(assignment.AttackerId, assignment.DefenderId);

        await matchRepo.UpdateAsync(match, ct);

        return new ResolveOpportunityAttackResponse(
            match.Id, request.AttackerAllyId, request.FleeingAllyId, damage);
    }
}
