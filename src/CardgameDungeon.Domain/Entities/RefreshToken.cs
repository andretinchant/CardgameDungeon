namespace CardgameDungeon.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; private set; }
    public Guid PlayerId { get; private set; }
    public string Token { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsRevoked { get; private set; }

    private RefreshToken() { Token = null!; } // EF Core

    public RefreshToken(Guid id, Guid playerId, string token, DateTime expiresAt)
    {
        Id = id;
        PlayerId = playerId;
        Token = token;
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.UtcNow;
        IsRevoked = false;
    }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;

    public void Revoke() => IsRevoked = true;
}
