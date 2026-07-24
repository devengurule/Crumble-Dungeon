using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxPlayerHealth;
    [SerializeField] private GameObject healthObject;

    [SerializeField] private int knightDamage;

    private EventManager eventManager;
    private TextMeshProUGUI healthText;
    private int currentPlayerHealth;

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        currentPlayerHealth = maxPlayerHealth;

        healthText = healthObject.GetComponent<TextMeshProUGUI>();
        UpdateHealthMonitor();

        if (eventManager != null)
        {
            eventManager.Subscribe(EventType.DealPlayerDamage, OnDealPlayerDamage);
            eventManager.Subscribe(EventType.HealPlayer, OnHealPlayer);
            eventManager.Subscribe(EventType.AttemptMeleeAttackOnPlayer, OnMeleeAttackOnPlayer);
        }
    }

    private void OnDealPlayerDamage(object target)
    {
        if (target is DamageData data)
        {
            if (data.target.position == GameController.instance.playerPosition)
            {
                currentPlayerHealth -= data.damage;
            }
        }
    }

    private void OnHealPlayer(object target)
    {
        if (target is int val)
        {
            currentPlayerHealth += val;
        }
    }

    private void OnMeleeAttackOnPlayer(object target)
    {
        if(target is int val)
        {
            currentPlayerHealth -= val;
            if(currentPlayerHealth < 0)
            {
                currentPlayerHealth = 0;
            }
            UpdateHealthMonitor();
            eventManager.Publish(EventType.EnemyAttackSuccessful);
        }
    }

    private void UpdateHealthMonitor()
    {
        healthText.text = currentPlayerHealth.ToString();
    }

    public int GetMaxHealth()
    {
        return maxPlayerHealth;
    }

    public int GetHealth()
    {
        return currentPlayerHealth;
    }

    public int GetKnightDamage()
    {
        return knightDamage;
    }
}
