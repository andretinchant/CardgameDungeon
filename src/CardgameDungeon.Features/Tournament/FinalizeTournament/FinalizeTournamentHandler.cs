using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Domain.Repositories;
using MediatR;

namespace CardgameDungeon.Features.Tournament.FinalizeTournament;

public class FinalizeTournamentHandler(
    ITournamentRepository tournamentRepo,
    IWalletRepository walletRepo)
    : IRequestHandler<FinalizeTournamentCommand, FinalizeTournamentResponse>
{
    public async Task<FinalizeTournamentResponse> Handle(FinalizeTournamentCommand request, CancellationToken ct)
    {
        var tournament = await tournamentRepo.GetByIdAsync(request.TournamentId, ct)
            ?? throw new KeyNotFoundException($"Tournament {request.TournamentId} not found.");

        var (firstId, secondId, thirdId) = tournament.GetTopThree();
        var (firstPrize, secondPrize, thirdPrize) = tournament.CalculatePrizes();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        await PayPrize(firstId, firstPrize, today, ct);
        await PayPrize(secondId, secondPrize, today, ct);
        await PayPrize(thirdId, thirdPrize, today, ct);

        await tournamentRepo.UpdateAsync(tournament, ct);

        return new FinalizeTournamentResponse(
            tournament.Id,
            firstId, firstPrize,
            secondId, secondPrize,
            thirdId, thirdPrize);
    }

    private async Task PayPrize(Guid playerId, int amount, DateOnly today, CancellationToken ct)
    {
        var wallet = await walletRepo.GetByPlayerIdAsync(playerId, ct)
            ?? throw new KeyNotFoundException($"Wallet for player {playerId} not found.");

        wallet.AddFunds(amount, FundSource.EventPrize, today);
        await walletRepo.UpdateAsync(wallet, ct);
    }
}
