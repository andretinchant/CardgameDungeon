using CardgameDungeon.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CardgameDungeon.Infrastructure.Data.Configurations;

internal class PlayerRatingConfiguration : IEntityTypeConfiguration<PlayerRating>
{
    public void Configure(EntityTypeBuilder<PlayerRating> builder)
    {
        builder.ToTable("PlayerRatings");
        builder.HasKey(pr => pr.PlayerId);

        builder.Property(pr => pr.Elo);
        builder.Property(pr => pr.Tier).HasConversion<string>().HasMaxLength(20);
        builder.Property(pr => pr.Wins);
        builder.Property(pr => pr.Losses);

        // Ignore computed property
        builder.Ignore(pr => pr.TotalGames);

        builder.HasIndex(pr => pr.Elo);
    }
}
