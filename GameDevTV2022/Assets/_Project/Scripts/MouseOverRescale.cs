using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOverRescale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private float newScale = 1.1f;

    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rectTransform.localScale = new Vector3(newScale, newScale, newScale);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rectTransform.localScale = Vector3.one;
    }

    public void OnSelect(BaseEventData eventData)
    {
        rectTransform.localScale = new Vector3(newScale, newScale, newScale);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        rectTransform.localScale = Vector3.one;
    }
}
