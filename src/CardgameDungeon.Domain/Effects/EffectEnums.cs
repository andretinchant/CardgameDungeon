namespace CardgameDungeon.Domain.Effects;

public enum EffectTrigger
{
    Passive,
    OnPlay,
    OnCombatStart,
    OnRoundStart,
    OnKill,
    OnDeath,
    OnInitiative,
    OnActivate,
    OnScrollUsed,
    OnEnemyFlee,
    OnMarkedKill,
    OnMarkedSurvive,
    WithAdvantage,
    WithDisadvantage
}

public enum EffectAction
{
    ModStr,
    ModHp,
    ModInit,
    Damage,
    Heal,
    ExileDeck,
    ExileHand,
    DiscardHand,
    Draw,
    ForfeitTreasure,
    ElimDouble,
    JoinCombat,
    RedirectDamage,
    CancelCombat,
    SearchDeck,
    ReturnHandTop,
    ReturnHandBottom,
    ReturnHandShuffle,
    RevealHand,
    RevealDeck,
    FavoredEnemy,
    MarkEnemy,
    TriggerOppAttack,
    CopyScroll,
    RecoverScroll,
    RecoverScrollFromExile,
    ScrollToBottom,
    ReduceDamage,
    OppAttackDouble,
    IgnoreOppAttackLimit
}

public enum EffectTarget
{
    Self,
    Ally,
    AllAllies,
    Group,
    Enemy,
    AllEnemies,
    EnemyGroup,
    MarkedEnemy,
    Opponent
}

public enum EffectCondition
{
    None,
    IfRace,
    IfClass,
    IfRaging,
    IfEquipped,
    IfScrollUsed,
    OncePerCombat,
    OncePerRoom
}
