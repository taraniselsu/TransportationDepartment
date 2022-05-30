using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TrackBuilder : MonoBehaviour
{
    public enum State { Placing, Deleting, None }

    [SerializeField] private Camera theCamera;
    [SerializeField] private TrackSection[] trackPrefabs;

    private readonly Plane groundPlane = new(Vector3.up, 0);

    private int selectedTrackType = -1;
    private TrackSection selectedObject = null;
    private State state = State.None;

    private GameData gameData;
    private float gridSize;

    private void Start()
    {
        GameManager gameManager = GameManager.Instance;
        gameData = gameManager.gameData;
        gridSize = gameManager.GridSize;
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard.digit1Key.wasPressedThisFrame)
        {
            SelectStraightTrack();
        }
        if (keyboard.digit2Key.wasPressedThisFrame)
        {
            SelectCurvedTrack();
        }
        if (keyboard.digit3Key.wasPressedThisFrame)
        {
            RemoveMode();
        }
        if (keyboard.rKey.wasPressedThisFrame)
        {
            Rotate();
        }
        if (keyboard.escapeKey.wasPressedThisFrame || keyboard.digit0Key.wasPressedThisFrame)
        {
            UnselectTrack();
        }

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            Mouse mouse = Mouse.current;
            Ray ray = theCamera.ScreenPointToRay(mouse.position.ReadValue());
            if (groundPlane.Raycast(ray, out float distance))
            {
                Vector3 point = ray.GetPoint(distance);
                Vector3 gridCoord = new(
                    Mathf.RoundToInt(point.x / gridSize) * gridSize,
                    0,
                    Mathf.RoundToInt(point.z / gridSize) * gridSize);

                if (state == State.Placing)
                {
                    selectedObject.transform.position = gridCoord;
                }

                if (mouse.leftButton.wasPressedThisFrame)
                {
                    if (state == State.Placing)
                    {
                        TrackSection newTrack = Instantiate(selectedObject, selectedObject.transform.position, selectedObject.transform.rotation);
                        gameData.track.Add(newTrack);
                    }
                    else if (state == State.Deleting)
                    {
                        List<TrackSection> tracksToDelete = new();

                        foreach (TrackSection trackSection in gameData.track)
                        {
                            if (trackSection.transform.position == gridCoord)
                            {
                                tracksToDelete.Add(trackSection);
                            }
                        }

                        foreach (TrackSection trackSection in tracksToDelete)
                        {
                            gameData.track.Remove(trackSection);
                            Destroy(trackSection.gameObject);
                        }
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
        state = State.Placing;
    }

    public void UnselectTrack()
    {
        if (selectedObject)
        {
            Destroy(selectedObject.gameObject);
            selectedObject = null;
            selectedTrackType = -1;
        }
        state = State.None;
    }

    public void SelectStraightTrack()
    {
        SelectTrack(0);
    }

    public void SelectCurvedTrack()
    {
        SelectTrack(1);
    }

    public void Rotate()
    {
        if (selectedObject)
        {
            selectedObject.transform.Rotate(0, 90f, 0);
        }
    }

    public void RemoveMode()
    {
        UnselectTrack();
        state = State.Deleting;
    }
}
