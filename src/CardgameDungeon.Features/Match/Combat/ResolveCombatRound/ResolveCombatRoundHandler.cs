using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Domain.Repositories;
using CardgameDungeon.Domain.Services;
using CardgameDungeon.Features.Match.Shared;
using MediatR;

namespace CardgameDungeon.Features.Match.Combat.ResolveCombatRound;

public class ResolveCombatRoundHandler(IMatchRepository matchRepo, CombatResolver combatResolver, IMatchNotifier notifier)
    : IRequestHandler<ResolveCombatRoundCommand, ResolveCombatRoundResponse>
{
    public async Task<ResolveCombatRoundResponse> Handle(ResolveCombatRoundCommand request, CancellationToken ct)
    {
        var match = await matchRepo.GetByIdAsync(request.MatchId, ct)
            ?? throw new KeyNotFoundException($"Match {request.MatchId} not found.");

        var attacker = match.GetAttacker();
        var defender = match.GetDefender();
        var assignments = match.CombatBoard.Assignments;

        if (assignments.Count == 0)
            throw new InvalidOperationException("No combat assignments to resolve.");

        // Group assignments by defender to resolve each confrontation
        var results = new List<CombatResultDto>();
        var eliminatedAttackers = new List<AllyCard>();
        var eliminatedDefenders = new List<AllyCard>();

        var groupedByDefender = assignments.GroupBy(a => a.DefenderId);

        foreach (var group in groupedByDefender)
        {
            var defenderAlly = defender.AlliesInPlay.FirstOrDefault(a => a.Id == group.Key);
            if (defenderAlly is null) continue;

            var attackerAllies = group
                .Select(a => attacker.AlliesInPlay.FirstOrDefault(al => al.Id == a.AttackerId))
                .Where(a => a is not null)
                .Cast<AllyCard>()
                .ToList();

            if (attackerAllies.Count == 0) continue;

            var battleResult = combatResolver.ResolveCombat(
                attackerAllies,
                new[] { defenderAlly },
                match.IsBossRoom);

            var atkEliminated = battleResult.Outcome is CombatOutcome.AttackerEliminated
                or CombatOutcome.SimultaneousElimination;
            var defEliminated = battleResult.Outcome is CombatOutcome.DefenderEliminated
                or CombatOutcome.SimultaneousElimination;

            if (atkEliminated)
                eliminatedAttackers.AddRange(attackerAllies);
            if (defEliminated)
                eliminatedDefenders.Add(defenderAlly);

            foreach (var atkAlly in attackerAllies)
            {
                results.Add(new CombatResultDto(
                    atkAlly.Id,
                    defenderAlly.Id,
                    battleResult.AttackerStrength,
                    battleResult.DefenderStrength,
                    battleResult.DamageToAttacker,
                    battleResult.DamageToDefender,
                    battleResult.Outcome,
                    atkEliminated,
                    defEliminated));
            }
        }

        // Apply eliminations
        foreach (var ally in eliminatedAttackers.Distinct())
            if (attacker.AlliesInPlay.Contains(ally))
                attacker.EliminateAlly(ally);

        foreach (var ally in eliminatedDefenders.Distinct())
            if (defender.AlliesInPlay.Contains(ally))
                defender.EliminateAlly(ally);

        // Determine overall outcome
        var bothEliminated = attacker.AlliesInPlay.Count == 0 && defender.AlliesInPlay.Count == 0;
        var overallOutcome = (attacker.AlliesInPlay.Count == 0, defender.AlliesInPlay.Count == 0) switch
        {
            (true, true) => CombatOutcome.SimultaneousElimination,
            (true, false) => CombatOutcome.AttackerEliminated,
            (false, true) => CombatOutcome.DefenderEliminated,
            _ => results.Any(r => r.Outcome == CombatOutcome.BothTakeDamage)
                ? CombatOutcome.BothTakeDamage
                : CombatOutcome.AttackerWins
        };

        // Apply player HP damage from combat
        var totalDmgToAttacker = results.Sum(r => r.DamageToAttacker) / Math.Max(1, results.Count(r => r.AttackerId == results[0].AttackerId));
        var totalDmgToDefender = results.Sum(r => r.DamageToDefender) / Math.Max(1, results.Select(r => r.DefenderId).Distinct().Count());

        match.ResolveCombat(
            totalDmgToAttacker,
            totalDmgToDefender,
            overallOutcome == CombatOutcome.SimultaneousElimination);

        await matchRepo.UpdateAsync(match, ct);

        var response = new ResolveCombatRoundResponse(
            match.Id,
            results,
            overallOutcome,
            overallOutcome == CombatOutcome.SimultaneousElimination,
            match.Phase.ToString());

        await notifier.CombatResolved(match.Id, response);

        if (match.IsFinished)
            await notifier.MatchFinished(match.Id, match.WinnerId!.Value, MatchMapper.ToResponse(match));

        return response;
    }
}
