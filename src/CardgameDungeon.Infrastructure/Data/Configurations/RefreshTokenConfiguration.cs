using CardgameDungeon.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CardgameDungeon.Infrastructure.Data.Configurations;

internal class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.PlayerId);
        builder.Property(rt => rt.Token).HasMaxLength(200).IsRequired();
        builder.Property(rt => rt.ExpiresAt);
        builder.Property(rt => rt.CreatedAt);
        builder.Property(rt => rt.IsRevoked);

        builder.HasIndex(rt => rt.Token).IsUnique();
        builder.HasIndex(rt => rt.PlayerId);
    }
}
