using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.Domain.Entities;

public class DeckList
{
    public const int RequiredAdventurerCards = 40;
    public const int RequiredEnemyCards = 40;
    public const int RequiredDungeonRooms = 5;
    public const int RequiredBossCards = 1;
    public const int MinCardCost = 1;
    public const int MaxUniquesCopies = 1;

    public Guid Id { get; private set; }
    public Guid PlayerId { get; private set; }
    public IReadOnlyList<Card> AdventurerCards => _adventurerCards.AsReadOnly();
    public IReadOnlyList<Card> EnemyCards => _enemyCards.AsReadOnly();
    public IReadOnlyList<DungeonRoomCard> DungeonRooms => _dungeonRooms.AsReadOnly();
    public BossCard Boss { get; private set; }

    private readonly List<Card> _adventurerCards;
    private readonly List<Card> _enemyCards;
    private readonly List<DungeonRoomCard> _dungeonRooms;

    public DeckList(
        Guid id,
        Guid playerId,
        IEnumerable<Card> adventurerCards,
        IEnumerable<Card> enemyCards,
        IEnumerable<DungeonRoomCard> dungeonRooms,
        BossCard boss)
    {
        _adventurerCards = adventurerCards.ToList();
        _enemyCards = enemyCards.ToList();
        _dungeonRooms = dungeonRooms.OrderBy(r => r.Order).ToList();
        Boss = boss ?? throw new ArgumentNullException(nameof(boss));
        Id = id;
        PlayerId = playerId;

        Validate();
    }

    private void Validate()
    {
        var errors = new List<string>();

        if (_adventurerCards.Count != RequiredAdventurerCards)
            errors.Add($"Adventurer deck must have exactly {RequiredAdventurerCards} cards, got {_adventurerCards.Count}.");

        if (_enemyCards.Count != RequiredEnemyCards)
            errors.Add($"Enemy deck must have exactly {RequiredEnemyCards} cards, got {_enemyCards.Count}.");

        if (_dungeonRooms.Count != RequiredDungeonRooms)
            errors.Add($"Dungeon must have exactly {RequiredDungeonRooms} rooms, got {_dungeonRooms.Count}.");

        ValidateAdventurerTypes(errors);
        ValidateEnemyTypes(errors);
        ValidateDungeonRoomOrder(errors);
        ValidateUniqueRarity(errors);
        ValidateMinimumCost(errors);

        if (errors.Count > 0)
            throw new InvalidOperationException(
                $"Invalid deck list:{Environment.NewLine}{string.Join(Environment.NewLine, errors)}");
    }

    private void ValidateAdventurerTypes(List<string> errors)
    {
        var invalidCards = _adventurerCards
            .Where(c => c.Type is not (CardType.Ally or CardType.Equipment))
            .ToList();

        if (invalidCards.Count > 0)
            errors.Add($"Adventurer deck contains invalid card types: {string.Join(", ", invalidCards.Select(c => $"{c.Name} ({c.Type})"))}.");
    }

    private void ValidateEnemyTypes(List<string> errors)
    {
        var invalidCards = _enemyCards
            .Where(c => c.Type is not (CardType.Monster or CardType.Trap))
            .ToList();

        if (invalidCards.Count > 0)
            errors.Add($"Enemy deck contains invalid card types: {string.Join(", ", invalidCards.Select(c => $"{c.Name} ({c.Type})"))}.");
    }

    private void ValidateDungeonRoomOrder(List<string> errors)
    {
        var orders = _dungeonRooms.Select(r => r.Order).OrderBy(o => o).ToList();
        var expected = Enumerable.Range(1, RequiredDungeonRooms).ToList();

        if (!orders.SequenceEqual(expected))
            errors.Add($"Dungeon rooms must be ordered 1 through {RequiredDungeonRooms}. Got: {string.Join(", ", orders)}.");
    }

    private void ValidateUniqueRarity(List<string> errors)
    {
        var allCards = _adventurerCards.Concat(_enemyCards).Append(Boss);

        var uniqueDuplicates = allCards
            .Where(c => c.Rarity == Rarity.Unique)
            .GroupBy(c => c.Name)
            .Where(g => g.Count() > MaxUniquesCopies)
            .Select(g => g.Key)
            .ToList();

        if (uniqueDuplicates.Count > 0)
            errors.Add($"Unique cards can only have {MaxUniquesCopies} copy per deck. Duplicates: {string.Join(", ", uniqueDuplicates)}.");
    }

    private void ValidateMinimumCost(List<string> errors)
    {
        var cheapCards = _adventurerCards.Concat(_enemyCards)
            .Where(c => c.Cost < MinCardCost)
            .ToList();

        if (cheapCards.Count > 0)
            errors.Add($"All cards must have a minimum cost of {MinCardCost}. Violations: {string.Join(", ", cheapCards.Select(c => $"{c.Name} (cost {c.Cost})"))}.");
    }
}
