using FluentValidation;

namespace CardgameDungeon.Features.Match.Combat.AdvanceRoom;

public class AdvanceRoomValidator : AbstractValidator<AdvanceRoomCommand>
{
    public AdvanceRoomValidator()
    {
        RuleFor(x => x.MatchId).NotEmpty();
    }
}
