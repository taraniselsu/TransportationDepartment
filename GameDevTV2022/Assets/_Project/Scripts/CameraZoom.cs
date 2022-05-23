using Cinemachine;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Vector2 range = new(30, 200);
    [SerializeField] private float speed = 1;

    private CinemachineFramingTransposer framingTransposer;

    private void Start()
    {
        Assert.IsNotNull(virtualCamera);
        framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        Assert.IsNotNull(framingTransposer);
    }

    private void Update()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;
        float newValue = framingTransposer.m_CameraDistance * (1 - scroll * speed);
        newValue = Mathf.Clamp(newValue, range.x, range.y);
        framingTransposer.m_CameraDistance = newValue;
    }
}
