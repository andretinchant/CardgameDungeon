namespace CardgameDungeon.Infrastructure.Data.Persistence;

internal class MatchStateEntity
{
    public Guid Id { get; set; }
    public int Phase { get; set; }
    public Guid? WinnerId { get; set; }
    public string StateJson { get; set; } = null!;
}
