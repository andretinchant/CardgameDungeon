using FluentValidation;

namespace CardgameDungeon.Features.Wallet.GetBalance;

public class GetBalanceValidator : AbstractValidator<GetBalanceQuery>
{
    public GetBalanceValidator()
    {
        RuleFor(x => x.PlayerId).NotEmpty();
    }
}
