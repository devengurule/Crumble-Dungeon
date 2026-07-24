using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int playerHealth;

    private EventManager eventManager;

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        if(eventManager != null)
        {
            eventManager.Subscribe(EventType.DealPlayerDamage, OnDealPlayerDamage);
            eventManager.Subscribe(EventType.HealPlayer, OnHealPlayer);
        }
    }

    private void OnDealPlayerDamage(object target)
    {
        if(target is DamageData data)
        {
            if(data.target.position == GameController.instance.playerPosition)
            {
                playerHealth -= data.damage;
            }
        }
    }

    private void OnHealPlayer(object target)
    {
        if(target is int val)
        {
            playerHealth += val;
        }
    }

    public int GetHealth()
    {
        return playerHealth;
    }
}
