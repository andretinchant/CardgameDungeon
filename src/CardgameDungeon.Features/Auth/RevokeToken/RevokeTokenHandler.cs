using CardgameDungeon.Domain.Repositories;
using MediatR;

namespace CardgameDungeon.Features.Auth.RevokeToken;

public class RevokeTokenHandler(
    IRefreshTokenRepository refreshTokenRepository) : IRequestHandler<RevokeTokenCommand, RevokeTokenResponse>
{
    public async Task<RevokeTokenResponse> Handle(RevokeTokenCommand request, CancellationToken ct)
    {
        var token = await refreshTokenRepository.GetByTokenAsync(request.RefreshToken, ct)
            ?? throw new InvalidOperationException("Refresh token not found.");

        if (!token.IsActive)
            return new RevokeTokenResponse(false);

        token.Revoke();
        await refreshTokenRepository.UpdateAsync(token, ct);

        return new RevokeTokenResponse(true);
    }
}
