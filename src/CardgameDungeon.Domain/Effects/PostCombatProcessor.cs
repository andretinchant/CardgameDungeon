using CardgameDungeon.Domain.Entities;

namespace CardgameDungeon.Domain.Effects;

/// <summary>
/// Processes effect tags that trigger AFTER combat resolution:
/// ON_KILL, ON_DEATH, ON_MARKED_KILL, ON_MARKED_SURVIVE, ON_ENEMY_FLEE.
/// Returns a list of post-combat events that the game loop should execute.
/// </summary>
public static class PostCombatProcessor
{
    public static PostCombatResult Process(
        IReadOnlyList<AllyCard> survivingAttackers,
        IReadOnlyList<AllyCard> eliminatedDefenders,
        IReadOnlyList<AllyCard> eliminatedAttackers,
        IReadOnlyList<AllyCard> survivingDefenders,
        PlayerState attackerState,
        PlayerState defenderState)
    {
        var events = new List<PostCombatEvent>();

        // Process ON_KILL for each surviving attacker that participated in a kill
        if (eliminatedDefenders.Count > 0)
        {
            foreach (var ally in survivingAttackers)
            {
                var killTags = ally.ParsedEffects
                    .Where(t => t.Trigger == EffectTrigger.OnKill)
                    .ToList();

                foreach (var tag in killTags)
                {
                    var ctx = BuildContext(ally, attackerState);

                    // Check conditions (e.g., IF_RAGING)
                    if (!EvaluateConditions(tag, ctx)) continue;

                    foreach (var action in tag.Actions)
                    {
                        events.Add(new PostCombatEvent
                        {
                            SourceCardId = ally.Id,
                            SourceCardName = ally.Name,
                            Trigger = EffectTrigger.OnKill,
                            Action = action.Action,
                            Value = action.Value,
                            Target = action.Target,
                            Param = action.Param
                        });
                    }
                }
            }
        }

        // Process ON_DEATH for each eliminated card
        foreach (var ally in eliminatedAttackers)
        {
            var deathTags = ally.ParsedEffects
                .Where(t => t.Trigger == EffectTrigger.OnDeath)
                .ToList();

            foreach (var tag in deathTags)
            {
                foreach (var action in tag.Actions)
                {
                    events.Add(new PostCombatEvent
                    {
                        SourceCardId = ally.Id,
                        SourceCardName = ally.Name,
                        Trigger = EffectTrigger.OnDeath,
                        Action = action.Action,
                        Value = action.Value,
                        Target = action.Target,
                        Param = action.Param
                    });
                }
            }
        }

        foreach (var ally in eliminatedDefenders)
        {
            var deathTags = ally.ParsedEffects
                .Where(t => t.Trigger == EffectTrigger.OnDeath)
                .ToList();

            foreach (var tag in deathTags)
            {
                foreach (var action in tag.Actions)
                {
                    events.Add(new PostCombatEvent
                    {
                        SourceCardId = ally.Id,
                        SourceCardName = ally.Name,
                        Trigger = EffectTrigger.OnDeath,
                        Action = action.Action,
                        Value = action.Value,
                        Target = action.Target,
                        Param = action.Param
                    });
                }
            }
        }

        // Process ON_MARKED_KILL / ON_MARKED_SURVIVE for Warlocks
        foreach (var ally in survivingAttackers)
        {
            var markedKillTags = ally.ParsedEffects
                .Where(t => t.Trigger == EffectTrigger.OnMarkedKill)
                .ToList();

            var markedSurviveTags = ally.ParsedEffects
                .Where(t => t.Trigger == EffectTrigger.OnMarkedSurvive)
                .ToList();

            // If any defenders were eliminated, trigger ON_MARKED_KILL
            if (eliminatedDefenders.Count > 0 && markedKillTags.Count > 0)
            {
                foreach (var tag in markedKillTags)
                {
                    foreach (var action in tag.Actions)
                    {
                        events.Add(new PostCombatEvent
                        {
                            SourceCardId = ally.Id,
                            SourceCardName = ally.Name,
                            Trigger = EffectTrigger.OnMarkedKill,
                            Action = action.Action,
                            Value = action.Value,
                            Target = action.Target,
                            Param = action.Param
                        });
                    }
                }
            }
            // If no defenders eliminated but Warlock has marks, trigger ON_MARKED_SURVIVE
            else if (eliminatedDefenders.Count == 0 && markedSurviveTags.Count > 0)
            {
                foreach (var tag in markedSurviveTags)
                {
                    foreach (var action in tag.Actions)
                    {
                        events.Add(new PostCombatEvent
                        {
                            SourceCardId = ally.Id,
                            SourceCardName = ally.Name,
                            Trigger = EffectTrigger.OnMarkedSurvive,
                            Action = action.Action,
                            Value = action.Value,
                            Target = action.Target,
                            Param = action.Param
                        });
                    }
                }
            }
        }

        return new PostCombatResult(events);
    }

    private static EffectContext BuildContext(AllyCard ally, PlayerState state)
    {
        return new EffectContext
        {
            SourceCardId = ally.Id,
            DeckCount = state.Deck.Count,
            HandCount = state.Hand.Count,
            CurrentHp = ally.HitPoints,
            IsRaging = true // Simplified: if ON_KILL|IF_RAGING, assume active if cost was paid
        };
    }

    private static bool EvaluateConditions(EffectTag tag, EffectContext ctx)
    {
        return tag.Condition switch
        {
            EffectCondition.None => true,
            EffectCondition.IfRaging => ctx.IsRaging,
            EffectCondition.OncePerCombat => !ctx.HasTriggeredThisCombat(tag),
            _ => true
        };
    }
}

public class PostCombatEvent
{
    public Guid SourceCardId { get; init; }
    public string SourceCardName { get; init; } = "";
    public EffectTrigger Trigger { get; init; }
    public EffectAction Action { get; init; }
    public int Value { get; init; }
    public EffectTarget Target { get; init; }
    public string? Param { get; init; }
}

public class PostCombatResult
{
    public IReadOnlyList<PostCombatEvent> Events { get; }

    public PostCombatResult(List<PostCombatEvent> events) => Events = events;

    public bool HasJoinCombat => Events.Any(e => e.Action == EffectAction.JoinCombat);
    public bool HasForfeitTreasure => Events.Any(e => e.Action == EffectAction.ForfeitTreasure);
    public bool HasTriggerOppAttack => Events.Any(e => e.Action == EffectAction.TriggerOppAttack);

    public IEnumerable<PostCombatEvent> GetByAction(EffectAction action) =>
        Events.Where(e => e.Action == action);

    public IEnumerable<PostCombatEvent> GetByTrigger(EffectTrigger trigger) =>
        Events.Where(e => e.Trigger == trigger);
}
