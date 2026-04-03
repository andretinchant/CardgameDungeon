using CardgameDungeon.Domain.Enums;
using MediatR;

namespace CardgameDungeon.Features.Matchmaking.JoinQueue;

public record JoinQueueCommand(
    Guid PlayerId,
    Guid DeckId,
    QueueType QueueType) : IRequest<JoinQueueResponse>;

public record JoinQueueResponse(Guid PlayerId, QueueType QueueType, bool Joined);
