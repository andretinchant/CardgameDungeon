using FluentValidation;

namespace CardgameDungeon.Features.Marketplace.CancelListing;

public class CancelListingValidator : AbstractValidator<CancelListingCommand>
{
    public CancelListingValidator()
    {
        RuleFor(x => x.SellerId).NotEmpty();
        RuleFor(x => x.ListingId).NotEmpty();
    }
}
