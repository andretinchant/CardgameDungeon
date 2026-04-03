using FluentValidation;

namespace CardgameDungeon.Features.Marketplace.ListCard;

public class ListCardValidator : AbstractValidator<ListCardCommand>
{
    public ListCardValidator()
    {
        RuleFor(x => x.SellerId).NotEmpty();
        RuleFor(x => x.OwnedCardId).NotEmpty();
        RuleFor(x => x.Price).GreaterThan(0);
    }
}
