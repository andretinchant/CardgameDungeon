using MediatR;

namespace CardgameDungeon.Features.Wallet.GetBalance;

public record GetBalanceQuery(Guid PlayerId) : IRequest<BalanceResponse>;

public record BalanceResponse(Guid PlayerId, int Balance);
