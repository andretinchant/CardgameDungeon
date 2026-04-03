using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Repositories;
using MediatR;

namespace CardgameDungeon.Features.Elo.UpdateElo;

public class UpdateEloHandler(IRatingRepository ratingRepo)
    : IRequestHandler<UpdateEloCommand, UpdateEloResponse>
{
    public async Task<UpdateEloResponse> Handle(UpdateEloCommand request, CancellationToken ct)
    {
        var winner = await ratingRepo.GetByPlayerIdAsync(request.WinnerId, ct)
            ?? throw new KeyNotFoundException($"Rating for player {request.WinnerId} not found.");
        var loser = await ratingRepo.GetByPlayerIdAsync(request.LoserId, ct)
            ?? throw new KeyNotFoundException($"Rating for player {request.LoserId} not found.");

        var loserEloBefore = loser.Elo;
        var winnerDelta = winner.ApplyResult(true, loserEloBefore);
        var loserDelta = loser.ApplyResult(false, winner.Elo - winnerDelta); // use pre-update elo

        await ratingRepo.UpdateAsync(winner, ct);
        await ratingRepo.UpdateAsync(loser, ct);

        return new UpdateEloResponse(
            winner.PlayerId, winner.Elo, winnerDelta,
            loser.PlayerId, loser.Elo, loserDelta);
    }
}
