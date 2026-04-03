using CardgameDungeon.Domain.Entities;
using FluentValidation;

namespace CardgameDungeon.Features.Match.SetupInitialTeam;

public class SetupInitialTeamValidator : AbstractValidator<SetupInitialTeamCommand>
{
    public SetupInitialTeamValidator()
    {
        RuleFor(x => x.MatchId).NotEmpty();
        RuleFor(x => x.PlayerId).NotEmpty();

        RuleFor(x => x.AllyCardIds)
            .NotEmpty()
            .Must(ids => ids.Count <= PlayerState.MaxAlliesInPlay)
            .WithMessage($"Cannot select more than {PlayerState.MaxAlliesInPlay} allies.");
    }
}
