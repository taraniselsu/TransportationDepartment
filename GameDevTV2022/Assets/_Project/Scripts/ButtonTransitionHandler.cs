using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class ButtonTransitionHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, IPointerDownHandler
{
    [SerializeField] private ButtonTransitionSettings settings;
    [SerializeField] private AudioSource audioSource;

    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHighlightedStart();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnHighlightedStop();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnHighlightedStart();
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        OnHighlightedStart();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        OnHighlightedStop();
    }

    private void OnHighlightedStart()
    {
        rectTransform.localScale = new Vector3(settings.selectedSize, settings.selectedSize, settings.selectedSize);
        StartCoroutine(Wiggle());
        audioSource.PlayOneShot(settings.click, settings.clickVolume);
    }

    private void OnHighlightedStop()
    {
        rectTransform.localScale = Vector3.one;
        StopAllCoroutines();
        rectTransform.localRotation = Quaternion.identity;
    }

    private IEnumerator Wiggle()
    {
        float startTime = Time.time;

        while (Time.time < startTime + settings.wiggleTime)
        {
            float z = (Random.value - 0.5f) * 2f * settings.wiggleAmount;
            rectTransform.localRotation = Quaternion.Euler(0, 0, z);

            yield return new WaitForSecondsRealtime(1f / 30);
        }

        rectTransform.localRotation = Quaternion.identity;
    }
}
