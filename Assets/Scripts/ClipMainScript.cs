using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClipMainScript : MonoBehaviour
{
    public enum ClipType
    {
        Arm,
        Drive,
        Claw
    }
    public ClipType clipType;
    public float duration = 2; //should be in seconds, does the action in the alotted time.
    public float startTime = 0;
    RectTransform rectTransform;

    Vector2 oldSize;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        oldSize = rectTransform.sizeDelta;
    }

    // Update is called once per frame
    void Update()
    {
        rectTransform.sizeDelta = new Vector2(duration * 100, oldSize.y);
    }
}
