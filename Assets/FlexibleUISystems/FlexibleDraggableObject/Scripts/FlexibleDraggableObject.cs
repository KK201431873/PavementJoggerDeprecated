using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class FlexibleDraggableObject : MonoBehaviour
{
    public GameObject Target;
    private EventTrigger _eventTrigger;
    public bool enableSnapping = false;
    public string snappingTarget;
    public float SnappingDistance;
    public Transform snappingFound;
    Vector3 previousPos;

    void Start ()
    {
        snappingFound = GameObject.Find(snappingTarget).transform;
        _eventTrigger = GetComponent<EventTrigger>();
        _eventTrigger.AddEventTrigger(OnDrag, EventTriggerType.Drag);
    }

    void OnDrag(BaseEventData data)
    {
        PointerEventData ped = (PointerEventData) data;
        Target.transform.Translate(ped.delta);

        if (transform.position.y < snappingFound.position.y + SnappingDistance)
        {
            Debug.Log("Snapping trying to occur");
            transform.position = new Vector3(transform.position.x, snappingFound.position.y, transform.position.z);
        }
        //if (enableSnapping == true && previousPos != null)
        //{
        //    if (Vector3.Distance(previousPos, transform.position) < SnappingDistance)
        //    {
        //        transform.position = previousPos;
        //    }
        //}
    }

    void OnRelease()
    {
        previousPos = transform.position;
    }
}