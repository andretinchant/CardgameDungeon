using CardgameDungeon.Infrastructure.Data.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CardgameDungeon.Infrastructure.Data.Configurations;

internal class DeckListEntityConfiguration : IEntityTypeConfiguration<DeckListEntity>
{
    public void Configure(EntityTypeBuilder<DeckListEntity> builder)
    {
        builder.ToTable("DeckLists");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.PlayerId);
        builder.Property(d => d.BossCardId);
        builder.Property(d => d.AdventurerCardIds).HasColumnType("jsonb");
        builder.Property(d => d.EnemyCardIds).HasColumnType("jsonb");
        builder.Property(d => d.DungeonRoomCardIds).HasColumnType("jsonb");

        builder.HasIndex(d => d.PlayerId);
    }
}
