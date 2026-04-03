using FluentValidation;

namespace CardgameDungeon.Features.Match.GetMatchState;

public class GetMatchStateValidator : AbstractValidator<GetMatchStateQuery>
{
    public GetMatchStateValidator()
    {
        RuleFor(x => x.MatchId).NotEmpty();
    }
}
