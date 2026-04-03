using CardgameDungeon.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CardgameDungeon.Infrastructure.Data.Configurations;

internal class TournamentConfiguration : IEntityTypeConfiguration<Tournament>
{
    public void Configure(EntityTypeBuilder<Tournament> builder)
    {
        builder.ToTable("Tournaments");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.RequiredTier).HasConversion<string>().HasMaxLength(20);
        builder.Property(t => t.EntryFee);
        builder.Property(t => t.PrizePool);
        builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(t => t.CurrentRound);

        // Ignore computed property
        builder.Ignore(t => t.ActiveParticipants);

        builder.HasMany(t => t.Participants)
            .WithOne()
            .HasForeignKey("TournamentId")
            .IsRequired();

        builder.Navigation(t => t.Participants).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

internal class TournamentParticipantConfiguration : IEntityTypeConfiguration<TournamentParticipant>
{
    public void Configure(EntityTypeBuilder<TournamentParticipant> builder)
    {
        builder.ToTable("TournamentParticipants");

        // Composite key: TournamentId + PlayerId
        builder.HasKey("TournamentId", nameof(TournamentParticipant.PlayerId));

        builder.Property(tp => tp.PlayerId);
        builder.Property(tp => tp.DeckId);
        builder.Property(tp => tp.IsEliminated);
        builder.Property(tp => tp.EliminatedInRound);
    }
}
