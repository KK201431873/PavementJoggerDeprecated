
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PrecisionSlider : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
    [SerializeField] private Slider precisionSlider;
    [SerializeField] private Text sliderText;
    public static bool beingDragged = false;

    void Update()
    {
        if (PJ.precision != precisionSlider.value)
        {
            PJ.precision = precisionSlider.value;
            sliderText.text = "" + precisionSlider.value;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!Input.GetMouseButton(0))
            beingDragged = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!Input.GetMouseButton(0))
            beingDragged = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        beingDragged = false;
    }
}
