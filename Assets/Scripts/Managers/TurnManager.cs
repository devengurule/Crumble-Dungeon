using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private GameObject playerDark;
    [SerializeField] private GameObject enemyDark;
    [SerializeField] private bool isPlayerTurn = true;

    private EventManager eventManager;

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        if (isPlayerTurn)
        {
            playerDark.SetActive(false);
            enemyDark.SetActive(true);
        }

        if (eventManager != null)
        {
            eventManager.Subscribe(EventType.EndOfPlayerTurn, OnEndOfPlayerTurn);
            eventManager.Subscribe(EventType.EndOfEnemiesTurn, OnEndOfEnemiesTurn);
        }
    }

    private void OnEndOfPlayerTurn(object target)
    {
        isPlayerTurn = false;
        playerDark.SetActive(true);
        enemyDark.SetActive(false);
        eventManager.Publish(EventType.TurnChange);
    }

    private void OnEndOfEnemiesTurn(object target)
    {
        isPlayerTurn = true;
        playerDark.SetActive(false);
        enemyDark.SetActive(true);
        eventManager.Publish(EventType.TurnChange);
    }

    public bool IsPlayerTurn()
    {
        return isPlayerTurn;
    }
}
