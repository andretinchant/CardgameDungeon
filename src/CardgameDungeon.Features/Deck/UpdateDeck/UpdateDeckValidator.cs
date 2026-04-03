using CardgameDungeon.Domain.Entities;
using FluentValidation;

namespace CardgameDungeon.Features.Deck.UpdateDeck;

public class UpdateDeckValidator : AbstractValidator<UpdateDeckCommand>
{
    public UpdateDeckValidator()
    {
        RuleFor(x => x.DeckId)
            .NotEmpty();

        RuleFor(x => x.AdventurerCardIds)
            .NotNull()
            .Must(ids => ids.Count == DeckList.RequiredAdventurerCards)
            .WithMessage($"Adventurer deck must have exactly {DeckList.RequiredAdventurerCards} cards.");

        RuleFor(x => x.EnemyCardIds)
            .NotNull()
            .Must(ids => ids.Count == DeckList.RequiredEnemyCards)
            .WithMessage($"Enemy deck must have exactly {DeckList.RequiredEnemyCards} cards.");

        RuleFor(x => x.DungeonRoomIds)
            .NotNull()
            .Must(ids => ids.Count == DeckList.RequiredDungeonRooms)
            .WithMessage($"Dungeon must have exactly {DeckList.RequiredDungeonRooms} rooms.");

        RuleFor(x => x.BossCardId)
            .NotEmpty();
    }
}
