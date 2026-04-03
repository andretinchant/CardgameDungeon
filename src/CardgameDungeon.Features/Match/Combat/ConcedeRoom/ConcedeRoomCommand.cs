using CardgameDungeon.Features.Match.Shared;
using MediatR;

namespace CardgameDungeon.Features.Match.Combat.ConcedeRoom;

public record ConcedeRoomCommand(
    Guid MatchId,
    Guid DefenderPlayerId) : IRequest<MatchResponse>;
