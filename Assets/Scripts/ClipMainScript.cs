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
    public float clipLength = 1; //should be in seconds, does the action in the alotted time.
    public int distance = 0; //distance traveled, either movement, or arm rotation

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
