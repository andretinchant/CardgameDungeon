using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Repositories;
using MediatR;

namespace CardgameDungeon.Features.Match.SetupInitialTeam;

public class SetupInitialTeamHandler(IMatchRepository matchRepo)
    : IRequestHandler<SetupInitialTeamCommand, SetupInitialTeamResponse>
{
    public async Task<SetupInitialTeamResponse> Handle(SetupInitialTeamCommand request, CancellationToken ct)
    {
        var match = await matchRepo.GetByIdAsync(request.MatchId, ct)
            ?? throw new KeyNotFoundException($"Match {request.MatchId} not found.");

        var player = match.GetPlayer(request.PlayerId);

        // Find the ally cards in the player's hand
        var allies = request.AllyCardIds
            .Select(id => player.Hand.OfType<AllyCard>().FirstOrDefault(a => a.Id == id)
                ?? throw new InvalidOperationException($"Ally {id} is not in player's hand."))
            .ToList();

        match.SubmitSetupTeam(request.PlayerId, allies);

        await matchRepo.UpdateAsync(match, ct);

        return new SetupInitialTeamResponse(
            match.Id,
            request.PlayerId,
            Submitted: true,
            BothReady: match.BothTeamsSubmitted);
    }
}
