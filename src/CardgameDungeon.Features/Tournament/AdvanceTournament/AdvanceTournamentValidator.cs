using FluentValidation;

namespace CardgameDungeon.Features.Tournament.AdvanceTournament;

public class AdvanceTournamentValidator : AbstractValidator<AdvanceTournamentCommand>
{
    public AdvanceTournamentValidator()
    {
        RuleFor(x => x.TournamentId).NotEmpty();
        RuleFor(x => x.LoserId).NotEmpty();
    }
}
