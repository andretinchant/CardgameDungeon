using CardgameDungeon.Domain.Enums;
using MediatR;

namespace CardgameDungeon.Features.Wallet.AddFunds;

public record AddFundsCommand(
    Guid PlayerId,
    int Amount,
    FundSource Source) : IRequest<AddFundsResponse>;

public record AddFundsResponse(Guid PlayerId, int NewBalance, FundSource Source);
