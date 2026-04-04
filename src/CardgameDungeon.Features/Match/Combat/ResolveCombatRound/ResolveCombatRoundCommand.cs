using CardgameDungeon.Domain.Enums;
using MediatR;

namespace CardgameDungeon.Features.Match.Combat.ResolveCombatRound;

public record ResolveCombatRoundCommand(Guid MatchId) : IRequest<ResolveCombatRoundResponse>;

public record CombatResultDto(
    Guid AttackerId,
    Guid DefenderId,
    int AttackerStrength,
    int DefenderStrength,
    int DamageToAttacker,
    int DamageToDefender,
    CombatOutcome Outcome,
    bool AttackerEliminated,
    bool DefenderEliminated);

public record PostCombatEventDto(
    Guid SourceCardId,
    string SourceCardName,
    string Trigger,
    string Action,
    int Value);

public record ResolveCombatRoundResponse(
    Guid MatchId,
    IReadOnlyList<CombatResultDto> Results,
    CombatOutcome OverallOutcome,
    bool SimultaneousElimination,
    string Phase,
    IReadOnlyList<PostCombatEventDto>? PostCombatEvents = null);
