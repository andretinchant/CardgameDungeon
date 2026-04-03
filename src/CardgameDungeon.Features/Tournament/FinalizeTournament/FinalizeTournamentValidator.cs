using FluentValidation;

namespace CardgameDungeon.Features.Tournament.FinalizeTournament;

public class FinalizeTournamentValidator : AbstractValidator<FinalizeTournamentCommand>
{
    public FinalizeTournamentValidator()
    {
        RuleFor(x => x.TournamentId).NotEmpty();
    }
}
