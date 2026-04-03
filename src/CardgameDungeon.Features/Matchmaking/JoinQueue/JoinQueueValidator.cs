using FluentValidation;

namespace CardgameDungeon.Features.Matchmaking.JoinQueue;

public class JoinQueueValidator : AbstractValidator<JoinQueueCommand>
{
    public JoinQueueValidator()
    {
        RuleFor(x => x.PlayerId).NotEmpty();
        RuleFor(x => x.DeckId).NotEmpty();
        RuleFor(x => x.QueueType).IsInEnum();
    }
}
