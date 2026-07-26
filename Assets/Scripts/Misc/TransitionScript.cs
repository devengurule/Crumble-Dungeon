
using System.Collections;
using UnityEngine;

public class TransitionScript : MonoBehaviour
{
    [SerializeField] private Vector2 openPosition;
    [SerializeField] private Vector2 closedPosition;

    [SerializeField] private float moveDuration;

    private Vector2 currentPosition;
    private EventManager eventManager;

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        if (eventManager != null)
        {
            eventManager.Subscribe(EventType.Transition, OnTransition);
            eventManager.Subscribe(EventType.SceneChange, OnSceneChange);
        }

        if (SceneController.GetCurrentSceneName() != "Intro")
        {
            currentPosition = closedPosition;
            GetComponent<RectTransform>().localPosition = currentPosition;
            eventManager.Publish(EventType.Transition);
        }
        else
        {
            currentPosition = openPosition;
            GetComponent<RectTransform>().localPosition = currentPosition;
        }
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.Transition, OnTransition);
            eventManager.Unsubscribe(EventType.SceneChange, OnSceneChange);
        }
    }

    private void OnTransition(object target)
    {
        if (currentPosition == closedPosition)
        {
            // Open
            StartCoroutine(MoveToPosition(openPosition));
        }
        else if (currentPosition == openPosition)
        {
            // Close
            StartCoroutine(MoveToPosition(closedPosition));
        }
    }

    private void OnSceneChange(object target)
    {
        eventManager.Publish(EventType.Transition);
    }

    private IEnumerator MoveToPosition(Vector2 position)
    {
        Vector2 target = position;
        Vector2 start = currentPosition;
        float duration = moveDuration;
        float time = 0f;

        if (start != target)
        {
            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;
                t = Mathf.SmoothStep(0f, 1f, t);

                GetComponent<RectTransform>().localPosition = Vector3.Lerp(start, target, t);

                yield return null;
            }
            GetComponent<RectTransform>().localPosition = target;
            currentPosition = GetComponent<RectTransform>().localPosition;

            if (target == closedPosition) eventManager.Publish(EventType.TransitionClosed);
            else if (target == openPosition) eventManager.Publish(EventType.TransitionOpen);
        }
    }
}
