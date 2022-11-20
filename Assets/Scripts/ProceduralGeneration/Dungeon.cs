using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores all the data from generated dungeon
/// </summary>
public class Dungeon : MonoBehaviour
{
    public List<Room> Rooms { get; set; } = new List<Room>();
    public HashSet<Vector2Int> Path { get; set; } = new HashSet<Vector2Int>();
    public List<GameObject> Players { get; set; } = new List<GameObject>();

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
