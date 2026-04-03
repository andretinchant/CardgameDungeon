using FluentValidation;

namespace CardgameDungeon.Features.Wallet.AddFunds;

public class AddFundsValidator : AbstractValidator<AddFundsCommand>
{
    public AddFundsValidator()
    {
        RuleFor(x => x.PlayerId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Source).IsInEnum();
    }
}
