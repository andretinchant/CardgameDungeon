using FluentValidation;

namespace CardgameDungeon.Features.Tournament.JoinTournament;

public class JoinTournamentValidator : AbstractValidator<JoinTournamentCommand>
{
    public JoinTournamentValidator()
    {
        RuleFor(x => x.TournamentId).NotEmpty();
        RuleFor(x => x.PlayerId).NotEmpty();
        RuleFor(x => x.DeckId).NotEmpty();
    }
}
