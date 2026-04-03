using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Features.Tournament.CreateTournament;
using CardgameDungeon.Tests.Matchmaking.Fakes;

namespace CardgameDungeon.Tests.Matchmaking;

public class CreateTournamentHandlerTests
{
    private readonly FakeTournamentRepository _tournamentRepo = new();
    private CreateTournamentHandler Handler => new(_tournamentRepo);

    [Fact]
    public async Task ValidCreate_ReturnsTournament()
    {
        var response = await Handler.Handle(
            new CreateTournamentCommand(Tier.Gold, 100), CancellationToken.None);

        Assert.Equal(Tier.Gold, response.RequiredTier);
        Assert.Equal(100, response.EntryFee);
        Assert.Equal(TournamentStatus.Registration, response.Status);
        Assert.Equal(0, response.ParticipantCount);
    }
}
