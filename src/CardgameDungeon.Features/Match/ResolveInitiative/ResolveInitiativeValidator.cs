using FluentValidation;

namespace CardgameDungeon.Features.Match.ResolveInitiative;

public class ResolveInitiativeValidator : AbstractValidator<ResolveInitiativeCommand>
{
    public ResolveInitiativeValidator()
    {
        RuleFor(x => x.MatchId).NotEmpty();
    }
}
