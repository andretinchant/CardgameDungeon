using FluentValidation;

namespace CardgameDungeon.Features.Match.CreateMatch;

public class CreateMatchValidator : AbstractValidator<CreateMatchCommand>
{
    public CreateMatchValidator()
    {
        RuleFor(x => x.Player1Id).NotEmpty();
        RuleFor(x => x.Player2Id).NotEmpty();
        RuleFor(x => x.Player1DeckId).NotEmpty();
        RuleFor(x => x.Player2DeckId).NotEmpty();
        RuleFor(x => x.StartingHitPoints).GreaterThan(0);

        RuleFor(x => x)
            .Must(x => x.Player1Id != x.Player2Id)
            .WithMessage("Players must be different.");
    }
}
