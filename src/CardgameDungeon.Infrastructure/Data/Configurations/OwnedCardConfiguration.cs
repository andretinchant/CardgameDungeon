using CardgameDungeon.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CardgameDungeon.Infrastructure.Data.Configurations;

internal class OwnedCardConfiguration : IEntityTypeConfiguration<OwnedCard>
{
    public void Configure(EntityTypeBuilder<OwnedCard> builder)
    {
        builder.ToTable("OwnedCards");
        builder.HasKey(oc => oc.Id);

        builder.Property(oc => oc.CardId);
        builder.Property(oc => oc.PlayerId);
        builder.Property(oc => oc.IsReserved);

        builder.HasIndex(oc => oc.PlayerId);
        builder.HasIndex(oc => oc.CardId);
    }
}
