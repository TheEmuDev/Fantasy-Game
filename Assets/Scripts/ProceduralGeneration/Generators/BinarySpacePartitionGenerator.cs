using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BinarySpacePartitionGenerator : IDungeonGenerator
{
    public Dungeon GenerateDungeon(DungeonParameters parameters)
    {
        Debug.Log("BSPGenerator:GenerateDungeon");
        if (parameters.dungeonWidth <= 0 || parameters.dungeonHeight <= 0)
        {
            throw new ArgumentException("Dungeon width and height must be positive");
        }

        Dungeon dungeon = new();

        dungeon.Rooms.AddRange(BinarySpacePartition(parameters));
        dungeon.Path = ConnectRooms(dungeon);

        return dungeon;
    }

    private List<Room> BinarySpacePartition(DungeonParameters parameters)
    {
        Debug.Log("BSPGenerator:BinarySpacePartition");
        Queue<RectInt> roomsQueue = new();
        List<RectInt> roomsList = new();

        int splitDirectionCoin = -1;

        RectInt startSpace = new(parameters.startPosition, new Vector2Int(parameters.dungeonWidth, parameters.dungeonHeight));
        roomsQueue.Enqueue(startSpace);

        while (roomsQueue.Count > 0)
        {
            Debug.Log("Number of rooms in queue: " + roomsQueue.Count);
            var roomCandidate = roomsQueue.Dequeue();
            Debug.Log("Current room: " + roomCandidate.ToString());

            if (parameters.favoredSplitDirection == Direction2D.SplitDirections.Random)
            {
                splitDirectionCoin = Mathf.RoundToInt(Random.value);
            }

            bool splitHorizontally = splitDirectionCoin == 0 || parameters.favoredSplitDirection == Direction2D.SplitDirections.Horizontal;

            if (CanBeSplit(roomCandidate, parameters, splitHorizontally))
            {
                Debug.Log("Room Candidate can be split with first check - isHorizontal: " + splitHorizontally);
                SplitRoom(parameters, roomsQueue, roomCandidate, splitHorizontally);
            }
            else if (CanBeSplit(roomCandidate, parameters, !splitHorizontally))
            {
                Debug.Log("Room Candidate can be split with second check - isHorizontal: " + !splitHorizontally);
                SplitRoom(parameters, roomsQueue, roomCandidate, !splitHorizontally);
            } 
            else
            {
                Debug.Log("Room Candidate cannot be split! Adding it to the list");
                roomsList.Add(roomCandidate);
            }
        }

        return ToRooms(roomsList, parameters);
    }

    private bool CanBeSplit(RectInt roomCandidate, DungeonParameters parameters, bool splitHorizontally) => splitHorizontally
            ? roomCandidate.size.y >= 2 * (parameters.minimumRoomHeight + parameters.roomMargin)
            : roomCandidate.size.x >= 2 * (parameters.minimumRoomWidth + parameters.roomMargin);


    private void SplitRoom(DungeonParameters parameters, Queue<RectInt> roomsQueue, RectInt candidate, bool splitHorizontally)
    {
        Debug.Log("BSPGenerator:SplitRoom");

        var splitMin = splitHorizontally 
            ? parameters.minimumRoomHeight + parameters.roomMargin 
            : parameters.minimumRoomWidth + parameters.roomMargin;

        var splitMax = splitHorizontally
            ? candidate.size.y - parameters.minimumRoomHeight - parameters.roomMargin
            : candidate.size.x - parameters.minimumRoomWidth - parameters.roomMargin;

        var split = Random.Range(splitMin, splitMax);
  
        RectInt room1 = splitHorizontally
            ? new(candidate.min.x, candidate.min.y, candidate.size.x, split - parameters.roomMargin)
            : new(candidate.min.x, candidate.min.y, split - parameters.roomMargin, candidate.size.y);
        
        RectInt room2 = splitHorizontally
            ? new(candidate.min.x, candidate.min.y + split + parameters.roomMargin, candidate.size.x, candidate.size.y - split - parameters.roomMargin)
            : new(candidate.min.x + split + parameters.roomMargin, candidate.min.y, candidate.size.x - split - parameters.roomMargin, candidate.size.y);

        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    private HashSet<Vector2Int> ConnectRooms(Dungeon dungeon)
    {
        Debug.Log("BSPGenerator:ConnectRooms");
        HashSet<Vector2Int> corridors = new();
        
        HashSet<int> connectedRooms = new();

        var roomIndex = Random.Range(0, dungeon.Rooms.Count);
        connectedRooms.Add(roomIndex);

        while(connectedRooms.Count < dungeon.Rooms.Count)
        {
            int closestRoomIndex = FindClosestRoom(dungeon, roomIndex, connectedRooms);
            connectedRooms.Add(closestRoomIndex);
            corridors.UnionWith(CreateCorridor(roomIndex, closestRoomIndex, dungeon));
            roomIndex = closestRoomIndex;
        }

        return corridors;
    }

    private int FindClosestRoom(Dungeon dungeon, int roomIndex, HashSet<int> connectedRooms)
    {
        Debug.Log("BSPGenerator:FindClosestRoom");
        int closest = -1;
        float distance = float.MaxValue;
        for(int i = 0; i < dungeon.Rooms.Count; i++)
        {
            if (i == roomIndex) continue;
            if (connectedRooms.Contains(i)) continue;

            float currentDistance = Vector2Int.Distance(dungeon.Rooms[roomIndex].RoomCenter, dungeon.Rooms[i].RoomCenter);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = i;
            }
        }

        return closest;
    }

    private List<Room> ToRooms(List<RectInt> roomsList, DungeonParameters parameters)
    {
        Debug.Log("BSPGenerator:ToRooms");
        List<Room> rooms = new();

        foreach (RectInt roomSpace in roomsList)
        {
            rooms.Add(new Room(roomSpace));
        }

        return rooms;
    }

    private HashSet<Vector2Int> CreateCorridor(int currentRoomIndex, int destinationRoomIndex, Dungeon dungeon)
    {
        Debug.Log("BSPGenerator:CreateCorridor");
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
}
