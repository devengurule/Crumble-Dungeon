using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RogueScript : MonoBehaviour
{
    [SerializeField] private float moveDuration;
    [SerializeField] private float attackDuration;
    [SerializeField] private int moveRange;

    private GameController gameController;
    private EventManager eventManager;

    List<Vector2Int> finalPath = new();
    private Coroutine moveCoroutine;
    private Vector2Int currentPosition;
    private CellData rogueData = new();
    private bool canAttack = true;

    private Vector2Int randomPoint;

    private void Start()
    {
        gameController = GameController.instance;
        eventManager = gameController.eventManager;

        currentPosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);

        rogueData.position = currentPosition;
        rogueData.cellType = CellType.enemy;

        gameController.UpdateCellData(rogueData);

        if (eventManager != null)
        {
            eventManager.Subscribe(EventType.EnemyAttackSuccessful, OnSuccessfulAttack);
        }
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.EnemyAttackSuccessful, OnSuccessfulAttack);
        }
    }

    private void OnSuccessfulAttack(object target)
    {
        if (!canAttack)
        {
            StartCoroutine(WaitForAttack());
        }
    }

    private IEnumerator MoveOnPath(List<Vector2Int> path)
    {
        Vector3 target = new Vector3(position.x, position.y, 0);
        Vector3 start = transform.position;
        float duration = moveDuration;
        float time = 0f;

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

    private IEnumerator WaitForAttack()
    {
        yield return new WaitForSeconds(attackDuration);

        canAttack = true;
        eventManager.Publish(EventType.EnemyActionComplete, this.gameObject);
    }

    private void ResetMove()
    {
        StopCoroutine(moveCoroutine);
        moveCoroutine = null;

        finalPath.RemoveAt(0);

        eventManager.Publish(EventType.EnemyActionComplete, this.gameObject);

        CellData oldCell = rogueData;
        oldCell.cellType = CellType.empty;

        rogueData.position = currentPosition;
        rogueData.cellType = CellType.enemy;

        gameController.UpdateCellData(oldCell, rogueData);
    }

    private float DistanceToPlayer()
    {
        return Mathf.Abs((currentPosition - gameController.playerPosition).magnitude);
    }

    private CellData RandomCellAroundEnemy(int range)
    {
        List<CellData> cells = new();
        CellData cell = new();

        Vector2Int myPosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);

        Vector2Int bottomLeft = new Vector2Int(myPosition.x - range, myPosition.y - range);
        Vector2Int topRight = new Vector2Int(myPosition.x + range, myPosition.y + range);

        // Top to bottom
        for (int y = bottomLeft.y; y < topRight.y + 1; y++)
        {
            // Left to right
            for (int x = bottomLeft.x; x < topRight.x + 1; x++)
            {
                Vector2Int position = new Vector2Int(x, y);

                CellType cellType = GameController.instance.GetCellType(position);
                if (cellType == CellType.empty)
                {
                    cell.position = position;
                    cell.cellType = cellType;

                    cells.Add(cell);
                }
            }
        }
        int cellCount = cells.Count;

        return cells[Random.Range(0,cellCount)];
    }

    public bool CanAttackPlayer()
    {
        if (DistanceToPlayer() <= Mathf.Sqrt(2) && canAttack)
        {
            return true;
        }
        return false;
    }

    public bool CanMoveToCell()
    {
        List<Vector2Int> path = GetComponent<PathFinder>().FindPath(rogueData, RandomCellAroundEnemy(moveRange)));

        if (path != null) return true;
        return false;
    }

    public void AttackPlayerAction()
    {
        canAttack = false;

        eventManager.Publish(EventType.AttemptMeleeAttackOnPlayer, gameController.GetRogueDamage());
    }

    public void MoveToPlayerAction()
    {
        finalPath = GetComponent<PathFinder>().FindPath(rogueData, gameController.GetCellData(gameController.playerPosition));
        moveCoroutine = StartCoroutine(MoveOnPath(finalPath));
    }
}
