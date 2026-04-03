using CardgameDungeon.Features.Wallet.AddFunds;
using CardgameDungeon.Features.Wallet.GetBalance;
using MediatR;

namespace CardgameDungeon.API.Endpoints;

public static class WalletEndpoints
{
    public static void MapWalletEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/wallet").WithTags("Wallet").RequireAuthorization();

        group.MapGet("/{playerId:guid}", async (Guid playerId, IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetBalanceQuery(playerId))));

        group.MapPost("/add-funds", async (AddFundsCommand command, IMediator mediator) =>
            Results.Ok(await mediator.Send(command)));
    }
}
