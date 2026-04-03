namespace CardgameDungeon.Infrastructure.Data.Persistence;

internal class MatchStateDto
{
    public Guid Id { get; set; }
    public PlayerStateDto Player1 { get; set; } = null!;
    public PlayerStateDto Player2 { get; set; } = null!;
    public int Phase { get; set; }
    public int CurrentRoom { get; set; }
    public Guid? InitiativeWinnerId { get; set; }
    public Guid? WinnerId { get; set; }
    public List<CardDto> DungeonRooms { get; set; } = [];
    public CardDto Boss { get; set; } = null!;
    public List<Guid> SetupSubmitted { get; set; } = [];
    public List<CardDto>? Player1PendingTeam { get; set; }
    public List<CardDto>? Player2PendingTeam { get; set; }
    public int Player1BetTotal { get; set; }
    public int Player2BetTotal { get; set; }
    public CombatBoardDto CombatBoard { get; set; } = new();
}

internal class PlayerStateDto
{
    public Guid PlayerId { get; set; }
    public int HitPoints { get; set; }
    public List<CardDto> Deck { get; set; } = [];
    public List<CardDto> Hand { get; set; } = [];
    public List<CardDto> Discard { get; set; } = [];
    public List<CardDto> Exile { get; set; } = [];
    public List<CardDto> AlliesInPlay { get; set; } = [];
}

internal class CombatBoardDto
{
    public List<CombatAssignmentDto> Assignments { get; set; } = [];
    public List<Guid> OpportunityAttacksUsed { get; set; } = [];
}

internal class CombatAssignmentDto
{
    public Guid AttackerId { get; set; }
    public Guid DefenderId { get; set; }
}

internal class CardDto
{
    public string CardType { get; set; } = null!;
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int Rarity { get; set; }
    public int Cost { get; set; }
    // Ally / Monster / Boss
    public int? Strength { get; set; }
    public int? HitPoints { get; set; }
    public int? Initiative { get; set; }
    public int? Treasure { get; set; }
    public bool? IsAmbusher { get; set; }
    public string? Effect { get; set; }
    // Equipment
    public int? StrengthModifier { get; set; }
    public int? HitPointsModifier { get; set; }
    public int? InitiativeModifier { get; set; }
    // Trap
    public int? Damage { get; set; }
    // DungeonRoom
    public int? Order { get; set; }
    public int? MonsterCostBudget { get; set; }
    public List<Guid>? MonsterIds { get; set; }
    public List<Guid>? TrapIds { get; set; }
}
