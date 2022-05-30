using UnityEngine;

public class Train : MonoBehaviour
{
    [SerializeField, Range(-20f, 20f)] private float speed = 0;

    private GameData gameData = null;

    public float Speed { get => speed; set => speed = value; }

    private void Start()
    {
        gameData = GameManager.Instance.gameData;
    }

    private void Update()
    {
        if (Mathf.Abs(speed) < 0.1f)
            return;

        Vector3 position = Move();
        Rotate(position);
    }

    private Vector3 Move()
    {
        Vector3 currentPosition = transform.position;
        float moveDistance = speed * Time.deltaTime;
        while (moveDistance > 0)
        {
            Vector3 nextWaypointPosition = gameData.trainRoute[gameData.nextWaypoint].transform.position;
            Vector3 delta = nextWaypointPosition - currentPosition;
            float distance = delta.magnitude;

            if (distance > moveDistance)
            {
                currentPosition += moveDistance * delta.normalized;
                break;
            }
            else
            {
                currentPosition = nextWaypointPosition;
                moveDistance -= distance;
                gameData.nextWaypoint = (gameData.nextWaypoint + 1) % gameData.trainRoute.Count;
            }
        }

        transform.position = currentPosition;
        return currentPosition;
    }

    private void Rotate(Vector3 currentPosition)
    {
        int previousWaypoint = gameData.nextWaypoint > 0 ? gameData.nextWaypoint - 1 : gameData.trainRoute.Count - 1;
        Transform w1 = gameData.trainRoute[previousWaypoint].transform;
        Transform w2 = gameData.trainRoute[gameData.nextWaypoint].transform;

        Vector3 relPos = currentPosition - w1.position;
        Vector3 lineSegment = w2.position - w1.position;
        Vector3 lineDir = lineSegment.normalized;
        float dot = Vector3.Dot(relPos, lineDir);

        float t = dot / lineSegment.magnitude;

        Quaternion startRot = w1.rotation;
        if (Mathf.Abs(Quaternion.Angle(transform.rotation, startRot)) > 100f)
        {
            startRot = Quaternion.AngleAxis(180f, Vector3.up) * startRot;
        }

        Quaternion endRot = w2.rotation;
        if (Mathf.Abs(Quaternion.Angle(transform.rotation, endRot)) > 100f)
        {
            endRot = Quaternion.AngleAxis(180f, Vector3.up) * endRot;
        }

        transform.rotation = Quaternion.Slerp(startRot, endRot, t);
    }

    private void OnDrawGizmos()
    {
        if (gameData != null && gameData.trainRoute.Count > 0)
        {
            Vector3 nextWaypointPosition = gameData.trainRoute[gameData.nextWaypoint].transform.position;

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, nextWaypointPosition);
            Gizmos.DrawSphere(nextWaypointPosition, 0.25f);
        }
    }
}
