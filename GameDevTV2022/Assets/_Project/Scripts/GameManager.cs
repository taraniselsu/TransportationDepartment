using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    public static GameManager Instance => instance;

    [SerializeField] private float gridSize = 18f;
    [SerializeField] private LevelDefinition[] levelDefinitions;
    [SerializeField] private Department departmentPrefab;
    [SerializeField] private Train train;

    [Header("TESTING")]
    [SerializeField] private int initialLevel = 0;

    public readonly GameData gameData = new();

    public float GridSize => gridSize;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        SpawnLevel(initialLevel);
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
}
