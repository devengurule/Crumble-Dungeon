using UnityEngine;

public class CellSelectScript : MonoBehaviour
{
    [SerializeField] private Color highlightColor;
    [SerializeField] private Color selectColor;

    private EventManager eventManager;
    private SpriteRenderer sr;
    
    private Vector2 mousePos;

    private void Start()
    {
        eventManager = GameController.instance.eventManager;
        sr = GetComponent<SpriteRenderer>();
        sr.color = highlightColor;
    }

    private void OnMouseEnter()
    {
        sr.color = selectColor;
    }

    private void OnMouseExit()
    {
        sr.color = highlightColor;
    }

    private void OnMouseDown()
    {
        eventManager.Publish(EventType.CellSelected, transform.position);
    }
}