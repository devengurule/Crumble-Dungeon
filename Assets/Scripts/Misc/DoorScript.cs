using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorScript : MonoBehaviour
{
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private string sceneName;
    [SerializeField] private Vector2Int spawnPosition;

    private GameController gameController;
    private EventManager eventManager;
    private bool isLocked;
    private bool isCollidingWithPlayer;

    private void Start()
    {
        gameController = GameController.instance;
        eventManager = gameController.eventManager;

        isLocked = !gameController.IsRoomAvailable(sceneName);

        if (isLocked) GetComponent<SpriteRenderer>().sprite = lockedSprite;

        if (eventManager != null)
        {
            eventManager.Subscribe(EventType.UseDoor, OnUseDoor);
            eventManager.Subscribe(EventType.CanUseDoor, OnPlayerHitsDoor);
            eventManager.Subscribe(EventType.CanNotUseDoor, OnPlayerLeavesDoor);
        }
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.UseDoor, OnUseDoor);
            eventManager.Unsubscribe(EventType.CanUseDoor, OnPlayerHitsDoor);
            eventManager.Unsubscribe(EventType.CanNotUseDoor, OnPlayerLeavesDoor);
        }
    }

    private void OnUseDoor(object target)
    {
        if (isCollidingWithPlayer && !isLocked)
        {
            eventManager.Publish(EventType.ChangePlayerPosition, spawnPosition);

            SceneController.GoToScene(sceneName);
        }
    }

    private void OnPlayerHitsDoor(object target)
    {
        if (target is GameObject obj)
        {
            if (obj == gameObject) isCollidingWithPlayer = true;
        }
    }

    private void OnPlayerLeavesDoor(object target)
    {
        if (target is GameObject obj)
        {
            if(obj == gameObject) isCollidingWithPlayer = false;
        }
    }
}
