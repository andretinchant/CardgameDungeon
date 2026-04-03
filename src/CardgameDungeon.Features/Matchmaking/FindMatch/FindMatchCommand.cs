using CardgameDungeon.Domain.Enums;
using MediatR;

namespace CardgameDungeon.Features.Matchmaking.FindMatch;

public record FindMatchCommand(QueueType QueueType) : IRequest<FindMatchResponse>;

public record FindMatchResponse(
    bool MatchFound,
    Guid? MatchId,
    Guid? Player1Id,
    Guid? Player2Id);
