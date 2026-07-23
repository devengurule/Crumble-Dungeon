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
    public Vector2 playerPosition { get; private set; }

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
    }

    private IEnumerator Start()
    {
        // Execute on Start Frame

        SpawnPlayer();

        yield return null;
        // Execute 1 Frame After Start Frame

        eventManager.Publish(EventType.PlayerPositionChange, playerStartPosition);
    }

    private void SpawnPlayer()
    {
        Vector3 spawnPos = new Vector3(playerStartPosition.x, playerStartPosition.y, 0);
        Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        playerPosition = spawnPos;
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
