using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnClickDrag : MonoBehaviour
{
    public GameObject createOnRelease;
    Transform parentForCreated;
    private Vector3 mousePos;

    public bool enableSnapping = false;
    public string snappingTarget;
    public float SnappingDistance;
    public Transform snappingFound;

    float offSetX;
    float offSetY;

    // Start is called before the first frame update
    void Start()
    {
        snappingFound = GameObject.Find(snappingTarget).transform;
        parentForCreated = gameObject.transform.parent.gameObject.transform;

        offSetX = transform.position.x - Input.mousePosition.x;
        offSetY = transform.position.y - Input.mousePosition.y;
    }
    private void Update()
    {

        mousePos = Input.mousePosition;
        transform.position = new Vector2(mousePos.x + offSetX, mousePos.y + offSetY);
        if (transform.position.y < snappingFound.position.y + SnappingDistance && transform.position.y > snappingFound.position.y - SnappingDistance)
        {
            transform.position = new Vector3(mousePos.x + offSetX, snappingFound.position.y, transform.position.z);
        }

        if (Input.GetMouseButton(0) == false)
        {
            GameObject InstantiatedGameObject = Instantiate(createOnRelease, transform.position, Quaternion.identity);
            InstantiatedGameObject.transform.SetParent(parentForCreated);
            Destroy(gameObject);
        }
    }
}
