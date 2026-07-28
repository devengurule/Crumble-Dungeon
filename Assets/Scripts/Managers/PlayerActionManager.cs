using TMPro;
using UnityEngine;

public class PlayerActionManager : MonoBehaviour
{
    [SerializeField] private int maxActionsPerTurn;
    [SerializeField] private GameObject actionsTextObject;

    private TextMeshProUGUI actionsText;
    private int currentActionsRemaining;
    private bool actionInProgress;
    private GameController gameController;
    private EventManager eventManager;

    private void Start()
    {
        gameController = GameController.instance;
        eventManager = gameController.eventManager;

        actionsText = actionsTextObject.GetComponent<TextMeshProUGUI>();
        
        currentActionsRemaining = maxActionsPerTurn;
        actionsText.text = currentActionsRemaining.ToString();

        if (eventManager != null)
        {
            eventManager.Subscribe(EventType.TurnChange, OnTurnChange);
            eventManager.Subscribe(EventType.RequestUseAction, OnRequestUseAction);
            eventManager.Subscribe(EventType.PlayerActionComplete, OnPlayerActionComplete);
            eventManager.Subscribe(EventType.GrantedUseAction, OnGrantSkip);
            eventManager.Subscribe(EventType.RestartGame, OnRestart);
        }
    }

    private void OnTurnChange(object target)
    {
        if (!gameController.IsPlayerTurn())
        {
            ResetActions();
        }
        else
        {
            actionsText.text = currentActionsRemaining.ToString();
        }
    }
    private void OnRestart(object target)
    {
        currentActionsRemaining = maxActionsPerTurn;
        actionsText.text = currentActionsRemaining.ToString();
    }

    private void OnRequestUseAction(object target)
    {
        if (gameController.IsPlayerTurn() && currentActionsRemaining > 0 && !actionInProgress)
        {
            actionInProgress = true;
            currentActionsRemaining--;
            actionsText.text = currentActionsRemaining.ToString();
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

    public void SkipTurn()
    {
        eventManager.Publish(EventType.RequestUseAction, gameObject);
    }

    private void OnGrantSkip(object target)
    {
        if (target is GameObject obj)
        {
            currentActionsRemaining = 0;
            actionsText.text = currentActionsRemaining.ToString();
            if (obj == gameObject) eventManager.Publish(EventType.PlayerActionComplete);
        }
    }
}
