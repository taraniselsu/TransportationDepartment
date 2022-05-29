using UnityEngine;
using UnityEngine.InputSystem;

public class TrackBuilder : MonoBehaviour
{
    [SerializeField] private Camera theCamera;
    [SerializeField] private GameObject[] trackPrefabs;
    [SerializeField] private float gridSize = 18f;

    private readonly Plane groundPlane = new(Vector3.up, 0);

    private int selectedTrackType = -1;
    private GameObject selectedObject = null;

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard.digit1Key.wasPressedThisFrame)
        {
            SelectTrack(0);
        }
        if (keyboard.digit2Key.wasPressedThisFrame)
        {
            SelectTrack(1);
        }
        if (keyboard.rKey.wasPressedThisFrame)
        {
            selectedObject.transform.Rotate(0, 90f, 0);
        }
        if (keyboard.escapeKey.wasPressedThisFrame)
        {
            UnselectTrack();
        }

        Mouse mouse = Mouse.current;
        Ray ray = theCamera.ScreenPointToRay(mouse.position.ReadValue());
        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 point = ray.GetPoint(distance);
            Vector3 gridCoord = new(
                Mathf.RoundToInt(point.x / gridSize) * gridSize,
                0,
                Mathf.RoundToInt(point.z / gridSize) * gridSize);

            if (selectedObject)
            {
                selectedObject.transform.position = gridCoord;

                if (mouse.leftButton.wasPressedThisFrame)
                {
                    //Debug.LogFormat(this, "Clicked at {0} which is grid square {1}", point, gridCoord);

                    GameObject newTrack = Instantiate(selectedObject);
                    newTrack.transform.SetParent(transform);
                    newTrack.transform.position = gridCoord;
                }
            }
            else if (keyboard.deleteKey.wasPressedThisFrame)
            {
                foreach (Transform child in transform)
                {
                    if (child.position == gridCoord)
                    {
                        Destroy(child.gameObject);
                    }
                }
            }
        }
    }

    private void SelectTrack(int index)
    {
        if (selectedTrackType != index)
        {
            UnselectTrack();

            selectedTrackType = index;
            selectedObject = Instantiate(trackPrefabs[selectedTrackType]);
        }
    }

    private void UnselectTrack()
    {
        if (selectedObject)
        {
            Destroy(selectedObject);
            selectedObject = null;
            selectedTrackType = -1;
        }
    }
}
