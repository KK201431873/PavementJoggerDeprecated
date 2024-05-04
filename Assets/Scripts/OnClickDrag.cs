using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnClickDrag : MonoBehaviour
{
    public GameObject createOnRelease;
    Transform Parent;
    private Vector3 mousePos;

    public bool enableSnapping = false;
    public string snappingTarget;
    public float SnappingDistance;
    public Transform snappingFound;

    // Start is called before the first frame update
    void Start()
    {
        snappingFound = GameObject.Find(snappingTarget).transform;
        Parent = gameObject.transform.parent.gameObject.transform;
    }
    private void Update()
    {
        mousePos = Input.mousePosition;
        transform.position = new Vector2(mousePos.x, mousePos.y);
        if (transform.position.y < snappingFound.position.y + SnappingDistance)
        {
            Debug.Log("Snapping trying to occur");
            transform.position = new Vector3(transform.position.x, snappingFound.position.y, transform.position.z);
        }

        if (Input.GetMouseButton(0) == false)
        {
            GameObject InstantiatedGameObject = Instantiate(createOnRelease, transform.position, Quaternion.identity);
            InstantiatedGameObject.transform.SetParent(Parent);
            Destroy(gameObject);
        }
    }
}
