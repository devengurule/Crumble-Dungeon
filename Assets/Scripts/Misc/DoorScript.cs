using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorScript : MonoBehaviour
{
    [SerializeField] private Sprite endSprite;
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

        if (GetComponent<SpriteRenderer>().sprite != endSprite)
        {
            isLocked = !gameController.IsRoomAvailable(sceneName);

            if (isLocked) GetComponent<SpriteRenderer>().sprite = lockedSprite;
        }
        else isLocked = false;

        if (eventManager != null)
        {
            eventManager.Subscribe(EventType.UseDoor, OnUseDoor);
            eventManager.Subscribe(EventType.CanUseDoor, OnPlayerHitsDoor);
            eventManager.Subscribe(EventType.CanNotUseDoor, OnPlayerLeavesDoor);
            eventManager.Subscribe(EventType.TransitionClosed, OnTransitionClosed);
        }
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.UseDoor, OnUseDoor);
            eventManager.Unsubscribe(EventType.CanUseDoor, OnPlayerHitsDoor);
            eventManager.Unsubscribe(EventType.CanNotUseDoor, OnPlayerLeavesDoor);
            eventManager.Unsubscribe(EventType.TransitionClosed, OnTransitionClosed);
        }
    }

    private void OnUseDoor(object target)
    {
        if (isCollidingWithPlayer && !isLocked)
        {
            if (GetComponent<SpriteRenderer>().sprite == endSprite)
            {
                eventManager.Publish(EventType.Escape);
            }
            else eventManager.Publish(EventType.Transition);
        }
    }

    private void OnTransitionClosed(object target)
    {
        if (isCollidingWithPlayer && !isLocked && GetComponent<SpriteRenderer>().sprite != endSprite)
        {
            eventManager.Publish(EventType.ChangePlayerPosition, spawnPosition);

            SceneController.GoToScene(sceneName);
        }
    }

    private void OnPlayerHitsDoor(object target)
    {
        if (target is GameObject obj)
        {
            
            if (obj.transform.position == gameObject.transform.position) isCollidingWithPlayer = true;
            else isCollidingWithPlayer = false;
        }
    }

    private void OnPlayerLeavesDoor(object target)
    {
        if (target is GameObject obj)
        {
            if(obj.transform.position == gameObject.transform.position) isCollidingWithPlayer = false;
            else isCollidingWithPlayer = false;
        }
    }
}
