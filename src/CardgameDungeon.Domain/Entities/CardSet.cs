namespace CardgameDungeon.Domain.Entities;

public class CardSet
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Code { get; private set; }
    public DateOnly ReleaseDate { get; private set; }
    public string Description { get; private set; }
    public IReadOnlyList<Card> Cards => _cards.AsReadOnly();

    private readonly List<Card> _cards;

    private CardSet() { Name = null!; Code = null!; Description = null!; _cards = []; } // EF Core

    public CardSet(
        Guid id,
        string name,
        string code,
        DateOnly releaseDate,
        string description,
        IEnumerable<Card>? cards = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Set name cannot be empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Set code cannot be empty.", nameof(code));

        Id = id;
        Name = name;
        Code = code;
        ReleaseDate = releaseDate;
        Description = description;
        _cards = cards?.ToList() ?? [];
    }

    public void AddCard(Card card) => _cards.Add(card);
    public int TotalCards => _cards.Count;
}
