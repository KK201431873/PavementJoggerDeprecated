using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeRenderer : MonoBehaviour
{

    public int index;

    [SerializeField] private Sprite DRIVE, DELAY, TURN_LEFT, TURN_RIGHT, CLAW, ARM, ELBOW, NULL;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public NodeRenderer SetIndex(int index)
    {
        this.index = index;
        name = "Node "+index;
        return this;
    }

    public void Kill()
    {
        Destroy(gameObject);
    }

    void Start()
    {
        transform.localScale = new Vector3(0.72f, 0.72f, 1);
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
                gameObject.GetComponent<SpriteRenderer>().sprite = DRIVE;
                spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
                break;
            case "DELAY":
                gameObject.GetComponent<SpriteRenderer>().sprite = DELAY;
                spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
                break;
            case "TURN_LEFT":
                gameObject.GetComponent<SpriteRenderer>().sprite = TURN_LEFT;
                spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
                break;
            case "TURN_RIGHT":
                gameObject.GetComponent<SpriteRenderer>().sprite = TURN_RIGHT;
                spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
                break;
            case "OPEN_LEFT_CLAW":
            case "CLOSE_LEFT_CLAW":
            case "OPEN_RIGHT_CLAW":
            case "CLOSE_RIGHT_CLAW":
            case "OPEN_BOTH_CLAWS":
            case "CLOSE_BOTH_CLAWS":
                gameObject.GetComponent<SpriteRenderer>().sprite = CLAW;
                spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
                break;
            case "ARM":
                gameObject.GetComponent<SpriteRenderer>().sprite = ARM;
                spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
                break;
            case "FLIP_ELBOW":
            case "REST_ELBOW":
                gameObject.GetComponent<SpriteRenderer>().sprite = ELBOW;
                spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
                break;
            default:
                gameObject.GetComponent<SpriteRenderer>().sprite = NULL;
                spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
                break;
        }

        transform.position = new Vector3((float)x, (float)y, transform.position.z);
        transform.eulerAngles = new Vector3(0, 0, (float)heading);
    }

}
