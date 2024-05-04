using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClipCreAPos : MonoBehaviour
{
    RectTransform rectTransform;
    public RectTransform objectToSetPos;
    public GameObject clip;
    public Transform Parent;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        rectTransform.position = objectToSetPos.position;
    }

    public void clicked()
    {
        GameObject InstantiatedGameObject = Instantiate(clip, transform.position, Quaternion.identity);
        InstantiatedGameObject.transform.SetParent(Parent);
    }
}
