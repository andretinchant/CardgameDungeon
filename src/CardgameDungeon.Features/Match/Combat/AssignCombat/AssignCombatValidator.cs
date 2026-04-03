using FluentValidation;

namespace CardgameDungeon.Features.Match.Combat.AssignCombat;

public class AssignCombatValidator : AbstractValidator<AssignCombatCommand>
{
    public AssignCombatValidator()
    {
        RuleFor(x => x.MatchId).NotEmpty();
        RuleFor(x => x.PlayerId).NotEmpty();
        RuleFor(x => x.Pairings).NotEmpty().WithMessage("At least one combat pairing is required.");
    }
}
