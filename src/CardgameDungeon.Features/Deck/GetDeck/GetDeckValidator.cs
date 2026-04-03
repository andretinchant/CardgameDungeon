using FluentValidation;

namespace CardgameDungeon.Features.Deck.GetDeck;

public class GetDeckValidator : AbstractValidator<GetDeckQuery>
{
    public GetDeckValidator()
    {
        RuleFor(x => x.DeckId).NotEmpty();
    }
}
