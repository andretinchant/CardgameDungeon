namespace CardgameDungeon.Domain.ValueObjects;

public record InitiativeResult
{
    public int Player1Total { get; }
    public int Player2Total { get; }
    public Guid? WinnerId { get; }
    public bool IsTied => WinnerId is null;

    private InitiativeResult(int player1Total, int player2Total, Guid? winnerId)
    {
        Player1Total = player1Total;
        Player2Total = player2Total;
        WinnerId = winnerId;
    }

    public static InitiativeResult Tied(int player1Total, int player2Total)
        => new(player1Total, player2Total, winnerId: null);

    public static InitiativeResult Won(int player1Total, int player2Total, Guid winnerId)
        => new(player1Total, player2Total, winnerId);
}
