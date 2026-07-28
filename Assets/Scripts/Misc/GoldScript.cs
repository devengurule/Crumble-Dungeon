using TMPro;
using UnityEngine;

public class GoldScript : MonoBehaviour
{
    [SerializeField] private GameObject goldObject;

    private EventManager eventManager;

    private int goldCollected;

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        UpdateText();

        if ( eventManager != null)
        {
            eventManager.Subscribe(EventType.CollectGold, OnCollectGold);
            eventManager.Subscribe(EventType.RestartGame, OnRestart);
        }
    }

    private void OnRestart(object target)
    {
        goldCollected = 0;
        UpdateText();
    }

    private void OnCollectGold(object target)
    {
        goldCollected++;
        UpdateText();
    }

    private void UpdateText()
    {
        goldObject.GetComponent<TextMeshProUGUI>().text = goldCollected.ToString();
    }

}
