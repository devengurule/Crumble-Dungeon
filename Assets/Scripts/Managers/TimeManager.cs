using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("Game Time")]
    [SerializeField] private int roomTime;
    [SerializeField] private int dungeonTime;

    [Header("Action Time")]
    [SerializeField] private int move;
    [SerializeField] private int nAtk;
    [SerializeField] private int sAtk;
    [SerializeField] private int hAtk;

    [SerializeField] private GameObject roomTimeObject;
    [SerializeField] private GameObject dungeonTimeObject;

    [SerializeField] private GameObject moveTimeObject;
    [SerializeField] private GameObject nATKTimeObject;
    [SerializeField] private GameObject sATKTimeObject;
    [SerializeField] private GameObject hATKTimeObject;

    private GameController gameController;
    private EventManager eventManager;

    private TextMeshProUGUI roomTimeText;
    private TextMeshProUGUI dungeonTimeText;

    private TextMeshProUGUI moveTimeText;
    private TextMeshProUGUI nATKTimeText;
    private TextMeshProUGUI sATKTimeText;
    private TextMeshProUGUI hATKTimeText;

    private int currentRoomTime;
    private int currentDungeonTime;

    private void Start()
    {
        gameController = GameController.instance;
        eventManager = gameController.eventManager;

        roomTimeText = roomTimeObject.GetComponent<TextMeshProUGUI>();
        dungeonTimeText = dungeonTimeObject.GetComponent<TextMeshProUGUI>();

        moveTimeText = moveTimeObject.GetComponent<TextMeshProUGUI>();
        nATKTimeText = nATKTimeObject.GetComponent<TextMeshProUGUI>();
        sATKTimeText = sATKTimeObject.GetComponent<TextMeshProUGUI>();
        hATKTimeText = hATKTimeObject.GetComponent<TextMeshProUGUI>();

        moveTimeText.text = move.ToString();
        nATKTimeText.text = nAtk.ToString();
        sATKTimeText.text = sAtk.ToString();
        hATKTimeText.text = hAtk.ToString();

        currentRoomTime = roomTime;
        currentDungeonTime = dungeonTime;

        UpdatateText(currentRoomTime, currentDungeonTime);

        if (eventManager != null)
        {
            eventManager.Subscribe(EventType.ChangePlayerPosition, OnPlayerMove);
            eventManager.Subscribe(EventType.PerformNormalAttack, OnNormalAttack);
            eventManager.Subscribe(EventType.PerformSweepAttack, OnPlayerSweepAttack);
            eventManager.Subscribe(EventType.PerformHeavyAttack, OnPlayerHeavyAttack);
        }
    }

    private void OnPlayerMove(object target)
    {
        currentRoomTime -= move;
        currentDungeonTime -= move;
        UpdatateText(currentRoomTime, currentDungeonTime);
    }

    private void OnNormalAttack(object target)
    {
        currentRoomTime -= nAtk;
        currentDungeonTime -= nAtk;
        UpdatateText(currentRoomTime, currentDungeonTime);
    }

    private void OnPlayerSweepAttack(object target)
    {
        currentRoomTime -= sAtk;
        currentDungeonTime -= sAtk;
        UpdatateText(currentRoomTime, currentDungeonTime);
    }

    private void OnPlayerHeavyAttack(object target)
    {
        currentRoomTime -= hAtk;
        currentDungeonTime -= hAtk;
        UpdatateText(currentRoomTime, currentDungeonTime);
    }

    private void UpdatateText(int roomTime, int dungeonTime)
    {
        roomTimeText.text = roomTime.ToString();
        dungeonTimeText.text = dungeonTime.ToString();
    }

    public void UseRoomTime(int timeAmount)
    {
        currentRoomTime -= timeAmount;

        if (currentDungeonTime <= 0) eventManager.Publish(EventType.GameOver, LoseType.RoomCollapse);

        UpdatateText(currentRoomTime, currentDungeonTime);
    }

    public void UseDungeonTime(int timeAmount)
    {
        currentDungeonTime -= timeAmount;

        if (currentDungeonTime <= 0) eventManager.Publish(EventType.GameOver, LoseType.DungeonCollapse);

        UpdatateText(currentRoomTime, currentDungeonTime);
    }

    public void ResetRoomTime()
    {
        currentRoomTime = roomTime;

        UpdatateText(currentRoomTime, currentDungeonTime);
    }
}
