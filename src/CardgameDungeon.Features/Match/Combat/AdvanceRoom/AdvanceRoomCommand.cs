using CardgameDungeon.Features.Match.Shared;
using MediatR;

namespace CardgameDungeon.Features.Match.Combat.AdvanceRoom;

public record AdvanceRoomCommand(Guid MatchId) : IRequest<MatchResponse>;
