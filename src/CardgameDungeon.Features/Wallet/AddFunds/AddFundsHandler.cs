using CardgameDungeon.Domain.Repositories;
using MediatR;

namespace CardgameDungeon.Features.Wallet.AddFunds;

public class AddFundsHandler(IWalletRepository walletRepo)
    : IRequestHandler<AddFundsCommand, AddFundsResponse>
{
    public async Task<AddFundsResponse> Handle(AddFundsCommand request, CancellationToken ct)
    {
        var wallet = await walletRepo.GetByPlayerIdAsync(request.PlayerId, ct)
            ?? throw new KeyNotFoundException($"Wallet for player {request.PlayerId} not found.");

        wallet.AddFunds(request.Amount, request.Source, DateOnly.FromDateTime(DateTime.UtcNow));

        await walletRepo.UpdateAsync(wallet, ct);

        return new AddFundsResponse(wallet.PlayerId, wallet.Balance, request.Source);
    }
}
