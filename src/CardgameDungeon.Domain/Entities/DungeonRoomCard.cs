using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.Domain.Entities;

public class DungeonRoomCard : Card
{
    public const int TotalRooms = 5;

    public override CardType Type => CardType.DungeonRoom;
    public int Order { get; private set; }
    public IReadOnlyList<Guid> MonsterIds => _monsterIds.AsReadOnly();
    public IReadOnlyList<Guid> TrapIds => _trapIds.AsReadOnly();

    private readonly List<Guid> _monsterIds;
    private readonly List<Guid> _trapIds;

    public DungeonRoomCard(
        Guid id,
        string name,
        Rarity rarity,
        int order,
        IEnumerable<Guid>? monsterIds = null,
        IEnumerable<Guid>? trapIds = null)
        : base(id, name, rarity, cost: 0)
    {
        if (order < 1 || order > TotalRooms)
            throw new ArgumentOutOfRangeException(nameof(order), $"Order must be between 1 and {TotalRooms}.");

        Order = order;
        _monsterIds = monsterIds?.ToList() ?? [];
        _trapIds = trapIds?.ToList() ?? [];
    }

    public bool HasMonsters => _monsterIds.Count > 0;
}
