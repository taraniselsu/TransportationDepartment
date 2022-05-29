using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class DepartmentSpawner : MonoBehaviour
{
    [SerializeField] private TextAsset departmentNamesFile;
    [SerializeField] private GameObject departmentPrefab;
    [SerializeField] private int numDepartments = 3;
    [SerializeField] private float gridSize = 18f;
    [SerializeField] private Bounds bounds = new(Vector3.zero, new Vector3(10, 0, 10));

    private void Start()
    {
        string text = departmentNamesFile.text;
        Debug.Log(text);

        string[] departmentNames = text.Split("\r\n", System.StringSplitOptions.RemoveEmptyEntries);
        Debug.LogFormat(this, "Department names: {0}", string.Join(", ", departmentNames));

        var names = departmentNames.OrderBy(n => Random.value).Take(numDepartments);
        foreach (string name in names)
        {
            GameObject department = Instantiate(departmentPrefab);
            department.name = name;
            department.GetComponentInChildren<TextMeshProUGUI>().text = name;

            Vector3 position = GetRandomPosition();
            Quaternion rotation = Quaternion.Euler(0, 90f * Random.Range(0, 4), 0);

            department.transform.SetPositionAndRotation(position, rotation);

            Debug.LogFormat(this, "Spawning {0} at {1}", name, position);
        }
    }

    private Vector3 GetRandomPosition()
    {
        float x1 = Random.Range(bounds.min.x, bounds.max.x);
        float x2 = Random.Range(bounds.min.x, bounds.max.x);
        float x = (x1 + x2) / 2f;

        float z1 = Random.Range(bounds.min.z, bounds.max.z);
        float z2 = Random.Range(bounds.min.z, bounds.max.z);
        float z = (z1 + z2) / 2f;

        return new Vector3(Mathf.RoundToInt(x) * gridSize, 0, Mathf.RoundToInt(z) * gridSize);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(bounds.center * gridSize, bounds.size * gridSize);
    }
}
