using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.Infrastructure.Data.Persistence;

internal static class MatchStateMapper
{
    public static MatchStateDto ToDto(MatchState match)
    {
        return new MatchStateDto
        {
            Id = match.Id,
            Player1 = ToPlayerDto(match.Player1),
            Player2 = ToPlayerDto(match.Player2),
            Phase = (int)match.Phase,
            CurrentRoom = GetCurrentRoom(match),
            InitiativeWinnerId = match.InitiativeWinnerId,
            AttackerId = match.AttackerId,
            WinnerId = match.WinnerId,
            DungeonRooms = match.DungeonRooms.Select(ToCardDto).ToList(),
            Boss = ToCardDto(match.Boss),
            SetupSubmitted = GetSetupSubmitted(match),
            Player1PendingTeam = GetPendingTeam(match, match.Player1.PlayerId),
            Player2PendingTeam = GetPendingTeam(match, match.Player2.PlayerId),
            Player1BetTotal = match.Player1BetTotal,
            Player2BetTotal = match.Player2BetTotal,
            CombatBoard = new CombatBoardDto
            {
                Assignments = match.CombatBoard.Assignments
                    .Select(a => new CombatAssignmentDto { AttackerId = a.AttackerId, DefenderId = a.DefenderId })
                    .ToList(),
                OpportunityAttacksUsed = match.CombatBoard.OpportunityAttacksUsed.ToList()
            }
        };
    }

    public static MatchState FromDto(MatchStateDto dto)
    {
        var player1Deck = dto.Player1.Deck.Select(FromCardDto).ToList();
        var player2Deck = dto.Player2.Deck.Select(FromCardDto).ToList();

        var player1 = new PlayerState(dto.Player1.PlayerId, dto.Player1.HitPoints, player1Deck);
        var player2 = new PlayerState(dto.Player2.PlayerId, dto.Player2.HitPoints, player2Deck);

        // Restore hand cards
        RestoreCardZone(player1, dto.Player1.Hand, (ps, c) => AddToHand(ps, c));
        RestoreCardZone(player2, dto.Player2.Hand, (ps, c) => AddToHand(ps, c));

        // Restore discard
        RestoreCardZone(player1, dto.Player1.Discard, (ps, c) => AddToDiscard(ps, c));
        RestoreCardZone(player2, dto.Player2.Discard, (ps, c) => AddToDiscard(ps, c));

        // Restore exile
        RestoreCardZone(player1, dto.Player1.Exile, (ps, c) => AddToExile(ps, c));
        RestoreCardZone(player2, dto.Player2.Exile, (ps, c) => AddToExile(ps, c));

        // Restore allies in play
        foreach (var allyDto in dto.Player1.AlliesInPlay)
        {
            var ally = (AllyCard)FromCardDto(allyDto);
            AddToAlliesInPlay(player1, ally);
        }
        foreach (var allyDto in dto.Player2.AlliesInPlay)
        {
            var ally = (AllyCard)FromCardDto(allyDto);
            AddToAlliesInPlay(player2, ally);
        }

        var dungeonRooms = dto.DungeonRooms.Select(d => (DungeonRoomCard)FromCardDto(d));
        var boss = (BossCard)FromCardDto(dto.Boss);

        var match = new MatchState(Guid.Empty, player1, player2, dungeonRooms, boss);

        // Use reflection to restore internal state that constructors don't accept
        var type = typeof(MatchState);
        SetPrivateField(match, "Id", dto.Id);  // Actually a property
        SetPrivateProperty(match, nameof(MatchState.Phase), (MatchPhase)dto.Phase);
        SetPrivateField(match, "CurrentRoom", dto.CurrentRoom);
        SetPrivateProperty(match, nameof(MatchState.InitiativeWinnerId), dto.InitiativeWinnerId);
        SetPrivateProperty(match, nameof(MatchState.AttackerId), dto.AttackerId);
        SetPrivateProperty(match, nameof(MatchState.WinnerId), dto.WinnerId);
        SetPrivateProperty(match, nameof(MatchState.Player1BetTotal), dto.Player1BetTotal);
        SetPrivateProperty(match, nameof(MatchState.Player2BetTotal), dto.Player2BetTotal);

        // Restore setup submitted
        var setupField = type.GetField("_setupSubmitted",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        var setupSet = (HashSet<Guid>)setupField.GetValue(match)!;
        foreach (var id in dto.SetupSubmitted)
            setupSet.Add(id);

        // Restore pending teams
        if (dto.Player1PendingTeam is not null)
        {
            var field = type.GetField("_player1PendingTeam",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
            field.SetValue(match, dto.Player1PendingTeam.Select(c => (AllyCard)FromCardDto(c)).ToList());
        }
        if (dto.Player2PendingTeam is not null)
        {
            var field = type.GetField("_player2PendingTeam",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
            field.SetValue(match, dto.Player2PendingTeam.Select(c => (AllyCard)FromCardDto(c)).ToList());
        }

        // Restore combat board
        foreach (var assignment in dto.CombatBoard.Assignments)
        {
            var boardType = match.CombatBoard.GetType();
            var assignmentsField = boardType.GetField("_assignments",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
            var list = (List<Domain.ValueObjects.CombatAssignment>)assignmentsField.GetValue(match.CombatBoard)!;
            list.Add(new Domain.ValueObjects.CombatAssignment(assignment.AttackerId, assignment.DefenderId));
        }
        foreach (var playerId in dto.CombatBoard.OpportunityAttacksUsed)
        {
            var boardType = match.CombatBoard.GetType();
            var oaField = boardType.GetField("_opportunityAttacksUsed",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
            var set = (HashSet<Guid>)oaField.GetValue(match.CombatBoard)!;
            set.Add(playerId);
        }

        return match;
    }

    private static PlayerStateDto ToPlayerDto(PlayerState ps) => new()
    {
        PlayerId = ps.PlayerId,
        HitPoints = ps.HitPoints,
        Deck = ps.Deck.Select(ToCardDto).ToList(),
        Hand = ps.Hand.Select(ToCardDto).ToList(),
        Discard = ps.Discard.Select(ToCardDto).ToList(),
        Exile = ps.Exile.Select(ToCardDto).ToList(),
        AlliesInPlay = ps.AlliesInPlay.Select(ToCardDto).ToList()
    };

    private static CardDto ToCardDto(Card card)
    {
        var dto = new CardDto
        {
            CardType = card.Type.ToString(),
            Id = card.Id,
            Name = card.Name,
            Rarity = (int)card.Rarity,
            Cost = card.Cost,
            EffectTags = card.EffectTags
        };

        switch (card)
        {
            case AllyCard a:
                dto.AllyClass = (int)a.Class;
                dto.Race = (int)a.Race;
                dto.Strength = a.Strength;
                dto.HitPoints = a.HitPoints;
                dto.Initiative = a.Initiative;
                dto.Treasure = a.Treasure;
                dto.IsAmbusher = a.IsAmbusher;
                dto.Effect = a.Effect;
                break;
            case EquipmentCard e:
                dto.EquipmentSlot = (int)e.Slot;
                dto.StrengthModifier = e.StrengthModifier;
                dto.HitPointsModifier = e.HitPointsModifier;
                dto.InitiativeModifier = e.InitiativeModifier;
                dto.Effect = e.Effect;
                break;
            case MonsterCard m:
                dto.Race = (int)m.Race;
                dto.Strength = m.Strength;
                dto.HitPoints = m.HitPoints;
                dto.Initiative = m.Initiative;
                dto.Treasure = m.Treasure;
                dto.Effect = m.Effect;
                break;
            case TrapCard t:
                dto.Damage = t.Damage;
                dto.Effect = t.Effect;
                break;
            case DungeonRoomCard d:
                dto.Order = d.Order;
                dto.MonsterCostBudget = d.MonsterCostBudget;
                dto.Effect = d.Effect;
                dto.MonsterIds = d.MonsterIds.ToList();
                dto.TrapIds = d.TrapIds.ToList();
                break;
            case BossCard b:
                dto.Race = (int)b.Race;
                dto.Strength = b.Strength;
                dto.HitPoints = b.HitPoints;
                dto.Initiative = b.Initiative;
                dto.Effect = b.Effect;
                break;
        }

        return dto;
    }

    internal static Card FromCardDto(CardDto dto)
    {
        var rarity = (Rarity)dto.Rarity;

        return dto.CardType switch
        {
            "Ally" => new AllyCard(dto.Id, dto.Name, rarity, dto.Cost,
                dto.Strength ?? 0, dto.HitPoints ?? 1, dto.Initiative ?? 0,
                dto.IsAmbusher ?? false, dto.Treasure ?? 0, dto.Effect,
                (Race)(dto.Race ?? 0), (AllyClass)(dto.AllyClass ?? 0),
                dto.EffectTags),

            "Equipment" => new EquipmentCard(dto.Id, dto.Name, rarity, dto.Cost,
                dto.StrengthModifier ?? 0, dto.HitPointsModifier ?? 0, dto.InitiativeModifier ?? 0,
                (EquipmentSlot)(dto.EquipmentSlot ?? 5), dto.Effect,
                dto.EffectTags),

            "Monster" => new MonsterCard(dto.Id, dto.Name, rarity, dto.Cost,
                dto.Strength ?? 0, dto.HitPoints ?? 1, dto.Initiative ?? 0,
                dto.Treasure ?? 0, dto.Effect,
                (Race)(dto.Race ?? 10), dto.EffectTags),

            "Trap" => new TrapCard(dto.Id, dto.Name, rarity, dto.Cost,
                dto.Damage ?? 0, dto.Effect ?? "", dto.EffectTags),

            "DungeonRoom" => new DungeonRoomCard(dto.Id, dto.Name, rarity,
                dto.Order ?? 1, dto.MonsterIds, dto.TrapIds,
                dto.MonsterCostBudget ?? 0, dto.Effect, dto.EffectTags),

            "Boss" => new BossCard(dto.Id, dto.Name, rarity, dto.Cost,
                dto.Strength ?? 1, dto.HitPoints ?? 1, dto.Initiative ?? 0, dto.Effect,
                (Race)(dto.Race ?? 15), dto.EffectTags),

            _ => throw new InvalidOperationException($"Unknown card type: {dto.CardType}")
        };
    }

    private static int GetCurrentRoom(MatchState match)
    {
        // Access via reflection since CurrentRoom has private set
        var prop = typeof(MatchState).GetProperty(nameof(MatchState.CurrentRoom))!;
        return (int)prop.GetValue(match)!;
    }

    private static List<Guid> GetSetupSubmitted(MatchState match)
    {
        var field = typeof(MatchState).GetField("_setupSubmitted",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        return ((HashSet<Guid>)field.GetValue(match)!).ToList();
    }

    private static List<CardDto>? GetPendingTeam(MatchState match, Guid playerId)
    {
        var fieldName = match.Player1.PlayerId == playerId
            ? "_player1PendingTeam"
            : "_player2PendingTeam";
        var field = typeof(MatchState).GetField(fieldName,
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        var team = field.GetValue(match) as List<AllyCard>;
        return team?.Select(ToCardDto).ToList();
    }

    private static void SetPrivateProperty(object obj, string name, object? value)
    {
        var prop = obj.GetType().GetProperty(name,
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        prop.SetValue(obj, value);
    }

    private static void SetPrivateField(object obj, string name, object? value)
    {
        // Try property first (many are auto-properties)
        var prop = obj.GetType().GetProperty(name,
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (prop != null)
        {
            prop.SetValue(obj, value);
            return;
        }
        var field = obj.GetType().GetField(name,
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        field.SetValue(obj, value);
    }

    private static void AddToHand(PlayerState ps, Card card)
    {
        var field = typeof(PlayerState).GetField("_hand",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        ((List<Card>)field.GetValue(ps)!).Add(card);
    }

    private static void AddToDiscard(PlayerState ps, Card card)
    {
        var field = typeof(PlayerState).GetField("_discard",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        ((List<Card>)field.GetValue(ps)!).Add(card);
    }

    private static void AddToExile(PlayerState ps, Card card)
    {
        var field = typeof(PlayerState).GetField("_exile",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        ((List<Card>)field.GetValue(ps)!).Add(card);
    }

    private static void AddToAlliesInPlay(PlayerState ps, AllyCard ally)
    {
        var field = typeof(PlayerState).GetField("_alliesInPlay",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        ((List<AllyCard>)field.GetValue(ps)!).Add(ally);
    }

    private static void RestoreCardZone(PlayerState ps, List<CardDto> cards, Action<PlayerState, Card> addAction)
    {
        foreach (var cardDto in cards)
            addAction(ps, FromCardDto(cardDto));
    }
}
