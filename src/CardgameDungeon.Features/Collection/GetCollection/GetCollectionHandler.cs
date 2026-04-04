using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Repositories;
using MediatR;

namespace CardgameDungeon.Features.Collection.GetCollection;

public class GetCollectionHandler(
    ICollectionRepository collectionRepo,
    ICardRepository cardRepo)
    : IRequestHandler<GetCollectionQuery, CollectionResponse>
{
    public async Task<CollectionResponse> Handle(GetCollectionQuery request, CancellationToken ct)
    {
        var collection = await collectionRepo.GetByPlayerIdAsync(request.PlayerId, ct)
            ?? throw new KeyNotFoundException($"Collection for player {request.PlayerId} not found.");

        var cardIds = collection.Cards
            .Select(c => c.CardId)
            .Distinct()
            .ToList();

        var cardsById = (await cardRepo.GetByIdsAsync(cardIds, ct))
            .ToDictionary(c => c.Id);

        var cards = collection.Cards.Select(c =>
        {
            cardsById.TryGetValue(c.CardId, out var card);
            return ToOwnedCardDto(c, card);
        }).ToList();

        return new CollectionResponse(
            collection.PlayerId,
            cards,
            cards.Count,
            cards.Count(c => !c.IsReserved));
    }

    private static OwnedCardDto ToOwnedCardDto(OwnedCard owned, Card? card)
    {
        if (card is null)
        {
            return new OwnedCardDto(
                owned.Id,
                owned.CardId,
                owned.IsReserved,
                "Unknown Card",
                "Unknown",
                "Unknown",
                0,
                "Card metadata not found.");
        }

        return new OwnedCardDto(
            owned.Id,
            owned.CardId,
            owned.IsReserved,
            card.Name,
            card.Type.ToString(),
            card.Rarity.ToString(),
            card.Cost,
            BuildDetailText(card));
    }

    private static string BuildDetailText(Card card)
        => card switch
        {
            AllyCard ally =>
                $"STR {ally.Strength} | HP {ally.HitPoints} | INIT {ally.Initiative} | Treasure {ally.Treasure}" +
                $"{(ally.IsAmbusher ? " | Ambusher" : string.Empty)}" +
                $"{(string.IsNullOrWhiteSpace(ally.Effect) ? string.Empty : $" | {ally.Effect}")}",

            MonsterCard monster =>
                $"STR {monster.Strength} | HP {monster.HitPoints} | INIT {monster.Initiative} | Treasure {monster.Treasure}" +
                $"{(string.IsNullOrWhiteSpace(monster.Effect) ? string.Empty : $" | {monster.Effect}")}",

            BossCard boss =>
                $"STR {boss.Strength} | HP {boss.HitPoints} | INIT {boss.Initiative}" +
                $"{(string.IsNullOrWhiteSpace(boss.Effect) ? string.Empty : $" | {boss.Effect}")}",

            EquipmentCard equipment =>
                $"Slot {equipment.Slot} | STR {Signed(equipment.StrengthModifier)} | HP {Signed(equipment.HitPointsModifier)} | INIT {Signed(equipment.InitiativeModifier)}" +
                $"{(string.IsNullOrWhiteSpace(equipment.Effect) ? string.Empty : $" | {equipment.Effect}")}",

            TrapCard trap =>
                $"Damage {trap.Damage}" +
                $"{(string.IsNullOrWhiteSpace(trap.Effect) ? string.Empty : $" | {trap.Effect}")}",

            DungeonRoomCard room =>
                $"Room {room.Order} | Budget {room.MonsterCostBudget}" +
                $"{(string.IsNullOrWhiteSpace(room.Effect) ? string.Empty : $" | {room.Effect}")}",

            _ => string.Empty
        };

    private static string Signed(int value) => value >= 0 ? $"+{value}" : value.ToString();
}
