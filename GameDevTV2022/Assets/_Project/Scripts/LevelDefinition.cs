using UnityEngine;

[CreateAssetMenu]
public class LevelDefinition : ScriptableObject
{
    [NonReorderable]
    public DepartmentDefinition[] departments;
}
