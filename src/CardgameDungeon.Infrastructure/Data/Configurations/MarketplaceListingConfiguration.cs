using CardgameDungeon.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CardgameDungeon.Infrastructure.Data.Configurations;

internal class MarketplaceListingConfiguration : IEntityTypeConfiguration<MarketplaceListing>
{
    public void Configure(EntityTypeBuilder<MarketplaceListing> builder)
    {
        builder.ToTable("MarketplaceListings");
        builder.HasKey(ml => ml.Id);

        builder.Property(ml => ml.SellerId);
        builder.Property(ml => ml.OwnedCardId);
        builder.Property(ml => ml.CardId);
        builder.Property(ml => ml.Price);
        builder.Property(ml => ml.IsActive);
        builder.Property(ml => ml.CreatedAt);

        builder.HasIndex(ml => ml.IsActive);
        builder.HasIndex(ml => ml.SellerId);
    }
}
