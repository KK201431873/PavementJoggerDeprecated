using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderedNode : MonoBehaviour
{

    public int index;

    public Sprite DRIVE, DELAY, TURN_LEFT, TURN_RIGHT, CLAW, ARM, ELBOW, NULL;

    public RenderedNode SetIndex(int index)
    {
        this.index = index;
        this.name = "Node "+index;
        return this;
    }

    void Update()
    {
        string action = PJ.ACTION[index];
        double ipp = PJ.in_per_px;
        double x = PJ.X[index] / ipp;
        double y = PJ.Y[index] / ipp;
        double heading = action.Equals("DRIVE") || action.Equals("TURN_LEFT") || action.Equals("TURN_RIGHT") ? PJ.HEADING[index] : 0;

        // set costume
        switch (action)
        {
            case "DRIVE":
                this.gameObject.GetComponent<SpriteRenderer>().sprite = DRIVE;
                break;
            case "DELAY":
                this.gameObject.GetComponent<SpriteRenderer>().sprite = DELAY;
                break;
            case "TURN_LEFT":
                this.gameObject.GetComponent<SpriteRenderer>().sprite = TURN_LEFT;
                break;
            case "TURN_RIGHT":
                this.gameObject.GetComponent<SpriteRenderer>().sprite = TURN_RIGHT;
                break;
            case "OPEN_LEFT_CLAW":
            case "CLOSE_LEFT_CLAW":
            case "OPEN_RIGHT_CLAW":
            case "CLOSE_RIGHT_CLAW":
            case "OPEN_BOTH_CLAWS":
            case "CLOSE_BOTH_CLAWS":
                this.gameObject.GetComponent<SpriteRenderer>().sprite = CLAW;
                break;
            case "ARM":
                this.gameObject.GetComponent<SpriteRenderer>().sprite = ARM;
                break;
            case "FLIP_ELBOW":
            case "REST_ELBOW":
                this.gameObject.GetComponent<SpriteRenderer>().sprite = ELBOW;
                break;
            default:
                this.gameObject.GetComponent<SpriteRenderer>().sprite = NULL;
                break;
        }

        transform.position = new Vector3((float)x, (float)y, transform.position.z);
        transform.eulerAngles = new Vector3(0, 0, (float)heading);
    }

}
