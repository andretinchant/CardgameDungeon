using CardgameDungeon.Domain.Entities;

namespace CardgameDungeon.Domain.Effects;

/// <summary>
/// Processes effect triggers from all allies in play and executes
/// resulting actions on the PlayerState. Called by handlers at
/// appropriate game flow points.
/// </summary>
public static class TriggerProcessor
{
    /// <summary>
    /// Fires a trigger for all allies in play and executes the resulting actions.
    /// Returns a list of events that occurred for client notification.
    /// </summary>
    public static List<PostCombatEvent> FireTrigger(
        EffectTrigger trigger, PlayerState state, PlayerState? opponent = null)
    {
        var events = new List<PostCombatEvent>();

        foreach (var ally in state.AlliesInPlay.ToList())
        {
            var tags = ally.ParsedEffects.Where(t => t.Trigger == trigger).ToList();

            foreach (var tag in tags)
            {
                // Check condition
                var ctx = new EffectContext
                {
                    SourceCardId = ally.Id,
                    DeckCount = state.Deck.Count,
                    HandCount = state.Hand.Count,
                    AlliesInPlay = state.AlliesInPlay.Count
                };
                foreach (var a in state.AlliesInPlay)
                    ctx.AddClassInPlay(a.Class.ToString());

                if (tag.Condition != EffectCondition.None)
                {
                    // Simple condition evaluation
                    var conditionMet = tag.Condition switch
                    {
                        EffectCondition.OncePerCombat => !ctx.HasTriggeredThisCombat(tag),
                        EffectCondition.OncePerRoom => !ctx.HasTriggeredThisRoom(tag),
                        EffectCondition.IfClass => ctx.HasClassInPlay(tag.ConditionParam),
                        EffectCondition.IfRaging => true, // Simplified — would need rage state tracking
                        _ => true
                    };
                    if (!conditionMet) continue;
                }

                // Check cost affordability
                if (tag.Cost is not null)
                {
                    var canPay = tag.Cost.Type switch
                    {
                        EffectCostType.ExileDeck => state.Deck.Count >= tag.Cost.Amount,
                        EffectCostType.ExileHand => state.Hand.Count >= tag.Cost.Amount,
                        EffectCostType.DiscardHand => state.Hand.Count >= tag.Cost.Amount,
                        _ => true
                    };
                    if (!canPay) continue;

                    // Pay the cost
                    switch (tag.Cost.Type)
                    {
                        case EffectCostType.ExileDeck:
                            state.ExileFromTop(tag.Cost.Amount);
                            break;
                        case EffectCostType.ExileHand:
                            for (var i = 0; i < tag.Cost.Amount && state.Hand.Count > 0; i++)
                                state.DiscardFromHand(state.Hand[0]); // Simplified
                            break;
                        case EffectCostType.DiscardHand:
                            for (var i = 0; i < tag.Cost.Amount && state.Hand.Count > 0; i++)
                                state.DiscardFromHand(state.Hand[0]);
                            break;
                    }
                }

                foreach (var action in tag.Actions)
                {
                    ExecuteAction(action, state, opponent, ally);

                    events.Add(new PostCombatEvent
                    {
                        SourceCardId = ally.Id,
                        SourceCardName = ally.Name,
                        Trigger = trigger,
                        Action = action.Action,
                        Value = action.Value,
                        Target = action.Target
                    });
                }
            }
        }

        return events;
    }

    /// <summary>Executes a single action on the player state.</summary>
    public static void ExecuteAction(
        EffectActionEntry action, PlayerState state, PlayerState? opponent, AllyCard? source)
    {
        switch (action.Action)
        {
            case EffectAction.Draw:
                state.DrawUpTo(action.Value);
                break;

            case EffectAction.ExileDeck when action.Target == EffectTarget.Opponent && opponent != null:
                opponent.ExileFromTop(Math.Min(action.Value, opponent.Deck.Count));
                break;

            case EffectAction.ExileDeck when action.Target == EffectTarget.Both && opponent != null:
                state.ExileFromTop(Math.Min(action.Value, state.Deck.Count));
                opponent.ExileFromTop(Math.Min(action.Value, opponent.Deck.Count));
                break;

            case EffectAction.ExileDeck:
                state.ExileFromTop(Math.Min(action.Value, state.Deck.Count));
                break;

            case EffectAction.DiscardHand:
                for (var i = 0; i < Math.Min(action.Value, state.Hand.Count); i++)
                    state.DiscardFromHand(state.Hand[0]);
                break;

            case EffectAction.ReshuffleHandRedraw:
                state.ReshuffleHandAndRedraw();
                break;

            case EffectAction.RecoverFromExile:
                state.RecoverFromExile(action.Value);
                break;

            case EffectAction.ReduceNextCost:
                state.AddCostReduction(action.Value);
                break;

            case EffectAction.ReturnHandBottom:
                state.ReturnHandToBottom(action.Value);
                break;

            case EffectAction.ReturnHandTop:
                state.ReturnHandToTop(action.Value);
                break;

            case EffectAction.ReturnHandShuffle:
                state.ReturnHandToBottom(action.Value);
                state.ShuffleDiscardIntoDeck(); // Approximation
                break;

            // Healing, damage, etc. are handled contextually by the caller
            // since they may need to target specific cards
        }
    }
}
