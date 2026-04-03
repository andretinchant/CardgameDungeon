using MediatR;

namespace CardgameDungeon.Features.Match.ResolveInitiative;

public record ResolveInitiativeCommand(Guid MatchId) : IRequest<InitiativeResponse>;

public record InitiativeResponse(
    Guid MatchId,
    int Player1Total,
    int Player2Total,
    Guid? WinnerId,
    bool IsTied);
