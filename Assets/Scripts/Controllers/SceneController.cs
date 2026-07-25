using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private EventManager eventManager;

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        SceneManager.activeSceneChanged += OnActiveSceneChange;
    }

    private void OnActiveSceneChange(Scene current, Scene next)
    {
        eventManager.Publish(EventType.SceneChange);
    }

    public static void GoToScene(string sceneName)
    {
        if (GetCurrentSceneName() != sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    public static string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    public static Scene GetCurrentScene()
    {
        return SceneManager.GetActiveScene();
    }
}
