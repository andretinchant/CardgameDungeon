namespace CardgameDungeon.Features.Auth;

public record AuthResponse(string AccessToken, string RefreshToken, Guid PlayerId, string Username);
