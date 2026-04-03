using CardgameDungeon.Domain.Repositories;
using MediatR;

namespace CardgameDungeon.Features.Matchmaking.LeaveQueue;

public class LeaveQueueHandler(IQueueRepository queueRepo)
    : IRequestHandler<LeaveQueueCommand, LeaveQueueResponse>
{
    public async Task<LeaveQueueResponse> Handle(LeaveQueueCommand request, CancellationToken ct)
    {
        var entry = await queueRepo.GetByPlayerIdAsync(request.PlayerId, ct)
            ?? throw new InvalidOperationException("Player is not in any queue.");

        await queueRepo.RemoveAsync(request.PlayerId, ct);

        return new LeaveQueueResponse(request.PlayerId, true);
    }
}
