using System.Collections;
using System;
using UnityEngine;

public class EnemyActionManager : MonoBehaviour
{
    [SerializeField] private int knightActionsPerTurn;
    [SerializeField] private int rougeActionsPerTurn;
    [SerializeField] private int archerActionsPerTurn;

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
            currentEnemyInProgress = enemy;

            if (!enemyActionInProgress && enemy != null)
            {
                if (enemy.GetComponent<KnightScript>() != null)
                {
                    int currentActionsRemaining = knightActionsPerTurn;
                    while (currentActionsRemaining > 0)
                    {
                        yield return new WaitForSeconds(actionPauseDuration);

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
                else if(enemy.GetComponent<RogueScript>() != null)
                {
                    int currentActionsRemaining = rougeActionsPerTurn;
                    while (currentActionsRemaining > 0)
                    {
                        yield return new WaitForSeconds(actionPauseDuration);

                        if (enemy.GetComponent<RogueScript>().CanAttackPlayer())
                        {
                            // Perform Attack Action
                            enemy.GetComponent<RogueScript>().AttackPlayerAction();
                            enemyActionInProgress = true;

                            yield return new WaitUntil(() => !enemyActionInProgress);
                            currentActionsRemaining--;
                        }
                        else if (enemy.GetComponent<RogueScript>().CanMoveToCell())
                        {
                            // Perform Move Action
                            enemy.GetComponent<RogueScript>().MoveAction();
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
                else if (enemy.GetComponent<ArcherScript>() != null)
                {
                    int currentActionsRemaining = archerActionsPerTurn;
                    while (currentActionsRemaining > 0)
                    {
                        yield return new WaitForSeconds(actionPauseDuration);

                        if (enemy.GetComponent<ArcherScript>().CanAttackPlayer())
                        {
                            // Perform Attack Action
                            enemy.GetComponent<ArcherScript>().AttackPlayerAction();
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
        }

        performingEnemyActions = false;
        eventManager.Publish(EventType.EndOfEnemiesTurn);
    }

    public float GetActionPauseDuration()
    {
        return actionPauseDuration;
    }
}
