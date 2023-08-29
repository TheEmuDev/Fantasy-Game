using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds all data associated with a dungeon room
/// </summary>
[System.Serializable]
public class Room
{
    public Vector2Int RoomCenter { get; set; }
    public HashSet<Vector2Int> FloorPositions { get; set; }

    public Room()
    {
        RoomCenter = new();
        FloorPositions = new();
    }

    public Room(RectInt roomBoundary)
    {
        RoomCenter = new((int)roomBoundary.center.x, (int)roomBoundary.center.y);
        HashSet<Vector2Int> room = new();
        for(int x = roomBoundary.min.x; x < roomBoundary.max.x; x++)
        {
            for(int y = roomBoundary.min.y; y < roomBoundary.max.y; y++)
            {
                room.Add(new Vector2Int(x, y));
            }
        }

        FloorPositions = room;
        Debug.Log("RectInt room - " + roomBoundary.ToString());
        PrintRoom();
    }

    public Room(Vector2Int roomCenter, HashSet<Vector2Int> floorPositions)
    {
        RoomCenter = roomCenter;
        FloorPositions = floorPositions;
    }

    public Room Clone()
    {
        return new Room(RoomCenter, FloorPositions);
    }

    public bool OverlapsWith(Room otherRoom, int padding = 0)
    {
        // Helper function to check if position is within the padding area of any tile in a room
        bool isWithinPadding(Vector2Int pos, Room room)
        {
            foreach (Vector2Int roomPos in room.FloorPositions)
            {
                if (Vector2Int.Distance(pos, roomPos) <= padding)
                {
                    return true;
                }
            }
            return false;
        }

        // Check all tiles in FloorPositions if they are within padding of room2
        foreach (Vector2Int pos in FloorPositions)
        {
            if (isWithinPadding(pos, otherRoom))
            {
                return true;
            }
        }

        return false;
    }

    public void ResetRoom()
    {
        RoomCenter = new();
        FloorPositions.Clear();
    }

    public void PrintRoom()
    {
        var message = "Room Center:(" + RoomCenter.x + ", " + RoomCenter.y + ")\nFloor Positions: [";

        foreach(Vector2Int roomPos in FloorPositions)
        {
            message += "(" + roomPos.x + ", " + roomPos.y + ")";
        }

        message += "]";

        Debug.Log(message);
    }
}