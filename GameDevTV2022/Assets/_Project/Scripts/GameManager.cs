using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    public static GameManager Instance => instance;

    [SerializeField] private float gridSize = 18f;
    [SerializeField] private int maxNumSteps = 100;
    [SerializeField] private LevelDefinition[] levelDefinitions;
    [SerializeField] private Department departmentPrefab;
    [SerializeField] private TrackBuilder trackBuilder;
    [SerializeField] private Train train;

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private LineRenderer lineRenderer2;
    [SerializeField] private float fadeTime = 1f;

    [Header("TESTING")]
    [SerializeField] private int initialLevel = 0;

    public readonly GameData gameData = new();

    public float GridSize => gridSize;

    private static readonly Vector3[] vectorDirections = { Vector3.forward, Vector3.right, Vector3.back, Vector3.left };

    public struct RouteStep
    {
        public TrackSection trackSection;
        public int direction;

        public RouteStep(TrackSection trackSection, int direction)
        {
            this.trackSection = trackSection;
            this.direction = direction;
        }
    }

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }

        instance = this;
    }

    private void OnEnable()
    {
        trackBuilder.OnTrackChanged += TrackChanged;
    }

    private void Start()
    {
        SpawnLevel(initialLevel);
    }

    private void OnDisable()
    {
        trackBuilder.OnTrackChanged -= TrackChanged;
    }

    private void SpawnLevel(int level)
    {
        gameData.currentLevel = level;
        gameData.departments.Clear();
        gameData.track.Clear();
        gameData.trainRoute.Clear();

        LevelDefinition levelDefinition = levelDefinitions[level];

        SpawnDepartments(levelDefinition);
        SpawnTrain(levelDefinition);
    }

    private void SpawnDepartments(LevelDefinition levelDefinition)
    {
        foreach (DepartmentDefinition definition in levelDefinition.departments)
        {
            Vector3 position = new(definition.position.x * gridSize, 0, definition.position.y * gridSize);
            Quaternion rotation = Quaternion.Euler(0, 90f * (int)definition.direction, 0);

            Department department = Instantiate(departmentPrefab, position, rotation);
            department.name = definition.name;
            department.GetComponentInChildren<TextMeshProUGUI>().text = definition.name;
            department.GetComponentInChildren<Canvas>().transform.rotation = Quaternion.Euler(89f, 0, 0);

            gameData.departments.Add(department);
            gameData.track.AddRange(department.GetComponentsInChildren<TrackSection>());

            Debug.LogFormat(this, "Spawning '{0}' at {1} facing {2}", department.name, department.transform.position, department.transform.rotation.eulerAngles);
        }
    }

    private void SpawnTrain(LevelDefinition levelDefinition)
    {
        train.transform.position = new(levelDefinition.trainStartingPosition.x * gridSize, 0, levelDefinition.trainStartingPosition.y * gridSize);
        train.transform.rotation = Quaternion.Euler(0, 90f * (int)levelDefinition.trainStartingDirection, 0);
        train.Speed = 0;
    }

    private void TrackChanged()
    {
        Debug.Log("GameManager.TrackChanged");

        TrackSection startingTrack = gameData.track.OrderBy(t => (t.transform.position - train.transform.position).sqrMagnitude).First();
        Assert.IsNotNull(startingTrack, "Cannot find a track near the train's current position");

        int rotation = Mathf.RoundToInt(WrapAngle360(startingTrack.transform.rotation.eulerAngles.y) / 90f % 4);
        Quaternion q1 = Quaternion.Euler(0, 90f * ((int)startingTrack.connections[0] + rotation), 0);
        Quaternion q2 = Quaternion.Euler(0, 90f * ((int)startingTrack.connections[1] + rotation), 0);

        float angle1 = Quaternion.Angle(train.transform.rotation, q1);
        float angle2 = Quaternion.Angle(train.transform.rotation, q2);
        Debug.LogFormat("{0}    {1}    {2}", train.transform.rotation, q1, angle1);
        Debug.LogFormat("{0}    {1}    {2}", train.transform.rotation, q2, angle2);

        int startingDirection;
        if (angle1 < angle2)
        {
            startingDirection = ((int)startingTrack.connections[0] + rotation) % 4;
        }
        else
        {
            startingDirection = ((int)startingTrack.connections[1] + rotation) % 4;
        }

        RouteStep step = new(startingTrack, startingDirection);
        Debug.LogFormat(this, "Starting at {0} in direction {1}", step.trackSection.transform.position, step.direction);

        List<RouteStep> route = FindRoute(new List<RouteStep>() { step });
        if (route != null)
        {
            StopAllCoroutines();

            Vector3[] positions = route.Select(s => s.trackSection.transform.position).ToArray();
            lineRenderer.positionCount = positions.Length;
            lineRenderer.SetPositions(positions);
            lineRenderer.enabled = true;
            StartCoroutine(FadeLineRenderer(lineRenderer));

            List<Waypoint> waypoints = CreateWaypointRoute(route, startingDirection);
            int closestWaypointIndex = FindClosestWaypoint(waypoints);

            lineRenderer2.positionCount = waypoints.Count;
            lineRenderer2.SetPositions(waypoints.Select(w => w.transform.position).ToArray());
            lineRenderer2.enabled = true;
            StartCoroutine(FadeLineRenderer(lineRenderer2));

            Debug.Log("Route waypoints:\n" + string.Join("\n", waypoints.Select(w => w.transform.position)));
            Debug.Log("Closest: " + closestWaypointIndex);

            gameData.trainRoute.Clear();
            gameData.trainRoute.AddRange(waypoints);
            gameData.nextWaypoint = (closestWaypointIndex + 1) % waypoints.Count;
            train.Speed = 10f;

            Debug.LogFormat(this, "Success! {0}", string.Join(", ", positions));
        }
        else
        {
            StopAllCoroutines();
            lineRenderer.enabled = false;
            lineRenderer2.enabled = false;
            train.Speed = 0f;

            gameData.nextWaypoint = 0;
            gameData.trainRoute.Clear();
        }
    }

    private List<RouteStep> FindRoute(List<RouteStep> route)
    {
        if (route.Count > maxNumSteps)
        {
            Debug.LogWarningFormat(this, "Exceeded max number of steps");
            return null;
        }

        RouteStep lastStep = route[^1];

        Vector3 nextPosition = lastStep.trackSection.transform.position + gridSize * vectorDirections[lastStep.direction];
        foreach (TrackSection nextTrack in gameData.track)
        {
            if ((nextTrack.transform.position - nextPosition).sqrMagnitude < 1f)
            {
                int rotation = Mathf.RoundToInt(WrapAngle360(nextTrack.transform.rotation.eulerAngles.y) / 90f % 4);

                // Important: assumes each track section only has two connections
                for (int i = 0; i < 2; i++)
                {
                    Direction connection = nextTrack.connections[i];
                    if (((int)connection + rotation) % 4 == (lastStep.direction + 2) % 4)
                    {
                        // there is a connection between the last track and the next one

                        // find the direction of the next track's other connection
                        Direction otherConnection = nextTrack.connections[(i + 1) % 2];
                        int newDirection = ((int)otherConnection + rotation) % 4;

                        RouteStep nextStep = new(nextTrack, newDirection);

                        if (route.Contains(nextStep))
                        {
                            // Must have found a loop because we already did this track section, in this direction
                            if (gameData.departments.All(d => route.Any(s => (s.trackSection.transform.position - d.transform.position).sqrMagnitude < 1f)))
                            {
                                Debug.LogFormat(this, "Already did the track section at {0} in direction {1}, does include all departments", nextStep.trackSection.transform.position, nextStep.direction);
                                return route;
                            }
                            else
                            {
                                Debug.LogWarningFormat(this, "Already did the track section at {0} in direction {1}, does not include all departments", nextStep.trackSection.transform.position, nextStep.direction);
                                return null;
                            }
                        }
                        else
                        {
                            Debug.LogFormat(this, "Found more track at {0} in direction {1}", nextStep.trackSection.transform.position, nextStep.direction);
                            List<RouteStep> temp = new(route);
                            temp.Add(nextStep);

                            List<RouteStep> newRoute = FindRoute(temp);
                            if (newRoute != null)
                            {
                                return newRoute;
                            }
                        }
                    }
                }
            }
        }

        return null;
    }

    private static List<Waypoint> CreateWaypointRoute(List<RouteStep> route, int startingDirection)
    {
        List<Waypoint> waypoints = new();
        int currentDirection = startingDirection;

        foreach (RouteStep step in route)
        {
            int rotation = Mathf.RoundToInt(WrapAngle360(step.trackSection.transform.rotation.eulerAngles.y) / 90f % 4);
            int connection0 = ((int)step.trackSection.connections[0] + rotation) % 4;

            var stepWaypoints = step.trackSection.GetComponentsInChildren<Waypoint>();

            if (connection0 != (currentDirection + 2) % 4)
            {
                waypoints.AddRange(stepWaypoints.Reverse());
                currentDirection = ((int)step.trackSection.connections[0] + rotation) % 4;
            }
            else
            {
                waypoints.AddRange(stepWaypoints);
                currentDirection = ((int)step.trackSection.connections[1] + rotation) % 4;
            }
        }

        return waypoints;
    }

    private int FindClosestWaypoint(List<Waypoint> waypoints)
    {
        int closestIndex = -1;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < waypoints.Count; i++)
        {
            float distance = (waypoints[i].transform.position - train.transform.position).sqrMagnitude;
            if (distance < closestDistance)
            {
                closestIndex = i;
                closestDistance = distance;
            }
        }

        return closestIndex;
    }

    private static float WrapAngle360(float angle)
    {
        float result = angle % 360;

        if (result < 0)
        {
            result += 360;
        }

        return result;
    }

    private IEnumerator FadeLineRenderer(LineRenderer lineRenderer)
    {
        float remainingTime = fadeTime;

        while (remainingTime > 0)
        {
            float t = Mathf.Clamp01(remainingTime / fadeTime);

            Color c = lineRenderer.startColor;
            c.a = t;
            lineRenderer.startColor = c;
            lineRenderer.endColor = c;

            remainingTime -= Time.deltaTime;

            yield return null;
        }
    }
}
