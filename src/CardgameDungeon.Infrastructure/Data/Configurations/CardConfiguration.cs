using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CardgameDungeon.Infrastructure.Data.Configurations;

internal class CardConfiguration : IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> builder)
    {
        builder.ToTable("Cards");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).HasMaxLength(200).IsRequired();
        builder.Property(c => c.Rarity).HasConversion<string>().HasMaxLength(20);
        builder.Property(c => c.Cost);

        // TPH discriminator
        builder.HasDiscriminator<string>("CardType")
            .HasValue<AllyCard>("Ally")
            .HasValue<EquipmentCard>("Equipment")
            .HasValue<MonsterCard>("Monster")
            .HasValue<TrapCard>("Trap")
            .HasValue<DungeonRoomCard>("DungeonRoom")
            .HasValue<BossCard>("Boss");

        // Ignore abstract computed property
        builder.Ignore(c => c.Type);

        // Indices
        builder.HasIndex(c => c.Rarity);
        builder.HasIndex("CardType");
    }
}

internal class AllyCardConfiguration : IEntityTypeConfiguration<AllyCard>
{
    public void Configure(EntityTypeBuilder<AllyCard> builder)
    {
        builder.Property(a => a.Race).HasColumnName("Race").HasConversion<string>().HasMaxLength(20);
        builder.Property(a => a.Strength).HasColumnName("Strength");
        builder.Property(a => a.HitPoints).HasColumnName("HitPoints");
        builder.Property(a => a.Initiative).HasColumnName("Initiative");
        builder.Property(a => a.Treasure).HasColumnName("Treasure");
        builder.Property(a => a.IsAmbusher);
        builder.Property(a => a.Effect).HasColumnName("Effect").HasMaxLength(500);
    }
}

internal class EquipmentCardConfiguration : IEntityTypeConfiguration<EquipmentCard>
{
    public void Configure(EntityTypeBuilder<EquipmentCard> builder)
    {
        builder.Property(e => e.Slot).HasConversion<string>().HasMaxLength(20);
        builder.Property(e => e.StrengthModifier);
        builder.Property(e => e.HitPointsModifier);
        builder.Property(e => e.InitiativeModifier);
        builder.Property(e => e.Effect).HasColumnName("Effect").HasMaxLength(500);
        builder.Ignore(e => e.IsConsumable);
    }
}

internal class MonsterCardConfiguration : IEntityTypeConfiguration<MonsterCard>
{
    public void Configure(EntityTypeBuilder<MonsterCard> builder)
    {
        builder.Property(m => m.Race).HasColumnName("Race").HasConversion<string>().HasMaxLength(20);
        builder.Property(m => m.Strength).HasColumnName("Strength");
        builder.Property(m => m.HitPoints).HasColumnName("HitPoints");
        builder.Property(m => m.Initiative).HasColumnName("Initiative");
        builder.Property(m => m.Treasure).HasColumnName("Treasure");
        builder.Property(m => m.Effect).HasColumnName("Effect").HasMaxLength(500);
    }
}

internal class TrapCardConfiguration : IEntityTypeConfiguration<TrapCard>
{
    public void Configure(EntityTypeBuilder<TrapCard> builder)
    {
        builder.Property(t => t.Damage);
        builder.Property(t => t.Effect).HasColumnName("Effect").HasMaxLength(500);
    }
}

internal class DungeonRoomCardConfiguration : IEntityTypeConfiguration<DungeonRoomCard>
{
    public void Configure(EntityTypeBuilder<DungeonRoomCard> builder)
    {
        builder.Property(d => d.Order);
        builder.Property(d => d.MonsterCostBudget);
        builder.Property(d => d.Effect).HasColumnName("Effect").HasMaxLength(500);

        // Store Guid lists as jsonb via backing fields
        builder.Property(d => d.MonsterIds)
            .HasColumnName("MonsterIds")
            .HasColumnType("jsonb")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Property(d => d.TrapIds)
            .HasColumnName("TrapIds")
            .HasColumnType("jsonb")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

internal class BossCardConfiguration : IEntityTypeConfiguration<BossCard>
{
    public void Configure(EntityTypeBuilder<BossCard> builder)
    {
        builder.Property(b => b.Race).HasColumnName("Race").HasConversion<string>().HasMaxLength(20);
        builder.Property(b => b.Strength).HasColumnName("Strength");
        builder.Property(b => b.HitPoints).HasColumnName("HitPoints");
        builder.Property(b => b.Initiative).HasColumnName("Initiative");
        builder.Property(b => b.Effect).HasColumnName("Effect").HasMaxLength(500);
    }
}
