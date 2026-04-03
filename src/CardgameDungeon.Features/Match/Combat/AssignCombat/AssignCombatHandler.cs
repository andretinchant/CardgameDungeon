using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Repositories;
using CardgameDungeon.Features.Match.Shared;
using MediatR;

namespace CardgameDungeon.Features.Match.Combat.AssignCombat;

public class AssignCombatHandler(IMatchRepository matchRepo, IMatchNotifier notifier)
    : IRequestHandler<AssignCombatCommand, AssignCombatResponse>
{
    public async Task<AssignCombatResponse> Handle(AssignCombatCommand request, CancellationToken ct)
    {
        var match = await matchRepo.GetByIdAsync(request.MatchId, ct)
            ?? throw new KeyNotFoundException($"Match {request.MatchId} not found.");

        if (match.InitiativeWinnerId != request.PlayerId)
            throw new InvalidOperationException("Only the initiative winner can assign combat.");

        var attacker = match.GetAttacker();
        var defender = match.GetDefender();

        foreach (var pairing in request.Pairings)
        {
            var attackerAlly = attacker.AlliesInPlay.FirstOrDefault(a => a.Id == pairing.AttackerAllyId)
                ?? throw new InvalidOperationException($"Attacker ally {pairing.AttackerAllyId} is not in play.");

            var defenderAlly = defender.AlliesInPlay.FirstOrDefault(a => a.Id == pairing.DefenderAllyId)
                ?? throw new InvalidOperationException($"Defender ally {pairing.DefenderAllyId} is not in play.");

            match.CombatBoard.Assign(attackerAlly, defenderAlly, defender.AlliesInPlay);
        }

        await matchRepo.UpdateAsync(match, ct);

        var response = new AssignCombatResponse(
            match.Id,
            match.CombatBoard.Assignments.Count,
            request.Pairings);

        await notifier.CombatAssigned(match.Id, response);

        return response;
    }
}
