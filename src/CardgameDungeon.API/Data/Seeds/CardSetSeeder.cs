using CardgameDungeon.Domain.Entities;

namespace CardgameDungeon.API.Data.Seeds;

public static partial class CardSetSeeder
{
    public static readonly Guid DND1SetId = new("10000000-0000-0000-0000-000000000001");

    public static CardSet CreateDND1CoreSet()
    {
        var set = new CardSet(
            DND1SetId,
            "Dungeons & Dragons: Core Set",
            "DND1",
            new DateOnly(2026, 4, 1),
            "The foundational set featuring iconic heroes, monsters, equipment, and dungeons from the world of Dungeons & Dragons.");

        var allies = CreateAllies();
        var equipment = CreateEquipment();
        var monsters = CreateMonsters();
        var traps = CreateTraps();
        var rooms = CreateDungeonRooms();
        var bosses = CreateBosses();

        foreach (var card in allies) set.AddCard(card);
        foreach (var card in equipment) set.AddCard(card);
        foreach (var card in monsters) set.AddCard(card);
        foreach (var card in traps) set.AddCard(card);
        foreach (var card in rooms) set.AddCard(card);
        foreach (var card in bosses) set.AddCard(card);

        return set;
    }
}
