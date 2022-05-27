using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOverAudioPlayer : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip click;
    [SerializeField] [Range(0, 1)] private float clickVolume = 1f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        audioSource.PlayOneShot(click, clickVolume);
    }

    public void OnSelect(BaseEventData eventData)
    {
        audioSource.PlayOneShot(click, clickVolume);
    }
}
