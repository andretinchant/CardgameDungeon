using FluentValidation;

namespace CardgameDungeon.Features.Match.Combat.ResolveOpportunityAttack;

public class ResolveOpportunityAttackValidator : AbstractValidator<ResolveOpportunityAttackCommand>
{
    public ResolveOpportunityAttackValidator()
    {
        RuleFor(x => x.MatchId).NotEmpty();
        RuleFor(x => x.AttackingPlayerId).NotEmpty();
        RuleFor(x => x.AttackerAllyId).NotEmpty();
        RuleFor(x => x.FleeingAllyId).NotEmpty();
    }
}
