using UnityEngine;
using UnityEngine.InputSystem;

public class ShowCredits : MonoBehaviour
{
    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            HideTheCredits();
        }
    }

    public void ShowTheCredits()
    {
        gameObject.SetActive(true);
    }

    public void HideTheCredits()
    {
        gameObject.SetActive(false);
    }
}
