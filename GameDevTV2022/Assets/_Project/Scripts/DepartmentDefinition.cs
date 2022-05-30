using System;
using UnityEngine;

[Serializable]
public struct DepartmentDefinition
{
    [Multiline] public string name;
    public Vector2Int position;
    public Direction direction;
}
