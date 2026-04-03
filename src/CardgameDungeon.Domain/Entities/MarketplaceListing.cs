namespace CardgameDungeon.Domain.Entities;

public class MarketplaceListing
{
    public const decimal TransactionFeeRate = 0.10m;

    public Guid Id { get; private set; }
    public Guid SellerId { get; private set; }
    public Guid OwnedCardId { get; private set; }
    public Guid CardId { get; private set; }
    public int Price { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public MarketplaceListing(Guid id, Guid sellerId, Guid ownedCardId, Guid cardId, int price)
    {
        if (price <= 0)
            throw new ArgumentOutOfRangeException(nameof(price), "Price must be positive.");

        Id = id;
        SellerId = sellerId;
        OwnedCardId = ownedCardId;
        CardId = cardId;
        Price = price;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public int CalculateFee() => (int)Math.Ceiling(Price * TransactionFeeRate);
    public int CalculateSellerProceeds() => Price - CalculateFee();

    public void Complete()
    {
        if (!IsActive)
            throw new InvalidOperationException("Listing is no longer active.");
        IsActive = false;
    }

    public void Cancel()
    {
        if (!IsActive)
            throw new InvalidOperationException("Listing is no longer active.");
        IsActive = false;
    }
}
