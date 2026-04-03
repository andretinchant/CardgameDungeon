using CardgameDungeon.Domain.Repositories;
using CardgameDungeon.Features.Match.Shared;
using MediatR;

namespace CardgameDungeon.Features.Match.Combat.ConcedeRoom;

public class ConcedeRoomHandler(IMatchRepository matchRepo, IMatchNotifier notifier)
    : IRequestHandler<ConcedeRoomCommand, MatchResponse>
{
    public async Task<MatchResponse> Handle(ConcedeRoomCommand request, CancellationToken ct)
    {
        var match = await matchRepo.GetByIdAsync(request.MatchId, ct)
            ?? throw new KeyNotFoundException($"Match {request.MatchId} not found.");

        // Only the defender can concede
        var defender = match.GetDefender();
        if (defender.PlayerId != request.DefenderPlayerId)
            throw new InvalidOperationException("Only the defender can concede the room.");

        match.ConcedeRoom();

        await matchRepo.UpdateAsync(match, ct);

        var response = MatchMapper.ToResponse(match);
        await notifier.RoomConceeded(match.Id, response);
        return response;
    }
}
