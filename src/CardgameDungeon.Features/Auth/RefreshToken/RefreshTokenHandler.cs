using CardgameDungeon.Domain.Repositories;
using MediatR;

namespace CardgameDungeon.Features.Auth.RefreshToken;

public class RefreshTokenHandler(
    IPlayerRepository playerRepository,
    IRatingRepository ratingRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IAuthTokenService tokenService) : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        // Validate the expired access token to extract PlayerId
        var playerId = tokenService.ValidateAccessToken(request.AccessToken)
            ?? throw new InvalidOperationException("Invalid access token.");

        // Validate the refresh token
        var storedToken = await refreshTokenRepository.GetByTokenAsync(request.RefreshToken, ct)
            ?? throw new InvalidOperationException("Invalid refresh token.");

        if (!storedToken.IsActive)
            throw new InvalidOperationException("Refresh token is expired or revoked.");

        if (storedToken.PlayerId != playerId)
            throw new InvalidOperationException("Token mismatch.");

        // Revoke old refresh token (rotation)
        storedToken.Revoke();
        await refreshTokenRepository.UpdateAsync(storedToken, ct);

        // Issue new tokens
        var player = await playerRepository.GetByIdAsync(playerId, ct)
            ?? throw new InvalidOperationException("Player not found.");

        var rating = await ratingRepository.GetByPlayerIdAsync(player.Id, ct);
        var tier = rating?.Tier.ToString() ?? "Bronze";

        var newAccessToken = tokenService.GenerateAccessToken(player.Id, player.Username, player.Email, tier);
        var newRefreshTokenValue = tokenService.GenerateRefreshToken();
        var newRefreshToken = new Domain.Entities.RefreshToken(
            Guid.NewGuid(), player.Id, newRefreshTokenValue, DateTime.UtcNow.AddDays(7));
        await refreshTokenRepository.SaveAsync(newRefreshToken, ct);

        return new AuthResponse(newAccessToken, newRefreshTokenValue, player.Id, player.Username);
    }
}
