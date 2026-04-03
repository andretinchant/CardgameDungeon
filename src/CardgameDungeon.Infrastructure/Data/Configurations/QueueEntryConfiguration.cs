using CardgameDungeon.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CardgameDungeon.Infrastructure.Data.Configurations;

internal class QueueEntryConfiguration : IEntityTypeConfiguration<QueueEntry>
{
    public void Configure(EntityTypeBuilder<QueueEntry> builder)
    {
        builder.ToTable("QueueEntries");
        builder.HasKey(qe => qe.PlayerId);

        builder.Property(qe => qe.DeckId);
        builder.Property(qe => qe.QueueType).HasConversion<string>().HasMaxLength(20);
        builder.Property(qe => qe.Elo);
        builder.Property(qe => qe.JoinedAt);

        builder.HasIndex(qe => qe.QueueType);
    }
}
