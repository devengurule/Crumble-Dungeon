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

    // Transition
    Transition,
    TransitionClosed,
    TransitionOpen,

    // Door
    UseDoor,
    CanUseDoor,
    CanNotUseDoor,

    // Misc
    ResetCellType,
    TurnChange,
    GameOver,
    RestartGame,
    SceneChange,
    ChangePlayerSpawnPoint,
    GridComplete,
    CollectGold,
    Escape
}