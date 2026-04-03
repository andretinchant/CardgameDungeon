namespace CardgameDungeon.Domain.Entities;

public class OwnedCard
{
    public Guid Id { get; private set; }
    public Guid CardId { get; private set; }
    public Guid PlayerId { get; private set; }
    public bool IsReserved { get; private set; }

    public OwnedCard(Guid id, Guid cardId, Guid playerId)
    {
        Id = id;
        CardId = cardId;
        PlayerId = playerId;
        IsReserved = false;
    }

    public void Reserve()
    {
        if (IsReserved)
            throw new InvalidOperationException("Card copy is already reserved.");
        IsReserved = true;
    }

    public void Release()
    {
        if (!IsReserved)
            throw new InvalidOperationException("Card copy is not reserved.");
        IsReserved = false;
    }

    public void TransferTo(Guid newPlayerId)
    {
        if (IsReserved)
            throw new InvalidOperationException("Cannot transfer a reserved card copy.");
        PlayerId = newPlayerId;
    }
}
