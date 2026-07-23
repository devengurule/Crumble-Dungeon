using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridController : MonoBehaviour
{
    [SerializeField] private Tilemap tileMap;
    [SerializeField] private TileBase floorTile;
    [SerializeField] private TileBase wallTile;
    [SerializeField] private Vector2Int gridTopLeft;

    private EventManager eventManager;
    private static int gridWidth = 13;
    private static int gridHeight = 9;
    private Vector3Int cellPos = new();
    private CellData[,] cells = new CellData[gridWidth, gridHeight];

    private void Awake()
    {
        InitializeGridArray();
    }

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        if (eventManager != null)
        {
            eventManager.Subscribe(EventType.PlayerPositionChange, OnPlayerChangePosition);
        }
        Debug.Log(GetCellType(new Vector2Int(0, 0)));
    }

    private void OnPlayerChangePosition(object target)
    {
        if(target is Vector2Int position)
        {
            CellData data = new CellData();
            data.position = position;
            data.cellType = CellType.player;

            UpdateCellData(position, data);
        }
    }

    private void InitializeGridArray()
    {
        // Top to bottom
        for (int y = 0; y < gridHeight; y++)
        {
            // Left to right
            for (int x = 0; x < gridWidth; x++)
            {
                cells[x, y] = GetSetCellData(x, y);
            }
        }
    }

    public void UpdateCellData(Vector2Int position, CellData data)
    {
        cells[position.x, position.y] = data;
    }

    private CellData GetSetCellData(int x, int y)
    {
        CellData data = new CellData();

        cellPos = new Vector3Int(x - 1, y - 1, 0);
        TileBase tile = tileMap.GetTile(cellPos);

        if (tile == floorTile) data.cellType = CellType.empty;
        else if (tile == wallTile) data.cellType = CellType.wall;

        data.position = new Vector2(cellPos.x, cellPos.y);

        return data;
    }

    public CellType GetCellType(Vector2Int position)
    {
        return cells[position.x, position.y].cellType;
    }
}
