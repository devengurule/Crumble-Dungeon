using UnityEngine;

public class PlayerActionManager : MonoBehaviour
{
    [SerializeField] private int maxActionsPerTurn;

    private int currentActionsRemaining;
    private bool actionInProgress;
    private GameController gameController;
    private EventManager eventManager;

    private void Start()
    {
        gameController = GameController.instance;
        eventManager = gameController.eventManager;

        currentActionsRemaining = maxActionsPerTurn;

        if(eventManager != null)
        {
            eventManager.Subscribe(EventType.TurnChange, OnTurnChange);
            eventManager.Subscribe(EventType.RequestUseAction, OnRequestUseAction);
            eventManager.Subscribe(EventType.PlayerActionComplete, OnPlayerActionComplete);
        }
    }

    private void OnTurnChange(object target)
    {
        if (!gameController.IsPlayerTurn())
        {
            ResetActions();
        }
    }

    private void OnRequestUseAction(object target)
    {
        if (gameController.IsPlayerTurn() && currentActionsRemaining > 0 && !actionInProgress)
        {
            actionInProgress = true;
            currentActionsRemaining--;
            eventManager.Publish(EventType.GrantedUseAction, target);
        }
    }

    private void OnPlayerActionComplete(object target)
    {
        if (currentActionsRemaining <= 0) eventManager.Publish(EventType.EndOfPlayerTurn);
        actionInProgress = false;
    }

    public void ResetActions()
    {
        currentActionsRemaining = maxActionsPerTurn;
    }
}
