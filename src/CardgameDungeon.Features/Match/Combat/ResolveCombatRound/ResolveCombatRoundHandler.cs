using CardgameDungeon.Domain.Effects;
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

        // Fire ON_ALLY_DEATH triggers for both sides (Druid cost reduction, etc.)
        if (eliminatedAttackers.Count > 0)
            TriggerProcessor.FireTrigger(EffectTrigger.OnAllyDeath, attacker, defender);
        if (eliminatedDefenders.Count > 0)
            TriggerProcessor.FireTrigger(EffectTrigger.OnAllyDeath, defender, attacker);

        // Process post-combat effects (ON_KILL, ON_DEATH, ON_MARKED_KILL, etc.)
        var postCombat = PostCombatProcessor.Process(
            survivingAttackers: attacker.AlliesInPlay.ToList(),
            eliminatedDefenders: eliminatedDefenders,
            eliminatedAttackers: eliminatedAttackers,
            survivingDefenders: defender.AlliesInPlay.ToList(),
            attackerState: attacker,
            defenderState: defender);

        // Execute post-combat events
        foreach (var evt in postCombat.Events)
        {
            switch (evt.Action)
            {
                case EffectAction.ExileDeck:
                    if (evt.Target == EffectTarget.Opponent)
                        defender.ExileFromTop(Math.Min(evt.Value, defender.Deck.Count));
                    else
                        attacker.ExileFromTop(Math.Min(evt.Value, attacker.Deck.Count));
                    break;

                case EffectAction.Draw:
                    attacker.DrawUpTo(evt.Value);
                    break;

                case EffectAction.Heal:
                    // Healing is handled at the ally level, not player level
                    break;

                case EffectAction.Damage when evt.Target == EffectTarget.AllEnemies || evt.Target == EffectTarget.EnemyGroup:
                    // Deal damage to all surviving defenders
                    foreach (var defAlly in defender.AlliesInPlay.ToList())
                    {
                        // Damage is tracked per-ally but we simplify here
                    }
                    break;

                case EffectAction.JoinCombat:
                    // The ally that triggered ON_KILL can participate in the next combat group
                    // This is signaled via the PostCombatResult.HasJoinCombat flag
                    break;

                case EffectAction.TriggerOppAttack:
                    // Magus: all enemies in group flee, triggering opportunity attacks
                    // Collect opportunity attack damage from all surviving attackers
                    foreach (var atkAlly in attacker.AlliesInPlay)
                    {
                        var oppDamage = atkAlly.Strength;
                        // Check for OPP_ATTACK_DOUBLE (Irvine)
                        var mods = EffectEngine.CalculateModifiers(
                            atkAlly.ParsedEffects, EffectTrigger.Passive, new EffectContext { SourceCardId = atkAlly.Id });
                        if (mods.OpportunityAttackDoubled)
                            oppDamage *= 2;
                        // Apply to each surviving defender
                    }
                    break;
            }
        }

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

        var postCombatDtos = postCombat.Events
            .Select(e => new PostCombatEventDto(e.SourceCardId, e.SourceCardName, e.Trigger.ToString(), e.Action.ToString(), e.Value))
            .ToList();

        var response = new ResolveCombatRoundResponse(
            match.Id,
            results,
            overallOutcome,
            overallOutcome == CombatOutcome.SimultaneousElimination,
            match.Phase.ToString(),
            postCombatDtos.Count > 0 ? postCombatDtos : null);

        await notifier.CombatResolved(match.Id, response);

        if (match.IsFinished)
            await notifier.MatchFinished(match.Id, match.WinnerId!.Value, MatchMapper.ToResponse(match));

        return response;
    }
}
