using CardgameDungeon.Domain.Repositories;
using MediatR;

namespace CardgameDungeon.Features.Wallet.GetBalance;

public class GetBalanceHandler(IWalletRepository walletRepo)
    : IRequestHandler<GetBalanceQuery, BalanceResponse>
{
    public async Task<BalanceResponse> Handle(GetBalanceQuery request, CancellationToken ct)
    {
        var wallet = await walletRepo.GetByPlayerIdAsync(request.PlayerId, ct)
            ?? throw new KeyNotFoundException($"Wallet for player {request.PlayerId} not found.");

        return new BalanceResponse(wallet.PlayerId, wallet.Balance);
    }
}
