using FluentValidation;

namespace CardgameDungeon.Features.Collection.OpenBooster;

public class OpenBoosterValidator : AbstractValidator<OpenBoosterCommand>
{
    public OpenBoosterValidator()
    {
        RuleFor(x => x.PlayerId).NotEmpty();
        RuleFor(x => x.BoosterPrice).GreaterThan(0);
        RuleFor(x => x.SetCode).NotEmpty().MaximumLength(20);
    }
}
