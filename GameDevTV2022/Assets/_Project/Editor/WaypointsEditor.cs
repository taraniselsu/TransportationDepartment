using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Waypoints))]
public class WaypointsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Populate Waypoints"))
        {
            var w = target as Waypoints;
            DeleteAllChildren(w.transform);
            PopulateWaypoints(w);
        }
    }

    private void PopulateWaypoints(Waypoints w)
    {
        float step = 1f / w.segments;

        Vector3 v = new(
            w.curve * w.v1.x + (1 - w.curve) * w.v2.x,
            0,
            (1 - w.curve) * w.v1.z + w.curve * w.v2.z);

        for (float t = 0; t < 1f + step / 2f; t += step)
        {
            Vector3 p0 = w.v1;
            Vector3 p1 = v;
            Vector3 p2 = w.v2;

            Vector3 p = BezierCurves.BezierCurve(p0, p1, p2, t);
            Vector3 normal = 2 * (1 - t) * (p1 - p0) + 2 * t * (p2 - p1);
            CreateWaypoint(w.transform, p, normal);
        }
    }

    private static void CreateWaypoint(Transform parent, Vector3 position, Vector3 normal)
    {
        GameObject go = new("Waypoint");
        go.transform.SetParent(parent);
        go.transform.localPosition = position;
        go.transform.localRotation = Quaternion.LookRotation(normal, Vector3.up);
    }

    private static void DeleteAllChildren(Transform transform)
    {
        for (int i = transform.childCount - 1; i >= 0; --i)
        {
            Transform child = transform.GetChild(i);
            DestroyImmediate(child.gameObject);
        }
    }
}
