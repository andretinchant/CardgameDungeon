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
    public int ExileCount => _exile.Count;

    private readonly List<Card> _deck;
    private readonly List<Card> _hand = [];
    private readonly List<Card> _discard = [];
    private readonly List<Card> _exile = [];
    private readonly List<AllyCard> _alliesInPlay = [];

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

    public void EliminateAlly(AllyCard ally)
    {
        if (!_alliesInPlay.Remove(ally))
            throw new InvalidOperationException($"Ally '{ally.Name}' is not in play.");

        _discard.Add(ally);
    }

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
}
