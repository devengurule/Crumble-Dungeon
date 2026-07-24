public enum EventType
{
    // Player
    ChangePlayerPosition,
    PlayerSpawned,
    PlayerActionComplete,
    GrantedUseAction,
    RequestUseAction,
    EndOfPlayerTurn,

    // Attacks
    PerformNormalAttack,
    PerformSweepAttack,
    PerformHeavyAttack,

    // Enemies
    AttemptMeleeAttackOnPlayer,
    EnemyAttackSuccessful,
    EnemyActionComplete,
    EndOfEnemiesTurn,

    // Health
    HealPlayer,
    DealEnemyDamage,
    DealPlayerDamage,
    EnemyDied,

    // Cell Selection
    MoveCellSelected,
    AtkCellSelected,
    SweepAtkCellSelected,
    HeavyAtkCellSelected,

    // Misc
    ResetCellType,
    TurnChange
}