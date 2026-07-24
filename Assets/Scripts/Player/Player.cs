using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveDuration;

    private EventManager eventManager;
    private GameController gameController;

    private void Start()
    {
        gameController = GameController.instance;
        eventManager = gameController.eventManager;


        if (eventManager != null)
        {
            eventManager.Subscribe(EventType.ChangePlayerPosition, OnPlayerPositionChange);
            eventManager.Subscribe(EventType.PerformNormalAttack, OnNormalAttack);
            eventManager.Subscribe(EventType.PerformSweepAttack, OnPlayerSweepAttack);
            eventManager.Subscribe(EventType.PerformHeavyAttack, OnPlayerHeavyAttack);
        }
    }
    private void OnPlayerPositionChange(object target)
    {
        if (target is Vector2Int vector)
        {
            Vector2Int currentPosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);
            eventManager.Publish(EventType.ResetCellType, currentPosition);
            StartCoroutine(MoveToCell(vector));
        }
    }

    private void OnNormalAttack(object target)
    {
        if (target is Vector2Int position) {
            DamageData data = new();
            data.position = position;
            data.damage = gameController.GetNormalAtkDamage();

            gameController.DealDamageToEnemy(data);

            eventManager.Publish(EventType.PlayerActionComplete);
        }
    }
    private void OnPlayerSweepAttack(object target)
    {
        DamageData data = new();
        data.damage = gameController.GetSweepAtkDamage();

        Vector2Int bottomLeft = new Vector2Int(gameController.playerPosition.x - 1, gameController.playerPosition.y - 1);
        Vector2Int topRight = new Vector2Int(gameController.playerPosition.x + 1, gameController.playerPosition.y + 1);

        // Top to bottom
        for (int y = bottomLeft.y; y < topRight.y + 1; y++)
        {
            // Left to right
            for (int x = bottomLeft.x; x < topRight.x + 1; x++)
            {
                Vector2Int position = new Vector2Int(x, y);

                CellType cellType = GameController.instance.GetCellType(position);
                if (cellType == CellType.enemy)
                {
                    data.position = position;
                    gameController.DealDamageToEnemy(data);
                }
            }
        }

        eventManager.Publish(EventType.PlayerActionComplete);
    }

    private void OnPlayerHeavyAttack(object target)
    {
        DamageData data = new();
        data.damage = gameController.GetHeavyAtkDamage();

        List<Vector2Int> cells = new();
        
        if (target is Vector2Int position)
        {
            Vector2Int center = new();
            Vector2Int left = new();
            Vector2Int right = new();
            Vector2Int up = new();
            Vector2Int down = new();

            center = position;
            left = new Vector2Int(position.x - 1, position.y);
            right = new Vector2Int(position.x + 1, position.y);
            up = new Vector2Int(position.x, position.y + 1);
            down = new Vector2Int(position.x, position.y - 1);

            cells.Add(center);
            cells.Add(left);
            cells.Add(right);
            cells.Add(up);
            cells.Add(down);

            foreach (Vector2Int cell in cells)
            {
                CellType cellType = GameController.instance.GetCellType(cell);
                if (cellType == CellType.enemy)
                {
                    data.position = cell;
                    gameController.DealDamageToEnemy(data);
                }
            }


        }

        eventManager.Publish(EventType.PlayerActionComplete);
    }

    private IEnumerator MoveToCell(Vector2Int position)
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
            eventManager.Publish(EventType.PlayerActionComplete);
        }
    }


}
