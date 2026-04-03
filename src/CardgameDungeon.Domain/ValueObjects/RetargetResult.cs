namespace CardgameDungeon.Domain.ValueObjects;

public record RetargetResult(
    Guid AllyId,
    int PrimaryDamageContribution,
    int SecondaryDamageContribution,
    int CostPaid);
