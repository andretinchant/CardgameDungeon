using FluentValidation;

namespace CardgameDungeon.Features.Match.PlaceBet;

public class PlaceBetValidator : AbstractValidator<PlaceBetCommand>
{
    public PlaceBetValidator()
    {
        RuleFor(x => x.MatchId).NotEmpty();
        RuleFor(x => x.PlayerId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Bet amount must be positive.");
    }
}
