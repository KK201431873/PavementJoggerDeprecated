using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipMainScript : MonoBehaviour
{
    public enum ClipType
    {
        Arm,
        Drive,
        Claw
    }
    public ClipType clipType;
    public float duration = 1; //should be in seconds, does the action in the alotted time.
    public float startTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
