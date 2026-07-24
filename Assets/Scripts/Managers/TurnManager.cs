using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private bool isPlayerTurn = true;

    private EventManager eventManager;

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        if(eventManager != null)
        {
            eventManager.Subscribe(EventType.EndOfPlayerTurn, OnEndOfPlayerTurn);
            eventManager.Subscribe(EventType.EndOfEnemiesTurn, OnEndOfEnemiesTurn);
        }
    }

    private void OnEndOfPlayerTurn(object target)
    {
        isPlayerTurn = false;
        eventManager.Publish(EventType.TurnChange);
    }

    private void OnEndOfEnemiesTurn(object target)
    {
        isPlayerTurn = true;
        eventManager.Publish(EventType.TurnChange);
    }

    public bool IsPlayerTurn()
    {
        return isPlayerTurn;
    }
}
