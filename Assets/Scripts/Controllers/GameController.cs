using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private Vector2Int playerStartPosition;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private int playerMoveRange;

    public static GameController instance;
    public EventManager eventManager { get; private set; }
    private GameObject parent;
    public Vector2Int playerPosition { get; private set; }

    private void Awake()
    {
        parent = transform.parent.gameObject;

        if (instance != this && instance != null)
        {
            if (parent != null) Destroy(parent);
        }

        DontDestroyOnLoad(parent);

        if (eventManager == null)
        {
            eventManager = GetComponent<EventManager>();
        }
        if (instance == null)
        {
            instance = this;
        }

        if(eventManager != null)
        {
            eventManager.Subscribe(EventType.ChangePlayerPosition, OnPlayerPositionChange);
        }
    }

    private void Start()
    {
        SpawnPlayer();
    }

    private void OnPlayerPositionChange(object target)
    {
        if(target is Vector2Int vector)
        {
            CellData oldCell = new();
            CellData newCell = new();

            oldCell.position = playerPosition;
            oldCell.cellType = CellType.empty;

            newCell.position = vector;
            newCell.cellType = CellType.player;
            
            UpdateCellData(oldCell, newCell);
            playerPosition = vector;
        }
    }

    private void SpawnPlayer()
    {
        Vector3 spawnPos = new Vector3(playerStartPosition.x, playerStartPosition.y, 0);
        Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        playerPosition = playerStartPosition;

        CellData data = new CellData();
        data.position = playerStartPosition;
        data.cellType = CellType.player;
        UpdateCellData(data);

        eventManager.Publish(EventType.PlayerSpawned);
    }

    public int PlayerMoveRange()
    {
        return playerMoveRange;
    }

    public CellType GetCellType(Vector2Int vector)
    {
        return GetComponent<GridController>().GetCellType(vector);
    }

    public CellData GetCellData(Vector2Int vector)
    {
        return GetComponent<GridController>().GetCellData(vector);
    }

    public int GridWidth()
    {
        return GetComponent<GridController>().Width();
    }

    public int GridHeight()
    {
        return GetComponent<GridController>().Height();
    }

    public void UpdateCellData(CellData cell)
    {
        GetComponent<GridController>().UpdateCellData(cell);
    }

    public void UpdateCellData(CellData oldCell, CellData newCell)
    {
        GetComponent<GridController>().UpdateCellData(oldCell, newCell);
    }

    public bool IsPlayerTurn()
    {
        return GetComponent<TurnManager>().IsPlayerTurn();
    }

    public int GetKnightDamage()
    {
        return GetComponent<PlayerHealth>().GetKnightDamage();
    }
}
