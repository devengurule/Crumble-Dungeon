using UnityEngine;
using UnityEngine.Tilemaps;

public class GridController : MonoBehaviour
{
    [SerializeField] private Tilemap tileMap;
    [SerializeField] private TileBase floorTile;
    [SerializeField] private TileBase wallTile;
    [SerializeField] private Vector2Int gridTopLeft;

    private static int gridWidth = 13;
    private static int gridHeight = 9;
    private float gridOffset = 0.5f;

    private CellData[,] cells = new CellData[gridWidth, gridHeight];

    Vector3Int cellPos = new();

    private void Awake()
    {
        cellPos = (Vector3Int)gridTopLeft;
        InitializeGridArray();
    }

    private void InitializeGridArray()
    {
        CellData newData = new CellData();

        // Top to bottom
        for (int y = 0; y < gridHeight ; y++)
        {
            // Left to right
            for (int x = 0; x < gridWidth; x++)
            {
                cellPos = new Vector3Int(x, y, 0);
                TileBase tile = tileMap.GetTile(cellPos);

                if (tile == floorTile) newData.cellType = CellType.empty;
                else if (tile == wallTile) newData.cellType = CellType.wall;

                newData.position = new Vector2(cellPos.x, cellPos.y);
                cells[x, y] = newData;
            }
        }
    }
}
