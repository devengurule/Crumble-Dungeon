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
            eventManager.Subscribe(EventType.ResetCellType, OnResetCellType);
        }

        //foreach (var cell in cells)
        //{
        //    Debug.Log($"Type: {cell.cellType} Pos: {cell.position}");
        //}
    }

    private void OnResetCellType(object target)
    {
        if(target is Vector2Int position)
        {
            CellData data = new CellData();
            data.position = position;
            data.cellType = CellType.empty;

            UpdateCellData(data);
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
                cells[x, y] = InitalizeCellData(x, y);
            }
        }
    }

    public void UpdateCellData(CellData data)
    {
        cells[data.position.x, data.position.y] = data;
    }

    public void UpdateCellData(CellData oldCell, CellData newCell)
    {
        cells[oldCell.position.x, oldCell.position.y] = oldCell;
        cells[newCell.position.x, newCell.position.y] = newCell;
    }

    private CellData InitalizeCellData(int x, int y)
    {
        CellData data = new CellData();

        cellPos = new Vector3Int(x - 1, y - 1, 0);
        TileBase tile = tileMap.GetTile(cellPos);

        if (tile == floorTile) data.cellType = CellType.empty;
        else if (tile == wallTile) data.cellType = CellType.wall;

        data.position = new Vector2Int(cellPos.x + 1, cellPos.y + 1);

        return data;
    }

    public CellType GetCellType(Vector2Int position)
    {
        return cells[position.x, position.y].cellType;
    }

    public CellData GetCellData(Vector2Int position)
    {
        return cells[position.x, position.y];
    }

    public int Width()
    {
        return gridWidth;
    }

    public int Height()
    {
        return gridHeight;
    }
}
