using UnityEngine;

public class Waypoints : MonoBehaviour
{
    [SerializeField] public Vector3 v1 = new(0, 0, -1);
    [SerializeField] public Vector3 v2 = new(0, 0, 1);

    [SerializeField, Range(0f, 1f)] public float curve = 0.5f;
    [SerializeField, Min(2)] public int segments = 10;

    private void OnDrawGizmosSelected()
    {
        if (transform.childCount < 2)
            return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            Vector3 v1 = transform.GetChild(i).position;
            Vector3 v2 = transform.GetChild(i + 1).position;
            Gizmos.DrawLine(v1, v2);
            Gizmos.DrawSphere(v1, 0.2f);
        }

        Gizmos.DrawSphere(transform.GetChild(transform.childCount - 1).position, 0.2f);
    }
}
