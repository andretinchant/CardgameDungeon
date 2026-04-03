using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Repositories;
using CardgameDungeon.Domain.Services;
using CardgameDungeon.Features.Match.Shared;
using MediatR;

namespace CardgameDungeon.Features.Match.Combat.Retarget;

public class RetargetHandler(IMatchRepository matchRepo, CombatResolver combatResolver, IMatchNotifier notifier)
    : IRequestHandler<RetargetCommand, RetargetResponse>
{
    public async Task<RetargetResponse> Handle(RetargetCommand request, CancellationToken ct)
    {
        var match = await matchRepo.GetByIdAsync(request.MatchId, ct)
            ?? throw new KeyNotFoundException($"Match {request.MatchId} not found.");

        var player = match.GetPlayer(request.PlayerId);
        var defender = match.GetDefender();

        var ally = player.AlliesInPlay.FirstOrDefault(a => a.Id == request.AllyId)
            ?? throw new InvalidOperationException($"Ally {request.AllyId} is not in play.");

        var newTarget = defender.AlliesInPlay.FirstOrDefault(a => a.Id == request.NewDefenderId)
            ?? throw new InvalidOperationException($"Defender {request.NewDefenderId} is not in play.");

        // Validate ambusher rule on new target
        combatResolver.ValidateTarget(newTarget, defender.AlliesInPlay);

        // Get existing assignments for this ally (primary combat group)
        var existingAssignments = match.CombatBoard.GetAssignmentsForAttacker(ally.Id);
        var primaryGroup = existingAssignments
            .Select(a => player.AlliesInPlay.FirstOrDefault(al => al.Id == a.AttackerId))
            .Where(a => a is not null)
            .Cast<AllyCard>()
            .ToList();

        if (primaryGroup.Count == 0)
            primaryGroup = [ally];

        var secondaryGroup = new[] { ally };

        var result = combatResolver.ResolveRetarget(
            ally, primaryGroup, secondaryGroup, request.Cost, player);

        // Add the secondary assignment
        match.CombatBoard.Assign(ally, newTarget, defender.AlliesInPlay);

        await matchRepo.UpdateAsync(match, ct);

        var response = new RetargetResponse(
            match.Id,
            result.AllyId,
            result.PrimaryDamageContribution,
            result.SecondaryDamageContribution,
            result.CostPaid);

        await notifier.RetargetCompleted(match.Id, response);

        return response;
    }
}
