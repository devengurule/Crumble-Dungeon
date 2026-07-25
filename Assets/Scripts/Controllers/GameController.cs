using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] private Vector2Int playerSpawnPosition;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private int playerMoveRange;

    public static GameController instance;
    public EventManager eventManager { get; private set; }
    private GameObject parent;
    private Coroutine spawnPlayerCoroutine;
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
            eventManager.Subscribe(EventType.ChangePlayerPosition, OnChangePlayerSpawnPosition);
            eventManager.Subscribe(EventType.SceneChange, OnSceneChange);
        }
    }

    private void Start()
    {
        SpawnPlayer(playerSpawnPosition);
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

    private void OnSceneChange(object target)
    {
        SpawnPlayerLate(playerSpawnPosition);
        
    }
    
    private void OnChangePlayerSpawnPosition(object target)
    {
        if (target is Vector2Int vector)
        {
            playerSpawnPosition = vector;
        }
    }

    private void SpawnPlayer(Vector2Int startPos)
    {
        Vector3 spawnPos = new Vector3(startPos.x, startPos.y, 0);
        Debug.Log(SceneController.GetCurrentSceneName());
        Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        playerPosition = startPos;

        CellData data = new CellData();
        data.position = startPos;
        data.cellType = CellType.player;
        UpdateCellData(data);

        eventManager.Publish(EventType.PlayerSpawned);
    }

    private void SpawnPlayerLate(Vector2Int playerSpawnPosition)
    {
        if (spawnPlayerCoroutine == null) spawnPlayerCoroutine = StartCoroutine(SpawnPlayerIEnumerator(playerSpawnPosition));
    }

    private IEnumerator SpawnPlayerIEnumerator(Vector2Int playerSpawnPosition)
    {
        yield return null;

        SpawnPlayer(playerSpawnPosition);

        StopCoroutine(spawnPlayerCoroutine);
        spawnPlayerCoroutine = null;
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

    public int GetRogueDamage()
    {
        return GetComponent<PlayerHealth>().GetRogueDamage();
    }

    public int GetNormalAtkDamage()
    {
        return GetComponent<PlayerHealth>().GetNormalAtkDamage();
    }

    public int GetSweepAtkDamage()
    {
        return GetComponent<PlayerHealth>().GetSweepAtkDamage();
    }

    public int GetHeavyAtkDamage()
    {
        return GetComponent<PlayerHealth>().GetHeavyAtkDamage();
    }

    public void DealDamageToEnemy(DamageData data)
    {
        eventManager.Publish(EventType.DealEnemyDamage, data);
    }

    public float GetEnemyActionPauseDuration()
    {
        return GetComponent<EnemyActionManager>().GetActionPauseDuration();
    }

    public void RestartGame()
    {
        eventManager.Publish(EventType.RestartGame);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void UseDoor()
    {
        eventManager.Publish(EventType.UseDoor);
    }
}
