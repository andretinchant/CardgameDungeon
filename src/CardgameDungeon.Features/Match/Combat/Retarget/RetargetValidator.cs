using FluentValidation;

namespace CardgameDungeon.Features.Match.Combat.Retarget;

public class RetargetValidator : AbstractValidator<RetargetCommand>
{
    public RetargetValidator()
    {
        RuleFor(x => x.MatchId).NotEmpty();
        RuleFor(x => x.PlayerId).NotEmpty();
        RuleFor(x => x.AllyId).NotEmpty();
        RuleFor(x => x.NewDefenderId).NotEmpty();
        RuleFor(x => x.Cost).GreaterThan(0).WithMessage("Retarget cost must be positive.");
    }
}
