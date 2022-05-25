using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Train : MonoBehaviour
{
    [SerializeField, Range(-5f, 5f)] private float speed = 0;

    private readonly List<Vector3> allWaypoints = new();

    private void Start()
    {
        GetAllWaypoints();
    }

    private void FixedUpdate()
    {
        if (Mathf.Abs(speed) < 0.1f)
            return;

        Vector3 temp = transform.position + speed * Time.fixedDeltaTime * transform.forward;

        CompareDistances comparer = new() { reference = temp };
        allWaypoints.Sort(comparer);

        Vector3 v1 = allWaypoints.First(w => Vector3.Dot(w - transform.position, transform.forward) > 0f);
        Vector3 v2 = allWaypoints.First(w => Vector3.Dot(w - transform.position, transform.forward) < 0f);
        Vector3 newPos = ClosestPointOnLine(temp, v1, v2);

        Vector3 dir = (newPos - transform.position).normalized;
        Quaternion q = Quaternion.LookRotation(dir, Vector3.up);

        transform.position = newPos;
        transform.rotation = q;
    }

    private void GetAllWaypoints()
    {
        allWaypoints.Clear();

        var allWaypointContainers = FindObjectsOfType<Waypoints>();
        foreach (var wc in allWaypointContainers)
        {
            foreach (Transform child in wc.transform)
            {
                allWaypoints.Add(child.position);
            }
        }
    }

    public static Vector3 ClosestPointOnLine(Vector3 pos, Vector3 start, Vector3 end)
    {
        Vector3 v = pos - start;
        Vector3 lineDir = (end - start).normalized;
        float dot = Vector3.Dot(v, lineDir);
        Vector3 result = start + dot * lineDir;
        return result;
    }

    private struct CompareDistances : IComparer<Vector3>
    {
        public Vector3 reference;

        public int Compare(Vector3 left, Vector3 right)
        {
            float leftDistance = (left - reference).sqrMagnitude;
            float rightDistance = (right - reference).sqrMagnitude;
            return leftDistance.CompareTo(rightDistance);
        }
    }

    private void OnDrawGizmos()
    {
        if (allWaypoints.Count < 2)
            return;

        CompareDistances comparer = new() { reference = transform.position };
        allWaypoints.Sort(comparer);

        Vector3 v1 = allWaypoints.First(w => Vector3.Dot(w - transform.position, transform.forward) > 0f);
        Vector3 v2 = allWaypoints.First(w => Vector3.Dot(w - transform.position, transform.forward) < 0f);
        Vector3 p = ClosestPointOnLine(transform.position, v1, v2);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, v1);
        Gizmos.DrawLine(transform.position, v2);
        Gizmos.DrawSphere(v1, 0.25f);
        Gizmos.DrawSphere(v2, 0.25f);

        Gizmos.color = new Color(0.5f, 1f, 0.5f);
        Gizmos.DrawSphere(p, 0.25f);
    }
}
