using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Domain.Repositories;
using CardgameDungeon.Features.Match.Shared;
using MediatR;

namespace CardgameDungeon.Features.Match.Combat.AdvanceRoom;

public class AdvanceRoomHandler(IMatchRepository matchRepo)
    : IRequestHandler<AdvanceRoomCommand, MatchResponse>
{
    public async Task<MatchResponse> Handle(AdvanceRoomCommand request, CancellationToken ct)
    {
        var match = await matchRepo.GetByIdAsync(request.MatchId, ct)
            ?? throw new KeyNotFoundException($"Match {request.MatchId} not found.");

        if (match.Phase != MatchPhase.RoomResolution)
            throw new InvalidOperationException(
                $"Cannot advance room during {match.Phase} phase. Expected: RoomResolution.");

        // Defender shuffles discard back into deck (treasure from defeated monsters)
        var defender = match.GetDefender();
        defender.ShuffleDiscardIntoDeck();

        // Advance room: both refill hand to 8, move to next room
        match.AdvanceRoom();

        await matchRepo.UpdateAsync(match, ct);

        return MatchMapper.ToResponse(match);
    }
}
