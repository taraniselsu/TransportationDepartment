using System;
using UnityEngine;

[Serializable]
public struct DepartmentDefinition
{
    public enum Direction { North, East, South, West };

    [Multiline] public string name;
    public Vector2Int position;
    public Direction direction;
}
