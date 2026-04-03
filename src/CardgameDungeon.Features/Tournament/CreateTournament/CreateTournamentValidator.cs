using FluentValidation;

namespace CardgameDungeon.Features.Tournament.CreateTournament;

public class CreateTournamentValidator : AbstractValidator<CreateTournamentCommand>
{
    public CreateTournamentValidator()
    {
        RuleFor(x => x.RequiredTier).IsInEnum();
        RuleFor(x => x.EntryFee).GreaterThan(0);
    }
}
