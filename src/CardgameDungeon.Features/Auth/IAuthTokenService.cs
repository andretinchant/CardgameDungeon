namespace CardgameDungeon.Features.Auth;

public interface IAuthTokenService
{
    string GenerateAccessToken(Guid playerId, string username, string email, string tier);
    string GenerateRefreshToken();
    Guid? ValidateAccessToken(string token);
}
