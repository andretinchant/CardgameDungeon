using CardgameDungeon.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CardgameDungeon.Infrastructure.Data.Configurations;

internal class PlayerWalletConfiguration : IEntityTypeConfiguration<PlayerWallet>
{
    public void Configure(EntityTypeBuilder<PlayerWallet> builder)
    {
        builder.ToTable("PlayerWallets");
        builder.HasKey(pw => pw.PlayerId);

        builder.Property(pw => pw.Balance);
        builder.Property(pw => pw.LastDailyRewardDate);
        builder.Property(pw => pw.DailyRewardsClaimedToday);
    }
}
