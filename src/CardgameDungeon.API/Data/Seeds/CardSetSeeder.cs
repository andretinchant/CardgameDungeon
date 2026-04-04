using CardgameDungeon.API.Data.Seeds.Themes;
using CardgameDungeon.Domain.Entities;

namespace CardgameDungeon.API.Data.Seeds;

public static partial class CardSetSeeder
{
    public static readonly Guid DND1SetId = new("10000000-0000-0000-0000-000000000001");

    public static List<CardSet> CreateAllSets()
    {
        var releaseBase = new DateOnly(2026, 4, 1);

        return
        [
            CreateDND1CoreSet(),
            CardSetGenerator.Generate(SetThemes.Ravenloft(), releaseBase.AddMonths(1)),
            CardSetGenerator.Generate(SetThemes.ForgottenRealms(), releaseBase.AddMonths(2)),
            CardSetGenerator.Generate(SetThemes.Underdark(), releaseBase.AddMonths(3)),
            CardSetGenerator.Generate(SetThemes.Dragonlance(), releaseBase.AddMonths(4)),
            CardSetGenerator.Generate(SetThemes.Planescape(), releaseBase.AddMonths(5)),
            CardSetGenerator.Generate(SetThemes.Eberron(), releaseBase.AddMonths(6)),
            CardSetGenerator.Generate(SetThemes.Greyhawk(), releaseBase.AddMonths(7)),
            CardSetGenerator.Generate(SetThemes.Spelljammer(), releaseBase.AddMonths(8)),
            CardSetGenerator.Generate(SetThemes.DarkSun(), releaseBase.AddMonths(9)),
        ];
    }

    public static CardSet CreateDND1CoreSet()
    {
        var set = new CardSet(
            DND1SetId,
            "Dungeons & Dragons: Core Set",
            "DND1",
            new DateOnly(2026, 4, 1),
            "The foundational set featuring iconic heroes, monsters, equipment, and dungeons from the world of Dungeons & Dragons.");

        var allies = CreateAllies();
        var bards = CreateBards();
        var equipment = CreateEquipment();
        var consumables = CreateConsumables();
        var monsters = CreateMonsters();
        var traps = CreateTraps();
        var rooms = CreateDungeonRooms();
        var bosses = CreateBosses();

        foreach (var card in allies) set.AddCard(card);
        foreach (var card in bards) set.AddCard(card);
        foreach (var card in equipment) set.AddCard(card);
        foreach (var card in consumables) set.AddCard(card);
        foreach (var card in monsters) set.AddCard(card);
        foreach (var card in traps) set.AddCard(card);
        foreach (var card in rooms) set.AddCard(card);
        foreach (var card in bosses) set.AddCard(card);

        return set;
    }
}
