using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxPlayerHealth;
    [SerializeField] private GameObject healthObject;

    private EventManager eventManager;
    private TextMeshProUGUI healthText;
    private int currentPlayerHealth;

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        currentPlayerHealth = maxPlayerHealth;

        healthText = healthObject.GetComponent<TextMeshProUGUI>();
        healthText.text = currentPlayerHealth.ToString();

        if (eventManager != null)
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
                currentPlayerHealth -= data.damage;
            }
        }
    }

    private void OnHealPlayer(object target)
    {
        if(target is int val)
        {
            currentPlayerHealth += val;
        }
    }

    public int GetMaxHealth()
    {
        return maxPlayerHealth;
    }

    public int GetHealth()
    {
        return currentPlayerHealth;
    }
}
