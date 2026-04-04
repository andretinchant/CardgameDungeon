namespace CardgameDungeon.Domain.Effects;

/// <summary>
/// Parses effect tag strings into structured EffectTag objects.
/// Format: lines separated by newline, tokens separated by |
/// Example:
///   "ON_ACTIVATE|COST:EXILE_DECK:4|+STR:2|ELIM_DOUBLE|ONCE_PER_COMBAT"
/// </summary>
public static class EffectParser
{
    public static IReadOnlyList<EffectTag> Parse(string? effectTags)
    {
        if (string.IsNullOrWhiteSpace(effectTags))
            return [];

        var lines = effectTags.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var tags = new List<EffectTag>();

        foreach (var line in lines)
        {
            var tag = ParseLine(line.Trim());
            if (tag is not null)
                tags.Add(tag);
        }

        return tags;
    }

    private static EffectTag? ParseLine(string line)
    {
        var tokens = line.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (tokens.Length == 0) return null;

        var trigger = ParseTrigger(tokens[0]);
        EffectCost? cost = null;
        var condition = EffectCondition.None;
        string? conditionParam = null;
        var actions = new List<EffectActionEntry>();

        for (var i = 1; i < tokens.Length; i++)
        {
            var token = tokens[i];

            if (token.StartsWith("COST:"))
            {
                cost = ParseCost(token);
            }
            else if (token.StartsWith("IF_") || token.StartsWith("ONCE_"))
            {
                (condition, conditionParam) = ParseCondition(token);
            }
            else
            {
                var action = ParseAction(token);
                if (action is not null)
                    actions.Add(action);
            }
        }

        return new EffectTag
        {
            Trigger = trigger,
            Cost = cost,
            Condition = condition,
            ConditionParam = conditionParam,
            Actions = actions
        };
    }

    private static EffectTrigger ParseTrigger(string token) => token switch
    {
        "PASSIVE" => EffectTrigger.Passive,
        "ON_PLAY" => EffectTrigger.OnPlay,
        "ON_COMBAT_START" => EffectTrigger.OnCombatStart,
        "ON_ROUND_START" => EffectTrigger.OnRoundStart,
        "ON_KILL" => EffectTrigger.OnKill,
        "ON_DEATH" => EffectTrigger.OnDeath,
        "ON_INITIATIVE" => EffectTrigger.OnInitiative,
        "ON_ACTIVATE" => EffectTrigger.OnActivate,
        "ON_SCROLL_USED" => EffectTrigger.OnScrollUsed,
        "ON_ENEMY_FLEE" => EffectTrigger.OnEnemyFlee,
        "ON_MARKED_KILL" => EffectTrigger.OnMarkedKill,
        "ON_MARKED_SURVIVE" => EffectTrigger.OnMarkedSurvive,
        "WITH_ADVANTAGE" => EffectTrigger.WithAdvantage,
        "WITH_DISADVANTAGE" => EffectTrigger.WithDisadvantage,
        "ON_ALLY_DEATH" => EffectTrigger.OnAllyDeath,
        "ON_ROOM_ADVANCE" => EffectTrigger.OnRoomAdvance,
        "ON_TRAP_TRIGGER" => EffectTrigger.OnTrapTrigger,
        "ON_EQUIPMENT_DESTROYED" => EffectTrigger.OnEquipmentDestroyed,
        _ => EffectTrigger.Passive
    };

    private static EffectCost? ParseCost(string token)
    {
        // COST:EXILE_DECK:4
        var parts = token.Split(':');
        if (parts.Length < 3) return null;

        var type = parts[1] switch
        {
            "EXILE_DECK" => EffectCostType.ExileDeck,
            "EXILE_HAND" => EffectCostType.ExileHand,
            "DISCARD_HAND" => EffectCostType.DiscardHand,
            "DISCARD_DECK" => EffectCostType.DiscardDeck,
            "HP" => EffectCostType.Hp,
            _ => EffectCostType.DiscardHand
        };

        int.TryParse(parts[2], out var amount);

        return new EffectCost { Type = type, Amount = amount };
    }

    private static (EffectCondition condition, string? param) ParseCondition(string token)
    {
        var parts = token.Split(':');

        return parts[0] switch
        {
            "IF_RACE" => (EffectCondition.IfRace, parts.Length > 1 ? parts[1] : null),
            "IF_CLASS" => (EffectCondition.IfClass, parts.Length > 1 ? parts[1] : null),
            "IF_RAGING" => (EffectCondition.IfRaging, null),
            "IF_EQUIPPED" => (EffectCondition.IfEquipped, parts.Length > 1 ? parts[1] : null),
            "IF_SCROLL_USED" => (EffectCondition.IfScrollUsed, null),
            "ONCE_PER_COMBAT" => (EffectCondition.OncePerCombat, null),
            "ONCE_PER_ROOM" => (EffectCondition.OncePerRoom, null),
            "IF_NO_DAMAGE" => (EffectCondition.IfNoDamage, null),
            "IF_EXILED_GT_8" or "IF_EXILED_GT" => (EffectCondition.IfExiledGt, parts.Length > 1 ? parts[1] : "8"),
            "IF_HAND_LT_3" or "IF_HAND_LT" => (EffectCondition.IfHandLt, parts.Length > 1 ? parts[1] : "3"),
            "IF_NO_BOOTS" => (EffectCondition.IfNoBoots, null),
            "IF_NO_ARMOR" => (EffectCondition.IfNoArmor, null),
            "IF_NO_POTION_BALM" => (EffectCondition.IfNoPotionBalm, null),
            "IF_INIT_LT_2" or "IF_INIT_LT" => (EffectCondition.IfInitLt, parts.Length > 1 ? parts[1] : "2"),
            "IF_COST_1" => (EffectCondition.IfCost1, null),
            "IF_EVERY_2_ROUNDS" or "IF_EVERY_X_ROUNDS" => (EffectCondition.IfEveryXRounds, parts.Length > 1 ? parts[1] : "2"),
            _ => (EffectCondition.None, null)
        };
    }

    private static EffectActionEntry? ParseAction(string token)
    {
        // +STR:2 / -STR:1 / +HP:3 / +INIT:2
        if (token.StartsWith("+STR:") || token.StartsWith("-STR:"))
            return new EffectActionEntry { Action = EffectAction.ModStr, Value = ParseSignedInt(token[4..]) };
        if (token.StartsWith("+HP:") || token.StartsWith("-HP:"))
            return new EffectActionEntry { Action = EffectAction.ModHp, Value = ParseSignedInt(token[3..]) };
        if (token.StartsWith("+INIT:") || token.StartsWith("-INIT:"))
            return new EffectActionEntry { Action = EffectAction.ModInit, Value = ParseSignedInt(token[5..]) };

        var parts = token.Split(':');
        var key = parts[0];
        var val = parts.Length > 1 ? parts[1] : "";
        var val2 = parts.Length > 2 ? parts[2] : "";
        var fullParam = parts.Length > 1 ? string.Join(":", parts[1..]) : "";

        return key switch
        {
            "DAMAGE" => new() { Action = EffectAction.Damage, Value = ParseInt(val), Target = ParseTarget(val2) },
            "HEAL" => new() { Action = EffectAction.Heal, Value = ParseInt(val), Target = ParseTarget(val2) },
            "EXILE_DECK" => new() { Action = EffectAction.ExileDeck, Value = ParseInt(val) },
            "EXILE_HAND" => new() { Action = EffectAction.ExileHand, Value = ParseInt(val) },
            "DISCARD_HAND" => new() { Action = EffectAction.DiscardHand, Value = ParseInt(val) },
            "DRAW" => new() { Action = EffectAction.Draw, Value = ParseInt(val) },
            "FORFEIT_TREASURE" => new() { Action = EffectAction.ForfeitTreasure },
            "ELIM_DOUBLE" => new() { Action = EffectAction.ElimDouble },
            "JOIN_COMBAT" => new() { Action = EffectAction.JoinCombat },
            "CANCEL_COMBAT" => new() { Action = EffectAction.CancelCombat },
            "REDIRECT_DAMAGE" => new() { Action = EffectAction.RedirectDamage, Target = ParseTarget(val) },
            "MARK_ENEMY" => new() { Action = EffectAction.MarkEnemy, Value = ParseInt(val) },
            "TRIGGER_OPP_ATTACK" => new() { Action = EffectAction.TriggerOppAttack, Target = ParseTarget(val) },
            "SEARCH_DECK" => new() { Action = EffectAction.SearchDeck, Param = val },
            "REVEAL_HAND" => new() { Action = EffectAction.RevealHand },
            "REVEAL_DECK" => new() { Action = EffectAction.RevealDeck, Value = ParseInt(val) },
            "FAVORED_ENEMY" => new() { Action = EffectAction.FavoredEnemy, Param = val },
            "COPY_SCROLL" => new() { Action = EffectAction.CopyScroll },
            "RECOVER_SCROLL" => new() { Action = EffectAction.RecoverScroll, Param = val },
            "RECOVER_SCROLL_EXILE" => new() { Action = EffectAction.RecoverScrollFromExile, Value = ParseInt(val) },
            "SCROLL_TO_BOTTOM" => new() { Action = EffectAction.ScrollToBottom },
            "REDUCE_DAMAGE" => new() { Action = EffectAction.ReduceDamage, Value = ParseInt(val) },
            "OPP_ATTACK_DOUBLE" => new() { Action = EffectAction.OppAttackDouble },
            "IGNORE_OPP_ATTACK_LIMIT" => new() { Action = EffectAction.IgnoreOppAttackLimit },
            "RETURN_HAND_TOP" => new() { Action = EffectAction.ReturnHandTop, Value = ParseInt(val) },
            "RETURN_HAND_BOTTOM" => new() { Action = EffectAction.ReturnHandBottom, Value = ParseInt(val) },
            "RETURN_HAND_SHUFFLE" => new() { Action = EffectAction.ReturnHandShuffle, Value = ParseInt(val) },
            "MATERIALIZE_ALLY" => new() { Action = EffectAction.MaterializeAlly, Param = fullParam },
            "REQUIRE_CLASS" => new() { Action = EffectAction.RequireClass, Param = val },
            "IGNORE_ALLY_LIMIT" => new() { Action = EffectAction.IgnoreAllyLimit },
            "RESHUFFLE_HAND_REDRAW" => new() { Action = EffectAction.ReshuffleHandRedraw },
            "RECOVER_FROM_EXILE" => new() { Action = EffectAction.RecoverFromExile, Value = ParseInt(val) },
            "REDUCE_NEXT_COST" => new() { Action = EffectAction.ReduceNextCost, Value = ParseInt(val) },
            "DETECT_TRAP" => new() { Action = EffectAction.DetectTrap, Value = ParseInt(val) },
            "PREVENT_RETARGET" => new() { Action = EffectAction.PreventRetarget },
            "PREVENT_ATTACK" => new() { Action = EffectAction.PreventAttack },
            "IMMUNE_CONSUMABLE" => new() { Action = EffectAction.ImmuneConsumable },
            "IMMUNE_EQUIPMENT" => new() { Action = EffectAction.ImmuneEquipment },
            "IMMUNE_SCROLL" => new() { Action = EffectAction.ImmuneScroll },
            "IMMUNE_BOMB" => new() { Action = EffectAction.ImmuneBomb },
            "IMMUNE_TRAP" => new() { Action = EffectAction.ImmuneTrap },
            "DISABLE_EQUIPMENT" => new() { Action = EffectAction.DisableEquipment, Value = ParseInt(val) },
            "SPAWN_SPIDER" or "SPAWN_MONSTER" => new() { Action = EffectAction.SpawnMonster, Value = ParseInt(val) },
            "SWAP_ASSIGNMENTS" => new() { Action = EffectAction.SwapAssignments },
            "FORCE_DISCARD_OR_EXILE_WEAKEST" => new() { Action = EffectAction.ForceDiscardOrExile },
            "RETURN_ALLY_TO_HAND" => new() { Action = EffectAction.ReturnAllyToHand, Value = ParseInt(val) },
            "ATTACK_TWO_TARGETS" => new() { Action = EffectAction.AttackTwoTargets },
            "SWAP_WITH_MONSTER" => new() { Action = EffectAction.SwapWithMonster },
            "ONLY_ELIM_BY_DOUBLE" => new() { Action = EffectAction.OnlyElimByDouble },
            "TRAP_DOUBLE" => new() { Action = EffectAction.TrapDouble },
            "RESTORE_CARD_FROM_DISCARD" => new() { Action = EffectAction.RecoverFromExile, Value = 1, Param = "DISCARD" },
            "EXILE_TARGET" => new() { Action = EffectAction.ExileDeck, Value = 1, Target = EffectTarget.Enemy },
            "IMMUNE" => new() { Action = EffectAction.ImmuneConsumable },
            _ => null
        };
    }

    private static EffectTarget ParseTarget(string val) => val.ToUpperInvariant() switch
    {
        "SELF" => EffectTarget.Self,
        "ALLY" => EffectTarget.Ally,
        "ALL_ALLIES" => EffectTarget.AllAllies,
        "GROUP" => EffectTarget.Group,
        "ENEMY" => EffectTarget.Enemy,
        "ALL_ENEMIES" => EffectTarget.AllEnemies,
        "ENEMY_GROUP" => EffectTarget.EnemyGroup,
        "MARKED" => EffectTarget.MarkedEnemy,
        "OPPONENT" => EffectTarget.Opponent,
        "BOTH" => EffectTarget.Both,
        _ => EffectTarget.Self
    };

    private static int ParseInt(string s) => int.TryParse(s, out var v) ? v : 0;
    private static int ParseSignedInt(string s) => int.TryParse(s.TrimStart(':'), out var v) ? v : 0;
}
