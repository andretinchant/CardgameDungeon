using CardgameDungeon.Infrastructure.Data.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CardgameDungeon.Infrastructure.Data.Configurations;

internal class MatchStateEntityConfiguration : IEntityTypeConfiguration<MatchStateEntity>
{
    public void Configure(EntityTypeBuilder<MatchStateEntity> builder)
    {
        builder.ToTable("MatchStates");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Phase);
        builder.Property(m => m.WinnerId);
        builder.Property(m => m.StateJson).HasColumnType("jsonb");

        builder.HasIndex(m => m.WinnerId);
    }
}
