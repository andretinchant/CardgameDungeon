namespace CardgameDungeon.Domain.Effects;

/// <summary>
/// A single parsed effect rule: trigger + optional cost + optional condition + actions.
/// Example tag string: "ON_ACTIVATE|COST:EXILE_DECK:4|+STR:2|ELIM_DOUBLE|ONCE_PER_COMBAT"
/// </summary>
public class EffectTag
{
    public EffectTrigger Trigger { get; init; }
    public EffectCondition Condition { get; init; }
    public string? ConditionParam { get; init; }
    public EffectCost? Cost { get; init; }
    public IReadOnlyList<EffectActionEntry> Actions { get; init; } = [];

    public override string ToString()
    {
        var parts = new List<string> { Trigger.ToString() };

        if (Condition != EffectCondition.None)
            parts.Add($"{Condition}:{ConditionParam}");
        if (Cost is not null)
            parts.Add($"COST:{Cost.Type}:{Cost.Amount}");
        foreach (var action in Actions)
            parts.Add(action.ToString());

        return string.Join("|", parts);
    }
}

public class EffectCost
{
    public EffectCostType Type { get; init; }
    public int Amount { get; init; }
}

public enum EffectCostType
{
    ExileDeck,
    ExileHand,
    DiscardHand,
    DiscardDeck,
    Hp
}

public class EffectActionEntry
{
    public EffectAction Action { get; init; }
    public int Value { get; init; }
    public EffectTarget Target { get; init; }
    public string? Param { get; init; }

    public override string ToString()
    {
        var actionStr = Action switch
        {
            EffectAction.ModStr when Value >= 0 => $"+STR:{Value}",
            EffectAction.ModStr => $"-STR:{Math.Abs(Value)}",
            EffectAction.ModHp when Value >= 0 => $"+HP:{Value}",
            EffectAction.ModHp => $"-HP:{Math.Abs(Value)}",
            EffectAction.ModInit when Value >= 0 => $"+INIT:{Value}",
            EffectAction.ModInit => $"-INIT:{Math.Abs(Value)}",
            EffectAction.Damage => $"DAMAGE:{Value}:{Target}",
            EffectAction.Heal => $"HEAL:{Value}:{Target}",
            EffectAction.ExileDeck => $"EXILE_DECK:{Value}",
            EffectAction.ExileHand => $"EXILE_HAND:{Value}",
            EffectAction.DiscardHand => $"DISCARD_HAND:{Value}",
            EffectAction.Draw => $"DRAW:{Value}",
            EffectAction.ForfeitTreasure => "FORFEIT_TREASURE",
            EffectAction.ElimDouble => "ELIM_DOUBLE",
            EffectAction.JoinCombat => "JOIN_COMBAT",
            EffectAction.CancelCombat => "CANCEL_COMBAT",
            EffectAction.RedirectDamage => $"REDIRECT_DAMAGE:{Target}",
            EffectAction.MarkEnemy => $"MARK_ENEMY:{Value}",
            EffectAction.TriggerOppAttack => $"TRIGGER_OPP_ATTACK:{Target}",
            EffectAction.SearchDeck => $"SEARCH_DECK:{Param}",
            EffectAction.RevealHand => "REVEAL_HAND",
            EffectAction.RevealDeck => $"REVEAL_DECK:{Value}",
            EffectAction.FavoredEnemy => $"FAVORED_ENEMY:{Param}",
            EffectAction.CopyScroll => "COPY_SCROLL",
            EffectAction.RecoverScroll => $"RECOVER_SCROLL:{Param}",
            EffectAction.RecoverScrollFromExile => $"RECOVER_SCROLL_EXILE:{Value}",
            EffectAction.ScrollToBottom => "SCROLL_TO_BOTTOM",
            EffectAction.ReduceDamage => $"REDUCE_DAMAGE:{Value}",
            EffectAction.OppAttackDouble => "OPP_ATTACK_DOUBLE",
            EffectAction.IgnoreOppAttackLimit => "IGNORE_OPP_ATTACK_LIMIT",
            EffectAction.ReturnHandTop => $"RETURN_HAND_TOP:{Value}",
            EffectAction.ReturnHandBottom => $"RETURN_HAND_BOTTOM:{Value}",
            EffectAction.ReturnHandShuffle => $"RETURN_HAND_SHUFFLE:{Value}",
            _ => Action.ToString()
        };
        return actionStr;
    }
}
