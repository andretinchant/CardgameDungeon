using CardgameDungeon.Domain.Repositories;
using MediatR;

namespace CardgameDungeon.Features.Tournament.AdvanceTournament;

public class AdvanceTournamentHandler(ITournamentRepository tournamentRepo)
    : IRequestHandler<AdvanceTournamentCommand, AdvanceTournamentResponse>
{
    public async Task<AdvanceTournamentResponse> Handle(AdvanceTournamentCommand request, CancellationToken ct)
    {
        var tournament = await tournamentRepo.GetByIdAsync(request.TournamentId, ct)
            ?? throw new KeyNotFoundException($"Tournament {request.TournamentId} not found.");

        tournament.EliminatePlayer(request.LoserId);

        // Check if round is complete (half remaining eliminated)
        var active = tournament.ActiveParticipants.Count;
        var expectedAfterRound = (int)Math.Pow(2, Domain.Entities.Tournament.TotalRounds - tournament.CurrentRound);

        if (active <= expectedAfterRound)
            tournament.AdvanceRound();

        await tournamentRepo.UpdateAsync(tournament, ct);

        return new AdvanceTournamentResponse(
            tournament.Id, tournament.CurrentRound, active, tournament.Status);
    }
}
