using CardgameDungeon.Features.Deck.UpdateDeck;
using CardgameDungeon.Tests.Deck.Fakes;

namespace CardgameDungeon.Tests.Deck;

public class UpdateDeckHandlerTests
{
    private readonly FakeCardRepository _cardRepo = new();
    private readonly FakeDeckRepository _deckRepo = new();

    private UpdateDeckHandler Handler => new(_cardRepo, _deckRepo);

    [Fact]
    public async Task ValidUpdate_ReturnsUpdatedDeck()
    {
        var existingDeck = DeckTestHelper.MakeValidDeck();
        _deckRepo.Seed(existingDeck);

        var (newAdventurers, newEnemies, newRooms, newBoss) = DeckTestHelper.MakeFullDeckCards();
        DeckTestHelper.SeedCards(_cardRepo, newAdventurers, newEnemies, newRooms, newBoss);

        var command = new UpdateDeckCommand(
            existingDeck.Id,
            newAdventurers.Select(c => c.Id).ToList(),
            newEnemies.Select(c => c.Id).ToList(),
            newRooms.Select(r => r.Id).ToList(),
            newBoss.Id);

        var response = await Handler.Handle(command, CancellationToken.None);

        Assert.Equal(existingDeck.Id, response.Id);
        Assert.Equal(existingDeck.PlayerId, response.PlayerId);
        Assert.Equal("TheBoss", response.Boss.Name);
        Assert.NotNull(_deckRepo.LastUpdated);
    }

    [Fact]
    public async Task DeckNotFound_Throws()
    {
        var (adventurers, enemies, rooms, boss) = DeckTestHelper.MakeFullDeckCards();
        DeckTestHelper.SeedCards(_cardRepo, adventurers, enemies, rooms, boss);

        var command = new UpdateDeckCommand(
            Guid.NewGuid(),
            adventurers.Select(c => c.Id).ToList(),
            enemies.Select(c => c.Id).ToList(),
            rooms.Select(r => r.Id).ToList(),
            boss.Id);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => Handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task InvalidUpdate_ThrowsDomainException()
    {
        var existingDeck = DeckTestHelper.MakeValidDeck();
        _deckRepo.Seed(existingDeck);

        var (newAdventurers, newEnemies, newRooms, newBoss) = DeckTestHelper.MakeFullDeckCards();
        newAdventurers.RemoveRange(0, 5); // Only 35 adventurers
        DeckTestHelper.SeedCards(_cardRepo, newAdventurers, newEnemies, newRooms, newBoss);

        var command = new UpdateDeckCommand(
            existingDeck.Id,
            newAdventurers.Select(c => c.Id).ToList(),
            newEnemies.Select(c => c.Id).ToList(),
            newRooms.Select(r => r.Id).ToList(),
            newBoss.Id);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => Handler.Handle(command, CancellationToken.None));
    }
}
