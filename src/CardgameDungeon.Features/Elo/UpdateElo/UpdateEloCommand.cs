using MediatR;

namespace CardgameDungeon.Features.Elo.UpdateElo;

public record UpdateEloCommand(
    Guid WinnerId,
    Guid LoserId) : IRequest<UpdateEloResponse>;

public record UpdateEloResponse(
    Guid WinnerId, int WinnerNewElo, int WinnerDelta,
    Guid LoserId, int LoserNewElo, int LoserDelta);
