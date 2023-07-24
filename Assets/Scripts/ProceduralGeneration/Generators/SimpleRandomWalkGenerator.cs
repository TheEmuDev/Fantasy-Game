using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimpleRandomWalkGenerator : IDungeonGenerator
{
    public Dungeon GenerateDungeon(DungeonParameters parameters)
    {
        Debug.Log("SRWGenerator:GenerateDungeon");
        Dungeon dungeon = new();

        dungeon.Rooms.AddRange(PerformRandomWalk(parameters));
        dungeon.Path = ConnectRooms(dungeon);

        return dungeon;
    }

    private List<Room> PerformRandomWalk(DungeonParameters parameters)
    {
        Debug.Log("SRWGenerator:PerformRandomWalk");
        var startPoint = parameters.startPosition;
        Vector2Int roomCenter;
        
        List<Room> rooms = new();
        int roomCount = 0;

        do
        {
            HashSet<Vector2Int> walkPath = new()
            {
                startPoint
            };
            var previousPosition = startPoint;
            int minX = previousPosition.x;
            int minY = previousPosition.y;
            int maxX = previousPosition.x;
            int maxY = previousPosition.y;

            for (int i = 0; i < parameters.iterations; i++)
            {
                for (int j = 0; j < parameters.walkLength; j++)
                {
                    var newPosition = previousPosition + Direction2D.GetRandomCardinalDirection();
                    walkPath.Add(newPosition);
                    previousPosition = newPosition;

                    if (previousPosition.x > maxX) { maxX = previousPosition.x; }
                    if (previousPosition.x < minX) { minX = previousPosition.x; }
                    if (previousPosition.y > maxY) { maxY = previousPosition.y; }
                    if (previousPosition.y < minY) { minY = previousPosition.y; }
                }

                if (parameters.walkType == RandomWalkStartType.Random)
                {
                    previousPosition = walkPath.ElementAt(Random.Range(0, walkPath.Count));
                }

                if (parameters.walkType == RandomWalkStartType.StartPoint)
                {
                    previousPosition = parameters.startPosition;
                }
            }

            roomCenter = new Vector2Int((minX + maxX) / 2, (minY + maxY) / 2);
            Room newRoom = new(roomCenter, walkPath);

            if (ValidateRoom(newRoom, rooms, parameters.minimumRoomPadding))
            {
                rooms.Add(newRoom);
                Debug.Log("number of rooms: " + rooms.Count);
                roomCount++;
                // Determine next SRW start point
                if (parameters.minimumRooms > 1)
                {
                    startPoint = FindNextStartPoint(parameters, rooms, minX, minY, maxX, maxY);
                }
            }
        } while (roomCount < parameters.minimumRooms);
        
        return rooms;
    }

    // This function connects all rooms by their center points
    private HashSet<Vector2Int> ConnectRooms(Dungeon dungeon)
    {
        Debug.Log("SRWGenerator:ConnectRooms");
        HashSet<Vector2Int> corridors = new();
        HashSet<int> connectedRooms = new();

        if(dungeon.Rooms.Count < 2) { 
            corridors.Add(dungeon.Rooms.First().RoomCenter);
            return corridors;
        }

        Debug.Log("Dungeon has more than 1 room");

        var roomIndex = Random.Range(0, dungeon.Rooms.Count);
        connectedRooms.Add(roomIndex);

        while(connectedRooms.Count < dungeon.Rooms.Count)
        {
            int closestRoomIndex = FindClosestRoom(dungeon, roomIndex, connectedRooms);
            if (closestRoomIndex != -1)
            {
                connectedRooms.Add(closestRoomIndex);
                corridors.UnionWith(CreateCorridor(roomIndex, closestRoomIndex, dungeon));
                roomIndex = closestRoomIndex;
            }
        }

        return corridors;
    }

    private int FindClosestRoom(Dungeon dungeon, int roomIndex, HashSet<int> connectedRooms)
    {
        Debug.Log("SRWGenerator:FindClosestRoom");
        int closest = -1;
        float distance = float.MaxValue;

        for(int i = 0; i < dungeon.Rooms.Count; i++)
        {
            if (i == roomIndex) continue;
            if (connectedRooms.Contains(i)) continue;

            float currentDistance = Vector2Int.Distance(dungeon.Rooms[roomIndex].RoomCenter, dungeon.Rooms[i].RoomCenter);
            if(currentDistance < distance)
            {
                distance = currentDistance;
                closest = i;
            }
        }
        return closest;
    }

    private IEnumerable<Vector2Int> CreateCorridor(int currentRoomIndex, int destinationRoomIndex, Dungeon dungeon)
    {
        Debug.Log("SRWGenerator:CreateCorridor");
        HashSet<Vector2Int> corridor = new();
        var position = dungeon.Rooms[currentRoomIndex].RoomCenter;
        var destination = dungeon.Rooms[destinationRoomIndex].RoomCenter;
        corridor.Add(position);
        while (position.y != destination.y)
        {
            if (destination.y > position.y)
            {
                position += Vector2Int.up;
            }
            else if (destination.y < position.y)
            {
                position += Vector2Int.down;
            }
            corridor.Add(position);
        }
        while (position.x != destination.x)
        {
            if (destination.x > position.x)
            {
                position += Vector2Int.right;
            }
            else if (destination.x < position.x)
            {
                position += Vector2Int.left;
            }
            corridor.Add(position);
        }

        return corridor;
    }

    private Vector2Int FindNextStartPoint(DungeonParameters parameters, List<Room> rooms, int minX, int minY, int maxX, int maxY)
    {
        Debug.Log("SRWGenerator:FindNextStartPoint");
        Vector2Int startPoint = new();
        List<Vector2Int> directionsList = Direction2D.GetRandomCardinalDirectionList();

        for(int i = 0; i < directionsList.Count; i++)
        {
            var candidate = directionsList[i];
            candidate.Scale(new Vector2Int(parameters.walkLength + parameters.minimumRoomPadding, parameters.walkLength + parameters.minimumRoomPadding));
            candidate += new Vector2Int((minX + maxX) / 2, (minY + maxY) / 2);

            if (ValidateStartingPoint(candidate, rooms))
            {
                startPoint = candidate;
                break;
            }    
        }

        return startPoint;
    }

    private bool ValidateRoom(Room newRoom, List<Room> Rooms, int padding)
    {
        Debug.Log("SRWGenerator:ValidateRoom");
        foreach(Room room in Rooms)
        {
            if(newRoom.OverlapsWith(room, padding))
            {
                Debug.Log("SRWGenerator:ValidateRoom - False");
                return false;
            }
        }

        Debug.Log("SRWGenerator:ValidateRoom - True");
        return true;
    }

    private bool ValidateStartingPoint(Vector2Int candidate, List<Room> rooms)
    {
        Debug.Log("SRWGenerator:ValidateStartingPoint");
        foreach(Room room in rooms)
        {
            if (room.FloorPositions.Contains(candidate))
            {
                return false;
            }
        }

        return true;
    }
}