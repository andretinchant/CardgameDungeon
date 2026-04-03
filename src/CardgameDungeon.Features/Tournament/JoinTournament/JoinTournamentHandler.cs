using CardgameDungeon.Domain.Repositories;
using MediatR;

namespace CardgameDungeon.Features.Tournament.JoinTournament;

public class JoinTournamentHandler(
    ITournamentRepository tournamentRepo,
    IWalletRepository walletRepo,
    IRatingRepository ratingRepo)
    : IRequestHandler<JoinTournamentCommand, JoinTournamentResponse>
{
    public async Task<JoinTournamentResponse> Handle(JoinTournamentCommand request, CancellationToken ct)
    {
        var tournament = await tournamentRepo.GetByIdAsync(request.TournamentId, ct)
            ?? throw new KeyNotFoundException($"Tournament {request.TournamentId} not found.");

        var rating = await ratingRepo.GetByPlayerIdAsync(request.PlayerId, ct)
            ?? throw new KeyNotFoundException($"Rating for player {request.PlayerId} not found.");

        var wallet = await walletRepo.GetByPlayerIdAsync(request.PlayerId, ct)
            ?? throw new KeyNotFoundException($"Wallet for player {request.PlayerId} not found.");

        wallet.Deduct(tournament.EntryFee);
        tournament.AddParticipant(request.PlayerId, request.DeckId, rating.Tier);

        await walletRepo.UpdateAsync(wallet, ct);
        await tournamentRepo.UpdateAsync(tournament, ct);

        return new JoinTournamentResponse(
            tournament.Id, request.PlayerId,
            tournament.Participants.Count, tournament.PrizePool);
    }
}
