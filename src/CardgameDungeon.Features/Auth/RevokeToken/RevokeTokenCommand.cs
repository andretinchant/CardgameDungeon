using MediatR;

namespace CardgameDungeon.Features.Auth.RevokeToken;

public record RevokeTokenCommand(string RefreshToken) : IRequest<RevokeTokenResponse>;

public record RevokeTokenResponse(bool Revoked);
