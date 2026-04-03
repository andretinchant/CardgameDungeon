using FluentValidation;

namespace CardgameDungeon.Features.Elo.UpdateElo;

public class UpdateEloValidator : AbstractValidator<UpdateEloCommand>
{
    public UpdateEloValidator()
    {
        RuleFor(x => x.WinnerId).NotEmpty();
        RuleFor(x => x.LoserId).NotEmpty();
        RuleFor(x => x).Must(x => x.WinnerId != x.LoserId).WithMessage("Winner and loser must be different.");
    }
}
