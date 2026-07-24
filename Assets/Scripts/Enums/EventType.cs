public enum EventType
{
    // Player
    ChangePlayerPosition,
    PlayerSpawned,
    PlayerActionComplete,
    GrantedUseAction,
    RequestUseAction,
    EndOfPlayerTurn,

    // Enemies
    AttemptMeleeAttackOnPlayer,
    EnemyAttackSuccessful,
    EnemyActionComplete,
    EndOfEnemiesTurn,

    // Health
    HealPlayer,
    DealEnemyDamage,
    DealPlayerDamage,

    // Cell Selection
    MoveCellSelected,
    AtkCellSelected,
    SweepAtkCellSelected,
    HeavyAtkCellSelected,

    // Misc
    ResetCellType,
    TurnChange
}