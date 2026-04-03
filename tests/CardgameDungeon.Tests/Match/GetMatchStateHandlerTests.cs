using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Features.Match.GetMatchState;
using CardgameDungeon.Tests.Match.Fakes;

namespace CardgameDungeon.Tests.Match;

public class GetMatchStateHandlerTests
{
    private readonly FakeMatchRepository _matchRepo = new();

    private GetMatchStateHandler Handler => new(_matchRepo);

    [Fact]
    public async Task ExistingMatch_ReturnsFullState()
    {
        var match = MatchTestHelper.MakeMatchInSetup();
        _matchRepo.Seed(match);

        var response = await Handler.Handle(
            new GetMatchStateQuery(match.Id),
            CancellationToken.None);

        Assert.Equal(match.Id, response.Id);
        Assert.Equal(MatchPhase.Setup, response.Phase);
        Assert.Equal(match.Player1.PlayerId, response.Player1.PlayerId);
        Assert.Equal(match.Player2.PlayerId, response.Player2.PlayerId);
    }

    [Fact]
    public async Task MatchNotFound_Throws()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            Handler.Handle(
                new GetMatchStateQuery(Guid.NewGuid()),
                CancellationToken.None));
    }
}
