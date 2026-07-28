using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject playerUI;
    [SerializeField] private GameObject titleScreen;
    [SerializeField] private Vector2Int playerSpawnPosition;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private int playerMoveRange;

    public static GameController instance;
    public EventManager eventManager { get; private set; }
    private GameObject parent;
    private Coroutine spawnPlayerCoroutine;
    private List<string> roomsVisitedList = new();
    public Vector2Int playerPosition { get; private set; }

    private bool restart;

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
            eventManager.Subscribe(EventType.TransitionClosed, OnTransitionClosed);
        }
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.ChangePlayerPosition, OnPlayerPositionChange);
            eventManager.Unsubscribe(EventType.ChangePlayerPosition, OnChangePlayerSpawnPosition);
            eventManager.Unsubscribe(EventType.SceneChange, OnSceneChange);
            eventManager.Unsubscribe(EventType.TransitionClosed, OnTransitionClosed);
        }
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
        if (SceneController.GetCurrentSceneName() == "A2")
        {
            playerUI.SetActive(true);
            titleScreen.SetActive(false);
        }
        else if (SceneController.GetCurrentSceneName() == "A2")
        {
            playerUI.SetActive(false);
            titleScreen.SetActive(true);
        }

        SpawnPlayerLate(playerSpawnPosition);
        roomsVisitedList.Add(SceneController.GetCurrentSceneName());
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
        Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        playerPosition = startPos;

        CellData data = new CellData();
        data.position = startPos;
        data.cellType = CellType.player;
        UpdateCellData(data);

        eventManager.Publish(EventType.PlayerSpawned);
    }

    private void OnTransitionClosed(object target)
    {
        if (SceneController.GetCurrentSceneName() == "Intro")
        {
            SceneController.GoToScene("A2");
        }

        if (restart)
        {
            roomsVisitedList.Clear();
            SceneController.GoToScene("A2");
            playerSpawnPosition = new Vector2Int(6, 4);
            eventManager.Publish(EventType.RestartGame);
            restart = false;
        }
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
        

        eventManager.Publish(EventType.Transition);
        restart = true;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Play()
    {
        eventManager.Publish(EventType.Transition);
    }

    public void UseDoor()
    {
        if (IsPlayerTurn()) eventManager.Publish(EventType.UseDoor);
    }

    public bool IsRoomAvailable(string roomName)
    {
        if (roomsVisitedList.Contains(roomName) || !SceneController.DoesSceneExist(roomName)) return false;
        return true;
    }
}
