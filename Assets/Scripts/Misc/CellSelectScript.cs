using UnityEngine;

public class CellSelectScript : MonoBehaviour
{
    [SerializeField] private SelectType selectType;
    [SerializeField] private Color highlightColor;
    [SerializeField] private Color selectColor;

    private EventManager eventManager;
    private SpriteRenderer sr;

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

        switch (selectType)
        {
            case SelectType.Move:
                eventManager.Publish(EventType.MoveCellSelected, transform.position);
                break;
            case SelectType.NormalAttack:
                eventManager.Publish(EventType.AtkCellSelected, transform.position);
                break;
            case SelectType.SweepAttack:
                eventManager.Publish(EventType.SweepAtkCellSelected);
                break;
            case SelectType.HeavyAttack:
                eventManager.Publish(EventType.HeavyAtkCellSelected, transform.position);
                break;
        }

        
    }
}