using CardgameDungeon.Domain.Entities;

namespace CardgameDungeon.Domain.Effects;

/// <summary>
/// Processes EffectTags during game phases, applying actions to the combat context.
/// This is the runtime engine that makes card text actually affect the game.
/// </summary>
public class EffectEngine
{
    /// <summary>
    /// Collects all stat modifications from effects that should be active for a given trigger.
    /// Returns cumulative ATK, HP, INIT modifiers.
    /// </summary>
    public static StatModifiers CalculateModifiers(
        IReadOnlyList<EffectTag> tags,
        EffectTrigger trigger,
        EffectContext context)
    {
        var mods = new StatModifiers();

        foreach (var tag in tags)
        {
            if (tag.Trigger != trigger) continue;
            if (!EvaluateCondition(tag, context)) continue;
            if (!CanPayCost(tag, context)) continue;

            foreach (var action in tag.Actions)
            {
                switch (action.Action)
                {
                    case EffectAction.ModAtk:
                        mods.Attack += action.Value;
                        break;
                    case EffectAction.ModHp:
                        mods.HitPoints += action.Value;
                        break;
                    case EffectAction.ModInit:
                        mods.Initiative += action.Value;
                        break;
                    case EffectAction.ElimDouble:
                        mods.EliminationDoubled = true;
                        break;
                    case EffectAction.ReduceDamage:
                        mods.DamageReduction += action.Value;
                        break;
                    case EffectAction.OppAttackDouble:
                        mods.OpportunityAttackDoubled = true;
                        break;
                    case EffectAction.IgnoreOppAttackLimit:
                        mods.IgnoreOpportunityAttackLimit = true;
                        break;
                    case EffectAction.ForfeitTreasure:
                        mods.ForfeitsTreasure = true;
                        break;
                }
            }
        }

        return mods;
    }

    /// <summary>
    /// Collects all triggered actions (non-stat-mod) for a given trigger.
    /// These are actions like DAMAGE, HEAL, DRAW, EXILE, etc. that need to be executed.
    /// </summary>
    public static IReadOnlyList<PendingAction> CollectActions(
        IReadOnlyList<EffectTag> tags,
        EffectTrigger trigger,
        EffectContext context)
    {
        var actions = new List<PendingAction>();

        foreach (var tag in tags)
        {
            if (tag.Trigger != trigger) continue;
            if (!EvaluateCondition(tag, context)) continue;

            foreach (var action in tag.Actions)
            {
                // Skip stat mods — those are handled by CalculateModifiers
                if (action.Action is EffectAction.ModAtk or EffectAction.ModHp
                    or EffectAction.ModInit or EffectAction.ElimDouble
                    or EffectAction.ReduceDamage or EffectAction.OppAttackDouble
                    or EffectAction.IgnoreOppAttackLimit or EffectAction.ForfeitTreasure)
                    continue;

                actions.Add(new PendingAction
                {
                    Action = action,
                    Cost = tag.Cost,
                    SourceCardId = context.SourceCardId
                });
            }
        }

        return actions;
    }

    private static bool EvaluateCondition(EffectTag tag, EffectContext context)
    {
        return tag.Condition switch
        {
            EffectCondition.None => true,
            EffectCondition.IfRace => context.TargetRace?.ToString() == tag.ConditionParam,
            EffectCondition.IfClass => context.HasClassInPlay(tag.ConditionParam),
            EffectCondition.IfRaging => context.IsRaging,
            EffectCondition.IfEquipped => context.HasEquipmentSlot(tag.ConditionParam),
            EffectCondition.IfScrollUsed => context.ScrollUsedThisRound,
            EffectCondition.OncePerCombat => !context.HasTriggeredThisCombat(tag),
            EffectCondition.OncePerRoom => !context.HasTriggeredThisRoom(tag),
            _ => true
        };
    }

    private static bool CanPayCost(EffectTag tag, EffectContext context)
    {
        if (tag.Cost is null) return true;

        return tag.Cost.Type switch
        {
            EffectCostType.ExileDeck => context.DeckCount >= tag.Cost.Amount,
            EffectCostType.ExileHand => context.HandCount >= tag.Cost.Amount,
            EffectCostType.DiscardHand => context.HandCount >= tag.Cost.Amount,
            EffectCostType.DiscardDeck => context.DeckCount >= tag.Cost.Amount,
            EffectCostType.Hp => context.CurrentHp > tag.Cost.Amount,
            _ => true
        };
    }
}

/// <summary>
/// Cumulative stat modifications from all active effects.
/// Applied on top of base card stats during combat resolution.
/// </summary>
public class StatModifiers
{
    public int Attack { get; set; }
    public int HitPoints { get; set; }
    public int Initiative { get; set; }
    public int DamageReduction { get; set; }
    public bool EliminationDoubled { get; set; }
    public bool OpportunityAttackDoubled { get; set; }
    public bool IgnoreOpportunityAttackLimit { get; set; }
    public bool ForfeitsTreasure { get; set; }
}

/// <summary>
/// A non-stat action that needs to be executed by the game loop.
/// </summary>
public class PendingAction
{
    public required EffectActionEntry Action { get; init; }
    public EffectCost? Cost { get; init; }
    public Guid SourceCardId { get; init; }
}

/// <summary>
/// Runtime context passed to the effect engine during evaluation.
/// Provides information about the current game state.
/// </summary>
public class EffectContext
{
    public Guid SourceCardId { get; init; }
    public Enums.Race? TargetRace { get; init; }
    public bool IsRaging { get; init; }
    public bool ScrollUsedThisRound { get; init; }
    public int DeckCount { get; init; }
    public int HandCount { get; init; }
    public int CurrentHp { get; init; }
    public int AlliesInPlay { get; init; }
    public int EnemiesInGroup { get; init; }
    public bool HasAdvantage { get; init; }
    public bool HasDisadvantage { get; init; }

    private readonly HashSet<string> _classesInPlay = [];
    private readonly HashSet<string> _equippedSlots = [];
    private readonly HashSet<int> _triggeredCombat = [];
    private readonly HashSet<int> _triggeredRoom = [];

    public void AddClassInPlay(string className) => _classesInPlay.Add(className);
    public bool HasClassInPlay(string? className) => className is not null && _classesInPlay.Contains(className);
    public void AddEquippedSlot(string slot) => _equippedSlots.Add(slot);
    public bool HasEquipmentSlot(string? slot) => slot is not null && _equippedSlots.Contains(slot);
    public void MarkTriggeredCombat(EffectTag tag) => _triggeredCombat.Add(tag.GetHashCode());
    public bool HasTriggeredThisCombat(EffectTag tag) => _triggeredCombat.Contains(tag.GetHashCode());
    public void MarkTriggeredRoom(EffectTag tag) => _triggeredRoom.Add(tag.GetHashCode());
    public bool HasTriggeredThisRoom(EffectTag tag) => _triggeredRoom.Contains(tag.GetHashCode());
}
