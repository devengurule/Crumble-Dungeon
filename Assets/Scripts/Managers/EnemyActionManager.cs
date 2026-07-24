using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActionManager : MonoBehaviour
{
    [SerializeField] private int maxEnemyActionsPerTurn;
    [SerializeField] private float actionPauseDuration;

    private GameController gameController;
    private EventManager eventManager;
    private bool performingEnemyActions;
    private GameObject[] enemiesList;
    private GameObject currentEnemyInProgress;
    private bool enemyActionInProgress;

    private void Start()
    {
        gameController = GameController.instance;
        eventManager = gameController.eventManager;

        enemiesList = GameObject.FindGameObjectsWithTag("Enemy");

        if ( eventManager != null )
        {
            eventManager.Subscribe(EventType.TurnChange, OnTurnChange);
            eventManager.Subscribe(EventType.EnemyActionComplete, OnEnemyActionComplete);
        }
    }

    private void Update()
    {
        Debug.Log(enemyActionInProgress);
    }

    private void OnTurnChange(object target)
    {
        if (!gameController.IsPlayerTurn())
        {
            PerformEnemyActions();
        }
    }

    private void OnEnemyActionComplete(object target)
    {
        Debug.Log("Action Complete");
        if (target is GameObject obj)
        {
            if (currentEnemyInProgress == obj)
            {
                enemyActionInProgress = false;
            }
        }
    }

    private void PerformEnemyActions()
    {
        if(!performingEnemyActions)
        {
            performingEnemyActions = true;
            StartCoroutine(ExecuteEnemyActions());
        }
    }

    private IEnumerator ExecuteEnemyActions()
    {
        foreach (GameObject enemy in enemiesList)
        {
            currentEnemyInProgress = enemy;
            int currentActionsRemaining = maxEnemyActionsPerTurn;
            while (currentActionsRemaining > 0)
            {
                yield return new WaitForSeconds(actionPauseDuration);
                if (!enemyActionInProgress)
                {
                    if (enemy.GetComponent<KnightScript>().CanAttackPlayer())
                    {
                        // Perform Attack Action

                        enemy.GetComponent<KnightScript>().AttackPlayerAction();
                        enemyActionInProgress = true;

                        Debug.Log("ATTACKING NOW");
                        yield return new WaitUntil(() => !enemyActionInProgress);
                        Debug.Log("ATTACKING END");
                        currentActionsRemaining--;
                    }
                    else if (enemy.GetComponent<KnightScript>().CanMoveToPlayer())
                    {
                        // Perform Move Action
                        enemy.GetComponent<KnightScript>().MoveToPlayerAction();
                        enemyActionInProgress = true;

                        yield return new WaitUntil(() => !enemyActionInProgress);
                        currentActionsRemaining--;
                    }
                    else
                    {
                        // No Actions can be Performed
                        currentActionsRemaining = 0;
                    }
                }
            }
        }
        Debug.Log("END OF ENEMY TURN");
        performingEnemyActions = false;
        eventManager.Publish(EventType.EndOfEnemiesTurn);
    }
}
