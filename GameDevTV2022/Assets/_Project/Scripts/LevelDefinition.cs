using UnityEngine;

[CreateAssetMenu]
public class LevelDefinition : ScriptableObject
{
    [Header("Departments")]
    [NonReorderable]
    public DepartmentDefinition[] departments;

    [Header("Train")]
    public Vector2Int trainStartingPosition;
    public Direction trainStartingDirection;
}
