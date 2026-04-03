using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Infrastructure.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CardgameDungeon.Infrastructure.Data;

public class CardgameDungeonDbContext : DbContext
{
    public DbSet<Card> Cards => Set<Card>();
    public DbSet<AllyCard> AllyCards => Set<AllyCard>();
    public DbSet<EquipmentCard> EquipmentCards => Set<EquipmentCard>();
    public DbSet<MonsterCard> MonsterCards => Set<MonsterCard>();
    public DbSet<TrapCard> TrapCards => Set<TrapCard>();
    public DbSet<DungeonRoomCard> DungeonRoomCards => Set<DungeonRoomCard>();
    public DbSet<BossCard> BossCards => Set<BossCard>();
    public DbSet<CardSet> CardSets => Set<CardSet>();
    public DbSet<OwnedCard> OwnedCards => Set<OwnedCard>();
    public DbSet<PlayerWallet> PlayerWallets => Set<PlayerWallet>();
    public DbSet<MarketplaceListing> MarketplaceListings => Set<MarketplaceListing>();
    public DbSet<QueueEntry> QueueEntries => Set<QueueEntry>();
    public DbSet<PlayerRating> PlayerRatings => Set<PlayerRating>();
    public DbSet<Tournament> Tournaments => Set<Tournament>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    internal DbSet<DeckListEntity> DeckLists => Set<DeckListEntity>();
    internal DbSet<MatchStateEntity> MatchStates => Set<MatchStateEntity>();

    public CardgameDungeonDbContext(DbContextOptions<CardgameDungeonDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CardgameDungeonDbContext).Assembly);
    }
}
