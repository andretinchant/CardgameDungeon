namespace CardgameDungeon.Domain.Entities;

public class PlayerCollection
{
    public Guid PlayerId { get; private set; }
    public IReadOnlyList<OwnedCard> Cards => _cards.AsReadOnly();

    private readonly List<OwnedCard> _cards;

    public PlayerCollection(Guid playerId, IEnumerable<OwnedCard>? cards = null)
    {
        PlayerId = playerId;
        _cards = cards?.ToList() ?? [];
    }

    public OwnedCard AddCard(Guid cardId)
    {
        var owned = new OwnedCard(Guid.NewGuid(), cardId, PlayerId);
        _cards.Add(owned);
        return owned;
    }

    public IReadOnlyList<OwnedCard> GetAvailableCopies(Guid cardId)
        => _cards.Where(c => c.CardId == cardId && !c.IsReserved).ToList();

    public OwnedCard GetOwnedCard(Guid ownedCardId)
        => _cards.FirstOrDefault(c => c.Id == ownedCardId)
           ?? throw new InvalidOperationException($"Owned card {ownedCardId} not found in collection.");

    public void RemoveCard(OwnedCard card)
    {
        if (!_cards.Remove(card))
            throw new InvalidOperationException("Card not found in collection.");
    }
}
