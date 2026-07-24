using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class KnightScript : MonoBehaviour
{
    [SerializeField] private float moveDuration;
    [SerializeField] private float pauseDuration;

    private GameController gameController;
    private EventManager eventManager;

    List<Vector2Int> finalPath = new();
    private Coroutine moveCoroutine;
    private Vector2Int currentPosition;
    private CellData knightData = new();
    private bool canAttack = true;

    private void Start()
    {
        gameController = GameController.instance;
        eventManager = gameController.eventManager;

        currentPosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);

        knightData.position = currentPosition;
        knightData.cellType = CellType.enemy;

        gameController.UpdateCellData(knightData);
    }

    public bool CanAttackPlayer()
    {
        if (DistanceToPlayer() <= Mathf.Sqrt(2) && canAttack)
        {
            return true;
        }
        return false;
    }

    public bool CanMoveToPlayer()
    {
        List<Vector2Int> path = GetComponent<PathFinder>().FindPath(knightData, gameController.GetCellData(gameController.playerPosition));

        if (path != null) return true;
        return false;
    }


    private IEnumerator MoveOneCell(Vector2Int position)
    {
        Vector3 target = new Vector3(position.x, position.y, 0);
        Vector3 start = transform.position;
        float duration = moveDuration;
        float time = 0f;

        yield return new WaitForSeconds(pauseDuration);

        if (start != target)
        {

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;
                t = Mathf.SmoothStep(0f, 1f, t);

                transform.position = Vector3.Lerp(start, target, t);

                yield return null;
            }

            transform.position = target;
            currentPosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        }
        ResetMove();
    }

    public void AttackPlayerAction()
    {
        canAttack = false;
        eventManager.Publish(EventType.EnemyActionComplete, this.gameObject);
        canAttack = true;
    }
    public void MoveToPlayerAction()
    {
        finalPath = GetComponent<PathFinder>().FindPath(knightData, gameController.GetCellData(gameController.playerPosition));
        moveCoroutine = StartCoroutine(MoveOneCell(finalPath[0]));
    }

    private void ResetMove()
    {
        StopCoroutine(moveCoroutine);
        moveCoroutine = null;

        finalPath.RemoveAt(0);
        
        eventManager.Publish(EventType.EnemyActionComplete, this.gameObject);

        CellData oldCell = knightData;
        oldCell.cellType = CellType.empty;

        knightData.position = currentPosition;
        knightData.cellType = CellType.enemy;

        gameController.UpdateCellData(oldCell, knightData);
    }

    private float DistanceToPlayer()
    {
        return Mathf.Abs((currentPosition - gameController.playerPosition).magnitude);
    }
}
