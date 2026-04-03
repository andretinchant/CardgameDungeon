using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.Tests.Deck;

public class DeckListTests
{
    private static AllyCard MakeAlly(Rarity rarity = Rarity.Common, string? name = null, int cost = 1)
        => new(Guid.NewGuid(), name ?? $"Ally-{Guid.NewGuid():N}"[..12],
            rarity, cost, strength: 3, hitPoints: 5, initiative: 2);

    private static EquipmentCard MakeEquipment(Rarity rarity = Rarity.Common, string? name = null, int cost = 1)
        => new(Guid.NewGuid(), name ?? $"Equip-{Guid.NewGuid():N}"[..12],
            rarity, cost, strengthModifier: 1, hitPointsModifier: 1, initiativeModifier: 0);

    private static MonsterCard MakeMonster(Rarity rarity = Rarity.Common, string? name = null, int cost = 1)
        => new(Guid.NewGuid(), name ?? $"Monster-{Guid.NewGuid():N}"[..12],
            rarity, cost, strength: 3, hitPoints: 5, initiative: 2);

    private static TrapCard MakeTrap(Rarity rarity = Rarity.Common, string? name = null, int cost = 1)
        => new(Guid.NewGuid(), name ?? $"Trap-{Guid.NewGuid():N}"[..12],
            rarity, cost, damage: 2, effect: "Deals damage");

    private static DungeonRoomCard MakeRoom(int order)
        => new(Guid.NewGuid(), $"Room-{order}", Rarity.Common, order);

    private static BossCard MakeBoss(Rarity rarity = Rarity.Unique)
        => new(Guid.NewGuid(), "TheBoss", rarity, cost: 1, strength: 10, hitPoints: 20, initiative: 5);

    private static List<Card> MakeAdventurers(int allyCount = 30, int equipCount = 10)
    {
        var cards = new List<Card>();
        for (var i = 0; i < allyCount; i++) cards.Add(MakeAlly());
        for (var i = 0; i < equipCount; i++) cards.Add(MakeEquipment());
        return cards;
    }

    private static List<Card> MakeEnemies(int monsterCount = 30, int trapCount = 10)
    {
        var cards = new List<Card>();
        for (var i = 0; i < monsterCount; i++) cards.Add(MakeMonster());
        for (var i = 0; i < trapCount; i++) cards.Add(MakeTrap());
        return cards;
    }

    private static List<DungeonRoomCard> MakeRooms()
        => Enumerable.Range(1, 5).Select(MakeRoom).ToList();

    #region Valid Deck

    [Fact]
    public void ValidDeck_DoesNotThrow()
    {
        var deck = new DeckList(
            Guid.NewGuid(), Guid.NewGuid(),
            MakeAdventurers(), MakeEnemies(), MakeRooms(), MakeBoss());

        Assert.Equal(40, deck.AdventurerCards.Count);
        Assert.Equal(40, deck.EnemyCards.Count);
        Assert.Equal(5, deck.DungeonRooms.Count);
        Assert.NotNull(deck.Boss);
    }

    [Fact]
    public void ValidDeck_AllAllies_NoEquipment()
    {
        var adventurers = MakeAdventurers(allyCount: 40, equipCount: 0);

        var deck = new DeckList(
            Guid.NewGuid(), Guid.NewGuid(),
            adventurers, MakeEnemies(), MakeRooms(), MakeBoss());

        Assert.Equal(40, deck.AdventurerCards.Count);
    }

    [Fact]
    public void ValidDeck_AllMonsters_NoTraps()
    {
        var enemies = MakeEnemies(monsterCount: 40, trapCount: 0);

        var deck = new DeckList(
            Guid.NewGuid(), Guid.NewGuid(),
            MakeAdventurers(), enemies, MakeRooms(), MakeBoss());

        Assert.Equal(40, deck.EnemyCards.Count);
    }

    [Fact]
    public void ValidDeck_RoomsOrderedAutomatically()
    {
        var rooms = new[] { MakeRoom(5), MakeRoom(3), MakeRoom(1), MakeRoom(4), MakeRoom(2) }.ToList();

        var deck = new DeckList(
            Guid.NewGuid(), Guid.NewGuid(),
            MakeAdventurers(), MakeEnemies(), rooms, MakeBoss());

        for (var i = 0; i < 5; i++)
            Assert.Equal(i + 1, deck.DungeonRooms[i].Order);
    }

    #endregion

    #region Adventurer Count Validation

    [Fact]
    public void TooFewAdventurers_Throws()
    {
        var adventurers = MakeAdventurers(allyCount: 30, equipCount: 5); // 35 total

        var ex = Assert.Throws<InvalidOperationException>(() =>
            new DeckList(Guid.NewGuid(), Guid.NewGuid(),
                adventurers, MakeEnemies(), MakeRooms(), MakeBoss()));

        Assert.Contains("Adventurer deck must have exactly 40", ex.Message);
    }

    [Fact]
    public void TooManyAdventurers_Throws()
    {
        var adventurers = MakeAdventurers(allyCount: 35, equipCount: 10); // 45 total

        var ex = Assert.Throws<InvalidOperationException>(() =>
            new DeckList(Guid.NewGuid(), Guid.NewGuid(),
                adventurers, MakeEnemies(), MakeRooms(), MakeBoss()));

        Assert.Contains("Adventurer deck must have exactly 40", ex.Message);
    }

    #endregion

    #region Enemy Count Validation

    [Fact]
    public void TooFewEnemies_Throws()
    {
        var enemies = MakeEnemies(monsterCount: 20, trapCount: 10); // 30 total

        var ex = Assert.Throws<InvalidOperationException>(() =>
            new DeckList(Guid.NewGuid(), Guid.NewGuid(),
                MakeAdventurers(), enemies, MakeRooms(), MakeBoss()));

        Assert.Contains("Enemy deck must have exactly 40", ex.Message);
    }

    [Fact]
    public void TooManyEnemies_Throws()
    {
        var enemies = MakeEnemies(monsterCount: 35, trapCount: 10); // 45 total

        var ex = Assert.Throws<InvalidOperationException>(() =>
            new DeckList(Guid.NewGuid(), Guid.NewGuid(),
                MakeAdventurers(), enemies, MakeRooms(), MakeBoss()));

        Assert.Contains("Enemy deck must have exactly 40", ex.Message);
    }

    #endregion

    #region Dungeon Room Validation

    [Fact]
    public void TooFewRooms_Throws()
    {
        var rooms = new[] { MakeRoom(1), MakeRoom(2), MakeRoom(3) }.ToList();

        var ex = Assert.Throws<InvalidOperationException>(() =>
            new DeckList(Guid.NewGuid(), Guid.NewGuid(),
                MakeAdventurers(), MakeEnemies(), rooms, MakeBoss()));

        Assert.Contains("Dungeon must have exactly 5 rooms", ex.Message);
    }

    [Fact]
    public void DuplicateRoomOrder_Throws()
    {
        var rooms = new[] { MakeRoom(1), MakeRoom(2), MakeRoom(3), MakeRoom(3), MakeRoom(5) }.ToList();

        var ex = Assert.Throws<InvalidOperationException>(() =>
            new DeckList(Guid.NewGuid(), Guid.NewGuid(),
                MakeAdventurers(), MakeEnemies(), rooms, MakeBoss()));

        Assert.Contains("Dungeon rooms must be ordered 1 through 5", ex.Message);
    }

    #endregion

    #region Unique Rarity Validation

    [Fact]
    public void SingleUniqueCard_Allowed()
    {
        var adventurers = MakeAdventurers(allyCount: 29, equipCount: 10);
        adventurers.Add(MakeAlly(rarity: Rarity.Unique, name: "UniqueHero"));

        var deck = new DeckList(
            Guid.NewGuid(), Guid.NewGuid(),
            adventurers, MakeEnemies(), MakeRooms(), MakeBoss());

        Assert.Equal(40, deck.AdventurerCards.Count);
    }

    [Fact]
    public void DuplicateUniqueCard_InAdventurers_Throws()
    {
        var adventurers = MakeAdventurers(allyCount: 28, equipCount: 10);
        adventurers.Add(MakeAlly(rarity: Rarity.Unique, name: "UniqueHero"));
        adventurers.Add(MakeAlly(rarity: Rarity.Unique, name: "UniqueHero"));

        var ex = Assert.Throws<InvalidOperationException>(() =>
            new DeckList(Guid.NewGuid(), Guid.NewGuid(),
                adventurers, MakeEnemies(), MakeRooms(), MakeBoss()));

        Assert.Contains("Unique cards can only have 1 copy", ex.Message);
        Assert.Contains("UniqueHero", ex.Message);
    }

    [Fact]
    public void DuplicateUniqueCard_InEnemies_Throws()
    {
        var enemies = MakeEnemies(monsterCount: 28, trapCount: 10);
        enemies.Add(MakeMonster(rarity: Rarity.Unique, name: "UniqueMonster"));
        enemies.Add(MakeMonster(rarity: Rarity.Unique, name: "UniqueMonster"));

        var ex = Assert.Throws<InvalidOperationException>(() =>
            new DeckList(Guid.NewGuid(), Guid.NewGuid(),
                MakeAdventurers(), enemies, MakeRooms(), MakeBoss()));

        Assert.Contains("UniqueMonster", ex.Message);
    }

    [Fact]
    public void DifferentUniqueCards_Allowed()
    {
        var adventurers = MakeAdventurers(allyCount: 28, equipCount: 10);
        adventurers.Add(MakeAlly(rarity: Rarity.Unique, name: "UniqueA"));
        adventurers.Add(MakeAlly(rarity: Rarity.Unique, name: "UniqueB"));

        var deck = new DeckList(
            Guid.NewGuid(), Guid.NewGuid(),
            adventurers, MakeEnemies(), MakeRooms(), MakeBoss());

        Assert.Equal(40, deck.AdventurerCards.Count);
    }

    #endregion

    #region Minimum Cost Validation

    [Fact]
    public void ZeroCostAdventurer_Throws()
    {
        var adventurers = MakeAdventurers(allyCount: 39, equipCount: 0);
        adventurers.Add(MakeAlly(cost: 0, name: "FreeAlly"));

        var ex = Assert.Throws<InvalidOperationException>(() =>
            new DeckList(Guid.NewGuid(), Guid.NewGuid(),
                adventurers, MakeEnemies(), MakeRooms(), MakeBoss()));

        Assert.Contains("minimum cost of 1", ex.Message);
        Assert.Contains("FreeAlly", ex.Message);
    }

    [Fact]
    public void ZeroCostEnemy_Throws()
    {
        var enemies = MakeEnemies(monsterCount: 39, trapCount: 0);
        enemies.Add(MakeMonster(cost: 0, name: "FreeMonster"));

        var ex = Assert.Throws<InvalidOperationException>(() =>
            new DeckList(Guid.NewGuid(), Guid.NewGuid(),
                MakeAdventurers(), enemies, MakeRooms(), MakeBoss()));

        Assert.Contains("minimum cost of 1", ex.Message);
        Assert.Contains("FreeMonster", ex.Message);
    }

    [Fact]
    public void Cost1_Allowed()
    {
        var adventurers = MakeAdventurers(allyCount: 39, equipCount: 0);
        adventurers.Add(MakeAlly(cost: 1));

        var deck = new DeckList(
            Guid.NewGuid(), Guid.NewGuid(),
            adventurers, MakeEnemies(), MakeRooms(), MakeBoss());

        Assert.Equal(40, deck.AdventurerCards.Count);
    }

    #endregion

    #region Card Type Validation

    [Fact]
    public void MonsterInAdventurerDeck_Throws()
    {
        var adventurers = MakeAdventurers(allyCount: 29, equipCount: 10);
        adventurers.Add(MakeMonster(name: "Infiltrator"));

        var ex = Assert.Throws<InvalidOperationException>(() =>
            new DeckList(Guid.NewGuid(), Guid.NewGuid(),
                adventurers, MakeEnemies(), MakeRooms(), MakeBoss()));

        Assert.Contains("Adventurer deck contains invalid card types", ex.Message);
        Assert.Contains("Infiltrator", ex.Message);
    }

    [Fact]
    public void AllyInEnemyDeck_Throws()
    {
        var enemies = MakeEnemies(monsterCount: 29, trapCount: 10);
        enemies.Add(MakeAlly(name: "Traitor"));

        var ex = Assert.Throws<InvalidOperationException>(() =>
            new DeckList(Guid.NewGuid(), Guid.NewGuid(),
                MakeAdventurers(), enemies, MakeRooms(), MakeBoss()));

        Assert.Contains("Enemy deck contains invalid card types", ex.Message);
        Assert.Contains("Traitor", ex.Message);
    }

    #endregion

    #region Multiple Validation Errors

    [Fact]
    public void MultipleErrors_AllReported()
    {
        var adventurers = MakeAdventurers(allyCount: 10, equipCount: 5); // 15, not 40
        var enemies = MakeEnemies(monsterCount: 10, trapCount: 5);       // 15, not 40

        var ex = Assert.Throws<InvalidOperationException>(() =>
            new DeckList(Guid.NewGuid(), Guid.NewGuid(),
                adventurers, enemies, MakeRooms(), MakeBoss()));

        Assert.Contains("Adventurer deck must have exactly 40", ex.Message);
        Assert.Contains("Enemy deck must have exactly 40", ex.Message);
    }

    #endregion

    #region Boss Validation

    [Fact]
    public void NullBoss_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new DeckList(Guid.NewGuid(), Guid.NewGuid(),
                MakeAdventurers(), MakeEnemies(), MakeRooms(), null!));
    }

    #endregion
}
