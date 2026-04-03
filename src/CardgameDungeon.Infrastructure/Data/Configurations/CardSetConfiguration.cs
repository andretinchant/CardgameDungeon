using CardgameDungeon.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CardgameDungeon.Infrastructure.Data.Configurations;

internal class CardSetConfiguration : IEntityTypeConfiguration<CardSet>
{
    public void Configure(EntityTypeBuilder<CardSet> builder)
    {
        builder.ToTable("CardSets");
        builder.HasKey(cs => cs.Id);

        builder.Property(cs => cs.Name).HasMaxLength(200).IsRequired();
        builder.Property(cs => cs.Code).HasMaxLength(20).IsRequired();
        builder.Property(cs => cs.ReleaseDate);
        builder.Property(cs => cs.Description).HasMaxLength(1000);

        builder.HasIndex(cs => cs.Code).IsUnique();

        // One CardSet has many Cards
        builder.HasMany(cs => cs.Cards)
            .WithOne()
            .HasForeignKey("CardSetId")
            .IsRequired(false);

        builder.Navigation(cs => cs.Cards).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
