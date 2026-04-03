using FluentValidation;

namespace CardgameDungeon.Features.Deck.ValidateDeck;

public class ValidateDeckValidator : AbstractValidator<ValidateDeckCommand>
{
    public ValidateDeckValidator()
    {
        RuleFor(x => x.DeckId).NotEmpty();
    }
}
