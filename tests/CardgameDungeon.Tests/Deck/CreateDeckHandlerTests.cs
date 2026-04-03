using CardgameDungeon.Features.Deck.CreateDeck;
using CardgameDungeon.Tests.Deck.Fakes;

namespace CardgameDungeon.Tests.Deck;

public class CreateDeckHandlerTests
{
    private readonly FakeCardRepository _cardRepo = new();
    private readonly FakeDeckRepository _deckRepo = new();

    private CreateDeckHandler Handler => new(_cardRepo, _deckRepo);

    [Fact]
    public async Task ValidDeck_CreatesAndReturnsDeck()
    {
        var (adventurers, enemies, rooms, boss) = DeckTestHelper.MakeFullDeckCards();
        DeckTestHelper.SeedCards(_cardRepo, adventurers, enemies, rooms, boss);

        var command = new CreateDeckCommand(
            Guid.NewGuid(),
            adventurers.Select(c => c.Id).ToList(),
            enemies.Select(c => c.Id).ToList(),
            rooms.Select(r => r.Id).ToList(),
            boss.Id);

        var response = await Handler.Handle(command, CancellationToken.None);

        Assert.Equal(40, response.AdventurerCards.Count);
        Assert.Equal(40, response.EnemyCards.Count);
        Assert.Equal(5, response.DungeonRooms.Count);
        Assert.Equal("TheBoss", response.Boss.Name);
        Assert.NotNull(_deckRepo.LastSaved);
    }

    [Fact]
    public async Task InvalidComposition_ThrowsDomainException()
    {
        var (adventurers, enemies, rooms, boss) = DeckTestHelper.MakeFullDeckCards();
        adventurers.RemoveAt(0); // Only 39 adventurers
        DeckTestHelper.SeedCards(_cardRepo, adventurers, enemies, rooms, boss);

        var command = new CreateDeckCommand(
            Guid.NewGuid(),
            adventurers.Select(c => c.Id).ToList(),
            enemies.Select(c => c.Id).ToList(),
            rooms.Select(r => r.Id).ToList(),
            boss.Id);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => Handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task BossNotFound_Throws()
    {
        var (adventurers, enemies, rooms, _) = DeckTestHelper.MakeFullDeckCards();
        DeckTestHelper.SeedCards(_cardRepo, adventurers, enemies, rooms, DeckTestHelper.MakeBoss());

        var command = new CreateDeckCommand(
            Guid.NewGuid(),
            adventurers.Select(c => c.Id).ToList(),
            enemies.Select(c => c.Id).ToList(),
            rooms.Select(r => r.Id).ToList(),
            Guid.NewGuid()); // Non-existent boss

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => Handler.Handle(command, CancellationToken.None));
    }
}
