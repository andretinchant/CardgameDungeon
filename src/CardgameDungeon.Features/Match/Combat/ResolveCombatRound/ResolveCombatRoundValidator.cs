using FluentValidation;

namespace CardgameDungeon.Features.Match.Combat.ResolveCombatRound;

public class ResolveCombatRoundValidator : AbstractValidator<ResolveCombatRoundCommand>
{
    public ResolveCombatRoundValidator()
    {
        RuleFor(x => x.MatchId).NotEmpty();
    }
}
