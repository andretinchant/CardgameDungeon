using CardgameDungeon.Domain.Enums;
using MediatR;

namespace CardgameDungeon.Features.Elo.GetPlayerRank;

public record GetPlayerRankQuery(Guid PlayerId) : IRequest<PlayerRankResponse>;

public record PlayerRankResponse(
    Guid PlayerId,
    int Elo,
    Tier Tier,
    int Wins,
    int Losses,
    int Rank);
