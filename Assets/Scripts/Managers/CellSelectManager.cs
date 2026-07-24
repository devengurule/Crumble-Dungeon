using UnityEngine;
using UnityEngine.UI;

public class CellSelectManager : MonoBehaviour
{
    [SerializeField] private GameObject moveSelectorFolder;
    [SerializeField] private GameObject attackSelectorFolder;
    [SerializeField] private GameObject moveBackButton;
    [SerializeField] private GameObject attackBackButton;

    [Header("Selectors")]
    [SerializeField] private GameObject moveSelectorPrefab;
    [SerializeField] private GameObject normalAtkSelectorPrefab;
    [SerializeField] private GameObject sweepAtkSelectorPrefab;
    [SerializeField] private GameObject heavyAtkSelectorPrefab;

    

    private SelectType selectType;
    private GameController gameController;
    private EventManager eventManager;

    private void Start()
    {
        gameController = GameController.instance;
        eventManager = gameController.eventManager;

        if(eventManager != null)
        {
            eventManager.Subscribe(EventType.MoveCellSelected, OnMoveCellSelected);
            eventManager.Subscribe(EventType.AtkCellSelected, OnAtkCellSelected);
            eventManager.Subscribe(EventType.SweepAtkCellSelected, OnSweepAtkCellSelected);
            eventManager.Subscribe(EventType.HeavyAtkCellSelected, OnHeavyAtkCellSelected);
            eventManager.Subscribe(EventType.GrantedUseAction, OnGrantedUseAction);
        }
    }

    private void OnMoveCellSelected(object target)
    {
        selectType = SelectType.Move;
        eventManager.Publish(EventType.RequestUseAction, target);
    }

    private void OnAtkCellSelected(object target)
    {
        selectType = SelectType.NormalAttack;
        eventManager.Publish(EventType.RequestUseAction, target);
    }

    private void OnSweepAtkCellSelected(object target)
    {
        selectType = SelectType.SweepAttack;
        eventManager.Publish(EventType.RequestUseAction, target);
    }

    private void OnHeavyAtkCellSelected(object target)
    {
        selectType = SelectType.HeavyAttack;
        eventManager.Publish(EventType.RequestUseAction, target);
    }

    private void OnGrantedUseAction(object target)
    {
        switch (selectType)
        {
            case SelectType.Move:
                if (target is Vector3 moveVector)
                {
                    eventManager.Publish(EventType.ChangePlayerPosition, new Vector2Int((int)moveVector.x, (int)moveVector.y));
                    DespawnCellSelectors();
                    moveBackButton.GetComponent<Button>().onClick.Invoke();
                }
                break;
            case SelectType.NormalAttack:
                if (target is Vector3 atkVector)
                {
                    eventManager.Publish(EventType.PerformNormalAttack, new Vector2Int((int)atkVector.x, (int)atkVector.y));
                    attackBackButton.GetComponent<Button>().onClick.Invoke();
                }
                break;
            case SelectType.SweepAttack:
                eventManager.Publish(EventType.PerformSweepAttack, gameController.playerPosition);
                attackBackButton.GetComponent<Button>().onClick.Invoke();
                break;
            case SelectType.HeavyAttack:
                if (target is Vector3 heavyAtkVector)
                {
                    eventManager.Publish(EventType.PerformHeavyAttack, new Vector2Int((int)heavyAtkVector.x, (int)heavyAtkVector.y));
                    attackBackButton.GetComponent<Button>().onClick.Invoke();
                }
                break;
        }
    }

    public void MoveSelectors()
    {
        SpawnCellSelectors(moveSelectorPrefab);
    }

    public void NormalAtkSelectors()
    {
        SpawnNormalCellSelectors(normalAtkSelectorPrefab);
    }

    public void SweepAtkSelectors()
    {
        SpawnSweepCellSector(sweepAtkSelectorPrefab);
    }

    public void HeavyAtkSelectors()
    {
        SpawnHeavyCellSelector(heavyAtkSelectorPrefab);
    }

    private void SpawnCellSelectors(GameObject prefab)
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
                    InstantiateSelectors(prefab, position);
                }
            }
        }
    }
    private void SpawnNormalCellSelectors(GameObject prefab)
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

                InstantiateSelectors(prefab, position);
            }
        }
    }

    private void SpawnSweepCellSector(GameObject prefab)
    {
        InstantiateSelectors(prefab, gameController.playerPosition);
    }

    private void SpawnHeavyCellSelector(GameObject prefab)
    {
        Vector2Int down = new Vector2Int(gameController.playerPosition.x, gameController.playerPosition.y - 2);
        Vector2Int up = new Vector2Int(gameController.playerPosition.x, gameController.playerPosition.y + 2);
        Vector2Int left = new Vector2Int(gameController.playerPosition.x - 2, gameController.playerPosition.y);
        Vector2Int right = new Vector2Int(gameController.playerPosition.x + 2, gameController.playerPosition.y);

        InstantiateSelectors(prefab, down);
        InstantiateSelectors(prefab, up);
        InstantiateSelectors(prefab, left);
        InstantiateSelectors(prefab, right);
    }

    public void DespawnCellSelectors()
    {
        for (int i = moveSelectorFolder.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(moveSelectorFolder.transform.GetChild(i).gameObject);
        }
        for (int i = attackSelectorFolder.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(attackSelectorFolder.transform.GetChild(i).gameObject);
        }
    }

    private void InstantiateSelectors(GameObject prefab, Vector2Int position)
    {
        Vector3 vector3Position = new Vector3(position.x, position.y, 0);
        GameObject selector = Instantiate(prefab, vector3Position, Quaternion.identity);

        if(selector.tag == "MoveSelector") selector.transform.parent = moveSelectorFolder.transform;
        else selector.transform.parent = attackSelectorFolder.transform;
    }
}
