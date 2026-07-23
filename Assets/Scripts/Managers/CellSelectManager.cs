using UnityEngine;

public class CellSelectManager : MonoBehaviour
{
    [SerializeField] private GameObject selectorFolder;
    [SerializeField] private GameObject selectorPrefab;
    private GameController gameController;
    private EventManager eventManager;

    private void Start()
    {
        gameController = GameController.instance;
        eventManager = gameController.eventManager;

        if(eventManager != null)
        {
            eventManager.Subscribe(EventType.CellSelected, OnCellSelected);
        }
    }

    private void OnCellSelected(object target)
    {
        if(target is Vector3 vector)
        {
            eventManager.Publish(EventType.ChangePlayerPosition, new Vector2Int((int)vector.x, (int)vector.y));
            eventManager.Publish(EventType.PlayerPositionChange, new Vector2Int((int)vector.x, (int)vector.y));
        }
    }

    public void SpawnCellSelectors()
    {
        Vector2 playerPos = gameController.playerPosition;
        int playerMoveRange = gameController.PlayerMoveRange();

        Vector2Int bottomLeft = new Vector2Int((int)playerPos.x - playerMoveRange, (int)playerPos.y - playerMoveRange);
        Vector2Int topRight = new Vector2Int((int)playerPos.x + playerMoveRange, (int)playerPos.y + playerMoveRange);

        // Top to bottom
        for (int y = bottomLeft.y; y < topRight.y + 1; y++)
        {
            // Left to right
            for (int x = bottomLeft.x; x < topRight.x + 1; x++)
            {
                Vector2Int position = new Vector2Int(x, y);

                CellType cellType = GameController.instance.GetCellType(position);
                if(cellType == CellType.empty)
                {
                    SpawnSelector(position);
                }
            }
        }
    }

    public void DespawnCellSelectors()
    {
        for (int i = selectorFolder.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(selectorFolder.transform.GetChild(i).gameObject);
        }
    }

    private void SpawnSelector(Vector2Int position)
    {
        Vector3 vector3Position = new Vector3(position.x, position.y, 0);
        GameObject selector = Instantiate(selectorPrefab, vector3Position, Quaternion.identity);
        selector.transform.parent = selectorFolder.transform;
    }
}
