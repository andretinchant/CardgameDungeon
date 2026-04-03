using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Tests.Deck.Fakes;

namespace CardgameDungeon.Tests.Deck;

public static class DeckTestHelper
{
    public static AllyCard MakeAlly(Rarity rarity = Rarity.Common, string? name = null, int cost = 1)
        => new(Guid.NewGuid(), name ?? $"Ally-{Guid.NewGuid():N}"[..12],
            rarity, cost, strength: 3, hitPoints: 5, initiative: 2);

    public static EquipmentCard MakeEquipment(int cost = 1)
        => new(Guid.NewGuid(), $"Equip-{Guid.NewGuid():N}"[..12],
            Rarity.Common, cost, 1, 1, 0);

    public static MonsterCard MakeMonster(int cost = 1)
        => new(Guid.NewGuid(), $"Monster-{Guid.NewGuid():N}"[..12],
            Rarity.Common, cost, strength: 3, hitPoints: 5, initiative: 2);

    public static TrapCard MakeTrap(int cost = 1)
        => new(Guid.NewGuid(), $"Trap-{Guid.NewGuid():N}"[..12],
            Rarity.Common, cost, damage: 2, effect: "Deals damage");

    public static DungeonRoomCard MakeRoom(int order)
        => new(Guid.NewGuid(), $"Room-{order}", Rarity.Common, order);

    public static BossCard MakeBoss()
        => new(Guid.NewGuid(), "TheBoss", Rarity.Unique, 1, 10, 20, 5);

    public static (List<Card> adventurers, List<Card> enemies, List<DungeonRoomCard> rooms, BossCard boss)
        MakeFullDeckCards()
    {
        var adventurers = new List<Card>();
        for (var i = 0; i < 30; i++) adventurers.Add(MakeAlly());
        for (var i = 0; i < 10; i++) adventurers.Add(MakeEquipment());

        var enemies = new List<Card>();
        for (var i = 0; i < 30; i++) enemies.Add(MakeMonster());
        for (var i = 0; i < 10; i++) enemies.Add(MakeTrap());

        var rooms = Enumerable.Range(1, 5).Select(MakeRoom).ToList();
        var boss = MakeBoss();

        return (adventurers, enemies, rooms, boss);
    }

    public static void SeedCards(
        FakeCardRepository cardRepo,
        List<Card> adventurers,
        List<Card> enemies,
        List<DungeonRoomCard> rooms,
        BossCard boss)
    {
        foreach (var c in adventurers) cardRepo.Add(c);
        foreach (var c in enemies) cardRepo.Add(c);
        foreach (var r in rooms) cardRepo.Add(r);
        cardRepo.Add(boss);
    }

    public static DeckList MakeValidDeck()
    {
        var (adventurers, enemies, rooms, boss) = MakeFullDeckCards();
        return new DeckList(Guid.NewGuid(), Guid.NewGuid(), adventurers, enemies, rooms, boss);
    }
}
