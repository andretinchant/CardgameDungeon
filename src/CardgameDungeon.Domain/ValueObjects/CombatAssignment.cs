using CardgameDungeon.Domain.Entities;

namespace CardgameDungeon.Domain.ValueObjects;

public record CombatAssignment(
    Guid AttackerId,
    Guid DefenderId);

public class CombatBoard
{
    private readonly List<CombatAssignment> _assignments = [];
    private readonly HashSet<Guid> _opportunityAttacksUsed = [];

    public IReadOnlyList<CombatAssignment> Assignments => _assignments.AsReadOnly();
    public IReadOnlySet<Guid> OpportunityAttacksUsed => _opportunityAttacksUsed;

    public void Assign(AllyCard attacker, AllyCard defender, IReadOnlyList<AllyCard> defenderAllies)
    {
        if (defender.IsAmbusher && defenderAllies.Any(a => !a.IsAmbusher && a.Id != defender.Id))
            throw new InvalidOperationException(
                $"Cannot target ambusher '{defender.Name}' while non-ambusher allies are available.");

        _assignments.Add(new CombatAssignment(attacker.Id, defender.Id));
    }

    public void UseOpportunityAttack(Guid playerId)
    {
        if (!_opportunityAttacksUsed.Add(playerId))
            throw new InvalidOperationException("This player has already used their opportunity attack this round.");
    }

    public bool HasUsedOpportunityAttack(Guid playerId) => _opportunityAttacksUsed.Contains(playerId);

    public IReadOnlyList<CombatAssignment> GetAssignmentsForDefender(Guid defenderId)
        => _assignments.Where(a => a.DefenderId == defenderId).ToList();

    public IReadOnlyList<CombatAssignment> GetAssignmentsForAttacker(Guid attackerId)
        => _assignments.Where(a => a.AttackerId == attackerId).ToList();

    public void RemoveAssignment(Guid attackerId, Guid defenderId)
    {
        var idx = _assignments.FindIndex(a => a.AttackerId == attackerId && a.DefenderId == defenderId);
        if (idx < 0)
            throw new InvalidOperationException("Assignment not found.");
        _assignments.RemoveAt(idx);
    }

    public void Clear()
    {
        _assignments.Clear();
        _opportunityAttacksUsed.Clear();
    }
}
