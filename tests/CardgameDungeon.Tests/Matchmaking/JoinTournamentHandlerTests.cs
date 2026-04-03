using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Features.Tournament.JoinTournament;
using CardgameDungeon.Tests.Matchmaking.Fakes;
using CardgameDungeon.Tests.MetaSystems.Fakes;

namespace CardgameDungeon.Tests.Matchmaking;

public class JoinTournamentHandlerTests
{
    private readonly FakeTournamentRepository _tournamentRepo = new();
    private readonly FakeWalletRepository _walletRepo = new();
    private readonly FakeRatingRepository _ratingRepo = new();
    private JoinTournamentHandler Handler => new(_tournamentRepo, _walletRepo, _ratingRepo);

    [Fact]
    public async Task ValidJoin_DeductsEntryFee()
    {
        var playerId = Guid.NewGuid();
        var tournament = new Tournament(Guid.NewGuid(), Tier.Gold, 100);
        _tournamentRepo.Seed(tournament);

        var rating = new PlayerRating(playerId, 1500);
        rating.SetTier(Tier.Gold);
        _ratingRepo.Seed(rating);
        _walletRepo.Seed(new PlayerWallet(playerId, 500));

        var response = await Handler.Handle(
            new JoinTournamentCommand(tournament.Id, playerId, Guid.NewGuid()),
            CancellationToken.None);

        Assert.Equal(1, response.ParticipantCount);
        Assert.Equal(100, response.PrizePool);

        var wallet = await _walletRepo.GetByPlayerIdAsync(playerId);
        Assert.Equal(400, wallet!.Balance);
    }

    [Fact]
    public async Task WrongTier_Throws()
    {
        var playerId = Guid.NewGuid();
        var tournament = new Tournament(Guid.NewGuid(), Tier.Gold, 100);
        _tournamentRepo.Seed(tournament);

        var rating = new PlayerRating(playerId, 800);
        rating.SetTier(Tier.Bronze);
        _ratingRepo.Seed(rating);
        _walletRepo.Seed(new PlayerWallet(playerId, 500));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Handler.Handle(new JoinTournamentCommand(tournament.Id, playerId, Guid.NewGuid()),
                CancellationToken.None));
    }

    [Fact]
    public async Task InsufficientBalance_Throws()
    {
        var playerId = Guid.NewGuid();
        var tournament = new Tournament(Guid.NewGuid(), Tier.Gold, 100);
        _tournamentRepo.Seed(tournament);

        var rating = new PlayerRating(playerId);
        rating.SetTier(Tier.Gold);
        _ratingRepo.Seed(rating);
        _walletRepo.Seed(new PlayerWallet(playerId, 10));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Handler.Handle(new JoinTournamentCommand(tournament.Id, playerId, Guid.NewGuid()),
                CancellationToken.None));
    }

    [Fact]
    public async Task DuplicateRegistration_Throws()
    {
        var playerId = Guid.NewGuid();
        var tournament = new Tournament(Guid.NewGuid(), Tier.Gold, 100);
        _tournamentRepo.Seed(tournament);

        var rating = new PlayerRating(playerId);
        rating.SetTier(Tier.Gold);
        _ratingRepo.Seed(rating);
        _walletRepo.Seed(new PlayerWallet(playerId, 500));

        await Handler.Handle(
            new JoinTournamentCommand(tournament.Id, playerId, Guid.NewGuid()),
            CancellationToken.None);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Handler.Handle(new JoinTournamentCommand(tournament.Id, playerId, Guid.NewGuid()),
                CancellationToken.None));
    }
}
