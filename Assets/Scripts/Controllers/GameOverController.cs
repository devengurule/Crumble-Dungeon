using UnityEngine;

public class GameOverController : MonoBehaviour
{
    [SerializeField] private GameObject UIElements;

    [SerializeField] private GameObject enemyDeath;
    [SerializeField] private GameObject roomDeath;
    [SerializeField] private GameObject dungeonDeath;

    private GameController gameController;
    private EventManager eventManager;

    private void Start()
    {
        gameController = GameController.instance;
        eventManager = gameController.eventManager;

        if(eventManager != null)
        {
            eventManager.Subscribe(EventType.GameOver, OnGameOver);
        }
    }

    private void OnGameOver(object target)
    {
        if(target is LoseType loseType)
        {
            UIElements.SetActive(false);

            enemyDeath.SetActive(false);
            roomDeath.SetActive(false);
            dungeonDeath.SetActive(false);

            switch (loseType)
            {
                case LoseType.Died:

                    enemyDeath.SetActive(true);
                    enemyDeath.transform.parent.gameObject.SetActive(true);

                    break;

                case LoseType.DungeonCollapse:

                    roomDeath.SetActive(true);
                    roomDeath.transform.parent.gameObject.SetActive(true);

                    break;

                case LoseType.RoomCollapse:

                    dungeonDeath.SetActive(true);
                    dungeonDeath.transform.parent.gameObject.SetActive(true);

                    break;
            }
        }
    }
}
