using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Repositories;
using CardgameDungeon.Features.Match.Shared;
using MediatR;

namespace CardgameDungeon.Features.Match.SetupInitialTeam;

public class SetupInitialTeamHandler(IMatchRepository matchRepo, IMatchNotifier notifier)
    : IRequestHandler<SetupInitialTeamCommand, SetupInitialTeamResponse>
{
    public async Task<SetupInitialTeamResponse> Handle(SetupInitialTeamCommand request, CancellationToken ct)
    {
        var match = await matchRepo.GetByIdAsync(request.MatchId, ct)
            ?? throw new KeyNotFoundException($"Match {request.MatchId} not found.");

        var player = match.GetPlayer(request.PlayerId);

        // Find the ally cards in the player's DECK (setup picks from deck, not hand)
        var allies = player.ExtractAlliesFromDeck(request.AllyCardIds);

        if (allies.Count != request.AllyCardIds.Count)
            throw new InvalidOperationException("Some requested allies were not found in the deck.");

        match.SubmitSetupTeam(request.PlayerId, allies);

        await matchRepo.UpdateAsync(match, ct);

        await notifier.SetupTeamSubmitted(match.Id, request.PlayerId, match.BothTeamsSubmitted);

        return new SetupInitialTeamResponse(
            match.Id,
            request.PlayerId,
            Submitted: true,
            BothReady: match.BothTeamsSubmitted);
    }
}
