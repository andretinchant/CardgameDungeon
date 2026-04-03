using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.Domain.Entities;

public class PlayerRating
{
    public const int DefaultElo = 1000;
    public const int KFactor = 32;

    public Guid PlayerId { get; private set; }
    public int Elo { get; private set; }
    public Tier Tier { get; private set; }
    public int Wins { get; private set; }
    public int Losses { get; private set; }

    public int TotalGames => Wins + Losses;

    private PlayerRating() { } // EF Core

    public PlayerRating(Guid playerId, int elo = DefaultElo)
    {
        PlayerId = playerId;
        Elo = elo;
        Tier = Tier.Bronze;
    }

    public int ApplyResult(bool won, int opponentElo)
    {
        var expected = 1.0 / (1.0 + Math.Pow(10, (opponentElo - Elo) / 400.0));
        var score = won ? 1.0 : 0.0;
        var delta = (int)Math.Round(KFactor * (score - expected));

        Elo = Math.Max(0, Elo + delta);

        if (won) Wins++;
        else Losses++;

        return delta;
    }

    public void SetTier(Tier tier) => Tier = tier;
}
