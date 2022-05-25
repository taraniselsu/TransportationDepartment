using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Train : MonoBehaviour
{
    [SerializeField, Range(-20f, 20f)] private float speed = 0;

    private readonly List<Transform> allWaypoints = new();

    private void Start()
    {
        GetAllWaypoints();
    }

    private void Update()
    {
        if (Mathf.Abs(speed) < 0.1f)
            return;

        Vector3 temp = transform.position + speed * Time.deltaTime * transform.forward;

        WaypointComparer comparer = new() { reference = temp };
        allWaypoints.Sort(comparer);

        Transform w1 = allWaypoints.First(w => Vector3.Dot(w.position - temp, transform.forward) < 0f);
        Transform w2 = allWaypoints.First(w => Vector3.Dot(w.position - temp, transform.forward) > 0f);

        Vector3 relPos = temp - w1.position;
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

        transform.position = w1.position + dot * lineDir;
        transform.rotation = Quaternion.Slerp(startRot, endRot, t);

        Debug.LogFormat(this, "dot = {0:f3}   t = {1:f3}", dot, t);
    }

    private void GetAllWaypoints()
    {
        allWaypoints.Clear();

        var allWaypointContainers = FindObjectsOfType<Waypoints>();
        foreach (var wc in allWaypointContainers)
        {
            foreach (Transform child in wc.transform)
            {
                allWaypoints.Add(child);
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

    private struct WaypointComparer : IComparer<Transform>
    {
        public Vector3 reference;

        public int Compare(Transform left, Transform right)
        {
            float leftDistance = (left.position - reference).sqrMagnitude;
            float rightDistance = (right.position - reference).sqrMagnitude;
            return leftDistance.CompareTo(rightDistance);
        }
    }

    private void OnDrawGizmos()
    {
        if (allWaypoints.Count < 2)
            return;

        WaypointComparer comparer = new() { reference = transform.position };
        allWaypoints.Sort(comparer);

        Transform w1 = allWaypoints.First(w => Vector3.Dot(w.position - transform.position, transform.forward) > 0f);
        Transform w2 = allWaypoints.First(w => Vector3.Dot(w.position - transform.position, transform.forward) < 0f);
        Vector3 p = ClosestPointOnLine(transform.position, w1.position, w2.position);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, w1.position);
        Gizmos.DrawLine(transform.position, w2.position);
        Gizmos.DrawSphere(w1.position, 0.25f);
        Gizmos.DrawSphere(w2.position, 0.25f);

        Gizmos.color = new Color(0.5f, 1f, 0.5f);
        Gizmos.DrawSphere(p, 0.25f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(w1.position, w1.forward);
        Gizmos.DrawRay(w2.position, w2.forward);
    }
}
