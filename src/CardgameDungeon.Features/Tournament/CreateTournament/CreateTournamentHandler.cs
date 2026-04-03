using CardgameDungeon.Domain.Repositories;
using MediatR;

namespace CardgameDungeon.Features.Tournament.CreateTournament;

public class CreateTournamentHandler(ITournamentRepository tournamentRepo)
    : IRequestHandler<CreateTournamentCommand, TournamentResponse>
{
    public async Task<TournamentResponse> Handle(CreateTournamentCommand request, CancellationToken ct)
    {
        var tournament = new Domain.Entities.Tournament(
            Guid.NewGuid(), request.RequiredTier, request.EntryFee);

        await tournamentRepo.SaveAsync(tournament, ct);

        return new TournamentResponse(
            tournament.Id, tournament.RequiredTier, tournament.EntryFee,
            tournament.PrizePool, tournament.Status, tournament.Participants.Count,
            tournament.CurrentRound);
    }
}
