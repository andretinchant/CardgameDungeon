using FluentValidation;

namespace CardgameDungeon.Features.Matchmaking.LeaveQueue;

public class LeaveQueueValidator : AbstractValidator<LeaveQueueCommand>
{
    public LeaveQueueValidator()
    {
        RuleFor(x => x.PlayerId).NotEmpty();
    }
}
