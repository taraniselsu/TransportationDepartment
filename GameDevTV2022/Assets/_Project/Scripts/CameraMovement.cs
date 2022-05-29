using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float keySpeed = 1f;
    [SerializeField] private float mouseSpeed = 1f;

    private void Update()
    {
        Vector3 move = Vector3.zero;

        Keyboard keyboard = Keyboard.current;
        if (keyboard.wKey.isPressed)
        {
            move.z += 1;
        }

        if (keyboard.sKey.isPressed)
        {
            move.z -= 1;
        }

        if (keyboard.aKey.isPressed)
        {
            move.x -= 1;
        }

        if (keyboard.dKey.isPressed)
        {
            move.x += 1;
        }

        move *= keySpeed * Time.deltaTime;

        Mouse mouse = Mouse.current;
        if (mouse.rightButton.isPressed)
        {
            Vector2 temp = mouse.delta.ReadValue();
            move += mouseSpeed * new Vector3(-temp.x, 0, -temp.y);
        }

        transform.position += move;
    }
}
