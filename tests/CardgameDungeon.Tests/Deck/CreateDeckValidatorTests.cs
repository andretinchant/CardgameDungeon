using CardgameDungeon.Features.Deck.CreateDeck;

namespace CardgameDungeon.Tests.Deck;

public class CreateDeckValidatorTests
{
    private readonly CreateDeckValidator _validator = new();

    private static CreateDeckCommand ValidCommand() => new(
        Guid.NewGuid(),
        Enumerable.Range(0, 40).Select(_ => Guid.NewGuid()).ToList(),
        Enumerable.Range(0, 40).Select(_ => Guid.NewGuid()).ToList(),
        Enumerable.Range(0, 5).Select(_ => Guid.NewGuid()).ToList(),
        Guid.NewGuid());

    [Fact]
    public void ValidCommand_Passes()
    {
        var result = _validator.Validate(ValidCommand());
        Assert.True(result.IsValid);
    }

    [Fact]
    public void EmptyPlayerId_Fails()
    {
        var result = _validator.Validate(ValidCommand() with { PlayerId = Guid.Empty });
        Assert.False(result.IsValid);
    }

    [Fact]
    public void WrongAdventurerCount_Fails()
    {
        var cmd = ValidCommand() with
        {
            AdventurerCardIds = Enumerable.Range(0, 35).Select(_ => Guid.NewGuid()).ToList()
        };
        var result = _validator.Validate(cmd);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("40"));
    }

    [Fact]
    public void WrongEnemyCount_Fails()
    {
        var cmd = ValidCommand() with
        {
            EnemyCardIds = Enumerable.Range(0, 30).Select(_ => Guid.NewGuid()).ToList()
        };
        var result = _validator.Validate(cmd);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("40"));
    }

    [Fact]
    public void WrongRoomCount_Fails()
    {
        var cmd = ValidCommand() with
        {
            DungeonRoomIds = Enumerable.Range(0, 3).Select(_ => Guid.NewGuid()).ToList()
        };
        var result = _validator.Validate(cmd);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("5"));
    }

    [Fact]
    public void EmptyBossId_Fails()
    {
        var result = _validator.Validate(ValidCommand() with { BossCardId = Guid.Empty });
        Assert.False(result.IsValid);
    }
}
