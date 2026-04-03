using FluentValidation;

namespace CardgameDungeon.Features.Match.RevealInitialTeams;

public class RevealInitialTeamsValidator : AbstractValidator<RevealInitialTeamsCommand>
{
    public RevealInitialTeamsValidator()
    {
        RuleFor(x => x.MatchId).NotEmpty();
    }
}
