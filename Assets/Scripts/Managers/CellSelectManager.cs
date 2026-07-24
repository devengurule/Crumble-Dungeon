using UnityEngine;
using UnityEngine.UI;

public class CellSelectManager : MonoBehaviour
{
    [SerializeField] private GameObject selectorFolder;
    [SerializeField] private GameObject selectorPrefab;
    [SerializeField] private GameObject backButton;

    private GameController gameController;
    private EventManager eventManager;

    private void Start()
    {
        gameController = GameController.instance;
        eventManager = gameController.eventManager;

        if(eventManager != null)
        {
            eventManager.Subscribe(EventType.CellSelected, OnCellSelected);
            eventManager.Subscribe(EventType.GrantedUseAction, OnGrantedUseAction);
        }
    }

    private void OnCellSelected(object target)
    {
        eventManager.Publish(EventType.RequestUseAction, target);
    }

    private void OnGrantedUseAction(object target)
    {
        if (target is Vector3 vector)
        {
            eventManager.Publish(EventType.ChangePlayerPosition, new Vector2Int((int)vector.x, (int)vector.y));
            DespawnCellSelectors();
            backButton.GetComponent<Button>().onClick.Invoke();
        }
    }

    public void SpawnCellSelectors()
    {
        Vector2Int playerPos = gameController.playerPosition;
        int playerMoveRange = gameController.PlayerMoveRange();

        Vector2Int bottomLeft = new Vector2Int(playerPos.x - playerMoveRange, playerPos.y - playerMoveRange);
        Vector2Int topRight = new Vector2Int(playerPos.x + playerMoveRange, playerPos.y + playerMoveRange);

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
