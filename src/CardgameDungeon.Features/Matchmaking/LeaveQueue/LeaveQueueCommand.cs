using MediatR;

namespace CardgameDungeon.Features.Matchmaking.LeaveQueue;

public record LeaveQueueCommand(Guid PlayerId) : IRequest<LeaveQueueResponse>;
public record LeaveQueueResponse(Guid PlayerId, bool Left);
