using System.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float moveDuration;

    private EventManager eventManager;

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        if (eventManager != null)
        {
            eventManager.Subscribe(EventType.ChangePlayerPosition, OnPlayerPositionChange);
        }
    }
    private void OnPlayerPositionChange(object target)
    {
        if (target is Vector2Int vector)
        {
            Vector2Int currentPosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);
            eventManager.Publish(EventType.ResetCellType, currentPosition);
            StartCoroutine(MoveToCell(vector));
        }
    }

    private IEnumerator MoveToCell(Vector2Int position)
    {
        Vector3 target = new Vector3(position.x, position.y, 0);
        Vector3 start = transform.position;
        float duration = moveDuration;
        float time = 0f;

        if (start != target)
        {
            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;
                t = Mathf.SmoothStep(0f, 1f, t);

                transform.position = Vector3.Lerp(start, target, t);

                yield return null;
            }
            transform.position = target;
            eventManager.Publish(EventType.PlayerActionComplete);
        }
    }
}
