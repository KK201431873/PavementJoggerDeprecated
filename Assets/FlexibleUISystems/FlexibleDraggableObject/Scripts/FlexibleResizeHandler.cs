using System;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public enum HandlerType
{
    TopRight,
    Right,
    BottomRight,
    Bottom,
    BottomLeft,
    Left,
    TopLeft,
    Top
}

[RequireComponent(typeof(EventTrigger))]
public class FlexibleResizeHandler : MonoBehaviour
{
    public HandlerType Type;
    public RectTransform Target, LabelBG;
    public Vector2 MinimumDimensions, MaximumDimensions;
    public static float topPosition;
    
    private EventTrigger _eventTrigger;
    
	void Start ()
	{
	    _eventTrigger = GetComponent<EventTrigger>();
        _eventTrigger.AddEventTrigger(OnDrag, EventTriggerType.Drag);

        if (Type == HandlerType.Top)
        {
            topPosition = transform.position.y;
            MinimumDimensions = new Vector2(1920, 200);
            MaximumDimensions = new Vector2(1920, 600);
        }

        if (Type == HandlerType.Right || Type == HandlerType.Left)
        {
            MinimumDimensions = new Vector2(200, 1080);
            MaximumDimensions = new Vector2(600, 1080);
        }

    }

    private void Update()
    {
        if (Type == HandlerType.Top)
        {
            topPosition = transform.position.y;
        }

        if (Type == HandlerType.Right || Type == HandlerType.Left)
        {
            transform.position = new Vector3(transform.position.x, (1080 + topPosition)/2f, transform.position.z);
        }

    }

    void OnDrag(BaseEventData data)
    {
        PointerEventData ped = (PointerEventData) data;
        //Target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Target.rect.width + ped.delta.x);
        //Target.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Target.rect.height + ped.delta.y);
        RectTransform.Edge? horizontalEdge = null;
        RectTransform.Edge? verticalEdge = null;

        switch (Type)
        {
            case HandlerType.TopRight:
                horizontalEdge = RectTransform.Edge.Left;
                verticalEdge = RectTransform.Edge.Bottom;
                break;
            case HandlerType.Right:
                horizontalEdge = RectTransform.Edge.Left;
                break;
            case HandlerType.BottomRight:
                horizontalEdge = RectTransform.Edge.Left;
                verticalEdge = RectTransform.Edge.Top;
                break;
            case HandlerType.Bottom:
                verticalEdge = RectTransform.Edge.Top;
                break;
            case HandlerType.BottomLeft:
                horizontalEdge = RectTransform.Edge.Right;
                verticalEdge = RectTransform.Edge.Top;
                break;
            case HandlerType.Left:
                horizontalEdge = RectTransform.Edge.Right;
                break;
            case HandlerType.TopLeft:
                horizontalEdge = RectTransform.Edge.Right;
                verticalEdge = RectTransform.Edge.Bottom;
                break;
            case HandlerType.Top:
                verticalEdge = RectTransform.Edge.Bottom;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        if (horizontalEdge != null)
        {
            if (horizontalEdge == RectTransform.Edge.Right)
            {
                float width = Mathf.Clamp(Target.rect.width - ped.delta.x, MinimumDimensions.x, MaximumDimensions.x);
                Target.SetInsetAndSizeFromParentEdge((RectTransform.Edge)horizontalEdge,
                    Screen.width - Target.position.x - Target.pivot.x * Target.rect.width,
                    width);
                LabelBG.sizeDelta = new Vector2(width, LabelBG.sizeDelta.y);
                LabelBG.localPosition = new Vector3(0, LabelBG.localPosition.y, LabelBG.localPosition.z);
            }
            else
            {
                float width = Mathf.Clamp(Target.rect.width + ped.delta.x, MinimumDimensions.x, MaximumDimensions.x);
                Target.SetInsetAndSizeFromParentEdge((RectTransform.Edge)horizontalEdge,
                    Target.position.x - Target.pivot.x * Target.rect.width,
                    width);
                LabelBG.sizeDelta = new Vector2(width, LabelBG.sizeDelta.y);
                LabelBG.localPosition = new Vector3(0, LabelBG.localPosition.y, LabelBG.localPosition.z);
            }
        }
        if (verticalEdge != null)
        {
            if (verticalEdge == RectTransform.Edge.Top)
            {
                Target.SetInsetAndSizeFromParentEdge((RectTransform.Edge)verticalEdge,
                    Screen.height - Target.position.y - Target.pivot.y * Target.rect.height,
                    Mathf.Clamp(Target.rect.height - ped.delta.y, MinimumDimensions.y, MaximumDimensions.y));
            }
            else
            {
                Target.SetInsetAndSizeFromParentEdge((RectTransform.Edge)verticalEdge,
                    Target.position.y - Target.pivot.y * Target.rect.height,
                    Mathf.Clamp(Target.rect.height + ped.delta.y, MinimumDimensions.y, MaximumDimensions.y));
            }
        }
    }
}
