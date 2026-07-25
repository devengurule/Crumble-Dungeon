using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxPlayerHealth;
    [SerializeField] private GameObject healthObject;

    [Header("Enemies")]
    [SerializeField] private int knightDamage;
    [SerializeField] private int rogueDamage;

    [Header("Player Attacks")]
    [SerializeField] private int normalAtkDamage;
    [SerializeField] private int sweepAtkDamage;
    [SerializeField] private int heavyAtkDamage;

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
            eventManager.Subscribe(EventType.HealPlayer, OnHealPlayer);
            eventManager.Subscribe(EventType.AttemptMeleeAttackOnPlayer, OnMeleeAttackOnPlayer);
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
            if(currentPlayerHealth <= 0)
            {
                currentPlayerHealth = 0;
                eventManager.Publish(EventType.GameOver, LoseType.Died);
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

    public int GetRogueDamage()
    {
        return rogueDamage;
    }

    public int GetNormalAtkDamage()
    {
        return normalAtkDamage;
    }

    public int GetSweepAtkDamage()
    {
        return sweepAtkDamage;
    }

    public int GetHeavyAtkDamage()
    {
        return heavyAtkDamage;
    }
}
