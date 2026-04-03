using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Features.Tournament.AdvanceTournament;
using CardgameDungeon.Tests.Matchmaking.Fakes;

namespace CardgameDungeon.Tests.Matchmaking;

public class AdvanceTournamentHandlerTests
{
    private readonly FakeTournamentRepository _tournamentRepo = new();
    private AdvanceTournamentHandler Handler => new(_tournamentRepo);

    private Tournament MakeFullTournament()
    {
        var tournament = new Tournament(Guid.NewGuid(), Tier.Gold, 100);
        for (var i = 0; i < 8; i++)
        {
            var playerId = Guid.NewGuid();
            var rating = new PlayerRating(playerId);
            rating.SetTier(Tier.Gold);
            tournament.AddParticipant(playerId, Guid.NewGuid(), Tier.Gold);
        }
        tournament.Start();
        return tournament;
    }

    [Fact]
    public async Task EliminatePlayer_ReducesActive()
    {
        var tournament = MakeFullTournament();
        _tournamentRepo.Seed(tournament);

        var loser = tournament.Participants[0].PlayerId;

        var response = await Handler.Handle(
            new AdvanceTournamentCommand(tournament.Id, loser), CancellationToken.None);

        Assert.Equal(7, response.RemainingPlayers);
    }

    [Fact]
    public async Task EliminateHalf_AdvancesRound()
    {
        var tournament = MakeFullTournament();
        _tournamentRepo.Seed(tournament);

        // Eliminate 4 players (round 1: 8→4)
        for (var i = 0; i < 4; i++)
        {
            await Handler.Handle(
                new AdvanceTournamentCommand(tournament.Id, tournament.Participants[i].PlayerId),
                CancellationToken.None);
        }

        Assert.Equal(2, tournament.CurrentRound);
        Assert.Equal(TournamentStatus.InProgress, tournament.Status);
    }

    [Fact]
    public async Task TournamentNotStarted_Throws()
    {
        var tournament = new Tournament(Guid.NewGuid(), Tier.Gold, 100);
        _tournamentRepo.Seed(tournament);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Handler.Handle(new AdvanceTournamentCommand(tournament.Id, Guid.NewGuid()),
                CancellationToken.None));
    }

    [Fact]
    public async Task TournamentNotFound_Throws()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            Handler.Handle(new AdvanceTournamentCommand(Guid.NewGuid(), Guid.NewGuid()),
                CancellationToken.None));
    }
}
