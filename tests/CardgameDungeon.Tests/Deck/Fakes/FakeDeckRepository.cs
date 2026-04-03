using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Repositories;

namespace CardgameDungeon.Tests.Deck.Fakes;

public class FakeDeckRepository : IDeckRepository
{
    private readonly Dictionary<Guid, DeckList> _decks = new();

    public DeckList? LastSaved { get; private set; }
    public DeckList? LastUpdated { get; private set; }

    public Task<DeckList?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(_decks.GetValueOrDefault(id));

    public Task SaveAsync(DeckList deck, CancellationToken ct = default)
    {
        _decks[deck.Id] = deck;
        LastSaved = deck;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(DeckList deck, CancellationToken ct = default)
    {
        _decks[deck.Id] = deck;
        LastUpdated = deck;
        return Task.CompletedTask;
    }

    public void Seed(DeckList deck) => _decks[deck.Id] = deck;
}
