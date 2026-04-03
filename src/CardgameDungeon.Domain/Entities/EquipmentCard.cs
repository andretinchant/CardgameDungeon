using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.Domain.Entities;

public class EquipmentCard : Card
{
    public override CardType Type => CardType.Equipment;
    public int StrengthModifier { get; private set; }
    public int HitPointsModifier { get; private set; }
    public int InitiativeModifier { get; private set; }

    private EquipmentCard() { } // EF Core

    public EquipmentCard(
        Guid id,
        string name,
        Rarity rarity,
        int cost,
        int strengthModifier,
        int hitPointsModifier,
        int initiativeModifier)
        : base(id, name, rarity, cost)
    {
        StrengthModifier = strengthModifier;
        HitPointsModifier = hitPointsModifier;
        InitiativeModifier = initiativeModifier;
    }
}
