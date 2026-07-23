using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public EventManager eventManager { get; private set; }

    private GameObject parent;

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
}
