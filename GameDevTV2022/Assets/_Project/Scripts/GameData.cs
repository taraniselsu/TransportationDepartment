using System.Collections.Generic;

public enum Direction { North, East, South, West };

public class GameData
{
    public int currentLevel = 0;
    public readonly List<Department> departments = new();
    public readonly List<TrackSection> track = new();
    public readonly List<Waypoint> trainRoute = new();
}
