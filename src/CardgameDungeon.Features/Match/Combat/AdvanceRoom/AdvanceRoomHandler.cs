using CardgameDungeon.Domain.Effects;
using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Domain.Repositories;
using CardgameDungeon.Features.Match.Shared;
using MediatR;

namespace CardgameDungeon.Features.Match.Combat.AdvanceRoom;

public class AdvanceRoomHandler(IMatchRepository matchRepo, IMatchNotifier notifier)
    : IRequestHandler<AdvanceRoomCommand, MatchResponse>
{
    public async Task<MatchResponse> Handle(AdvanceRoomCommand request, CancellationToken ct)
    {
        var match = await matchRepo.GetByIdAsync(request.MatchId, ct)
            ?? throw new KeyNotFoundException($"Match {request.MatchId} not found.");

        if (match.Phase != MatchPhase.RoomResolution)
            throw new InvalidOperationException(
                $"Cannot advance room during {match.Phase} phase. Expected: RoomResolution.");

        var defender = match.GetDefender();
        defender.ShuffleDiscardIntoDeck();

        match.AdvanceRoom();

        // Fire ON_ROOM_ADVANCE triggers for both players (Druid cost reduction, etc.)
        TriggerProcessor.FireTrigger(EffectTrigger.OnRoomAdvance, match.Player1, match.Player2);
        TriggerProcessor.FireTrigger(EffectTrigger.OnRoomAdvance, match.Player2, match.Player1);

        await matchRepo.UpdateAsync(match, ct);

        var response = MatchMapper.ToResponse(match);
        await notifier.RoomAdvanced(match.Id, response);
        return response;
    }
}
