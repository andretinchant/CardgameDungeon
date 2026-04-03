using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Repositories;
using MediatR;

namespace CardgameDungeon.Features.Auth.Login;

public class LoginHandler(
    IPlayerRepository playerRepository,
    IRatingRepository ratingRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IAuthTokenService tokenService,
    IPasswordHasher passwordHasher) : IRequestHandler<LoginCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken ct)
    {
        var player = await playerRepository.GetByUsernameAsync(request.Username, ct)
            ?? throw new InvalidOperationException("Invalid username or password.");

        if (!passwordHasher.Verify(request.Password, player.PasswordHash))
            throw new InvalidOperationException("Invalid username or password.");

        player.RecordLogin();
        await playerRepository.UpdateAsync(player, ct);

        // Get player tier for JWT claims
        var rating = await ratingRepository.GetByPlayerIdAsync(player.Id, ct);
        var tier = rating?.Tier.ToString() ?? "Bronze";

        var accessToken = tokenService.GenerateAccessToken(player.Id, player.Username, player.Email, tier);
        var refreshTokenValue = tokenService.GenerateRefreshToken();
        var refreshToken = new Domain.Entities.RefreshToken(Guid.NewGuid(), player.Id, refreshTokenValue, DateTime.UtcNow.AddDays(7));
        await refreshTokenRepository.SaveAsync(refreshToken, ct);

        return new AuthResponse(accessToken, refreshTokenValue, player.Id, player.Username);
    }
}
