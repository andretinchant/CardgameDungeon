using CardgameDungeon.Features.Auth.Login;
using CardgameDungeon.Features.Auth.RefreshToken;
using CardgameDungeon.Features.Auth.Register;
using CardgameDungeon.Features.Auth.RevokeToken;
using MediatR;

namespace CardgameDungeon.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth").AllowAnonymous();

        group.MapPost("/register", async (RegisterCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Created($"/api/auth/profile/{result.PlayerId}", result);
        });

        group.MapPost("/login", async (LoginCommand command, IMediator mediator) =>
            Results.Ok(await mediator.Send(command)));

        group.MapPost("/refresh", async (RefreshTokenCommand command, IMediator mediator) =>
            Results.Ok(await mediator.Send(command)));

        group.MapPost("/revoke", async (RevokeTokenCommand command, IMediator mediator) =>
            Results.Ok(await mediator.Send(command)));
    }
}
