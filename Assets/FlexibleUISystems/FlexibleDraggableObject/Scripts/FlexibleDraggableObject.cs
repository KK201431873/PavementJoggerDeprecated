using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class FlexibleDraggableObject : MonoBehaviour
{
    public GameObject Target;
    //public bool enableSnapping = false;
    public string snappingTarget;
    public float SnappingDistance;
    public Transform snappingFound;
    public Vector3 previousPos;
    Vector3 mousePos;

    float offSetX;
    float offSetY;
    void Start ()
    {
        previousPos = transform.position;
        snappingFound = GameObject.Find(snappingTarget).transform;
    }
    public void OnbeginDrag()
    {
        mousePos = Input.mousePosition;
        offSetX = transform.position.x - mousePos.x;
        offSetY = transform.position.y - mousePos.y;
    }

    public void OnDrag() //used to have BaseEventData data
    {
        mousePos = Input.mousePosition;
        Target.transform.position = new Vector2(mousePos.x + offSetX, mousePos.y + offSetY);
        if (transform.position.y < previousPos.y + SnappingDistance && transform.position.y > previousPos.y - SnappingDistance)
        {
            transform.position = new Vector2(mousePos.x + offSetX, previousPos.y);
        }
    }

    public void OnRelease()
    {
        previousPos = transform.position;
    }
}