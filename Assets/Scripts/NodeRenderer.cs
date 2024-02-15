using System.Collections;
using System;
using System.Linq;
using UnityEngine;

public class NodeRenderer : MonoBehaviour
{

    public int index;

    [SerializeField] private Sprite DRIVE, DELAY, TURN_LEFT, TURN_RIGHT, CLAW, ARM, ELBOW, NULL;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private CircleCollider2D nodeCollider;

    public bool dragging = false, hovering = false;

    public NodeRenderer SetIndex(int index)
    {
        this.index = index;
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
        nodeCollider.enabled = PJ.ACTION[index].Equals("DRIVE");

        string action = PJ.ACTION[index];
        double ipp = PJ.in_per_px;
        double x = PJ.X[index] / ipp;
        double y = PJ.Y[index] / ipp;
        double heading = action.Equals("DRIVE") || action.Equals("TURN_LEFT") || action.Equals("TURN_RIGHT") ? PJ.HEADING[index] : 0;

        SwitchToSprites(action);
        if (PJ.ACTION[index].Equals("DRIVE"))
            HandleMouseMovement();

        transform.position = new Vector3((float)x, (float)y, transform.position.z);
        transform.eulerAngles = new Vector3(0, 0, (float)heading);
        name = "Node " + index + " " + PJ.ACTION[index];
    }

    private void SwitchToSprites(string action)
    {
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

    }

    private void HandleMouseMovement()
    {

        // change size if touching mouse pointer
        if (hovering)
            transform.localScale += ExtensionMethods.round(10 * (new Vector3(1.44f, 1.44f, 1) - transform.localScale) * Time.deltaTime, 2);
        else if (!hovering && !dragging)
            transform.localScale += ExtensionMethods.round(10 * (new Vector3(0.72f, 0.72f, 1) - transform.localScale) * Time.deltaTime, 2);

        if (dragging && (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Delete)))
        {
            int end = index + 1;
            while (end < PJ.X.Count && !PJ.ACTION[end].Equals("DRIVE")) end++;
            for (int i = end - 1; i >= index; i--)
                PJ.RemoveAt(i);
            return;
        }

        // drag logic
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (dragging && !(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.RightCommand)))
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
            if (index == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (Hypot(snapPoints[i, 0] - mousePos.x, snapPoints[i, 1] - mousePos.y) < 0.5)
                    {
                        GoTo(new Vector2((float)snapPoints[i, 0], (float)snapPoints[i, 1]));
                        snapped = true;
                        break;
                    }
                }
            }

            if (!snapped)
            {
                // mode controls
                if (PJ.MODE.Equals("ANGULAR") || index == 0)
                {
                    GoTo(new Vector3(
                        (float)RoundToNearest(mousePos.x, step),
                        (float)RoundToNearest(mousePos.y, step),
                        0));
                }
                if (PJ.MODE.Equals("CARDINAL") && index > 0)
                {
                    float clickX = (float)(PJ.X[Math.Max(index - 1, 0)] / PJ.in_per_px);
                    float clickY = (float)(PJ.Y[Math.Max(index - 1, 0)] / PJ.in_per_px);

                    double dir = Mathf.Atan2(mousePos.y - clickY, mousePos.x - clickX) * (180 / Mathf.PI);
                    double snappedAngle = RoundToNearest(dir, 45.0);

                    double magnitude = Hypot(mousePos.y - clickY, mousePos.x - clickX);

                    float dx = (float)(RoundToNearest(magnitude, step) * Math.Cos(snappedAngle * (Mathf.PI / 180)));
                    float dy = (float)(RoundToNearest(magnitude, step) * Math.Sin(snappedAngle * (Mathf.PI / 180)));

                    float new_x, new_y;

                    if (snappedAngle % 90 != 0)
                    {
                        float diff_snapped_x = Math.Abs(Mathf.Min(5, Mathf.Max(-5, clickX + dx)) - (clickX + dx));
                        float diff_snapped_y = Math.Abs(Mathf.Min(5, Mathf.Max(-5, clickY + dy)) - (clickY + dy));

                        if (diff_snapped_x > diff_snapped_y)
                        {
                            new_x = clickX + dx + diff_snapped_x * -Math.Sign(dx);
                            new_y = clickY + dy + diff_snapped_x * -Math.Sign(dy);
                        }
                        else
                        {
                            new_x = clickX + dx + diff_snapped_y * -Math.Sign(dx);
                            new_y = clickY + dy + diff_snapped_y * -Math.Sign(dy);
                        }
                    }
                    else
                    {
                        new_x = clickX + dx;
                        new_y = clickY + dy;
                    }


                    GoTo(new Vector2(new_x, new_y));
                }
            }

        }

        // turning
        if (Input.GetMouseButton(0) && dragging && !PrecisionSlider.beingDragged && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.RightCommand)))
        {
            double ipp = PJ.in_per_px;
            double x = PJ.X[index] / ipp, y = PJ.Y[index] / ipp;
            double step = PJ.MODE.Equals("ANGULAR") ? 30.0 / PJ.precision : (PJ.MODE.Equals("CARDINAL") ? 45.0 : 0);
            double dir = RoundToNearest(Mathf.Atan2(mousePos.y - (float)y, mousePos.x - (float)x) * (180 / Mathf.PI), step);


            string nextAction = PJ.ACTION[Math.Min(index + 1, PJ.X.Count - 1)];

            double delta = 0;
            bool nextIsTurn = false;
            if (index != PJ.X.Count - 1 && (nextAction.Equals("TURN_LEFT") || nextAction.Equals("TURN_RIGHT")))
            {
                double diff = NormalizeAngle(dir - PJ.HEADING[index]);
                delta = NormalizeAngle(dir - PJ.HEADING[index + 1]);

                if (diff == 0)
                {
                    PJ.RemoveAt(index + 1);
                }
                else
                {
                    string turnAction = diff > 0 ? "TURN_LEFT" : "TURN_RIGHT";
                    PJ.HEADING[index + 1] = dir;
                    PJ.ACTION[index + 1] = turnAction;
                }

                nextIsTurn = true;

            } 
            else if (index != 0 && NormalizeAngle(dir - PJ.HEADING[index]) != 0)
            {
                delta = NormalizeAngle(dir - PJ.HEADING[index]);
                double diff = NormalizeAngle(dir - PJ.HEADING[index]);
                string turnAction = diff > 0 ? "TURN_LEFT" : "TURN_RIGHT";
                PJ.Add(index + 1, (PJ.X[index], PJ.Y[index], dir, PJ.ARM[index], turnAction, 0L));
            } 
            else if (index == 0)
            {
                delta = NormalizeAngle(dir - PJ.HEADING[index]);
            }

            int startIndex = nextIsTurn ? index + 2 : (index > 0 ? index + 1 : index);
            for (int i = startIndex; i < PJ.X.Count; i++)
            {
                PJ.HEADING[i] = NormalizeAngle(PJ.HEADING[i] + (float)delta);
            }

        }

    }

    private void OnMouseDown()
    {
        dragging = true;
    }

    private void OnMouseUp()
    {
        dragging = false;
    }

    private void OnMouseEnter()
    {
        if (!Input.GetMouseButton(0))
            hovering = true;
    }

    private void OnMouseExit()
    {
        hovering = false;
    }

    private void GoTo(Vector2 vector2)
    {
        for (int i = index; i < PJ.X.Count; i++)
        {
            if (i == index || !PJ.ACTION[i].Equals("DRIVE"))
            {
                PJ.X[i] = Mathf.Min(5, Mathf.Max(-5, vector2.x)) * PJ.in_per_px;
                PJ.Y[i] = Mathf.Min(5, Mathf.Max(-5, vector2.y)) * PJ.in_per_px;
            } else break;
        }

    }

    private double Round(double x)
    {
        return System.Math.Round(x);
    }

    private double Round(double x, int places)
    {
        return System.Math.Round(x, places);
    }

    private double RoundToNearest(double x, double multiple)
    {
        return System.Math.Round(x / multiple) * multiple;
    }

    private double Hypot(double x, double y)
    {
        return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
    }

    private double NormalizeAngle(double theta)
    {
        double th = theta;
        while (th > 180)
            th -= 360;
        while (th <= -180)
            th += 360;
        return th;
    }

}
