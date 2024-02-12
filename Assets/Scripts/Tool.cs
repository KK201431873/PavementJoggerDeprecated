
using System;
using System.Linq;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Tool : MonoBehaviour
{

    public Text poseText, modeText;

    private double x, y, heading;
    private double px, py;
    private float delay=0;

    private bool dragging = false, hovering = false, arrowsPressed = false;

    private Transform tf;

    // Start is called before the first frame update
    void Start()
    {
        tf = transform;
        x = 0;
        y = 0;
        heading = -90;
        goTo(new Vector2(0, 0));
    }

    // Update is called once per frame
    void Update()
    {
        // handle mode
        if (Input.GetKeyDown(KeyCode.A))
            PJ.MODE = PJ.MODE.Equals("CARDINAL") ? "ANGULAR" : (PJ.MODE.Equals("ANGULAR") ? "CARDINAL" : "null");

        // handle editing event
        HandleMouseMovement();
        HandleKeyboardMovement();
        HandleNodeCreation();

        if ((Input.GetKeyDown(KeyCode.Q) && Input.GetKey(KeyCode.R)) ||
                (Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.Q)))
        { // reset 
            PJ.ClearFrom(0);
        }

        // render current position
        poseText.text = "X: "+round(x*PJ.in_per_px,2)+
            "in\nY: "+round(y*PJ.in_per_px,2)+
            "in\nR: " + round(heading, 2) + "°";
        modeText.text = PJ.MODE;
        tf.position = new Vector3((float)x, (float)y, tf.position.z);
        tf.eulerAngles = new Vector3(0,0,(float)heading);
    }


    private void HandleNodeCreation()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            double rx = round(x * PJ.in_per_px, 2),
                ry = round(y * PJ.in_per_px, 2),
                rheading = round(heading, 2);

            if (PJ.X.Count == 0)
            {
                // empty case
                PJ.Add((rx, ry, rheading, 0, "DRIVE", 0L));
            } else
            {
                double prx = PJ.X.Last(),
                    pry = PJ.Y.Last(),
                    prheading = PJ.HEADING.Last(),
                    prarm = PJ.ARM.Last();

                if (rx != prx || ry != pry) 
                { // generic move case
                    PJ.Add((rx, ry, rheading, prarm, "DRIVE", 0L));
                } 
                if (rheading != prheading)
                { // turn cases
                    double diff = rheading - prheading;
                    diff = diff + (diff > 180 ? -360 : diff < -180 ? 360 : 0);
                    string turnAction = diff > 0 ? "TURN_LEFT" : "TURN_RIGHT";
                    PJ.Add((rx, ry, rheading, prarm, turnAction, 0L));
                }
            }
        }
    }

    private void HandleKeyboardMovement()
    {
        bool keyStatus = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow);

        if (!keyStatus)
            arrowsPressed = false;

        // can only drag or arrow key to move, not both simultaneously
        if (dragging)
            return;

        double ipp = PJ.in_per_px;
        double precision = PJ.precision;

        if (keyStatus)
            arrowsPressed = true;

        if ((Input.GetKeyDown(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow)) ||
            (Input.GetKeyDown(KeyCode.DownArrow) && !Input.GetKey(KeyCode.UpArrow)))
            py = y;
        if ((Input.GetKeyDown(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow)) ||
            (Input.GetKeyDown(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))) 
            px = x;

        if (delay > 1.0 / 30.0 || !arrowsPressed)
            delay = 0;
        else
            delay += Time.deltaTime;
        if (delay == 0)
        {
            if (Input.GetKey(KeyCode.UpArrow))
                goTo(new Vector2((float)x, (float)roundToNearest(
                    y + 3 / (precision * ipp) * Mathf.Ceil(0.00001f + Mathf.Abs((float)(y - py))) / (1 * 3 / precision), 0.25 / ipp)));
            if (Input.GetKey(KeyCode.DownArrow))
                goTo(new Vector2((float)x, (float)roundToNearest(
                    y - 3 / (precision * ipp) * Mathf.Ceil(0.00001f + Mathf.Abs((float)(y - py))) / (1 * 3 / precision), 0.25 / ipp)));
            if (Input.GetKey(KeyCode.LeftArrow))
                goTo(new Vector2((float)roundToNearest(
                    x - 3 / (precision * ipp) * Mathf.Ceil(0.00001f + Mathf.Abs((float)(x - px))) / (1 * 3 / precision), 0.25 / ipp), (float)y));
            if (Input.GetKey(KeyCode.RightArrow))
                goTo(new Vector2((float)roundToNearest(
                    x + 3 / (precision * ipp) * Mathf.Ceil(0.00001f + Mathf.Abs((float)(x - px))) / (1 * 3 / precision), 0.25 / ipp), (float)y));
        }

    }


    private void HandleMouseMovement()
    {
        // can only drag or arrow key to move, not both simultaneously
        if (arrowsPressed)
            return;

        // change size if touching mouse pointer
        if (hovering)
            transform.localScale += ExtensionMethods.round(10 * (new Vector3(2, 2, 1) - transform.localScale) * Time.deltaTime, 2);
        else if (!hovering && !dragging)
            transform.localScale += ExtensionMethods.round(10 * (new Vector3(1, 1, 1) - transform.localScale) * Time.deltaTime, 2);


        // drag logic
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (dragging)
        {
            // snapping test
            double ipp = PJ.in_per_px;
            double step = 3 / (PJ.precision * ipp);
            double[,] snapPoints = {
                {-40.75/ipp, 63.5/ipp}, // farBlue
                {40.75/ipp, 63.5/ipp}, // closeBlue
                {-40.75/ipp, -63.5/ipp}, // farRed
                {40.75/ipp, -63.5/ipp}  // closeRed
            };
            bool snapped = false;
            if (PJ.X.Count == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (hypot(snapPoints[i, 0] - mousePos.x, snapPoints[i, 1] - mousePos.y) < 0.5)
                    {
                        goTo(new Vector2((float)snapPoints[i, 0], (float)snapPoints[i, 1]));
                        snapped = true;
                        break;
                    }
                }
            }

            if (!snapped)
            {
                // mode controls
                if (PJ.MODE.Equals("ANGULAR") || PJ.X.Count == 0)
                {
                    goTo(new Vector3(
                        (float)roundToNearest(mousePos.x, step),
                        (float)roundToNearest(mousePos.y, step),
                        0));
                }
                if (PJ.MODE.Equals("CARDINAL") && PJ.X.Count > 0)
                {
                    double clickX = PJ.X.Last() / PJ.in_per_px;
                    double clickY = PJ.Y.Last() / PJ.in_per_px;
                    if (Mathf.Abs(mousePos.x - (float)clickX) > Mathf.Abs(mousePos.y - (float)clickY))
                    {
                        goTo(new Vector2((float)roundToNearest(mousePos.x, step), (float)clickY));
                    } else
                    {
                        goTo(new Vector2((float)clickX, (float)roundToNearest(mousePos.y, step)));
                    }
                }
            }

        }

        // turning
        if (Input.GetMouseButton(0) && !dragging && !PrecisionSlider.beingDragged)
        {
            double dir = Mathf.Atan2(mousePos.y - (float)y, mousePos.x - (float)x) * (180 / Mathf.PI);
            double step = PJ.MODE.Equals("ANGULAR") ? 30.0 / PJ.precision : (PJ.MODE.Equals("CARDINAL") ? 90.0 : 0);
            heading = round(dir/step)*step;
        }

    }

    private void OnMouseDown()
    {
        if (!arrowsPressed)
        {
            dragging = true;
        }

    }

    private void OnMouseUp()
    {
        dragging = false;
    }

    private void OnMouseEnter()
    {
        hovering = true;
    }

    private void OnMouseExit()
    {
        hovering = false;
    }

    private void goTo(Vector2 vector2) {
        x = Mathf.Min(5, Mathf.Max(-5, vector2.x)); 
        y = Mathf.Min(5, Mathf.Max(-5, vector2.y));
    }

    private double round(double x)
    {
        return System.Math.Round(x);
    }

    private double round(double x, int places)
    {
        return System.Math.Round(x, places);
    }

    private double roundToNearest(double x, double multiple)
    {
        return System.Math.Round(x / multiple) * multiple;
    }

    private double hypot(double x, double y)
    {
        return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
    }

}
