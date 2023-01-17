using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// Stores all the data from generated dungeon
/// </summary>
public class Dungeon : NetworkBehaviour
{
    public List<Room> Rooms { get; set; } = new();
    public HashSet<Vector2Int> Path { get; set; } = new();
    public List<GameObject> Players { get; set; } = new();

    public HashSet<Vector2Int> GetDungeonFloorTiles()
    {
        HashSet<Vector2Int> dungeonFloor = new();
        foreach (Room room in Rooms)
        {
            dungeonFloor.UnionWith(room.FloorTiles);
        }

        dungeonFloor.UnionWith(Path);
        return dungeonFloor;
    }

    public void Reset()
    {
        foreach (Room room in Rooms)
        {
            foreach(var item in room.Props)
            {
                Destroy(item);
            }
            foreach(var item in room.Enemies)
            {
                Destroy(item);
            }
        }
        foreach (var player in Players)
        {
            Destroy(player);
        }
        Rooms = new List<Room>();
        Path = new HashSet<Vector2Int>();
        Players = new List<GameObject>();
    }
}
