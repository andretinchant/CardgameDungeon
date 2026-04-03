using FluentValidation;

namespace CardgameDungeon.Features.Elo.GetPlayerRank;

public class GetPlayerRankValidator : AbstractValidator<GetPlayerRankQuery>
{
    public GetPlayerRankValidator()
    {
        RuleFor(x => x.PlayerId).NotEmpty();
    }
}
