using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.Domain.Entities;

public class PlayerState
{
    public const int MaxAlliesInPlay = 5;
    public const int HandRefillSize = 8;

    public Guid PlayerId { get; private set; }
    public int HitPoints { get; private set; }

    public IReadOnlyList<Card> Deck => _deck.AsReadOnly();
    public IReadOnlyList<Card> Hand => _hand.AsReadOnly();
    public IReadOnlyList<Card> Discard => _discard.AsReadOnly();
    public IReadOnlyList<Card> Exile => _exile.AsReadOnly();
    public IReadOnlyList<AllyCard> AlliesInPlay => _alliesInPlay.AsReadOnly();
    public IReadOnlyList<AllyCard> MaterializedAllies => _materializedAllies.AsReadOnly();
    public int ExileCount => _exile.Count;
    public int DeadAlliesInDiscard => _discard.Count(c => c is AllyCard);

    /// <summary>Equipment cards currently attached to allies. Key = ally ID, Value = list of equipment.</summary>
    public IReadOnlyDictionary<Guid, List<EquipmentCard>> EquippedItems => _equippedItems;

    /// <summary>Cost reduction for the next card played (from Druid effects).</summary>
    public int NextCostReduction { get; private set; }

    private readonly List<Card> _deck;
    private readonly List<Card> _hand = [];
    private readonly List<Card> _discard = [];
    private readonly List<Card> _exile = [];
    private readonly List<AllyCard> _alliesInPlay = [];
    private readonly List<AllyCard> _materializedAllies = [];
    private readonly Dictionary<Guid, List<EquipmentCard>> _equippedItems = new();

    public bool IsAlive => HitPoints > 0;
    public bool IsDeckEmpty => _deck.Count == 0;

    public PlayerState(Guid playerId, int hitPoints, IEnumerable<Card> deck)
    {
        if (hitPoints <= 0)
            throw new ArgumentOutOfRangeException(nameof(hitPoints), "Starting HP must be positive.");

        PlayerId = playerId;
        HitPoints = hitPoints;
        _deck = deck.ToList();
    }

    // ── Card Draw ──

    public Card DrawFromTop()
    {
        if (_deck.Count == 0)
            throw new InvalidOperationException("Cannot draw from an empty deck.");

        var card = _deck[0];
        _deck.RemoveAt(0);
        _hand.Add(card);
        return card;
    }

    public void DrawUpTo(int count)
    {
        var toDraw = Math.Min(count, _deck.Count);
        for (var i = 0; i < toDraw; i++)
            DrawFromTop();
    }

    public void RefillHand()
    {
        var needed = HandRefillSize - _hand.Count;
        if (needed > 0)
            DrawUpTo(needed);
    }

    // ── Ally Management ──

    public void PlayAlly(AllyCard ally)
    {
        if (_alliesInPlay.Count >= MaxAlliesInPlay)
            throw new InvalidOperationException($"Cannot have more than {MaxAlliesInPlay} allies in play.");

        if (!_hand.Remove(ally))
            throw new InvalidOperationException($"Ally '{ally.Name}' is not in hand.");

        _alliesInPlay.Add(ally);
    }

    public void RemoveAlly(AllyCard ally)
    {
        if (!_alliesInPlay.Remove(ally))
            throw new InvalidOperationException($"Ally '{ally.Name}' is not in play.");

        _discard.Add(ally);
    }

    public void EliminateAlly(AllyCard ally)
    {
        if (!_alliesInPlay.Remove(ally))
            throw new InvalidOperationException($"Ally '{ally.Name}' is not in play.");

        // Also remove any materialized allies attached to this ally
        RemoveMaterializedForAlly(ally.Id);

        _discard.Add(ally);
    }

    // ── Equipment ──

    public void EquipItem(Guid allyId, EquipmentCard equipment)
    {
        var ally = _alliesInPlay.FirstOrDefault(a => a.Id == allyId)
            ?? throw new InvalidOperationException("Ally not in play.");

        // Validate slot restrictions
        if (equipment.Slot.IsGear())
        {
            var existing = GetEquippedForAlly(allyId);
            if (existing.Any(e => e.Slot == equipment.Slot))
                throw new InvalidOperationException($"Ally already has {equipment.Slot} equipped.");
        }

        // Validate class restrictions for companions/summons
        if (equipment.Slot == EquipmentSlot.Companion)
        {
            if (ally.Class is not (AllyClass.Ranger or AllyClass.Druid))
                throw new InvalidOperationException("Companions can only be equipped on Rangers or Druids.");
        }
        else if (equipment.Slot == EquipmentSlot.Summon)
        {
            if (ally.Class is not (AllyClass.Mage or AllyClass.Sorcerer))
                throw new InvalidOperationException("Summons can only be equipped on Mages or Sorcerers.");
        }

        if (!_hand.Remove(equipment))
            throw new InvalidOperationException($"Equipment '{equipment.Name}' is not in hand.");

        if (!_equippedItems.ContainsKey(allyId))
            _equippedItems[allyId] = [];

        _equippedItems[allyId].Add(equipment);
    }

    public IReadOnlyList<EquipmentCard> GetEquippedForAlly(Guid allyId)
    {
        return _equippedItems.TryGetValue(allyId, out var items) ? items : [];
    }

    public bool HasEquipmentSlot(Guid allyId, EquipmentSlot slot)
    {
        return GetEquippedForAlly(allyId).Any(e => e.Slot == slot);
    }

    public bool HasClassInPlay(AllyClass allyClass)
    {
        return _alliesInPlay.Any(a => a.Class == allyClass);
    }

    // ── Companion/Summon Materialization ──

    /// <summary>
    /// Materializes all Companion/Summon equipment on an ally as extra allies in combat.
    /// Called when the carrier ally enters combat. Does NOT count toward 5-ally limit.
    /// </summary>
    public IReadOnlyList<AllyCard> MaterializeCompanionsAndSummons(Guid carrierAllyId)
    {
        var equipped = GetEquippedForAlly(carrierAllyId);
        var materialized = new List<AllyCard>();

        foreach (var equip in equipped.Where(e => e.Slot.IsCompanionOrSummon()))
        {
            // Create a temporary AllyCard from the equipment's stats
            var ally = new AllyCard(
                equip.Id,
                equip.Name,
                equip.Rarity,
                equip.Cost,
                equip.StrengthModifier,
                Math.Max(1, equip.HitPointsModifier),
                equip.InitiativeModifier,
                effectTags: equip.EffectTags);

            _materializedAllies.Add(ally);
            materialized.Add(ally);
        }

        return materialized;
    }

    private void RemoveMaterializedForAlly(Guid carrierAllyId)
    {
        var equipped = GetEquippedForAlly(carrierAllyId);
        var companionIds = equipped.Where(e => e.Slot.IsCompanionOrSummon()).Select(e => e.Id).ToHashSet();
        _materializedAllies.RemoveAll(a => companionIds.Contains(a.Id));
    }

    // ── Deck/Discard/Exile Operations ──

    public void DiscardFromTop(int count)
    {
        for (var i = 0; i < count; i++)
        {
            if (_deck.Count == 0)
                throw new InvalidOperationException("Cannot discard from an empty deck.");

            var card = _deck[0];
            _deck.RemoveAt(0);
            _discard.Add(card);
        }
    }

    public void ExileFromTop(int count)
    {
        for (var i = 0; i < count; i++)
        {
            if (_deck.Count == 0)
                throw new InvalidOperationException("Cannot exile from an empty deck.");

            var card = _deck[0];
            _deck.RemoveAt(0);
            _exile.Add(card);
        }
    }

    public void DiscardFromHand(Card card)
    {
        if (!_hand.Remove(card))
            throw new InvalidOperationException($"Card '{card.Name}' is not in hand.");

        _discard.Add(card);
    }

    public Card RestoreFromOpponentDiscard(PlayerState opponent)
    {
        if (opponent._discard.Count == 0)
            throw new InvalidOperationException("Opponent discard pile is empty.");

        var card = opponent._discard[^1];
        opponent._discard.RemoveAt(opponent._discard.Count - 1);
        _hand.Add(card);
        return card;
    }

    public void TakeDamage(int damage)
    {
        if (damage < 0)
            throw new ArgumentOutOfRangeException(nameof(damage), "Damage cannot be negative.");

        HitPoints = Math.Max(0, HitPoints - damage);
    }

    public void PayCostFromDeck(int cost, bool exile = false)
    {
        if (exile)
            ExileFromTop(cost);
        else
            DiscardFromTop(cost);
    }

    // ── Druid Mechanics ──

    /// <summary>Reshuffle entire hand into deck, shuffle, then draw the same number.</summary>
    public void ReshuffleHandAndRedraw()
    {
        var handSize = _hand.Count;
        _deck.AddRange(_hand);
        _hand.Clear();

        var rng = Random.Shared;
        for (var i = _deck.Count - 1; i > 0; i--)
        {
            var j = rng.Next(i + 1);
            (_deck[i], _deck[j]) = (_deck[j], _deck[i]);
        }

        DrawUpTo(handSize);
    }

    /// <summary>Move cards from exile to discard pile (recovery, not to hand).</summary>
    public int RecoverFromExile(int count)
    {
        var toRecover = Math.Min(count, _exile.Count);
        for (var i = 0; i < toRecover; i++)
        {
            var card = _exile[_exile.Count - 1];
            _exile.RemoveAt(_exile.Count - 1);
            _discard.Add(card);
        }
        return toRecover;
    }

    /// <summary>Apply cost reduction for the next card played.</summary>
    public void AddCostReduction(int amount) => NextCostReduction += amount;

    /// <summary>Consume and return the current cost reduction, then reset it.</summary>
    public int ConsumeCostReduction()
    {
        var reduction = NextCostReduction;
        NextCostReduction = 0;
        return reduction;
    }

    // ── Shuffle ──

    public void ShuffleDiscardIntoDeck()
    {
        _deck.AddRange(_discard);
        _discard.Clear();

        var rng = Random.Shared;
        for (var i = _deck.Count - 1; i > 0; i--)
        {
            var j = rng.Next(i + 1);
            (_deck[i], _deck[j]) = (_deck[j], _deck[i]);
        }
    }

    /// <summary>Return cards from hand to bottom of deck.</summary>
    public void ReturnHandToBottom(int count)
    {
        var toReturn = Math.Min(count, _hand.Count);
        for (var i = 0; i < toReturn; i++)
        {
            var card = _hand[0];
            _hand.RemoveAt(0);
            _deck.Add(card); // Add to end = bottom
        }
    }

    /// <summary>Return cards from hand to top of deck.</summary>
    public void ReturnHandToTop(int count)
    {
        var toReturn = Math.Min(count, _hand.Count);
        for (var i = toReturn - 1; i >= 0; i--)
        {
            var card = _hand[Math.Min(i, _hand.Count - 1)];
            _hand.Remove(card);
            _deck.Insert(0, card); // Insert at 0 = top
        }
    }
}
