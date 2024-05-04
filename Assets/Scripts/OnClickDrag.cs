using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnClickDrag : MonoBehaviour
{
    public GameObject createOnRelease;
    Transform Parent;
    private Vector3 mousePos;
    
    // Start is called before the first frame update
    void Start()
    {
        Parent = gameObject.transform.parent.gameObject.transform;
    }
    private void Update()
    {
        mousePos = Input.mousePosition;
        transform.position = new Vector2(mousePos.x, mousePos.y);

        if (Input.GetMouseButton(0) == false)
        {
            GameObject InstantiatedGameObject = Instantiate(createOnRelease, transform.position, Quaternion.identity);
            InstantiatedGameObject.transform.SetParent(Parent);
            Destroy(gameObject);
        }
    }
}
