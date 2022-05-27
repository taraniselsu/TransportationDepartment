using UnityEngine;

[CreateAssetMenu()]
public class ButtonTransitionSettings : ScriptableObject
{
    public float selectedSize = 1.1f;

    [Tooltip("Duration of wiggle")] public float wiggleTime = 0.2f;
    [Tooltip("Max angle to deflect during wiggle")] public float wiggleAmount = 30f;

    public AudioClip click;
    [Range(0, 1)] public float clickVolume = 1f;
}
