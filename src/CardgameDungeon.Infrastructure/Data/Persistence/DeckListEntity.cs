namespace CardgameDungeon.Infrastructure.Data.Persistence;

internal class DeckListEntity
{
    public Guid Id { get; set; }
    public Guid PlayerId { get; set; }
    public Guid BossCardId { get; set; }
    public List<Guid> AdventurerCardIds { get; set; } = [];
    public List<Guid> EnemyCardIds { get; set; } = [];
    public List<Guid> DungeonRoomCardIds { get; set; } = [];
}
