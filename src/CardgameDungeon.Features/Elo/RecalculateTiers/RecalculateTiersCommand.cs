using MediatR;

namespace CardgameDungeon.Features.Elo.RecalculateTiers;

public record RecalculateTiersCommand : IRequest<RecalculateTiersResponse>;

public record RecalculateTiersResponse(
    int TotalPlayers,
    int BronzeCount,
    int SilverCount,
    int GoldCount,
    int BronzeCutoff,
    int GoldCutoff);
