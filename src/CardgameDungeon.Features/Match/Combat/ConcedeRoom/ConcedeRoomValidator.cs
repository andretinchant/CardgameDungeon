using FluentValidation;

namespace CardgameDungeon.Features.Match.Combat.ConcedeRoom;

public class ConcedeRoomValidator : AbstractValidator<ConcedeRoomCommand>
{
    public ConcedeRoomValidator()
    {
        RuleFor(x => x.MatchId).NotEmpty();
        RuleFor(x => x.DefenderPlayerId).NotEmpty();
    }
}
