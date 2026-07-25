using UnityEngine;

public class UseActionController : MonoBehaviour
{
    [SerializeField] private GameObject useButtonObject;

    private GameController gameController;
    private EventManager eventManager;

    private GameObject currentDoorObject = null;

    private void Start()
    {
        gameController = GameController.instance;
        eventManager = gameController.eventManager;

        useButtonObject.SetActive(false);

        if(eventManager != null)
        {
            eventManager.Subscribe(EventType.CanUseDoor, OnCanUseDoor);
            eventManager.Subscribe(EventType.CanNotUseDoor, OnCanNotUseDoor);
        }
    }

    private void OnCanUseDoor(object target)
    {
        if(target is GameObject door)
        {
            currentDoorObject = door;

            ActivateUseUI();
        }
    }

    private void OnCanNotUseDoor(object target)
    {
        if (target is GameObject door)
        {
            if (currentDoorObject == door) currentDoorObject = null;

            DeactivateUseUI();
        }
    }

    private void ActivateUseUI()
    {
        useButtonObject.SetActive(true);
    }

    private void DeactivateUseUI()
    {
        useButtonObject.SetActive(false);
    }
}
