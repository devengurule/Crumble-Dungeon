using System.Collections;
using System;
using UnityEngine;

public class EnemyActionManager : MonoBehaviour
{
    [SerializeField] private int maxEnemyActionsPerTurn;
    [SerializeField] private float actionPauseDuration;
    [SerializeField] private float turnChangeDelayDuration;

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
            eventManager.Subscribe(EventType.EnemyDied, OnEnemyDied);
        }
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
        if (target is GameObject obj)
        {
            if (currentEnemyInProgress == obj)
            {
                enemyActionInProgress = false;
            }
        }
    }

    private void OnEnemyDied(object target)
    {
        if (target is GameObject obj)
        {
            foreach (GameObject gameObject in enemiesList)
            {
                if(gameObject == obj)
                {
                    enemiesList[Array.IndexOf(enemiesList, gameObject)] = null;
                    currentEnemyInProgress = null;
                }
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
        yield return new WaitForSeconds(turnChangeDelayDuration);

        enemiesList = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemiesList)
        {
            Debug.Log(enemy.name);
            currentEnemyInProgress = enemy;
            int currentActionsRemaining = maxEnemyActionsPerTurn;
            while (currentActionsRemaining > 0 && enemy != null)
            {
                yield return new WaitForSeconds(actionPauseDuration);
                if (!enemyActionInProgress)
                {
                    if (enemy.GetComponent<KnightScript>().CanAttackPlayer())
                    {
                        // Perform Attack Action

                        enemy.GetComponent<KnightScript>().AttackPlayerAction();
                        enemyActionInProgress = true;

                        yield return new WaitUntil(() => !enemyActionInProgress);
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
        performingEnemyActions = false;
        eventManager.Publish(EventType.EndOfEnemiesTurn);
    }
}
