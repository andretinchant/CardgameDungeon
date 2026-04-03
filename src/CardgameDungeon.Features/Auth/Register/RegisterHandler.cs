using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Repositories;
using MediatR;

namespace CardgameDungeon.Features.Auth.Register;

public class RegisterHandler(
    IPlayerRepository playerRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IAuthTokenService tokenService,
    IPasswordHasher passwordHasher) : IRequestHandler<RegisterCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken ct)
    {
        if (await playerRepository.ExistsAsync(request.Username, request.Email, ct))
            throw new InvalidOperationException("Username or email already taken.");

        var passwordHash = passwordHasher.Hash(request.Password);
        var player = new Player(Guid.NewGuid(), request.Username, request.Email, passwordHash);
        await playerRepository.SaveAsync(player, ct);

        var accessToken = tokenService.GenerateAccessToken(player.Id, player.Username, player.Email, "Bronze");
        var refreshTokenValue = tokenService.GenerateRefreshToken();
        var refreshToken = new Domain.Entities.RefreshToken(Guid.NewGuid(), player.Id, refreshTokenValue, DateTime.UtcNow.AddDays(7));
        await refreshTokenRepository.SaveAsync(refreshToken, ct);

        return new AuthResponse(accessToken, refreshTokenValue, player.Id, player.Username);
    }
}
