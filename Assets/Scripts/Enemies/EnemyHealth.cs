using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxEnemyHealth;

    private int currentEnemyHealth;

    private GameController gameController;
    private EventManager eventManager;

    private void Start()
    {
        gameController = GameController.instance;
        eventManager = gameController.eventManager;

        currentEnemyHealth = maxEnemyHealth;
        
        if (eventManager != null)
        {
            eventManager.Subscribe(EventType.DealEnemyDamage, OnMeleeAttackOnEnemy);
        }
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.DealEnemyDamage, OnMeleeAttackOnEnemy);
        }
    }

    private void OnMeleeAttackOnEnemy(object target)
    {
        if (target is DamageData data)
        {
            if(data.position == new Vector2Int((int)transform.position.x, (int)transform.position.y))
            {
                
                currentEnemyHealth -= data.damage;
                if (currentEnemyHealth <= 0)
                {
                    currentEnemyHealth = 0;
                    Dead();
                }
            }
        }
    }

    private void Dead()
    {
        CellData data = new();
        data.position = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        data.cellType = CellType.empty;

        gameController.UpdateCellData(data);

        eventManager.Publish(EventType.EnemyDied, gameObject);
        Destroy(gameObject);
    }
}
