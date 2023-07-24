using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Stores all the data from generated dungeon
/// </summary>
/// 
[System.Serializable]
public class Dungeon
{
    public List<Room> Rooms { get; set; }
    public HashSet<Vector2Int> Path { get; set; }

    public Dungeon()
    {
        Rooms = new List<Room>();
        Path = new HashSet<Vector2Int>();
    }

    public Dungeon(Dungeon otherDungeon)
    {
        Rooms = otherDungeon.Rooms.ConvertAll(room => room.Clone()).ToList();
        Path = otherDungeon.Path;
    }

    public HashSet<Vector2Int> GetDungeonFloorPlan()
    {
        HashSet<Vector2Int> floorPlan = new();

        foreach (var room in Rooms)
        {
            floorPlan.UnionWith(room.FloorPositions);
        }

        floorPlan.UnionWith(Path);

        return floorPlan;
    }

    public void ResetDungeon()
    {
        foreach (Room room in Rooms)
        {
            room.ResetRoom();
        }

        Path.Clear();
    }

    public void PrintDungeon()
    {
        var message = "Tile Coordinates: [";

        foreach (var room in Rooms)
        {
            foreach(var pos in room.FloorPositions)
            {
                message += "(" + pos.x + ", " + pos.y + ")";
            }
        }

        message += "]\nPath:[";

        foreach(var pos in Path)
        {
            message += "(" + pos.x + ", " + pos.y + ")";
        }

        message += "]";

        Debug.Log(message);
    }
}