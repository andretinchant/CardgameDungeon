using CardgameDungeon.Features.Match.Shared;
using MediatR;

namespace CardgameDungeon.Features.Match.CreateMatch;

public record CreateMatchCommand(
    Guid Player1Id,
    Guid Player2Id,
    Guid Player1DeckId,
    Guid Player2DeckId,
    int StartingHitPoints = 20) : IRequest<MatchResponse>;
