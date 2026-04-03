using FluentValidation;

namespace CardgameDungeon.Features.Marketplace.BuyCard;

public class BuyCardValidator : AbstractValidator<BuyCardCommand>
{
    public BuyCardValidator()
    {
        RuleFor(x => x.BuyerId).NotEmpty();
        RuleFor(x => x.ListingId).NotEmpty();
    }
}
