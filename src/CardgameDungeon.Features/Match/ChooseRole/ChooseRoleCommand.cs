using CardgameDungeon.Features.Match.Shared;
using MediatR;

namespace CardgameDungeon.Features.Match.ChooseRole;

public record ChooseRoleCommand(
    Guid MatchId,
    Guid PlayerId,
    bool ChoosesToAttack) : IRequest<MatchResponse>;
