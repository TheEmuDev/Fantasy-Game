using Mirror;
using System.Collections.Generic;
using UnityEngine;

public static class CustomSerialization
{
    public static void WriteDungeon(this NetworkWriter writer, Dungeon dungeon)
    {
        writer.WriteInt(dungeon.Rooms.Count);
        foreach( Room room in dungeon.Rooms )
        {
            writer.WriteRoom(room);
        }

        // Convert Path to a List or array to serialize it
        List<Vector2Int> pathList = new(dungeon.Path);
        writer.WriteInt(pathList.Count);
        foreach( Vector2Int pos in pathList)
        {
            writer.WriteVector2Int(pos);
        }
    }

    public static Dungeon ReadDungeon(this NetworkReader reader)
    {
        int roomCount = reader.ReadInt();
        List<Room> rooms = new();
        for(int i = 0; i < roomCount; i++)
        {
            rooms.Add(reader.ReadRoom());
        }

        int pathCount = reader.ReadInt();
        HashSet<Vector2Int> path = new();
        for(int i =0; i < pathCount; i++)
        {
            path.Add(reader.ReadVector2Int());
        }

        Dungeon dungeon = new Dungeon { Rooms = rooms, Path = path };
        return dungeon;
    }

    public static void WriteRoom(this NetworkWriter writer, Room room)
    {
        writer.WriteVector2Int(room.RoomCenter);

        // convert FloorPositions to List or array to serialize it
        List<Vector2Int> positionsList = new(room.FloorPositions);
        writer.WriteInt(positionsList.Count);
        foreach(Vector2Int pos in positionsList)
        {
            writer.WriteVector2Int(pos);
        }
    }

    public static Room ReadRoom(this NetworkReader reader)
    {
        Vector2Int roomCenter = reader.ReadVector2Int();

        int posCount = reader.ReadInt();
        HashSet<Vector2Int> floorPositions = new();
        for(int i = 0; i < posCount; i++)
        {
            floorPositions.Add(reader.ReadVector2Int());
        }

        Room room = new Room { RoomCenter = roomCenter, FloorPositions = floorPositions };
        return room;
    }
}
