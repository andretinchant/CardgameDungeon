using CardgameDungeon.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CardgameDungeon.Infrastructure.Data.Configurations;

internal class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.ToTable("Players");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Username).HasMaxLength(50).IsRequired();
        builder.Property(p => p.Email).HasMaxLength(200).IsRequired();
        builder.Property(p => p.PasswordHash).HasMaxLength(200).IsRequired();
        builder.Property(p => p.CreatedAt);
        builder.Property(p => p.LastLoginAt);

        builder.HasIndex(p => p.Username).IsUnique();
        builder.HasIndex(p => p.Email).IsUnique();
    }
}
