using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Features.Tournament.FinalizeTournament;
using CardgameDungeon.Tests.Matchmaking.Fakes;
using CardgameDungeon.Tests.MetaSystems.Fakes;

namespace CardgameDungeon.Tests.Matchmaking;

public class FinalizeTournamentHandlerTests
{
    private readonly FakeTournamentRepository _tournamentRepo = new();
    private readonly FakeWalletRepository _walletRepo = new();
    private FinalizeTournamentHandler Handler => new(_tournamentRepo, _walletRepo);

    private Tournament MakeFinishedTournament()
    {
        var tournament = new Tournament(Guid.NewGuid(), Tier.Gold, 100);
        var players = new List<Guid>();

        for (var i = 0; i < 8; i++)
        {
            var playerId = Guid.NewGuid();
            players.Add(playerId);
            tournament.AddParticipant(playerId, Guid.NewGuid(), Tier.Gold);
            _walletRepo.Seed(new PlayerWallet(playerId, 0));
        }

        tournament.Start();

        // Round 1: eliminate players 0-3
        for (var i = 0; i < 4; i++)
            tournament.EliminatePlayer(players[i]);
        tournament.AdvanceRound();

        // Round 2: eliminate players 4-5
        tournament.EliminatePlayer(players[4]);
        tournament.EliminatePlayer(players[5]);
        tournament.AdvanceRound();

        // Round 3 (final): eliminate player 6
        tournament.EliminatePlayer(players[6]);
        tournament.AdvanceRound(); // → Finished

        return tournament;
    }

    [Fact]
    public async Task FinishedTournament_DistributesPrizes()
    {
        var tournament = MakeFinishedTournament();
        _tournamentRepo.Seed(tournament);

        var response = await Handler.Handle(
            new FinalizeTournamentCommand(tournament.Id), CancellationToken.None);

        // Prize pool: 8 * 100 = 800
        Assert.Equal(400, response.FirstPrize);   // 50%
        Assert.Equal(240, response.SecondPrize);   // 30%
        Assert.Equal(160, response.ThirdPrize);    // 20% (remainder)

        // Verify wallets received prizes
        var first = await _walletRepo.GetByPlayerIdAsync(response.FirstPlaceId);
        var second = await _walletRepo.GetByPlayerIdAsync(response.SecondPlaceId);
        var third = await _walletRepo.GetByPlayerIdAsync(response.ThirdPlaceId);

        Assert.Equal(400, first!.Balance);
        Assert.Equal(240, second!.Balance);
        Assert.Equal(160, third!.Balance);
    }

    [Fact]
    public async Task PrizePoolSumsCorrectly()
    {
        var tournament = MakeFinishedTournament();
        _tournamentRepo.Seed(tournament);

        var response = await Handler.Handle(
            new FinalizeTournamentCommand(tournament.Id), CancellationToken.None);

        var totalDistributed = response.FirstPrize + response.SecondPrize + response.ThirdPrize;
        Assert.Equal(tournament.PrizePool, totalDistributed);
    }

    [Fact]
    public async Task TournamentNotFound_Throws()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            Handler.Handle(new FinalizeTournamentCommand(Guid.NewGuid()), CancellationToken.None));
    }
}
