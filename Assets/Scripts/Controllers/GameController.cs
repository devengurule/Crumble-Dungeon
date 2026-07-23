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

    private IEnumerator Start()
    {
        // Execute on Start Frame

        SpawnPlayer();

        yield return null;
        // Execute 1 Frame After Start Frame

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
            
            GetComponent<GridController>().UpdateCellData(oldCell, newCell);
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
        GetComponent<GridController>().UpdateCellData(data);
    }

    public int PlayerMoveRange()
    {
        return playerMoveRange;
    }

    public CellType GetCellType(Vector2Int vector)
    {
        return GetComponent<GridController>().GetCellType(vector);
    }
}
