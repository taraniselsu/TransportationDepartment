using TMPro;
using UnityEngine;

public class DepartmentSpawner : MonoBehaviour
{
    [SerializeField] private LevelDefinition levelDefinition;
    [SerializeField] private GameObject departmentPrefab;
    [SerializeField] private float gridSize = 18f;

    private void Start()
    {
        foreach (DepartmentDefinition department in levelDefinition.departments)
        {
            Vector3 position = new(department.position.x * gridSize, 0, department.position.y * gridSize);
            Quaternion rotation = Quaternion.Euler(0, 90f * (int)department.direction, 0);

            GameObject go = Instantiate(departmentPrefab, position, rotation);
            go.name = department.name;
            go.GetComponentInChildren<Canvas>().transform.rotation = Quaternion.Euler(89f, 0, 0);
            go.GetComponentInChildren<TextMeshProUGUI>().text = department.name;

            Debug.LogFormat(this, "Spawning '{0}' at {1} facing {2}", department.name, position, department.direction);
        }
    }
}
